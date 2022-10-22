namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

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

    protected override bool DoDraw(Player p, Vec3S32[] marks, object state, BlockID block)
    {
        DrawArgs dArgs = (DrawArgs)state;
        dArgs.Block = block;
        GetMarks(dArgs, ref marks);
        if (marks == null) return false;

        BrushFactory factory = MakeBrush(dArgs);
        BrushArgs bArgs = new BrushArgs(p, dArgs.BrushArgs, dArgs.Block);
        Brush brush = factory.Construct(bArgs);
        if (brush == null) return false;

        Erode2DDrawOp op = (Erode2DDrawOp)dArgs.Op;
        op.ErodedBlockID = block;
        DrawOpPerformer.Do(dArgs.Op, brush, p, marks);
        return true;
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        return new Erode2DDrawOp();
    }
}
