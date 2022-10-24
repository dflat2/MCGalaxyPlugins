using System.Collections.Generic;
using MCGalaxy.Commands;
using MCGalaxy.Maths;
using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;
using BlockID = System.UInt16;

namespace MarksSaverPlugin {
	
	public static class SavedMarks
	{
	    private static Dictionary<Player, Vec3S32[]> table = new Dictionary<Player, Vec3S32[]>();
	
	    internal static Vec3S32[] Get(Player p)
	    {
	        if (table.ContainsKey(p)) return table[p];
	        return new List<Vec3S32>().ToArray();
	    }
	
	    internal static void Set(Player p, Vec3S32[] marks)
	    {
	        if (!table.ContainsKey(p))
	            OnPlayerDisconnectEvent.Register(SavedMarks.OnPlayerDisconnect, Priority.Normal);
	
	        table[p] = marks;
	    }
	
	    public static void OnPlayerDisconnect(Player p, string reason)
	    {
	        if (table.ContainsKey(p))
	            table.Remove(p);
	    }
	}
	
	public static class SaveMarksUtils
	{
	    public static string ToStringNoComma(this Vec3S32 vector)
	    {
	        return $"{vector.X} {vector.Y} {vector.Z}";
	    }
	}	
	public class CmdLoadMarks : Command
	{
	    public override string name => "loadmarks";
	    public override string shortcut => "lm";
	    public override string type => "building";
	
	    public override bool museumUsable => false;
	    public override bool SuperUseable => false;
	
	    public override void Help(Player p)
	    {
	        p.Message("&T/loadmarks");
	        p.Message("&HMarks what's been saved with &T/savemarks");
	        p.Message("&HSee also: &T/savemarks");
	    }
	
	    public override void Use(Player p, string message)
	    {
	        if (message.Length != 0)
	        {
	            p.Message("&HUsage: &T/loadmarks");
	            return;
	        }
	
	        Vec3S32[] marks = SavedMarks.Get(p);
	
	        if (marks.Length == 0)
	        {
	            p.Message("&HYou did not save any marks yet. Run &T/savemarks &Hfirst.");
	            return;
	        }
	
	        Command cmdMark = Command.Find("mark");
	
	        foreach (Vec3S32 mark in marks)
	        {
	            cmdMark.Use(p, mark.ToStringNoComma());
	        }
	    }
	}	
	public sealed class CmdSaveMarks : Command
	{
	    public override string name => "savemarks";
	    public override string shortcut => "sm";
	    public override string type => "building";
	
	    public override bool museumUsable => false;
	    public override bool SuperUseable => false;
	
	    private int MarksCount;
	
	    public override void Help(Player p)
	    {
	        p.Message("&T/savemarks <count>");
	        p.Message("&HPrompt for marks and save them for later use.");
	        p.Message("&T<count> &Hdefaults to 2 if not given.");
	        p.Message("&HSee also: &H/loadmarks");
	    }
	
	    public override void Use(Player p, string message)
	    {
	        if (!ParseArguments(p, message))
	        {
	            p.Message("&HUsage: &T/savemarks <count> &H where %T<count> %His 1, 2 or 3.");
	            return;
	        }
	
	        bool plural = (MarksCount >= 2);
	
	        if (plural)
	        {
	            p.Message($"Place or break {MarksCount} blocks to determine the edges.");
	            p.MakeSelection(MarksCount, $"Selecting {MarksCount} blocks for &S/savemarks", null, DoSaveMarks);
	        }
	        else
	        {
	            p.Message("Place or break a block.");
	            p.MakeSelection(MarksCount, "Selecting a block for &S/savemarks", null, DoSaveMarks);
	        }
	    }
	
	    private bool ParseArguments(Player p, string arguments)
	    {
	        string[] parts = arguments.SplitSpaces();
	
	        if (parts.Length == 0 || arguments == "")
	        {
	            MarksCount = 2;
	            return true;
	        }
	        else if (parts.Length == 1)
	        {
	            int marksCount = 2;
	            if (CommandParser.GetInt(p, parts[0], "Marks count", ref marksCount, 1, 3))
	            {
	                MarksCount = marksCount;
	                return true;
	            }
	            else
	            {
	                return false;
	            }
	        }
	
	        return false;
	    }
	
	    public bool DoSaveMarks(Player p, Vec3S32[] marks, object state, BlockID block)
	    {
	        if (marks == null) return true;
	        SavedMarks.Set(p, marks);
	        return true;
	    }
	}	
	public sealed class MarksSaverPlugin : Plugin
	{
	    public override string name { get { return "MarksSaver"; } }
	    public override string creator { get { return "D_Flat"; } }
	    public override string MCGalaxy_Version { get { return "1.9.4.3"; } }
	
	    public override void Load(bool startup)
	    {
	        Command.Register(new CmdSaveMarks());
	        Command.Register(new CmdLoadMarks());
	        Chat.MessageGlobal("&aSaveMarks plugin successfuly loaded.");
	    }
	
	    public override void Unload(bool shutdown)
	    {
	        Command.Unregister(Command.Find("savemarks"));
	        Command.Unregister(Command.Find("loadmarks"));
	        Chat.MessageGlobal("&cSaveMarks plugin successfuly unloaded.");
	    }
	
	    public override void Help(Player p)
	    {
	        Command cmdHelp = Command.Find("help");
	        cmdHelp.Use(p, "savemarks");
	    }
	}
}