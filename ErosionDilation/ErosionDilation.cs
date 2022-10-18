using System;
using MCGalaxy;
using MCGalaxy.Commands;

namespace ErosionDilation
{
    public sealed class ErosionDilation : Plugin
    {
        public override string name { get { return "Erosion And Dilation"; } }
        public override string creator { get { return "D_Flat"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.3"; } }

        public override void Load(bool startup)
        {
            Command.Register(new CmdErode());
            Command.Register(new CmdErode2D());
            Command.Register(new CmdDilate());
            Command.Register(new CmdDilate2D());
            Chat.MessageGlobal("&aErosion And Dilation plugin successfuly loaded.");
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("erode"));
            Command.Unregister(Command.Find("erode2d"));
            Command.Unregister(Command.Find("dilate"));
            Command.Unregister(Command.Find("dilate2d"));
            Chat.MessageGlobal("&cErosion And Dilation plugin successfuly unloaded.");
        }
    }

    public class CmdErode : Command
    {
        public override string name { get { return "erode"; } }
        public override string shortcut { get { return "er"; } }
        public override string type { get { return "Building"; } }

        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string args)
        {
            // TODO
            p.Message("%cNot implemented yet.");
        }

        public override void Help(Player p)
        {
            p.Message("%T/Erode [block]");
            p.Message("%HShrinks structures made of [block].");
            p.Message("%HIf no block is given, erodes your held block.");
            p.Message("%T/Erode <amount> [block]");
            p.Message("%HRun the shrink <amount> times.");
        }
    }

    public class CmdErode2D : Command
    {
        public override string name { get { return "erode2d"; } }
        public override string shortcut { get { return "er2d"; } }
        public override string type { get { return "Building"; } }

        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string args)
        {
            // TODO
            p.Message("%cNot implemented yet.");
        }

        public override void Help(Player p)
        {
            p.Message("%T/Erode2D <amount> [block]");
            p.Message("%HSame as %T/Erode %Hbut for plane selections.");
        }
    }

    public class CmdDilate : Command
    {
        public override string name { get { return "dilate"; } }
        public override string shortcut { get { return "di"; } }
        public override string type { get { return "Building"; } }

        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string args)
        {
            // TODO
            p.Message("%cNot implemented yet.");
        }

        public override void Help(Player p)
        {
            p.Message("%T/Dilate [block]");
            p.Message("%HDilates structures made of [block].");
            p.Message("%HIf no block is given, dilates your held block.");
            p.Message("%T/Dilate <amount> [block]");
            p.Message("%HRun the dilation <amount> times.");
        }
    }

    public class CmdDilate2D : Command
    {
        public override string name { get { return "dilate2d"; } }
        public override string shortcut { get { return "di2d"; } }
        public override string type { get { return "Building"; } }

        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string args)
        {
            // TODO
            p.Message("%cNot implemented yet.");
        }

        public override void Help(Player p)
        {
            p.Message("%T/Dilate2D <amount> [block]");
            p.Message("%HSame as %T/Dilate %Hbut for plane selections.");
        }
    }
}
