namespace PluginSaveMarks;

using System;
using System.Data.Common;
using MCGalaxy;

public sealed class SaveMarksPlugin : Plugin
{
    public override string name { get { return "SaveMarks"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.3"; } }

    public override void Load(bool startup)
    {
        Command.Register(new CmdSaveMarks());
        Chat.MessageGlobal("&aSaveMarks plugin successfuly loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Command.Unregister(Command.Find("savemarks"));
        Chat.MessageGlobal("&cSaveMarks plugin successfuly unloaded.");
    }
}