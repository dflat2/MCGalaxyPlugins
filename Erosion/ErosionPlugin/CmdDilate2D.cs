namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;

public class CmdDilate2D : DrawCmd
{
    public override string name { get { return "dilate2d"; } }
    public override string shortcut { get { return "di2d"; } }
    public override string type { get { return "Building"; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

    public override void Help(Player p)
    {
        p.Message("%T/Dilate2D <amount> [block]");
        p.Message("%HSame as %T/Dilate %Hbut for plane selections.");
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        throw new NotImplementedException();
    }
}

