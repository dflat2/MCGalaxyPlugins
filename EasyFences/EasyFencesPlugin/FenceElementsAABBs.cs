namespace PluginEasyFences;
using MCGalaxy.Maths;
using System;

internal static class FenceElementsAABBs
{
    private const int BOTTOM_BARRIER_MIN_Y = 6;
    private const int BOTTOM_BARRIER_MAX_Y = 9;
    private const int TOP_BARRIER_MIN_Y = 12;
    private const int TOP_BARRIER_MAX_Y = 15;

    private static AABB DEFAULT_AABB = new AABB(0, 0, 0, 16, 16, 16);

    internal static AABB Post()
    {
        return new AABB(6, 0, 6, 10, 16, 10);
    }

    internal static AABB Corner(ElementDirection direction, ElementPosition position)
    {
        switch (direction)
        {
            case (ElementDirection.X):
                switch (position)
                {
                    case (ElementPosition.BottomLeft):
                        return new AABB(0, BOTTOM_BARRIER_MIN_Y, 7, 6, BOTTOM_BARRIER_MAX_Y, 9);
                    case (ElementPosition.BottomRight):
                        return new AABB(10, BOTTOM_BARRIER_MIN_Y, 7, 16, BOTTOM_BARRIER_MAX_Y, 9);
                    case (ElementPosition.TopLeft):
                        return new AABB(0, TOP_BARRIER_MIN_Y, 7, 6, TOP_BARRIER_MAX_Y, 9);
                    case (ElementPosition.TopRight):
                        return new AABB(10, TOP_BARRIER_MIN_Y, 7, 16, TOP_BARRIER_MAX_Y, 9);
                    default:
                        return DEFAULT_AABB;
                }
            case (ElementDirection.Z):
                switch (position)
                {
                    case (ElementPosition.BottomLeft):
                        return new AABB(7, BOTTOM_BARRIER_MIN_Y, 0, 9, BOTTOM_BARRIER_MAX_Y, 6);
                    case (ElementPosition.BottomRight):
                        return new AABB(7, BOTTOM_BARRIER_MIN_Y, 10, 9, BOTTOM_BARRIER_MAX_Y, 16);
                    case (ElementPosition.TopLeft):
                        return new AABB(7, TOP_BARRIER_MIN_Y, 0, 9, TOP_BARRIER_MAX_Y, 6);
                    case (ElementPosition.TopRight):
                        return new AABB(7, TOP_BARRIER_MIN_Y, 10, 9, TOP_BARRIER_MAX_Y, 16);
                    default:
                        return DEFAULT_AABB;
                }
            default:
                return DEFAULT_AABB;
        }
    }

    internal static AABB Barrier(ElementDirection direction, ElementPosition position)
    {
        switch (direction)
        {
            case (ElementDirection.X):
                switch (position)
                {
                    case (ElementPosition.Bottom):
                        return new AABB(0, BOTTOM_BARRIER_MIN_Y, 7, 16, BOTTOM_BARRIER_MAX_Y, 9);
                    case (ElementPosition.Top):
                        return new AABB(0, TOP_BARRIER_MIN_Y, 7, 16, TOP_BARRIER_MAX_Y, 9);
                    default:
                        return DEFAULT_AABB;
                }
            case (ElementDirection.Z):
                switch (position)
                {
                    case (ElementPosition.Bottom):
                        return new AABB(7, BOTTOM_BARRIER_MIN_Y, 0, 9, BOTTOM_BARRIER_MAX_Y, 16);
                    case (ElementPosition.Top):
                        return new AABB(7, TOP_BARRIER_MIN_Y, 0, 9, TOP_BARRIER_MAX_Y, 16);
                    default:
                        return DEFAULT_AABB;
                }
            default:
                return DEFAULT_AABB;
        }
    }

    internal static AABB AntiJumpOver(ElementDirection direction)
    {
        switch (direction)
        {
            case (ElementDirection.X):
                return new AABB(0, 0, 6, 16, 16, 10);
            case (ElementDirection.Z):
                return new AABB(6, 0, 0, 10, 16, 16);
            default:
                return DEFAULT_AABB;
        }
    }
}
