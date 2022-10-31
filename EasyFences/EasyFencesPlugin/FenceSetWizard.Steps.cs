namespace PluginEasyFences;
using System;
using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.Commands;

internal partial class FenceSetWizard
{
    private bool StepSourceID(string input)
    {
        int result = 0;
        bool success = CommandParser.GetInt(player, input, "block-id", ref result, 1, 1024);
        SetProps.CopiedFrom = (ushort) result;
        return success;
    }

    private bool StepCanJumpOver(string input)
    {
        bool result = false;
        bool success = CommandParser.GetBool(player, input, ref result);
        SetProps.CanJumpOver = result;
        return success;
    }

    private bool StepDoBury(string input)
    {
        bool result = false;
        bool success = CommandParser.GetBool(player, input, ref result);
        SetProps.DoBury = result;
        return success;
    }

    private bool StepCrossIntersect(string input)
    {
        bool result = false;
        bool success = CommandParser.GetBool(player, input, ref result);
        SetProps.CrossIntersect = result;
        return success;
    }

    private bool StepTIntersect(string input)
    {
        bool result = false;
        bool success = CommandParser.GetBool(player, input, ref result);
        SetProps.TIntersect = result;
        return success;
    }

    private bool StepDestID(string input)
    {
        int result = 0;
        bool success = CommandParser.GetInt(player, input, "block-id", ref result, 1, 1024);
        SetProps.CopiedTo = (ushort)result;
        return success;
    }
}
