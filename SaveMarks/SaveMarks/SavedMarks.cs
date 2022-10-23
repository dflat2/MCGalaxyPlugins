namespace PluginSaveMarks;

using System.Collections.Generic;
using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;

public static class SavedMarks
{
    private static Dictionary<Player, Vec3S32[]> table = new Dictionary<Player, Vec3S32[]>();

    internal static Vec3S32[] Get(Player p)
    {
        if (table.ContainsKey(p)) return table[p];
        return new List<Vec3S32>().ToArray();
    }

    internal static void Set(Player p, Vec3S32[] marks)
    {
        if (!table.ContainsKey(p))
            OnPlayerDisconnectEvent.Register(SavedMarks.OnPlayerDisconnect, Priority.Normal);

        table.Add(p, marks);
    }

    public static void OnPlayerDisconnect(Player p, string reason)
    {
        if (table.ContainsKey(p))
            table.Remove(p);
    }
}
