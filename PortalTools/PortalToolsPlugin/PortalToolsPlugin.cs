namespace PluginPortalTools;
using MCGalaxy;

public sealed class PortalToolsPlugin : Plugin
{
    public override string name { get { return "PortalTools"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    public override void Load(bool startup)
    {
        Chat.MessageGlobal("&aPortalTools plugin successfully loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Chat.MessageGlobal("&cPortalTools plugin successfully unloaded.");
    }

    public override void Help(Player p)
    {

    }
}