# Zenvix Clan ID Changer Plugin

A Counter-Strike plugin that allows you to change the clan ID of players in the game. The plugin checks every player and, if they do not belong to the desired clan, automatically updates their clan ID. The plugin also respects the exclusion of specific clan IDs, preventing certain players from having their clan ID changed.

## Features

- Change players' clan ID automatically to the desired one.
- Exclude specific clan IDs to prevent certain players' clan IDs from being changed.
- Configurable through a simple JSON configuration file.
- Works for all players except bots.

## Installation

1. **Download the Plugin**
   - Download the plugin files and place them into the correct directory of your server.
   
2. **Configuration File**
   - The plugin uses a configuration file named `ZenvixClanId.json` located in the `configs` directory. If the file doesn't exist, it will be automatically created with default settings.
   - The plugin will try to create a `configs` directory inside your plugin's folder if it doesn't exist.

3. **Load the Plugin**
   - Once installed, load the plugin through your server's plugin management interface.

## Configuration

The configuration file (`ZenvixClanId.json`) includes the following parameters:

```json
{
  "DesiredClanId": "[İhtişam JB]",
  "ExcludedClanId": "[Komutçu Admin]"
}
