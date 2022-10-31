namespace PluginEasyFences;

using System;
using MCGalaxy.Maths;

internal static class EasyFencesUtils
{
    internal static string ToStringNoComma(this Vec3S32 vector)
    {
        return $"{vector.X} {vector.Y} {vector.Z}";
    }
}