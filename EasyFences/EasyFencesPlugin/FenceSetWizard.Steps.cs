namespace PluginEasyFences;
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
        bool success = CommandParser.GetInt(player, input, "block-id", ref result, 0, Block.MaxRaw - SetProps.BlocksCount);

        if (success)
        {
            if (!IsRangeFree((ushort) result, (ushort)(result + SetProps.BlocksCount - 1), player.Level))
            {
                player.Message($"&WThe {result}-{result + SetProps.BlocksCount - 1} range already have level blocks.");
                player.Message($"&WPlease remove them or choose another range.");
                success = false;
            }
            else
            {
                SetProps.CopiedTo = (ushort)result;
            }
        }

        return success;
    }
}
