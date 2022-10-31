namespace PluginEasyFences;
using MCGalaxy;
using MCGalaxy.Commands;
using System.Collections.Generic;
using MCGalaxy.Commands.CPE;
using BlockID = System.UInt16;

public class CmdEasyFences : Command2
{
    public override string name => "EasyFences";
    public override string type => "Building";
    public override string shortcut => "ezf";
    public override bool SuperUseable => false;

    private string usage = "&T/makefences";
    
    public override void Help(Player p)
    {
        p.Message(usage);
        p.Message("&HRun the Minecraft-fences creation process.");
    }

    public override void Use(Player p, string message)
    {
        bool hasWizard = p.Extras.Contains("FenceSetWizard");
        FenceSetWizard wizard;

        if (!hasWizard)
        {
            wizard = new FenceSetWizard(p);
            p.Extras["FenceSetWizard"] = wizard;
            return;
        }

        wizard = (FenceSetWizard)p.Extras["FenceSetWizard"];
        string wizardInput = message.SplitSpaces()[0];

        if (wizardInput.ToLower() == "abort" || wizardInput.ToLower() == "cancel")
        {
            p.Extras.Remove("FenceSetWizard");
            p.Message("&SAborted the fence creation process.");
            return;
        }

        bool wizardEnd = wizard.ManageInput(wizardInput);

        if (wizardEnd)
        {
            List<FenceElement> elements = wizard.BuildFenceElements();
            AddFencesElements(p, elements);
            p.Extras.Remove("FenceSetWizard");
        }
    }

    private void AddFencesElements(Player p, List<FenceElement> elements)
    {
        Command cmdLevelBlock = Command.Find("levelblock");

        if (!p.CanUse(cmdLevelBlock))
        {
            p.Message("&WYou do not have the permissions to edit level blocks on this map.");
            return;
        }

        List<string> rawCommands;

        for (int i = 0; i < elements.Count; i++)
        {
            rawCommands = elements[i].RawCommands(p, i);

            foreach (string command in rawCommands)
            {
                cmdLevelBlock.Use(p, command);
            }
        }
    }
}

