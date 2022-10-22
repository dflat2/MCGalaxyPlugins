namespace PluginErosion;

using BlockID = System.UInt16;
using MCGalaxy.Commands.Building;
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;
using MCGalaxy;
using System;
using System.Collections.Generic;

public sealed class ErosionPlugin : Plugin
{
    public override string name { get { return "Erosion"; } }
    public override string creator { get { return "D_Flat"; } }
    public override string MCGalaxy_Version { get { return "1.9.4.3"; } }

    public override void Load(bool startup)
    {
        Command.Register(new CmdErode());
        Command.Register(new CmdErode2D());
        Chat.MessageGlobal("&aErosion And Dilation plugin successfuly loaded.");
    }

    public override void Unload(bool shutdown)
    {
        Command.Unregister(Command.Find("erode"));
        Command.Unregister(Command.Find("erode2d"));
        Chat.MessageGlobal("&cErosion And Dilation plugin successfuly unloaded.");
    }
}