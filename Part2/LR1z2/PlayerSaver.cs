using System;
using System.IO;
using System.Text.Json;

namespace EnemyEditor
{
    public class PlayerSaver : ISaveList<CPlayer>
    {
        private readonly JsonSerializerOptions _options;

        public PlayerSaver()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new BigNumberConverter() }
            };
        }

        public CPlayer Load(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var player = JsonSerializer.Deserialize<CPlayer>(json, _options);
                    return player ?? new CPlayer();
                }
                return new CPlayer();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading player from {path}: {ex.Message}");
                return new CPlayer();
            }
        }

        public void Save(CPlayer player, string path)
        {
            try
            {
                string json = JsonSerializer.Serialize(player, _options);
                File.WriteAllText(path, json);
                System.Diagnostics.Debug.WriteLine($"Player progress saved to {path}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving player to {path}: {ex.Message}");
                throw;
            }
        }
    }
}