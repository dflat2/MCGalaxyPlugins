﻿namespace PluginBlockDBClean;
using System;
using MCGalaxy;
using MCGalaxy.Commands;

public class CmdBlockDBClean : Command {
    public override string name { get { return "BlockDBClean"; } }
    public override string type { get { return CommandTypes.World; } }
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