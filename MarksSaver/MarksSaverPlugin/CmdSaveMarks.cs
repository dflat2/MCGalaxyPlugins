namespace PluginMarksSaver;
using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Maths;
using BlockID = UInt16;

public sealed class CmdSaveMarks : Command
{
    public override string name => "savemarks";
    public override string shortcut => "sm";
    public override string type => "building";

    public override bool museumUsable => false;
    public override bool SuperUseable => false;

    private int MarksCount;

    public override void Help(Player p)
    {
        p.Message("&T/savemarks <count>");
        p.Message("&HPrompt for marks and save them for later use.");
        p.Message("&T<count> &Hdefaults to 2 if not given.");
        p.Message("&HSee also: &H/loadmarks");
    }

    public override void Use(Player p, string message)
    {
        if (!ParseArguments(p, message))
        {
            p.Message("&HUsage: &T/savemarks <count> &H where %T<count> %His 1, 2 or 3.");
            return;
        }

        bool plural = (MarksCount >= 2);

        if (plural)
        {
            p.Message($"Place or break {MarksCount} blocks to determine the edges.");
            p.MakeSelection(MarksCount, $"Selecting {MarksCount} blocks for &S/savemarks", null, DoSaveMarks);
        }
        else
        {
            p.Message("Place or break a block.");
            p.MakeSelection(MarksCount, "Selecting a block for &S/savemarks", null, DoSaveMarks);
        }
    }

    private bool ParseArguments(Player p, string arguments)
    {
        string[] parts = arguments.SplitSpaces();

        if (parts.Length == 0 || arguments == "")
        {
            MarksCount = 2;
            return true;
        }
        else if (parts.Length == 1)
        {
            int marksCount = 2;
            if (CommandParser.GetInt(p, parts[0], "Marks count", ref marksCount, 1, 3))
            {
                MarksCount = marksCount;
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public bool DoSaveMarks(Player p, Vec3S32[] marks, object state, BlockID block)
    {
        if (marks == null) return true;
        SavedMarks.Set(p, marks);
        return true;
    }
}