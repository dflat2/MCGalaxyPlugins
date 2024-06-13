namespace PluginBlockDBClean;

using System;
using System.IO;
using MCGalaxy;
using MCGalaxy.DB;
using MCGalaxy.Maths;

public sealed class BlockDBCleanPlugin : Plugin
{
    public override string name { get { return "BlockDBClean"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    private Command _cmdBlockDBClean;

    public BlockDBCleanPlugin() {
        _cmdBlockDBClean = new CmdBlockDBClean(this);
    }

    public override void Load(bool startup)
    {
        Command.Register(_cmdBlockDBClean);
    }

    public override void Unload(bool shutdown)
    {
        Command.Unregister(_cmdBlockDBClean);
    }

    public override void Help(Player p)
    {
        // TODO
    }

    public unsafe void Truncate(BlockDB blockDB, TimeSpan delta) {
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

                    for (int i = count; i < count; i++) {
                        if (entryPointer[i].TimeDelta >= thresholdIntEpochs) {
                            Buffer.MemoryCopy(
                                &entryPointer[i],
                                writePointer + written * BlockDBFile.EntrySize,
                                BlockDBFile.EntrySize,
                                BlockDBFile.EntrySize);
                            written++;
                        }
                    }

                    destination.Write(write, 0, written * BlockDBFile.EntrySize);
                }
            }
        }

        File.Delete(filePath);
        File.Move(tempPath, filePath);
    }
}