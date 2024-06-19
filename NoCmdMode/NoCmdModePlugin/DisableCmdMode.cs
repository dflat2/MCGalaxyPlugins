using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;

namespace NoCmdModePlugin;

public class DisableCmdMode : Plugin {
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
    public override string name { get { return "NoCmdMode"; } }

    private Command cmdMode;

    public override void Load(bool startup) {
        cmdMode = Command.Find("Mode");

        if (cmdMode != null) {
            Command.Unregister(cmdMode);
        }

        OnPlayerCommandEvent.Register(HandlePlayerCommand, Priority.Normal);
    }

    public override void Unload(bool shutdown) {
        if (cmdMode != null) {
            Command.Register(cmdMode);
        }

        OnPlayerCommandEvent.Unregister(HandlePlayerCommand);
    }

    public override void Help(Player player) {
        player.Message("&HPlugin: &TDisableCmdMode");
        player.Message("&HDisables &T/Mode &Hand all &T/<block> &Hcommands&H, such as");
        player.Message("&T/Stone&H, &T/Water&H, etc.");
    }

    public void HandlePlayerCommand(Player player, string commandName, string args, CommandData data) {
        Command command = Command.Find(commandName);

        if (command != null) {
            return;
        }

        if (Block.Parse(player, commandName) != Block.Invalid) {
            player.cancelcommand = true;
            player.Message("Placing blocks with &T/Mode &Sis disabled.");
        }
    }
}