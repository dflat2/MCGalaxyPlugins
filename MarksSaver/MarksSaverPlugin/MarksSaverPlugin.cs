namespace PluginMarksSaver;
using MCGalaxy;

public sealed class MarksSaverPlugin : Plugin {
    public override string name { get { return "MarksSaver"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    public override void Load(bool startup) {
        Command.Register(new CmdSaveMarks());
        Command.Register(new CmdLoadMarks());
    }

    public override void Unload(bool shutdown) {
        Command.Unregister(Command.Find("SaveMarks"));
        Command.Unregister(Command.Find("LoadMarks"));
    }

    public override void Help(Player player) {
        Command cmdHelp = Command.Find("help");
        cmdHelp.Use(player, "savemarks");
    }
}