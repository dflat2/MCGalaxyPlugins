using MCGalaxy;
using MCGalaxy.DB;
using MCGalaxy.Maths;
using System;
using System.IO;

namespace PluginBlockDBClean;

public class BlockDBCleaner {
    private string FormatBytes(int bytes) {
        if (bytes < 0) {
            throw new ArgumentException(string.Format("{0} cannot be negative.", nameof(bytes)));
        }

        double size = bytes;
        string[] units = { "b", "Kb", "Mb", "Gb" };
        int unitIndex = 0;

        while (size >= 1000 && unitIndex < units.Length - 1) {
            size /= 1000;
            unitIndex++;
        }

        string format = size == Math.Floor(size) ? "{0}{1}" : "{0:0.##}{1}";
        return string.Format(format, size, units[unitIndex]);
    }

    public void ShowPurged(Player player, int bytes) {
        player.Message(string.Format("Purged &T{0}&S.", FormatBytes(bytes)));
    }

    /// <summary> Truncate the block database, only keep records from the past 'delta'. </summary>
    /// <returns> Size of purged block datas (in bytes). </returns>
    public unsafe int Truncate(BlockDB blockDB, TimeSpan delta) {
        blockDB.FlushCache();

        if (!File.Exists(BlockDBFile.FilePath(blockDB.MapName))) {
            return 0;
        }

        int purged = 0;

        string filePath = BlockDBFile.FilePath(blockDB.MapName);
        string tempPath = BlockDBFile.TempPath(blockDB.MapName);

        DateTime thresholdDateTime = DateTime.UtcNow - delta;
        int thresholdIntEpochs = (int)(thresholdDateTime - BlockDB.Epoch).TotalSeconds;

        using (Stream source = File.OpenRead(filePath), destination = File.Create(tempPath)) {
            Vec3U16 dims;
            BlockDBFile.ReadHeader(source, out dims);
            BlockDBFile.WriteHeader(destination, blockDB.Dims);
            byte[] read = new byte[BlockDBFile.BulkEntries * BlockDBFile.EntrySize];
            byte[] write = new byte[BlockDBFile.BulkEntries * BlockDBFile.EntrySize];
            int written;

            fixed (byte* pointer = read, writePointer = write) {
                BlockDBEntry* entryPointer = (BlockDBEntry*)pointer;
                while (true) {
                    written = 0;

                    int count = BlockDBFile.V1.ReadForward(source, read, entryPointer);

                    if (count == 0) {
                        break;
                    }

                    for (int i = 0; i < count; i++) {
                        if (entryPointer[i].TimeDelta >= thresholdIntEpochs) {
                            Buffer.MemoryCopy(
                                &entryPointer[i],
                                writePointer + written * BlockDBFile.EntrySize,
                                BlockDBFile.EntrySize,
                                BlockDBFile.EntrySize);
                            written++;
                        } else {
                            purged += BlockDBFile.EntrySize;
                        }
                    }

                    destination.Write(write, 0, written * BlockDBFile.EntrySize);
                }
            }
        }

        File.Delete(filePath);
        File.Move(tempPath, filePath);
        return purged;
    }

    public void TruncateAll(Player player, TimeSpan delta) {
        int totalPurged = 0;
        string[] maps = LevelInfo.AllMapNames();
        Level level;
        bool unloadAfter;

        foreach (string map in maps) {
            unloadAfter = false;
            level = LevelInfo.FindExact(map);

            if (level == null) {
                unloadAfter = true;
                level = LevelActions.Load(player, map, false);
            }

            if (level == null) {
                player.Message(string.Format("Failed to load &T{0}&S, skipping.", map));
                continue;
            }

            totalPurged += Truncate(level.BlockDB, delta);

            if (unloadAfter) {
                level.Unload(true);
            }
        }

        player.Message(string.Format("Purged &T{0}&S.", FormatBytes(totalPurged)));
    }
}