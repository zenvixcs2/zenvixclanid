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
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "zenvix";

    private ZenvixClanIdConfig ConfigData = null!;
    private string ConfigPath = string.Empty;

    public override void Load(bool hotReload)
    {
        LoadConfig();
        RegisterListener<Listeners.OnTick>(OnTick);
    }

    private void LoadConfig()
    {
        try
        {
            // ModuleDirectory kullanarak daha güvenli path oluşturma
            string configDir = Path.Combine(ModuleDirectory, "configs");
            ConfigPath = Path.Combine(configDir, "ZenvixClanId.json");

            // Dizin yoksa oluştur
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
                Console.WriteLine($"[ZenvixClanId] Config dizini oluşturuldu: {configDir}");
            }

            // Config dosyası varsa yükle
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                ConfigData = JsonSerializer.Deserialize<ZenvixClanIdConfig>(json) ?? new ZenvixClanIdConfig();
                Console.WriteLine($"[ZenvixClanId] Config dosyası yüklendi: {ConfigPath}");
            }
            else
            {
                // Config dosyası yoksa varsayılan config oluştur
                ConfigData = new ZenvixClanIdConfig();
                SaveConfig();
                Console.WriteLine($"[ZenvixClanId] Yeni config dosyası oluşturuldu: {ConfigPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] Config yüklenirken hata: {ex.Message}");
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
            Console.WriteLine($"[ZenvixClanId] Config dosyası kaydedildi: {ConfigPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] Config kaydedilirken hata: {ex.Message}");
        }
    }

    private void OnTick()
    {
        try
        {
            foreach (var player in Utilities.GetPlayers())
            {
                if (player is { IsValid: true, IsBot: false })
                {
                    // Excluded clan kontrolü
                    if (!string.IsNullOrWhiteSpace(ConfigData.ExcludedClanId) &&
                        player.Clan == ConfigData.ExcludedClanId)
                        continue;

                    // Desired clan atama
                    if (!string.IsNullOrWhiteSpace(ConfigData.DesiredClanId) &&
                        player.Clan != ConfigData.DesiredClanId)
                    {
                        player.Clan = ConfigData.DesiredClanId;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ZenvixClanId] OnTick hatası: {ex.Message}");
        }
    }
}