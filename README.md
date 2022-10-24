# MCGalaxyPlugins

This repository contains source codes for my MCGalaxy Plugins.

## Installing a plugin

**Requierement.** An MCGalaxy server, preferably version 1.9.4.3 or greater (I didn't test the plugins on earlier versions but they will most likely work on them).

Each plugin has its own folder. Let's say for instance that you want to install the Erosion plugin.

1. From the Github directory browser, click on the Erosion folder, then Erosion.cs, then on the Raw button.
2. Download this file into you mcgalaxy/plugins folder.
3. If your server isn't running already, run it (CLI or GUI, doesn't matter).
4. Once the server is loaded, type `/pcompile Erosion` in the console. If this step causes an error, please post it in the issues section of this repository so that I can investigate it.
5. Then, run `/pload Erosion` in the console. 
6. The plugin is now loaded. Connect to your server and run `/help erosion` to get started!

When restarting your server, the plugin will be loaded automatically.

## Plugins

### Erosion

Download link: [Erosion.cs](https://raw.githubusercontent.com/dflat2/MCGalaxyPlugins/main/Erosion/Erosion.cs)

This plugin adds a new `/erode <mode> [block]` command that simulates erosion. It works by removing each `[block]` that is not surrounded by itself.

### MarksSaver

Download link: [MarksSaver.cs](https://github.com/dflat2/MCGalaxyPlugins/blob/main/MarksSaver/MarksSaver.cs)

This plugins adds two commands: `/savemarks` and `/loadmarks`. When building you often need to run long distances to `/mark` or to memorize coordinates. `/savemarks` lets you mark a region once and for all, and experiment multiple draw commands over this region by marking with `/loadmarks` (or its shorter version `/lm`). For instance, running `/z` then `/lm` would cuboid over the saved region.
