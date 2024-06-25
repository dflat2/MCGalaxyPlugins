using MCGalaxy.Maths;
using System;
using MCGalaxy;
using MCGalaxy.Commands;
using System.IO;
using MCGalaxy.DB;

namespace BlockDBCleanPlugin
{

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
    public sealed class BlockDBCleanPlugin : Plugin
    {
        public override string name { get { return "BlockDBClean"; } }
        public override string creator { get { return "D_Flat"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

        private readonly Command _cmdBlockDBClean;
        private readonly Command _cmdBlockDBCleanAll;
        private readonly BlockDBCleaner _blockDBCleaner;

        public BlockDBCleanPlugin() {
            _blockDBCleaner = new BlockDBCleaner();
            _cmdBlockDBClean = new CmdBlockDBClean(_blockDBCleaner);
            _cmdBlockDBCleanAll = new CmdBlockDBCleanAll(_blockDBCleaner);
        }

        public override void Load(bool startup)
        {
            Command.Register(_cmdBlockDBClean);
            Command.Register(_cmdBlockDBCleanAll);
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(_cmdBlockDBClean);
            Command.Unregister(_cmdBlockDBCleanAll);
        }

        public override void Help(Player p)
        {
            // TODO
        }
    }
    public class CmdBlockDBClean : Command {
        public override string name => "BlockDBClean";
        public override string type => CommandTypes.World;
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        private readonly BlockDBCleaner _cleaner;

        public CmdBlockDBClean(BlockDBCleaner cleaner) {
            _cleaner = cleaner;
        }

        public override void Use(Player player, string message) {
            string[] arguments = message.SplitSpaces();

            if (arguments.Length != 2) {
                player.Message("Usage: &T/BlockDBClean <map> <duration>&S.");
                return;
            }

            string mapName = Matcher.FindMaps(player, arguments[0]);

            if (mapName == null) {
                return;
            }

            Level level = LevelInfo.FindExact(mapName);

            if (level == null) {
                player.Message(string.Format("Cannot clean BlockDB for {0} because it is currently unloaded.", mapName));
                return;
            }

            TimeSpan delta = TimeSpan.FromDays(365);

            if (!CommandParser.GetTimespan(player, arguments[1], ref delta, "truncate the past", "s")) {
                return;
            }

            int purged = _cleaner.Truncate(level.BlockDB, delta);
            _cleaner.ShowPurged(player, purged);
        }

        public override void Help(Player player) {
            player.Message("&T/BlockDBClean <map> <duration>");
            player.Message("Truncates the BlockDB on given &Tmap&S.");
            player.Message("Only keeps records for the past &Tduration&S.");
        }
    }
    public class CmdBlockDBCleanAll : Command {
        public override string name { get { return "BlockDBCleanAll"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        private readonly BlockDBCleaner _blockDBCleaner;

        public CmdBlockDBCleanAll(BlockDBCleaner blockDBCleaner) {
            _blockDBCleaner = blockDBCleaner;
        }

        public override void Use(Player player, string message) {
            string[] arguments = message.SplitSpaces();

            if (arguments.Length >= 2 || message.Equals(string.Empty)) {
                player.Message("Usage: &T/BlockDBCleanAll <duration>&S.");
                return;
            }

            TimeSpan delta = TimeSpan.FromDays(365);

            if (!CommandParser.GetTimespan(player, arguments[0], ref delta, "truncate the past", "s")) {
                return;
            }

            _blockDBCleaner.TruncateAll(player, delta);
        }

        public override void Help(Player player) {
            player.Message("&T/BlockDBCleanAll <duration>");
            player.Message("Truncates all BlockDBs. Only keep records for the past &Tduration&S.");
        }
    }
}