namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

public class ErodeDrawOp : DrawOp
{
    public ErodeDrawOp()
    {
        AffectedByTransform = false;
    }

    public override string Name { get { return "erode"; } }

    public override long BlocksAffected(Level lvl, Vec3S32[] marks)
    {
        return SizeX * SizeY * SizeZ / 2;
    }

    public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output)
    {
        throw new NotImplementedException();
    }
}