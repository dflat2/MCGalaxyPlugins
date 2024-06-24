namespace PluginErosion;
using System;
using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

public class CmdErode : DrawCmd {
    public override string name { get { return "erode"; } }
    public override string shortcut { get { return "er"; } }
    public override string type { get { return "Building"; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

    public override void Help(Player player) {
        player.Message("%T/Erode <mode> [block]");
        player.Message("%HReplaces each [block] with air if the ids of its six neighbors aren't also [block].");
        player.Message("%HModes: %fnormal/natural/2d-x/2d-y/2d-z (default normal)");
        player.Message("%H+ %fnatural %Hignores the neighbor below. Looks like gravity erosion.");
        player.Message("%H+ %f2d-x %Hignores x-axis neighbors.");
        player.Message("%H+ %f2d-y %Hignores y-axis neighbors.");
        player.Message("%H+ %f2d-z %Hignores z-axis neighbors.");
    }

    protected override bool DoDraw(Player player, Vec3S32[] marks, object state, BlockID block) {
        DrawArgs dArgs = (DrawArgs)state;
        ErodeDrawOp op = (ErodeDrawOp)dArgs.Op;

        op.ErodedBlockID = GetBlock(player, dArgs.Message);

        if (op.ErodedBlockID == Block.Invalid) {
            op.ErodedBlockID = block;
        }

        DrawOpPerformer.Do(dArgs.Op, null, player, marks);
        return true;
    }

    private BlockID GetBlock(Player player, string parameters) {
        string[] parts = parameters.SplitSpaces();
        BlockID block;

        switch (parts.Length) {
            case 0:
                return Block.Invalid;
            case 1:
                if (IsMode(parts[0]) || parts[0].Equals(string.Empty)) {
                    return Block.Invalid;
                }
                else {
                    CommandParser.GetBlock(player, parts[0], out block);
                    return block;
                }
            default:
                CommandParser.GetBlock(player, parts[1], out block);
                return block;
        }
    }

    private bool IsMode(string text) {
        string[] modes = { "normal", "natural", "2d-x", "2d-y", "2d-z" };
        return (Array.IndexOf(modes, text.ToLower()) != -1);
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs) {
        string[] parts = dArgs.Message.SplitSpaces();

        if (parts.Length == 0) {
            return new ErodeDrawOp(ErodeMode.Normal);
        }

        switch (parts[0].ToLower()) {
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

    protected override void GetBrush(DrawArgs dArgs) {
        bool messageIsEmpty = dArgs.Message.Length == 0;

        if (messageIsEmpty) {
            dArgs.BrushArgs = "";
            return;
        }

        string[] parts = dArgs.Message.SplitSpaces();
        dArgs.BrushArgs = dArgs.Message.Splice(IsMode(parts[0]) ? 1 : 0, 0);
    }
}