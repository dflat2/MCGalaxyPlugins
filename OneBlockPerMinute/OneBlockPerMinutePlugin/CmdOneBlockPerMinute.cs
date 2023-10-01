using MCGalaxy;

namespace PluginOneBlockPerMinute;

public class CmdOneBlockPerMinute : Command
{
    public override string name => "OneBlockPerMinute";
    public override string type => CommandTypes.World;
    public override bool SuperUseable => false;

    public override void Help(Player player)
    {
        player.Message("&T/OneBlockPerMinute enable/disable");
        player.Message("&H[De]activates 1BPM on current level.");
    }

    public override void Use(Player p, string message)
    {
        
    }
}
