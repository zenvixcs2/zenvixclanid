using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using System.IO;
using System.Text.Json;

namespace ZenvixClanId;

public class ZenvixClanIdConfig
{
    public string DesiredClanId { get; set; } = "[Zenvix gg/cs2store]";
    public string ExcludedClanId { get; set; } = "[Komutçu Admin]";
}

[MinimumApiVersion(100)]
public class ZenvixClanId : BasePlugin
{
    public override string ModuleName => "Zenvix Clan ID Changer";
    public override string ModuleVersion => "1.1.0";
    public override string ModuleAuthor => "zenvix";

    private ZenvixClanIdConfig ConfigData = null!;
    private string ConfigPath = string.Empty;

    public override void Load(bool hotReload)
    {
        LoadConfig();

        // OnClientPutInServer - Called when player is fully connected to server (like OnPlayerConnectFull)
        // Much more efficient than OnTick - only runs once when player connects, not continuously
        RegisterListener<Listeners.OnClientPutInServer>(OnPlayerConnectFull);

        // Process existing players if hot reload
        if (hotReload)
        {
            ProcessExistingPlayers();
        }
    }

    private void LoadConfig()
    {
        try
        {
            string configDir = Path.Combine(ModuleDirectory, "configs");
            ConfigPath = Path.Combine(configDir, "ZenvixClanId.json");

            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
                Console.WriteLine($"[ZenvixClanId] Config directory created: {configDir}");
            }

            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                ConfigData = JsonSerializer.Deserialize<ZenvixClanIdConfig>(json) ?? new ZenvixClanIdConfig();
                Console.WriteLine($"[ZenvixClanId] Config file loaded: {ConfigPath}");
            }
            else
            {
                ConfigData = new ZenvixClanIdConfig();
                SaveConfig();
                Console.WriteLine($"[ZenvixClanId] New config file created: {ConfigPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] Error loading config: {ex.Message}");
            ConfigData = new ZenvixClanIdConfig();
        }
    }

    private void SaveConfig()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(ConfigData, options);
            File.WriteAllText(ConfigPath, json);
            Console.WriteLine($"[ZenvixClanId] Config file saved: {ConfigPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] Error saving config: {ex.Message}");
        }
    }

    // OnPlayerConnectFull equivalent - Called when player is fully connected to server
    // Much more efficient than OnTick: only runs once, not continuously!
    private void OnPlayerConnectFull(int slot)
    {
        try
        {
            // Get player from slot
            var player = Utilities.GetPlayerFromSlot(slot);

            // Process if player is valid and not a bot
            if (player is { IsValid: true, IsBot: false })
            {
                // Small delay to ensure player is fully loaded before setting clan
                AddTimer(0.5f, () => SetPlayerClan(player));
                Console.WriteLine($"[ZenvixClanId] Player fully connected: {player.PlayerName} (Slot: {slot})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] OnPlayerConnectFull error: {ex.Message}");
        }
    }

    private void SetPlayerClan(CCSPlayerController player)
    {
        try
        {
            // Check if player is still valid
            if (player is not { IsValid: true, IsBot: false })
                return;

            // Check excluded clan
            if (!string.IsNullOrWhiteSpace(ConfigData.ExcludedClanId) &&
                player.Clan == ConfigData.ExcludedClanId)
            {
                Console.WriteLine($"[ZenvixClanId] Player {player.PlayerName} is in excluded clan, skipping");
                return;
            }

            // Set desired clan
            if (!string.IsNullOrWhiteSpace(ConfigData.DesiredClanId) &&
                player.Clan != ConfigData.DesiredClanId)
            {
                player.Clan = ConfigData.DesiredClanId;
                Console.WriteLine($"[ZenvixClanId] Player {player.PlayerName} clan ID updated: {ConfigData.DesiredClanId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] SetPlayerClan error: {ex.Message}");
        }
    }

    // Process existing players on hot reload
    private void ProcessExistingPlayers()
    {
        try
        {
            foreach (var player in Utilities.GetPlayers())
            {
                if (player is { IsValid: true, IsBot: false })
                {
                    AddTimer(0.1f, () => SetPlayerClan(player));
                }
            }
            Console.WriteLine("[ZenvixClanId] Hot reload: Existing players processed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] ProcessExistingPlayers error: {ex.Message}");
        }
    }
}
