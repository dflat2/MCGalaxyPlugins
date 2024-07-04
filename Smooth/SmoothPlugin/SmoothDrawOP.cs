namespace PluginSmooth;
using System;
using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

public class SmoothDrawOP : DrawOp
{
    public override string Name { get { return "Smooth"; } }

    private readonly BlockID _block;
    private readonly int _radius;
    private readonly int _intensity;
    private int _CubeSide { get { return _radius * 2 + 1; } }

    public SmoothDrawOP(BlockID block, int radius, int intensity) {
        AffectedByTransform = false;
        _block = block;
        _radius = radius;
        _intensity = intensity;
    }

    public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
        return (long)Math.Pow(_CubeSide, 3);
    }

    private int _Clamp(int value, int min, int max) {
        if (value <= min) {
            return min;
        } else if (value >= max) {
            return max;
        }

        return value;
    }


    private int _CountBlocksAroundXY(int x, int y, int z) {
        if (z < 0 || z > Level.MaxZ) {
            return 0;
        }

        int total = 0;
        int xMin = x - _intensity;
        int yMin = y - _intensity;
        int xMax = x + _intensity;
        int yMax = y + _intensity;

        for (int i = xMin; i <= xMax; i++) {
            for (int j = yMin; j <= yMax; j++) {
                if (!Level.IsValidPos(i, j, z)) {
                    continue;
                } else if (Level.FastGetBlock((ushort)i, (ushort)j, (ushort)z) == _block) {
                    total++;
                }
            }
        }

        return total;
    }

    private int _CountBlocksAround(int x, int y, int z) {
        int total = 0;
        int xMin = x - _intensity;
        int yMin = y - _intensity;
        int zMin = z - _intensity;
        int xMax = x + _intensity;
        int yMax = y + _intensity;
        int zMax = z + _intensity;

        for (int i = xMin; i <= xMax; i++) {
            for (int j = yMin; j <= yMax; j++) {
                for (int k = zMin; k <= zMax; k++) {
                    if (!Level.IsValidPos(i, j, k)) {
                        continue;
                    } else if (Level.FastGetBlock((ushort)i, (ushort)j, (ushort)k) == _block) {
                        total++;
                    }
                }
            }
        }

        return total;
    }

    public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
        int threshold = (int)(Math.Pow((_intensity * 2) + 1, 3) / 2);
        Queue<Vec3U16> shouldAdd = new Queue<Vec3U16>();
        Queue<Vec3U16> shouldRemove = new Queue<Vec3U16>();
        Vec3S32 center = marks[0];
        int blocksAround;

        int xMin = _Clamp(center.X - _radius, 0, Level.MaxX);
        int yMin = _Clamp(center.Y - _radius, 0, Level.MaxY);
        int zMin = _Clamp(center.Z - _radius, 0, Level.MaxZ);
        int xMax = _Clamp(center.X + _radius, 0, Level.MaxX);
        int yMax = _Clamp(center.Y + _radius, 0, Level.MaxY);
        int zMax = _Clamp(center.Z + _radius, 0, Level.MaxZ);

        for (ushort x = (ushort)xMin; x <= xMax; x++) {
            for (ushort y = (ushort)yMin; y <= yMax; y++) {
                // Optimization: (1) only count blocks in volume once per horizontal column.
                blocksAround = _CountBlocksAround(x, y, zMin);

                for (ushort z = (ushort)zMin; z <= zMax; z++) {
                    if (Level.FastGetBlock(x, y, z) == Block.Air && blocksAround > threshold) {
                        shouldAdd.Enqueue(new Vec3U16(x, y, z));
                    } else if (Level.FastGetBlock(x, y, z) == _block && blocksAround <= threshold) {
                        shouldRemove.Enqueue(new Vec3U16(x, y, z));
                    }

                    // (2) And guess the number of blocks in subsequent volumes by calculating the differences on opposite faces.
                    blocksAround = blocksAround - _CountBlocksAroundXY(x, y, z - _intensity) + _CountBlocksAroundXY(x, y, z + _intensity + 1);
                }
            }
        }
        Vec3U16 vector;

        while (shouldAdd.Count != 0) {
            vector = shouldAdd.Dequeue();
            output(Place(vector.X, vector.Y, vector.Z, _block));
        }

        while (shouldRemove.Count != 0) {
            vector = shouldRemove.Dequeue();
            output(Place(vector.X, vector.Y, vector.Z, 0));
        }
    }
}