namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

public class CmdErode : DrawCmd
{
    public override string name { get { return "erode"; } }
    public override string shortcut { get { return "er"; } }
    public override string type { get { return "Building"; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

    public override void Help(Player p)
    {
        p.Message("%T/Erode [block]");
        p.Message("%HShrinks structures made of [block].");
        p.Message("%HIf no block is given, erodes your held block.");
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        return new ErodeDrawOp();
    }
}