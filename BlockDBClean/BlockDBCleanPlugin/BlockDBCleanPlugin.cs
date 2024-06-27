using MCGalaxy;

namespace PluginBlockDBClean;
public sealed class BlockDBCleanPlugin : Plugin {
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

    public override void Load(bool startup) {
        Command.Register(_cmdBlockDBClean);
        Command.Register(_cmdBlockDBCleanAll);
    }

    public override void Unload(bool shutdown) {
        Command.Unregister(_cmdBlockDBClean);
        Command.Unregister(_cmdBlockDBCleanAll);
    }

    public override void Help(Player player) {
        player.Message("&TBlockDBCleanPlugin");
        player.Message("Adds &T/BlockDBClean &Hand &T/BlockDBCleanAll &Hto truncate BlockDBs on given maps.");
    }
}