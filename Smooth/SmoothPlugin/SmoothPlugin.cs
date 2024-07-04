namespace PluginSmooth;
using MCGalaxy;

public sealed class SmoothPlugin : Plugin {
    public override string name { get { return "Smooth"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    public override void Load(bool startup) {
        Command.Register(new CmdSmooth());
    }

    public override void Unload(bool shutdown) {
        Command.Unregister(Command.Find("Smooth"));
    }

    public override void Help(Player player) {
        Command cmdSmooth = Command.Find("Smooth");
        cmdSmooth.Help(player);
    }
}