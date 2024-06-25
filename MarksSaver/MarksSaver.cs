using System.Collections.Generic;
using BlockID = System.UInt16;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy;
using MCGalaxy.Commands;

namespace MarksSaverPlugin
{
    public static class SavedMarks {
        private static Dictionary<Player, Vec3S32[]> table = new Dictionary<Player, Vec3S32[]>();

        internal static Vec3S32[] Get(Player p) {
            if (table.ContainsKey(p)) {
                return table[p];
            }

            return new List<Vec3S32>().ToArray();
        }

        internal static void Set(Player p, Vec3S32[] marks) {
            if (!table.ContainsKey(p)) {
                OnPlayerDisconnectEvent.Register(SavedMarks.OnPlayerDisconnect, Priority.Normal);
            }

            table[p] = marks;
        }

        public static void OnPlayerDisconnect(Player p, string reason) {
            if (table.ContainsKey(p)) {
                table.Remove(p);
            }
        }
    }

    public static class SaveMarksUtils {
        public static string ToStringNoComma(this Vec3S32 vector) {
            return $"{vector.X} {vector.Y} {vector.Z}";
        }
    }
    public class CmdLoadMarks : Command {
        public override string name { get { return "LoadMarks"; } }
        public override string shortcut { get { return "LM"; } }
        public override string type { get { return "building"; } }

        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }

        public override void Help(Player player) {
            player.Message("&T/LoadMarks");
            player.Message("&HMarks what's been saved with &T/SaveMarks&S.");
            player.Message("&HSee also: &T/savemarks&S.");
        }

        public override void Use(Player player, string message) {
            if (message.Length != 0) {
                player.Message("&HUsage: &T/loadmarks");
                return;
            }

            Vec3S32[] marks = SavedMarks.Get(player);

            if (marks.Length == 0) {
                player.Message("&HYou did not save any marks yet. Run &T/SaveMarks &Hfirst.");
                return;
            }

            Command cmdMark = Command.Find("mark");

            foreach (Vec3S32 mark in marks) {
                cmdMark.Use(player, mark.ToStringNoComma());
            }
        }
    }
    public sealed class CmdSaveMarks : Command
    {
        public override string name { get { return "SaveMarks"; } }
        public override string shortcut { get { return "SM"; } }
        public override string type { get { return "building"; } }

        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }

        private int MarksCount;

        public override void Help(Player player) {
            player.Message("&T/SaveMarks [count]");
            player.Message("&HPrompt for marks and save them for later use.");
            player.Message("&Tcount &Hdefaults to 2 if not given.");
            player.Message("&HSee also: &H/LoadMarks&H.");
        }

        public override void Use(Player player, string message) {
            if (!ParseArguments(player, message)) {
                player.Message("&HUsage: &T/SaveMarks [count] &H where %Tcount %His 1, 2 or 3.");
                return;
            }

            bool plural = (MarksCount >= 2);

            if (plural) {
                player.Message($"Place or break {MarksCount} blocks to determine the edges.");
                player.MakeSelection(MarksCount, $"Selecting {MarksCount} blocks for &S/savemarks", null, DoSaveMarks);
            } else {
                player.Message("Place or break a block.");
                player.MakeSelection(MarksCount, "Selecting a block for &S/savemarks", null, DoSaveMarks);
            }
        }

        private bool ParseArguments(Player player, string arguments) {
            string[] parts = arguments.SplitSpaces();

            if (parts.Length == 0 || arguments.Equals(string.Empty)) {
                MarksCount = 2;
                return true;
            } else if (parts.Length == 1) {
                int marksCount = 2;

                if (CommandParser.GetInt(player, parts[0], "Marks count", ref marksCount, 1, 3)) {
                    MarksCount = marksCount;
                    return true;
                } else {
                    return false;
                }
            }

            return false;
        }

        public bool DoSaveMarks(Player player, Vec3S32[] marks, object state, BlockID block) {
            if (marks == null) {
                return true;
            }

            SavedMarks.Set(player, marks);
            return true;
        }
    }
    public sealed class MarksSaverPlugin : Plugin {
        public override string name { get { return "MarksSaver"; } }
        public override string creator { get { return "D_Flat"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.9"; } }

        public override void Load(bool startup) {
            Command.Register(new CmdSaveMarks());
            Command.Register(new CmdLoadMarks());
        }

        public override void Unload(bool shutdown) {
            Command.Unregister(Command.Find("SaveMarks"));
            Command.Unregister(Command.Find("LoadMarks"));
        }

        public override void Help(Player player) {
            Command cmdHelp = Command.Find("help");
            cmdHelp.Use(player, "savemarks");
        }
    }
}