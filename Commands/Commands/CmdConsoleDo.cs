using MCGalaxy;

public class CmdConsoleDo : Command {
    public override string name { get { return "ConsoleDo"; } }
    public override string type { get { return CommandTypes.Moderation; } }
    public override bool museumUsable { get { return true; } }
    public override bool SuperUseable { get { return false; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

    public override void Use(Player player, string message) {
        string[] arguments = message.SplitSpaces();

        if (string.IsNullOrEmpty(message)) {
            Help(player);
            return;
        }

        Command command = Find(arguments[0]);

        if (command == null) {
            player.Message($"&SUnknown command: \"{arguments[0]}\".");
            return;
        }

        command.Use(Player.Console, message.Splice(1, 0));
    }

    public override void Help(Player player) {
        player.Message("&T/ConsoleDo <command> [arguments]");
        player.Message("&HRuns &Tcommand &Has console.");
        player.Message("&HNote: command output is only visible in the console and the logs."); 
    }
}
