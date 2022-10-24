namespace PluginMarksSaver;
using MCGalaxy.Maths;

public static class SaveMarksUtils
{
    public static string ToStringNoComma(this Vec3S32 vector)
    {
        return $"{vector.X} {vector.Y} {vector.Z}";
    }
}