namespace PluginEasyFences;
using System;
using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.DB;

internal partial class FenceSetWizard
{
    private FenceSetProps set;
    private delegate bool Step(string input);
    private Player p;

    internal FenceSetWizard(Player p)
    {
        this.set = new FenceSetProps();
        this.p = p;
    }

    internal void ManageInput(string input)
    {

    }

    private bool StepCanJumpOver(string input)
    {
        return true;
    }

    private bool StepDoBury(string input)
    {
        return true;
    }

    private bool StepSourceID(string input)
    {
        return true;
    }

    private bool StepDestID(string input)
    {
        return true;
    }

    private bool StepCrossIntersect(string input)
    {
        return true;
    }

    private bool StepTIntersect(string input)
    {
        return true;
    }
}
