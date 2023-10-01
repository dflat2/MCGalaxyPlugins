using System.Collections.Generic;
using MCGalaxy;

namespace PluginCommandsUnloader {
    public class CommandsUnloader : Plugin
    {
        public override string name => "CommandsUnloader";
        public override string creator => "D_Flat";
        public override string MCGalaxy_Version => "1.9.4.9";

        private List<Command> m_unloadedCommands = new List<Command>();

        public override void Load(bool auto)
        {
            m_unloadedCommands = new List<Command>();
            List<Command> allCommands = Command.CopyAll();

            foreach (Command command in allCommands)
            {
                UnloadIfNobodyCanUse(command);
            }
        }

        public override void Unload(bool shuttingDown)
        {
            if (shuttingDown) return;

            foreach (Command command in m_unloadedCommands)
            {
                Command.Register(command);
            }

            m_unloadedCommands.Clear();
        }

        public override void Help(Player player)
        {
            player.Message("&SCommandsUnloader");
            player.Message("&HUnloads all commands with permission 120.");
        }

        private void UnloadIfNobodyCanUse(Command command)
        {
            if (command.Permissions.MinRank == LevelPermission.Nobody)
            {
                Command.Unregister(command);
                m_unloadedCommands.Add(command);
            }
        }
    }
}
