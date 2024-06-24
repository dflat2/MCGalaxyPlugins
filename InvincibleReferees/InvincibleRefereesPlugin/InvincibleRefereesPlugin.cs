namespace PluginInvincibleReferees;
using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;

public class InvincibleReferees : Plugin
{
    public override string name { get { return "InvincibleReferees"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
    public override string creator { get { return "D_Flat"; } }

    public override void Load(bool auto) {
        OnPlayerActionEvent.Register(HandlePlayerAction, Priority.Normal);
    }

    public override void Unload(bool auto) {
        OnPlayerActionEvent.Unregister(HandlePlayerAction);
    }

    public override void Help(Player player) {
        player.Message("&TInvincibleReferees");
        player.Message("&HMakes you invincible when using &T/Ref&H.");
    }

    private void HandlePlayerAction(Player player, PlayerAction action, string message, bool stealth) {
        if (action == PlayerAction.Referee) {
            player.invincible = true;
        } else if (action == PlayerAction.UnReferee) {
            player.invincible = false;
        }
    }
}
