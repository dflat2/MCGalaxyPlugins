namespace PluginEasyFences;
using MCGalaxy;
using System.Collections.Generic;

public class CmdEasyFences : Command2
{
    public override string name { get { return "EasyFences"; } }
    public override string type { get { return CommandTypes.Building; } }
    public override string shortcut { get { return "EZF"; } }
    public override bool SuperUseable { get { return false; } }
    
    public override void Help(Player player)
    {
        player.Message("&T/EasyFences");
        player.Message("&HRun the Minecraft-fences creation process.");
    }

    public override void Use(Player player, string message)
    {
        bool hasWizard = player.Extras.Contains("FenceSetWizard");
        FenceSetWizard wizard;

        if (!hasWizard) {
            wizard = new FenceSetWizard(player);
            player.Extras["FenceSetWizard"] = wizard;
            return;
        }

        wizard = (FenceSetWizard)player.Extras["FenceSetWizard"];
        string wizardInput = message.SplitSpaces()[0];

        if (wizardInput.ToLower() == "abort" || wizardInput.ToLower() == "cancel") {
            player.Extras.Remove("FenceSetWizard");
            player.Message("&SAborted the fence creation process.");
            return;
        }

        bool wizardEnd = wizard.ManageInput(wizardInput);

        if (wizardEnd) {
            List<FenceElement> elements = wizard.BuildFenceElements();
            AddFencesElements(player, elements);
            player.Extras.Remove("FenceSetWizard");
        }
    }

    private void AddFencesElements(Player p, List<FenceElement> elements) {
        Command cmdLevelBlock = Command.Find("levelblock");
        Command cmdOverseer = Command.Find("overseer");

        Command lbCmd = cmdLevelBlock;
        string prefix = "";

        if (!p.CanUse(cmdLevelBlock)) {
            if (LevelInfo.IsRealmOwner(p.Level, p.name)) {
                lbCmd = cmdOverseer;
                prefix = "lb ";
            }
            else {
                p.Message("&WYou do not have the permissions to edit level blocks on this map.");
                return;
            }
        }

        List<string> rawCommands;

        for (int i = 0; i < elements.Count; i++) {
            rawCommands = elements[i].RawCommands(player: p, count: i, prefix: prefix);

            foreach (string command in rawCommands) {
                lbCmd.Use(p, command);
            }
        }
    }
}

