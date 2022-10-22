namespace ErosionPlugin.Test;

using System;
using System.Collections.Generic;
using Xunit;
using PluginErosion;
using MCGalaxy.Maths;

public class ErosionUtilsTest
{
    [Fact]
    public void OnBounds_ShouldReturnTrueIfOnBounds()
    {
        bool expected = true;

        Vec3S32 target, mark1, mark2;
        mark1 = new Vec3S32(0, 1, 2);
        mark2 = new Vec3S32(10, 9, 8);
        target = new Vec3S32(0, 5, 5);

        bool actual = ErosionUtils.OnBounds(target, mark1, mark2);
        Assert.Equal(expected, actual);        
    }

    [Fact]
    public void OnBounds_ShouldReturnFalseIfNotOnBounds()
    {
        bool expected = false;

        Vec3S32 target, mark1, mark2;
        mark1 = new Vec3S32(0, 1, 2);
        mark2 = new Vec3S32(10, 9, 8);
        target = new Vec3S32(5, 5, 5);

        bool actual = ErosionUtils.OnBounds(target, mark1, mark2);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Neighbors3D_SimpleCoordinates()
    {
        Vec3S32 coordinates = new Vec3S32(5, -5, 5);
        List<Vec3S32> result = ErosionUtils.Neighbors3D(coordinates);

        Assert.Contains(new Vec3S32(4, -5, 5), result);
        Assert.Contains(new Vec3S32(6, -5, 5), result);
        Assert.Contains(new Vec3S32(5, -6, 5), result);
        Assert.Contains(new Vec3S32(5, -4, 5), result);
        Assert.Contains(new Vec3S32(5, -5, 4), result);
        Assert.Contains(new Vec3S32(5, -5, 6), result);
        Assert.Equal(6, result.Count);
    }

    [Fact]
    public void NormalAxis_ShouldReturnNormalAxisX()
    {
        Axis expected = Axis.X;

        Vec3S32 mark1 = new Vec3S32(-25, 32, 8);
        Vec3S32 mark2 = new Vec3S32(-25, 80, -40);
        Axis result = ErosionUtils.NormalAxis(mark1, mark2);
        Assert.Equal<Axis>(expected, result);
    }

    [Fact]
    public void NormalAxis_ShouldReturnNormalAxisY()
    {
        Axis expected = Axis.Y;

        Vec3S32 mark1 = new Vec3S32(-25, 32, -88);
        Vec3S32 mark2 = new Vec3S32(-26, 32, -40);
        Axis result = ErosionUtils.NormalAxis(mark1, mark2);
        Assert.Equal<Axis>(expected, result);
    }

    [Fact]
    public void Neighbors2D_OnZPlane()
    {
        Vec3S32 coordinates = new Vec3S32(-32, 20, 8);
        Axis axis = Axis.Z;
        List<Vec3S32> result = ErosionUtils.Neighbors2D(coordinates, axis);

        Assert.Contains(new Vec3S32(-31, 20, 8), result);
        Assert.Contains(new Vec3S32(-33, 20, 8), result);
        Assert.Contains(new Vec3S32(-32, 19, 8), result);
        Assert.Contains(new Vec3S32(-32, 21, 8), result);
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Neighbors2D_OnYPlane()
    {
        Vec3S32 coordinates = new Vec3S32(-32, 20, 8);
        Axis axis = Axis.Y;
        List<Vec3S32> result = ErosionUtils.Neighbors2D(coordinates, axis);

        Assert.Contains(new Vec3S32(-31, 20, 8), result);
        Assert.Contains(new Vec3S32(-33, 20, 8), result);
        Assert.Contains(new Vec3S32(-32, 20, 7), result);
        Assert.Contains(new Vec3S32(-32, 20, 9), result);
        Assert.Equal(4, result.Count);
    }
}
