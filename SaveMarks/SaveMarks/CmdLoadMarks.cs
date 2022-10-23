namespace PluginSaveMarks;

using System;
using MCGalaxy;
using MCGalaxy.DB;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

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
        p.Message("&HIt doesn't trigger block activation (e.g. doors) like &T/mark &Hwould.")
        p.Message("&HSee also: &H/savemarks");
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

        if (!p.HasBlockChange()) return;

        BlockID block = p.GetHeldBlock();

        foreach (Vec3S32 mark in marks)
        {
            if (!p.Ignores.DrawOutput)
                p.Message("Mark placed at &b({0}, {1}, {2})", mark.x, mark.y, mark.z);

            p.DoBlockchangeCallback((ushort)mark.x, (ushort)mark.y, (ushort)mark.z, block);
        }
    }
}