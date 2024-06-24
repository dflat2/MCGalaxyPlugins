namespace PluginMarksSaver;
using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

public sealed class CmdSaveMarks : Command
{
    public override string name { get { return "SaveMarks"; } }
    public override string shortcut { get { return "SM"; } }
    public override string type { get { return "building"; } }

    public override bool museumUsable { get { return false; } }
    public override bool SuperUseable { get { return false; } }

    private int MarksCount;

    public override void Help(Player player) {
        player.Message("&T/SaveMarks [count]");
        player.Message("&HPrompt for marks and save them for later use.");
        player.Message("&Tcount &Hdefaults to 2 if not given.");
        player.Message("&HSee also: &H/LoadMarks&H.");
    }

    public override void Use(Player player, string message) {
        if (!ParseArguments(player, message)) {
            player.Message("&HUsage: &T/SaveMarks [count] &H where %Tcount %His 1, 2 or 3.");
            return;
        }

        bool plural = (MarksCount >= 2);

        if (plural) {
            player.Message($"Place or break {MarksCount} blocks to determine the edges.");
            player.MakeSelection(MarksCount, $"Selecting {MarksCount} blocks for &S/savemarks", null, DoSaveMarks);
        } else {
            player.Message("Place or break a block.");
            player.MakeSelection(MarksCount, "Selecting a block for &S/savemarks", null, DoSaveMarks);
        }
    }

    private bool ParseArguments(Player player, string arguments) {
        string[] parts = arguments.SplitSpaces();

        if (parts.Length == 0 || arguments.Equals(string.Empty)) {
            MarksCount = 2;
            return true;
        } else if (parts.Length == 1) {
            int marksCount = 2;

            if (CommandParser.GetInt(player, parts[0], "Marks count", ref marksCount, 1, 3)) {
                MarksCount = marksCount;
                return true;
            } else {
                return false;
            }
        }

        return false;
    }

    public bool DoSaveMarks(Player player, Vec3S32[] marks, object state, BlockID block) {
        if (marks == null) {
            return true;
        }

        SavedMarks.Set(player, marks);
        return true;
    }
}