namespace PluginEasyFences;

internal enum ElementType
{
    Post,
    Corner,
    Barrier,
    AntiJumpOver,
    AntiJumpOverCorner
}

internal enum ElementDirection
{
    X,
    Z
}

internal enum ElementPosition
{
    // For barriers
    Top,
    Bottom,
    // For corners
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft
}