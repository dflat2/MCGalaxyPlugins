using System;
using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;
using BlockID = System.UInt16;

public class CmdSmooth : Command {
    public override string name { get { return "Smooth"; } }
    public override string type { get { return CommandTypes.Building; } }
    public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

    private bool _TryParseArguments(Player player, string[] arguments, ref BlockID block, ref int radius, ref int intensity) {
        if (arguments.Length < 2 || arguments.Length > 3) {
            player.Message("&SUsage: &T/Smooth <block> <radius> [intensity]&S.");
            return false;
        }

        bool hasIntensity = arguments.Length == 3;

        if (!CommandParser.GetBlock(player, arguments[0], out block)) {
            return false;
        } else if (!CommandParser.GetInt(player, arguments[1], "Radius", ref radius, 2, 10)) {
            return false;
        } else if (hasIntensity && !CommandParser.GetInt(player, arguments[2], "Intensity", ref intensity, 1)) {
            return false;
        } else if (!hasIntensity) {
            intensity = (int)Math.Floor((double)(radius / 2));
        }

        if (intensity * 2 > radius) {
            player.Message("&cIntensity must be half of the radius (or less)");
            return false;
        }

        return true;
    }

    public override void Use(Player player, string message) {
        string[] arguments = message.SplitSpaces();

        BlockID block = Block.Invalid;
        int radius = 0;
        int intensity = 0;

        if (!_TryParseArguments(player, arguments, ref block, ref radius, ref intensity)) {
            return;
        }

        var smoothArguments = new SmoothArguments(block, radius, intensity);

        player.Message("Place or break a block to indicate location.");
        player.MakeSelection(1, "Selecting location for &TSmooth&S.", smoothArguments, _DoDraw);
    }

    private bool _DoDraw(Player player, Vec3S32[] marks, object state, ushort block) {
        if (marks == null || marks.Length != 1) {
            return false;
        }

        SmoothArguments arguments = (SmoothArguments)state;
        return DrawOpPerformer.Do(new SmoothDrawOP(arguments.Block, arguments.Radius, arguments.Intensity), null, player, marks);
    }

    public override void Help(Player player) {
        player.Message("&T/Smooth <block> <radius> [intensity]");
        player.Message("&HSmooths &Tblock &Hwithin &Tradius &H(recommended &T6&H). Low");
        player.Message("&Tintensity &H(default &TFloor(radius / 2&H) means sharp edges.");
        player.Message("&HThe &Tintensity &Hmust be half of the &Tradius &H(or less).");
    }
}

public class SmoothArguments {
    public BlockID Block { get; private set; }
    public int Radius { get; private set; }
    public int Intensity { get; private set; }

    public SmoothArguments(BlockID block, int radius, int intensity) {
        Block = block;
        Radius = radius;
        Intensity = intensity;
    }
}

public class SmoothDrawOP : DrawOp {
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
