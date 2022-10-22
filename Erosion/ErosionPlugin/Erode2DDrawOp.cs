namespace PluginErosion;

using System;
using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

// TODO: Refactor Erode2DDrawOp and ErodeDrawOp into a single class
public class Erode2DDrawOp : DrawOp
{
    public BlockID ErodedBlockID;

    public Erode2DDrawOp()
    {
        AffectedByTransform = false;
    }

    public override string Name { get { return "erode"; } }

    public override long BlocksAffected(Level lvl, Vec3S32[] marks)
    {
        return SizeX * SizeY * SizeZ / 2;
    }

    private bool ShouldErode(Vec3U16 uvector, Axis axis)
    {
        Vec3S32 vector = uvector.ToVec3S32();

        bool outsideMap = !Level.IsValidPos(vector.X, vector.Y, vector.Z);
        if (outsideMap) { return false; }

        bool isBlockID = (Level.FastGetBlock(uvector.X, uvector.Y, uvector.Z) == ErodedBlockID);
        if (!isBlockID) { return false; }

        List<Vec3S32> neighbors = ErosionUtils.Neighbors2D(vector, axis);

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
        Axis axis = ErosionUtils.NormalAxis(Min, Max);

        for (ushort x = uMark1.X; x <= uMark2.X; x++)
            for (ushort y = uMark1.Y; y <= uMark2.Y; y++)
                for (ushort z = uMark1.Z; z <= uMark2.Z; z++)
                {
                    current = new Vec3U16(x, y, z);
                    if (ShouldErode(current, axis)) { erosionList.Add(current); }
                }

        foreach (Vec3U16 vector in erosionList)
        {
            output(Place(vector.X, vector.Y, vector.Z, 0));
        }
    }
}