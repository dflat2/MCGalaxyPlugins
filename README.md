# MCGalaxyPlugins

This repository contains source codes for my MCGalaxy Plugins.

## Installing a plugin

**Requierement.** An MCGalaxy server, preferably version 1.9.4.3 or greater (I didn't test the plugins on earlier versions but they will most likely work on them).

Each plugin has its own folder. Let's say for instance that you want to install the Erosion plugin.

1. In the *Plugins* section below, download `Erosion.cs` into your `mcgalaxy/plugins` folder.
2. If your server isn't running already, run it (CLI or GUI, doesn't matter).
3. Once the server is loaded, type `/pcompile Erosion` in the console. If this step causes an error, please post it in the issues section of this repository so that I can investigate it.
4. Then, run `/pload Erosion` in the console. 
5. The plugin is now loaded. Connect to your server and run `/help erosion` to get started.

When restarting your server, the plugin will be loaded automatically.

## Plugins

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
