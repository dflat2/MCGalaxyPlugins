# MCGalaxyPlugins

This repository contains source codes for my MCGalaxy Plugins.

## Installing a plugin

**Requirement.** An MCGalaxy server, preferably version 1.9.4.3 or greater (I didn't test the plugins on earlier versions but they will most likely work on them).

Each plugin has its own folder. Let's say for instance that you want to install the Erosion plugin.

1. In the *Plugins* section below, download `Erosion.cs` into your `mcgalaxy/plugins` folder.
2. If your server isn't running already, run it (CLI or GUI, doesn't matter).
3. Once the server is loaded, type `/pcompile Erosion` in the console. If this step causes an error, please post it in the issues section of this repository so that I can investigate it.
4. Then, run `/pload Erosion` in the console. 
5. The plugin is now loaded. Connect to your server and run `/help erosion` to get started.

When restarting your server, the plugin will be loaded automatically.

## Plugins

### DotStatus

Download link: [DotStatus.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/DotStatus/DotStatus.cs)

Displays zombie survival status when a player sends a message with a single `.` or a single `t`. To use it with lava survival instead, change the corresponding line in the plugin.

### Erosion

Download link: [Erosion.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/Erosion/Erosion.cs)

This plugin is inspired by erosion in binary image processing. It adds a new `/erode <mode> [block]` command that simulates erosion. It works by removing each `[block]` that is not surrounded by itself.

### MarksSaver

Download link: [MarksSaver.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/MarksSaver/MarksSaver.cs)

This plugins adds two commands: `/savemarks` and `/loadmarks`. When building you often need to run long distances to `/mark` or to memorize coordinates. `/savemarks` lets you mark a region once and for all, and experiment multiple draw commands over this region by marking with `/loadmarks` (or its shorter version `/lm`). For instance, running `/z` then `/lm` would cuboid over the saved region.

### EasyFences

Download link: [EasyFences.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/EasyFences/EasyFences.cs)

`/easyfences` (or its shorthand `/ezf`) guides you through the process of adding all blocks necessary to build Minecraft-like fences. Features:

+ Ability to control the offsets of fence-elements (underground or above the posts)
+ Generates as few blocks as necessary depending on your needs
+ You can turn any block into a fence-set in less than a minute
+ Decide whether players should be able to jump over the fences

Known issues:

+ When using a texture pack with more than 256 textures (i.e. 512), fences' side textures  will be very likely wrong if fences are built from a block with either a low texture ID (less than 4) or a high texture ID (above 252).
+ If you decide to bury fences, it's recommended to disable jumping over the fences, because collisions with barriers and corners sometimes go wrong.

### SeeReportsOnLogin

Download link: [SeeReportsOnLogin](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/SeeReportsOnLogin/SeeReportsOnLogin.cs)

Automatically runs `/Report list` when an operator joins.

### CommandsUnloader

Download link: [CommandsUnloader.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/CommandsUnloader/CommandsUnloader.cs)

Unloads all commands with permission 120.

### InvincibleReferees

Download link: [InvincibleReferees.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/InvincibleReferees/InvincibleReferees.cs)

Makes you invincible automatically when using `/Ref`.

## Commands

### LastModifiedLevels

Download link: [CmdLastModifiedLevels.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/Commands/Commands/CmdLastModifiedLevels.cs)

Displays the last 10 modified levels. Note that it only takes blocks into account (ie. adding portals won't count as an update).

### ConsoleDo

Download link: [CmdConsoleDo.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/Commands/Commands/CmdConsoleDo.cs)

Runs a command as console while in-game.
