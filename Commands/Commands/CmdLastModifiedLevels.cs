//reference System.Core.dll
using System;
using System.Linq;
using System.IO;
using MCGalaxy;

public class CmdLastmodifiedlevels : Command
{
	public override string name { get { return "LastModifiedLevels"; } }
	public override string shortcut { get { return "lml"; } }
	public override string type { get { return CommandTypes.Information; } }
	public override bool museumUsable { get { return true; } }

	public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

	public override void Use(Player player, string message)
	{
		DirectoryInfo directory = new DirectoryInfo("./levels");

		FileInfo[] lastModified = directory.GetFiles("*.lvl")
                .OrderByDescending(f => f.LastWriteTime)
                .Take(10)
                .ToArray();

		player.Message("%aLast modified levels:");

		for (int i = 0; i < lastModified.Length; i++)
		{
			ShowLine(player, i + 1, lastModified[i].Name, lastModified[i].LastWriteTime);
		}
    }

	private void ShowLine(Player player, int count, string fileName, DateTime lastModified)
	{
		// From TopStat.FormatDate
		TimeSpan delta = DateTime.Now - lastModified;
        string when = string.Format("{0:H:mm} on {0:d} ({1} ago)", lastModified, delta.Shorten());

		player.Message($"{count}) {Path.GetFileNameWithoutExtension(fileName)} - {when}");
    }

	public override void Help(Player player)
	{
		player.Message("&T/LastModifiedLevels");
		player.Message("&HDisplays the last 10 modified levels.");
        player.Message("&TNote: &Hit only takes blocks into account (ie. adding portals won't count as an update).");
    }
}