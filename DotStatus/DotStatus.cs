using MCGalaxy.Events.PlayerEvents;
using MCGalaxy;
using System;

namespace DotStatusPlugin {
	
	
	public class DotStatus : Plugin
	{
	    private string m_commandName = "ZombieSurvival"; // CHANGE THIS
	
	    public override string name => "DotStatus";
	    public override string creator => "D_Flat";
	    public override string MCGalaxy_Version => "1.9.4.3";
	
	    public override void Load(bool auto)
	    {
	        OnPlayerChatEvent.Register(HandlePlayerChat, Priority.Normal);
	    }
	
	    public override void Unload(bool auto)
	    {
	        OnPlayerChatEvent.Unregister(HandlePlayerChat);
	    }
	
	    public override void Help(Player player)
	    {
	        player.Message("&SDotStatus");
	        player.Message("&HWhen a player sends &T'.' &Hor &T't'&H, cancels chat and displays game status.");
	    }
	
	    private void HandlePlayerChat(Player player, string message)
	    {
	        if (message == "t" || message == ".")
	        {
	            ShowGameStatus(player);
	            player.cancelchat = true;
	        }
	    }
	
	    private void ShowGameStatus(Player player)
	    {
	        Command gameCommand = Command.Find(m_commandName);
	
	        if (gameCommand == null)
	        {
	            player.Message($"&WDotStatus: unable to find command &T{m_commandName}&W.");
	            return;
	        }
	
	        gameCommand.Use(player, "status");
	    }
	}
}