namespace PluginEasyFences;
using System;
using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.Blocks;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

internal class FenceElement
{
    private int offset = 0;
    private ElementDirection direction = ElementDirection.X;
    private ElementPosition position = ElementPosition.Top;
    private BlockID copiedTo;
    private ElementType type;

    public BlockID copiedFrom;

    internal FenceElement(ElementType type, BlockID copiedFrom, BlockID copiedTo,
                          ElementDirection direction, ElementPosition position, int offset)
    {
        this.type = type;
        this.copiedFrom = copiedFrom;
        this.copiedTo = copiedTo;
        this.direction = direction;
        this.position = position;
        this.offset = offset;
    }

    internal FenceElement(ElementType type, BlockID copiedFrom, BlockID copiedTo,
                      ElementDirection direction)
    {
        this.type = type;
        this.copiedFrom = copiedFrom;
        this.copiedTo = copiedTo;
        this.direction = direction;
    }

    internal FenceElement(ElementType type, BlockID copiedFrom, BlockID copiedTo)
    {
        this.type = type;
        this.copiedFrom = copiedFrom;
        this.copiedTo = copiedTo;
    }

    private AABB Aabb
    {
        get
        {
            switch (type)
            {
                case ElementType.Barrier:
                    return ApplyOffset(FenceElementsAABBs.Barrier(direction, position));
                case ElementType.Post:
                case ElementType.AntiJumpOverCorner:
                    return FenceElementsAABBs.Post();
                case ElementType.Corner:
                    return ApplyOffset(FenceElementsAABBs.Corner(direction, position));
                case ElementType.AntiJumpOver:
                    return FenceElementsAABBs.AntiJumpOver(direction);
                default:
                    return new AABB(0, 0, 0, 16, 16, 16);
            }
        }
    }

    private AABB ApplyOffset(AABB aabb)
    {
        int offsetUnits = 16 * offset;
        return aabb.Offset(0, -offsetUnits, 0);
    }

    public override string ToString()
    {
        string direction = this.direction.ToString();
        string position = this.position.ToString();

        switch (type)
        {
            case ElementType.Post:
                return "Fence post";
            case ElementType.AntiJumpOver:
                return $"Fence {direction} anti-jump";
            case ElementType.AntiJumpOverCorner:
                return "Fence anti-jump corner";
            case ElementType.Barrier:
            case ElementType.Corner:
                return $"Fence {direction} {position} Offset {offset}";
            default:
                return "";
        }
    }

    internal List<string> RawCommands(Player player, int count, string prefix)
    {
        int targetID = copiedTo + count;
        List<string> commands = new List<string>();
        commands.Add($"{prefix}copy {copiedFrom} {targetID}");

        string minimum = Aabb.Min.ToStringNoComma();
        string maximum = Aabb.Max.ToStringNoComma();

        commands.Add($"{prefix}edit {targetID} min {minimum}");
        commands.Add($"{prefix}edit {targetID} max {maximum}");
        commands.Add($"{prefix}edit {targetID} blockslight no");

        if (type == ElementType.AntiJumpOver || type == ElementType.AntiJumpOverCorner)
            commands.Add($"{prefix}edit {targetID} blockdraw 4");

        if (offset != 0)
        {
            BlockDefinition def = CopiedFromBD(player);
            int sideTexID = def.FrontTex;

            // Known issue: things will go wrong when wrapping if current texture pack has 512 textures
            if (sideTexID - offset >= 0)
                commands.Add($"{prefix}edit {targetID} sidetex {(sideTexID - offset) % 256}");
            else
                commands.Add($"{prefix}edit {targetID} sidetex {(sideTexID - offset) % 256 + 256}");
        }

        commands.Add($"{prefix}edit {targetID} name {this.ToString()}");

        return commands;
    }

    private BlockDefinition CopiedFromBD(Player p)
    {
        Level lvl = p.Level;
        BlockID serverBlockID = Block.FromRaw(copiedFrom);

        bool levelBlockExists = (lvl.CustomBlockDefs[Block.FromRaw(copiedFrom)] != null);
        bool globalBlockExists = (BlockDefinition.GlobalDefs[Block.FromRaw(copiedFrom)] != null);

        if (levelBlockExists)
            return lvl.CustomBlockDefs[serverBlockID];
        else if (globalBlockExists)
            return BlockDefinition.GlobalDefs[serverBlockID];
        else
            return DefaultSet.MakeCustomBlock(serverBlockID);
    }
}


