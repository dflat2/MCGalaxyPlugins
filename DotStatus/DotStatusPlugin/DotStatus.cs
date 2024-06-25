namespace DotStatusPlugin;
using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;

public class DotStatus : Plugin {
    private string _commandName = "ZombieSurvival"; // CHANGE THIS

    public override string name => "DotStatus";
    public override string creator => "D_Flat";
    public override string MCGalaxy_Version => "1.9.4.9";

    public override void Load(bool auto) {
        OnPlayerChatEvent.Register(HandlePlayerChat, Priority.Normal);
    }

    public override void Unload(bool auto) {
        OnPlayerChatEvent.Unregister(HandlePlayerChat);
    }

    public override void Help(Player player) {
        player.Message("&SDotStatus");
        player.Message("&HWhen a player sends &T'.' &Hor &T't'&H, cancels chat and displays game status.");
    }

    private void HandlePlayerChat(Player player, string message) {
        if (message.Equals("t") || message.Equals(".")) {
            ShowGameStatus(player);
            player.cancelchat = true;
        }
    }

    private void ShowGameStatus(Player player) {
        Command gameCommand = Command.Find(_commandName);

        if (gameCommand == null) {
            player.Message($"&WDotStatus: unable to find command &T{_commandName}&W.");
            return;
        }

        gameCommand.Use(player, "status");
    }
}