namespace PluginEasyFences;

public enum ElementType {
    Post,
    Corner,
    Barrier,
    AntiJumpOver,
    AntiJumpOverCorner
}

public enum ElementDirection {
    X,
    Z
}

public enum ElementPosition {
    // For barriers
    Top,
    Bottom,
    // For corners
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft
}