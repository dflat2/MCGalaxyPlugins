using System;
using System.IO;
using MCGalaxy;

public class CmdConsoleDo : Command
{
    public override string name { get { return "ConsoleDo"; } }
    public override string type { get { return CommandTypes.Information; } }
    public override bool museumUsable { get { return true; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

    public override void Use(Player player, string message)
    {
        string[] arguments = message.SplitSpaces();

        if (arguments.Length == 0)
        {
            Help(player);
            return;
        }

        Command command = Command.Find(arguments[0]);

        if (command == null)
        {
            player.Message($"&SUnknown command: \"{arguments[0]}\".");
            return;
        }

        command.Use(Player.Console, message.Splice(1, 0));
    }

    public override void Help(Player player)
    {
        player.Message("&T/ConsoleDo <command> [arguments]");
        player.Message("&HRuns &T<command> &Has console.");
        player.Message("&HNote: command output is only visible in the console and the logs."); 
    }
}
