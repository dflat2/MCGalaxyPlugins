namespace ErosionPlugin {
using MCGalaxy.Maths;
using MCGalaxy.Drawing.Ops;
using System;
using BlockID = System.UInt16;
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;
using MCGalaxy;
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;

public enum ErodeMode
{
    Normal,
    Natural,
    X_2D,
    Y_2D,
    Z_2D
}

internal enum Axis
{
    X,
    Y,
    Z
}


internal static class ErosionUtils
{
    internal static bool OnBounds(Vec3S32 target, Vec3S32 min, Vec3S32 max)
    {
        bool onXBound = (target.X == min.X || target.X == max.X);
        bool onYBound = (target.Y == min.Y || target.Y == max.Y);
        bool onZBound = (target.Z == min.Z || target.Z == max.Z);
        return onXBound || onYBound || onZBound;
    }

    internal static List<Vec3S32> Neighbors3D(Vec3S32 coordinates)
    {
        List<Vec3S32> neighbors = new List<Vec3S32>();
        neighbors.Add(coordinates + new Vec3S32(-1, 0, 0));
        neighbors.Add(coordinates + new Vec3S32(1, 0, 0));
        neighbors.Add(coordinates + new Vec3S32(0, 1, 0));
        neighbors.Add(coordinates + new Vec3S32(0, -1, 0));
        neighbors.Add(coordinates + new Vec3S32(0, 0, 1));
        neighbors.Add(coordinates + new Vec3S32(0, 0, -1));
        return neighbors;
    }

    internal static List<Vec3S32> Neighbors3DExceptBelow(Vec3S32 coordinates)
    {
        List<Vec3S32> neighbors = new List<Vec3S32>();
        neighbors.Add(coordinates + new Vec3S32(-1, 0, 0));
        neighbors.Add(coordinates + new Vec3S32(1, 0, 0));
        neighbors.Add(coordinates + new Vec3S32(0, 1, 0));
        neighbors.Add(coordinates + new Vec3S32(0, 0, 1));
        neighbors.Add(coordinates + new Vec3S32(0, 0, -1));
        return neighbors;
    }

    internal static List<Vec3S32> Neighbors2D(Vec3S32 coordinates, Axis normalAxis)
    {
        List<Vec3S32> neighbors = new List<Vec3S32>();

        if (normalAxis != Axis.X)
        {
            neighbors.Add(coordinates + new Vec3S32(-1, 0, 0));
            neighbors.Add(coordinates + new Vec3S32(1, 0, 0));
        }

        if (normalAxis != Axis.Y)
        {
            neighbors.Add(coordinates + new Vec3S32(0, 1, 0));
            neighbors.Add(coordinates + new Vec3S32(0, -1, 0));
        }

        if (normalAxis != Axis.Z)
        {
            neighbors.Add(coordinates + new Vec3S32(0, 0, 1));
            neighbors.Add(coordinates + new Vec3S32(0, 0, -1));
        }

        return neighbors;
    }

    internal static Axis NormalAxis(Vec3S32 mark1, Vec3S32 mark2)
    {
        if (mark1.X == mark2.X)
        {
            return Axis.X;
        }
        else if (mark1.Y == mark2.Y)
        {
            return Axis.Y;
        }

        return Axis.Z;
    }

    internal static Vec3S32 ToVec3S32(this Vec3U16 vec)
    {
        return new Vec3S32((int)vec.X, (int)vec.Y, (int)vec.Z);
    }
}

public class CmdErode : DrawCmd
{
    public override string name { get { return "erode"; } }
    public override string shortcut { get { return "er"; } }
    public override string type { get { return "Building"; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

    public override void Help(Player p)
    {
        p.Message("%T/Erode <mode> [block]");
        p.Message("%HReplaces each [block] with air if the ids of its six neighbors aren't also [block].");
        p.Message("%HModes: %fnormal/natural/2d-x/2d-y/2d-z (default normal)");
        p.Message("%H+ %fnatural %Hignores the neighbor below. Looks like gravity erosion.");
        p.Message("%H+ %f2d-x %Hignores x-axis neighbors.");
        p.Message("%H+ %f2d-y %Hignores y-axis neighbors.");
        p.Message("%H+ %f2d-z %Hignores z-axis neighbors.");
    }

    protected override bool DoDraw(Player p, Vec3S32[] marks, object state, BlockID block)
    {
        DrawArgs dArgs = (DrawArgs)state;
        ErodeDrawOp op = (ErodeDrawOp)dArgs.Op;

        op.ErodedBlockID = GetBlock(p, dArgs.Message);
        if (op.ErodedBlockID == Block.Invalid) op.ErodedBlockID = block;

        DrawOpPerformer.Do(dArgs.Op, null, p, marks);
        return true;
    }

    private BlockID GetBlock(Player p, string parameters)
    {
        string[] parts = parameters.SplitSpaces();
        BlockID block;

        switch (parts.Length)
        {
            case 0:
                return Block.Invalid;
            case 1:
                if (IsMode(parts[0]) || parts[0] == "")
                {
                    return Block.Invalid;
                }
                else
                {
                    CommandParser.GetBlock(p, parts[0], out block);
                    return block;
                }
            default:
                CommandParser.GetBlock(p, parts[1], out block);
                return block;
        }
    }

    private bool IsMode(string text)
    {
        string[] modes = { "normal", "natural", "2d-x", "2d-y", "2d-z" };
        return (Array.IndexOf(modes, text.ToLower()) != -1);
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        string[] parts = dArgs.Message.SplitSpaces();

        if (parts.Length == 0) return new ErodeDrawOp(ErodeMode.Normal);

        switch (parts[0].ToLower())
        {
            case "natural":
                return new ErodeDrawOp(ErodeMode.Natural);
            case "2d-x":
                return new ErodeDrawOp(ErodeMode.X_2D);
            case "2d-y":
                return new ErodeDrawOp(ErodeMode.Y_2D);
            case "2d-z":
                return new ErodeDrawOp(ErodeMode.Z_2D);
            default:
                return new ErodeDrawOp(ErodeMode.Normal);
        }
    }

    protected override void GetBrush(DrawArgs dArgs)
    {
        bool messageIsEmpty = dArgs.Message.Length == 0;

        if (messageIsEmpty)
        {
            dArgs.BrushArgs = "";
            return;
        }

        string[] parts = dArgs.Message.SplitSpaces();
        dArgs.BrushArgs = dArgs.Message.Splice(IsMode(parts[0]) ? 1 : 0, 0);
    }
}
public sealed class ErosionPlugin : Plugin
{
    public override string name { get { return "Erosion"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.3"; } }

    public override void Load(bool startup)
    {
        Command.Register(new CmdErode());
        Chat.MessageGlobal("&aErosion And Dilation plugin successfuly loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Command.Unregister(Command.Find("erode"));
        Chat.MessageGlobal("&cErosion And Dilation plugin successfuly unloaded.");
    }
}

public class ErodeDrawOp : DrawOp
{
    public BlockID ErodedBlockID;
    private ErodeMode mode;

    public ErodeDrawOp(ErodeMode mode)
    {
        AffectedByTransform = false;
        this.mode = mode;
    }

    public override string Name { get { return "erode"; } }

    public override long BlocksAffected(Level lvl, Vec3S32[] marks)
    {
        return SizeX * SizeY * SizeZ / 2;
    }

    private bool ShouldErode(Vec3U16 uvector)
    {
        Vec3S32 vector = uvector.ToVec3S32();

        bool outsideMap = !Level.IsValidPos(vector.X, vector.Y, vector.Z);
        if (outsideMap) { return false; }

        bool isBlockID = (Level.FastGetBlock(uvector.X, uvector.Y, uvector.Z) == ErodedBlockID);
        if (!isBlockID) { return false; }

        List<Vec3S32> neighbors = new List<Vec3S32>();

        switch (mode)
        {
            case ErodeMode.Normal:
                neighbors = ErosionUtils.Neighbors3D(vector);
                break;
            case ErodeMode.Natural:
                neighbors = ErosionUtils.Neighbors3DExceptBelow(vector);
                break;
            case ErodeMode.X_2D:
                neighbors = ErosionUtils.Neighbors2D(vector, Axis.X);
                break;
            case ErodeMode.Y_2D:
                neighbors = ErosionUtils.Neighbors2D(vector, Axis.Y);
                break;
            case ErodeMode.Z_2D:
                neighbors = ErosionUtils.Neighbors2D(vector, Axis.Z);
                break;
        }

        BlockID neighborBlockID;
        Vec3U16 uneighbor;
        foreach (Vec3S32 neighbor in neighbors)
        {
            uneighbor = Clamp(neighbor);
            if (!Level.IsValidPos(uneighbor)) { return true; }

            neighborBlockID = Level.FastGetBlock(uneighbor.X, uneighbor.Y, uneighbor.Z);
            if (neighborBlockID != ErodedBlockID) { return true; }
        }

        return false;
    }

    public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output)
    {
        List<Vec3U16> erosionList = new List<Vec3U16>();
        Vec3U16 uMark1 = Clamp(Min);
        Vec3U16 uMark2 = Clamp(Max);
        Vec3U16 current;

        for (ushort x = uMark1.X; x <= uMark2.X; x++)
            for (ushort y = uMark1.Y; y <= uMark2.Y; y++)
                for (ushort z = uMark1.Z; z <= uMark2.Z; z++)
                {
                    current = new Vec3U16(x, y, z);
                    if (ShouldErode(current)) { erosionList.Add(current); }
                }

        foreach (Vec3U16 vector in erosionList)
        {
            output(Place(vector.X, vector.Y, vector.Z, 0));
        }
    }
}}