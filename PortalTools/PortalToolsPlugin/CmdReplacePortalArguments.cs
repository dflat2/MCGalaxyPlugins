namespace PortalToolsPlugin;
using BlockID = System.UInt16;

public class CmdReplacePortalArguments
{
    public BlockID BlockToReplace { get; private set; }
    public BlockID PortalBlock { get; private set; }

    public CmdReplacePortalArguments(BlockID blockToReplace, BlockID portalBlock)
    {
        BlockToReplace = blockToReplace;
        PortalBlock = portalBlock;
    }
}

