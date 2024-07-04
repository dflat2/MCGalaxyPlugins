using System;
using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;
namespace PluginSmooth;

public class CmdSmooth : Command {
    public override string name { get { return "Smooth"; } }
    public override string type { get { return CommandTypes.Building; } }

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
        player.MakeSelection(1, "Selecting location for &TSmooth&S.", smoothArguments, DoDraw);
    }

    private bool DoDraw(Player player, Vec3S32[] marks, object state, ushort block) {
        if (marks == null || marks.Length != 1) {
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
