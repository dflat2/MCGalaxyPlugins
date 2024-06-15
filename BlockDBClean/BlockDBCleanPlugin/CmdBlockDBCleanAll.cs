namespace PluginBlockDBClean;
using System;
using MCGalaxy;
using MCGalaxy.Commands;

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