namespace PluginMarksSaver;
using MCGalaxy;
using MCGalaxy.Maths;

public class CmdLoadMarks : Command {
    public override string name { get { return "LoadMarks"; } }
    public override string shortcut { get { return "LM"; } }
    public override string type { get { return "building"; } }

    public override bool museumUsable { get { return false; } }
    public override bool SuperUseable { get { return false; } }

    public override void Help(Player player) {
        player.Message("&T/LoadMarks");
        player.Message("&HMarks what's been saved with &T/SaveMarks&S.");
        player.Message("&HSee also: &T/savemarks&S.");
    }

    public override void Use(Player player, string message) {
        if (message.Length != 0) {
            player.Message("&HUsage: &T/loadmarks");
            return;
        }

        Vec3S32[] marks = SavedMarks.Get(player);

        if (marks.Length == 0) {
            player.Message("&HYou did not save any marks yet. Run &T/SaveMarks &Hfirst.");
            return;
        }

        Command cmdMark = Command.Find("mark");

        foreach (Vec3S32 mark in marks) {
            cmdMark.Use(player, mark.ToStringNoComma());
        }
    }
}