namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt32;

public class CmdErode2D : DrawCmd
{
    public override string name { get { return "erode2d"; } }
    public override string shortcut { get { return "er2d"; } }
    public override string type { get { return "Building"; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

    public override void Help(Player p)
    {
        p.Message("%T/Erode2D <amount> [block]");
        p.Message("%HSame as %T/Erode %Hbut for plane selections.");
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        throw new NotImplementedException();
    }
}
