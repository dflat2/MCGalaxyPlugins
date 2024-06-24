namespace PluginEasyFences;
using MCGalaxy.Maths;

public static class EasyFencesUtils {
    public static string ToStringNoComma(this Vec3S32 vector) {
        return $"{vector.X} {vector.Y} {vector.Z}";
    }
}