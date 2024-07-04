namespace PluginSmooth;
using BlockID = System.UInt16;

public class SmoothArguments {
    public BlockID Block { get; private set; }
    public int Radius { get; private set; }
    public int Intensity { get; private set; }

    public SmoothArguments(BlockID block, int radius, int intensity) {
        Block = block;
        Radius = radius;
        Intensity = intensity;
    }
}