using System;
using Xunit;
using MCGalaxy;
using PluginErosion;

namespace ErosionPlugin.Test;

public class CmdErodeTest
{
    [Fact]
    public void foo()
    {
        Player p = new Player("Joe");
        string message = "2d-y stone";
        Command cmdErode = new CmdErode();
        cmdErode.Use(p, message);
    }
}