namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Commands.Building;
using MCGalaxy.DB;
using MCGalaxy.Drawing.Brushes;
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

        ErodeDrawOp op = (ErodeDrawOp)dArgs.Op;
        op.ErodedBlockID = block;
        DrawOpPerformer.Do(dArgs.Op, brush, p, marks);
        return true;
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        return new ErodeDrawOp();
    }
}