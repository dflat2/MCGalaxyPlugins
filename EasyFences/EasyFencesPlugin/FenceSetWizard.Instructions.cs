namespace PluginEasyFences;
using System;
using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.DB;

internal class FenceSetWizard
{
    private FenceSetProps set;
    private delegate bool Process(string input);
    private Player p;

    private string abortMsg = "&SUse &T/ezf abort &Sat any time to stop making the fences.";
    private string revertMsg = "&SUse &T/ezf revert &Sto go back a step.";
    private string promptInputMsg = "&SUse &T/ezf [input] &Sto provide input.";
    private string dashesMsg = "&f--------------------------";

    internal FenceSetWizard(Player p)
    {
        this.set = new FenceSetProps();
        this.p = p;
    }

    internal void ManageInput(string input)
    {

    }

    private bool ProcessCanJumpOver(string input)
    {
        return true;
    }

    private bool ProcessDoBury(string input)
    {
        return true;
    }

    private bool ProcessSourceID(string input)
    {
        return true;
    }

    private bool ProcessDestID(string input)
    {
        return true;
    }

    private bool ProcessCrossIntersect(string input)
    {
        return true;
    }

    private bool ProcessTIntersect(string input)
    {
        return true;
    }
}
