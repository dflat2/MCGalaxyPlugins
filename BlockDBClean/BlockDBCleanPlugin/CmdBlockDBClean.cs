namespace PluginBlockDBClean;
using System;
using MCGalaxy;
using MCGalaxy.Commands;

public class CmdBlockDBClean : Command {
    public override string name => "BlockDBClean";
    public override string type => CommandTypes.World;
    public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
    private readonly BlockDBCleanPlugin _blockDBCleanPlugin;


    public CmdBlockDBClean(BlockDBCleanPlugin blockDBCleanPlugin) {
        _blockDBCleanPlugin = blockDBCleanPlugin;
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

        _blockDBCleanPlugin.Truncate(level.BlockDB, delta);
        player.Message("Done.");
    }

    public override void Help(Player player) {
        player.Message("&T/BlockDBClean <map> <duration>");
        player.Message("Truncates the BlockDB on given &Tmap&S.");
        player.Message("Only keeps records for the past &Tduration&S.");
    }
}