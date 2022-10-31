namespace PluginEasyFences;
using System;
using MCGalaxy;
using BlockID = System.UInt16;

internal class FenceSetProps
{
    internal BlockID CopiedFrom     = Block.Wood;
    internal BlockID CopiedTo       = Block.CPE_MAX_BLOCK + 1;
    internal bool    CanJumpOver    = true;
    internal bool    DoBury         = false;
    internal bool    CrossIntersect = false;
    internal bool    TIntersect     = false;

    internal int BlocksCount {
        get {
            int post = 1;
            int antiJumpOver = CanJumpOver ? 0 : 2;
            int corners = 8;
            int barriers = 2;

            if (TIntersect)     corners += 4;
            if (CrossIntersect) barriers += 1;

            return post + antiJumpOver + corners + barriers;
        }
    }
}
