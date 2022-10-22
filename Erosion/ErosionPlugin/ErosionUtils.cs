namespace PluginErosion;

using MCGalaxy.Maths;
using System;
using System.Collections.Generic;

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