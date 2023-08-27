namespace Plugin__PLUGINNAME__;
using MCGalaxy;

public sealed class __PLUGINNAME__Plugin : Plugin
{
    public override string name { get { return "__PLUGINNAME__"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    public override void Load(bool startup)
    {
        Chat.MessageGlobal("&a__PLUGINNAME__ plugin successfully loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Chat.MessageGlobal("&c__PLUGINNAME__ plugin successfully unloaded.");
    }

    public override void Help(Player p)
    {

    }
}