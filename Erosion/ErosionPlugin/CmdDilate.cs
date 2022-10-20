namespace PluginErosion;

using System;
using MCGalaxy;
using MCGalaxy.Commands.Building;
using MCGalaxy.Drawing.Ops;

public class CmdDilate : DrawCmd
{
    public override string name { get { return "dilate"; } }
    public override string shortcut { get { return "di"; } }
    public override string type { get { return "Building"; } }

    public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

    public override void Help(Player p)
    {
        p.Message("%T/Dilate [block]");
        p.Message("%HDilates structures made of [block].");
        p.Message("%HIf no block is given, dilates your held block.");
    }

    protected override DrawOp GetDrawOp(DrawArgs dArgs)
    {
        throw new NotImplementedException();
    }
}

