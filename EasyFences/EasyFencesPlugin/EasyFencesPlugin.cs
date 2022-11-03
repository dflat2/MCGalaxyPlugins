namespace PluginEasyFences;
using MCGalaxy;

public class EasyFencesPlugin : Plugin
{
    public override string name => "EasyFencesPlugin";
    public override string MCGalaxy_Version => "1.9.4.3";

    public override void Load(bool auto)
    {
        Command.Register(new CmdEasyFences());
    }

    public override void Unload(bool auto)
    {
        Command.Unregister(Command.Find("easyfences"));
    }
}