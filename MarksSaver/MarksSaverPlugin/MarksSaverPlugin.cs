namespace PluginMarksSaver;
using MCGalaxy;

public sealed class MarksSaverPlugin : Plugin
{
    public override string name { get { return "MarksSaver"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.3"; } }

    public override void Load(bool startup)
    {
        Command.Register(new CmdSaveMarks());
        Command.Register(new CmdLoadMarks());
        Chat.MessageGlobal("&aSaveMarks plugin successfuly loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Command.Unregister(Command.Find("savemarks"));
        Command.Unregister(Command.Find("loadmarks"));
        Chat.MessageGlobal("&cSaveMarks plugin successfuly unloaded.");
    }

    public override void Help(Player p)
    {
        Command cmdHelp = Command.Find("help");
        cmdHelp.Use(p, "savemarks");
    }
}