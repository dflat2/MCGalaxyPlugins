using System;
using MCGalaxy;
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Maths;
using MCGalaxy.Util;
using BlockID = System.UInt16;

namespace PortalToolsPlugin;

public class CmdReplacePortal : Command
{
    public override string name => "ReplacePortal";
    public override bool SuperUseable => false;
    public override bool museumUsable => false;
    public override LevelPermission defaultRank => LevelPermission.AdvBuilder;
    public override string type => CommandTypes.Building;

    public override void Use(Player player, string message)
    {
        string[] arguments = message.SplitSpaces();

        if (arguments.Length != 2)
        {
            ShowUsage(player);
            return;
        }

        if (!CanUsePortal(player))
        {
            player.Message("Cannot use &T/ReplacePortal &Sas you don't have the permission to use &T/Portal&S.");
            return;
        }

        BlockID blockToReplace = Block.Invalid;
        BlockID portalBlock = Block.Invalid;

        if (!TryGetBlockToReplace(player, arguments[0], out blockToReplace))
        {
            return;
        }

        if (!TryGetPortalBlock(player, arguments[1], out portalBlock))
        {
            return;
        }

        var cmdReplacePortalArguments = new CmdReplacePortalArguments(blockToReplace, portalBlock);

        player.Message("Place or break two blocks to determine the edges.");
        player.MakeSelection(2, "Selecting region for &SReplacePortal", cmdReplacePortalArguments, DoPortalReplace);
    }

    public override void Help(Player player)
    {
        player.Message("&T/ReplacePortal <block> <portal block>");
        player.Message("&HReplaces &T<block> &Hwith a portal between two points.");
    }

    private bool TryGetBlockToReplace(Player player, string blockName, out BlockID block)
    {
        block = Block.Parse(player, blockName);

        if (block == Block.Invalid)
        {
            return false;
        }

        return true;
    }

    private bool TryGetPortalBlock(Player player, string blockName, out BlockID block)
    {
        block = Block.Parse(player, blockName);

        if (block == Block.Invalid || !player.level.Props[block].IsPortal)
        {
            return false;
        }

        return true;
    }

    private bool DoPortalReplace(Player player, Vec3S32[] marks, object state, ushort block)
    {
        if (!player.level.Config.Drawing)
        {
            player.Message("Cannot use &T/ReplacePortal &Sas drawing commands are disabled on this map.");
            return false;
        }

        throw new NotImplementedException();
    }

    private bool CanUsePortal(Player p)
    {
        return p.CanUse(Find("Portal"));
    }

    private void ShowUsage(Player player)
    {
        player.Message("Usage: &T/ReplacePortal <block> <portal block>&S.");
    }

    private void SetPortal(string map, Vec3U16[] coordinates, Vec3U16 destination, string mapDestination)
    {
        foreach (Vec3U16 coordinate in coordinates)
        {
            object locker = ThreadSafeCache.DBCache.GetLocker(map);

            lock (locker)
            {
                Portal.Set(map, coordinate.X, coordinate.Y, coordinate.Z,
                    destination.X, destination.Y, destination.Z, mapDestination);
            }
        }
    }
}