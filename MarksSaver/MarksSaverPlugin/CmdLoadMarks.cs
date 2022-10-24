namespace PluginMarksSaver;
using MCGalaxy;
using MCGalaxy.Maths;

public class CmdLoadMarks : Command
{
    public override string name => "loadmarks";
    public override string shortcut => "lm";
    public override string type => "building";

    public override bool museumUsable => false;
    public override bool SuperUseable => false;

    public override void Help(Player p)
    {
        p.Message("&T/loadmarks");
        p.Message("&HMarks what's been saved with &T/savemarks");
        p.Message("&HSee also: &T/savemarks");
    }

    public override void Use(Player p, string message)
    {
        if (message.Length != 0)
        {
            p.Message("&HUsage: &T/loadmarks");
            return;
        }

        Vec3S32[] marks = SavedMarks.Get(p);

        if (marks.Length == 0)
        {
            p.Message("&HYou did not save any marks yet. Run &T/savemarks &Hfirst.");
            return;
        }

        Command cmdMark = Command.Find("mark");

        foreach (Vec3S32 mark in marks)
        {
            cmdMark.Use(p, mark.ToStringNoComma());
        }
    }
}