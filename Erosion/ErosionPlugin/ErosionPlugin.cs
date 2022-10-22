namespace PluginErosion;
using MCGalaxy;

public sealed class ErosionPlugin : Plugin
{
    public override string name { get { return "Erosion"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.3"; } }

    public override void Load(bool startup)
    {
        Command.Register(new CmdErode());
        Chat.MessageGlobal("&aErosion And Dilation plugin successfuly loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Command.Unregister(Command.Find("erode"));
        Chat.MessageGlobal("&cErosion And Dilation plugin successfuly unloaded.");
    }
}