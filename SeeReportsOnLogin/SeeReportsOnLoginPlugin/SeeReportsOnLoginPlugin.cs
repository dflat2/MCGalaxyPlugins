using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;

namespace PluginSeeReportsOnLogin;

public sealed class SeeReportsOnLoginPlugin : Plugin
{
    public override string name { get { return "SeeReportsOnLogin"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

    public override void Load(bool startup)
    {
        OnPlayerConnectEvent.Register(ShowReportsOnPlayerConnect, Priority.Low);
    }

    public override void Unload(bool shutdown)
    {
        OnPlayerConnectEvent.Unregister(ShowReportsOnPlayerConnect);
    }

    private void ShowReportsOnPlayerConnect(Player player)
    {
        if (player == null || player.Rank < LevelPermission.Operator)
        {
            return;
        }

        Command cmdReport = Command.Find("Report");

        if (cmdReport == null)
        {
            return;
        }

        cmdReport.Use(player, "list");
    }

    public override void Help(Player player)
    {
        player.Message("&TSeeReportsOnLogin");
        player.Message("&HAutomatically runs &T/Report list &Hwhen an operator joins.");
    }
}