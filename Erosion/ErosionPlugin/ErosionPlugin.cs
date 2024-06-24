namespace PluginErosion;
using MCGalaxy;

public sealed class ErosionPlugin : Plugin {
    public override string name { get { return "Erosion"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    public override void Load(bool startup) {
        Command.Register(new CmdErode());
        Chat.MessageGlobal("&aErosion plugin successfuly loaded.");
    }

    public override void Unload(bool shutdown) {
        Command.Unregister(Command.Find("Erode"));
        Chat.MessageGlobal("&cErosion plugin successfuly unloaded.");
    }

    public override void Help(Player player) {
        Command cmdHelp = Command.Find("help");
        cmdHelp.Use(player, "erode");
    }
}