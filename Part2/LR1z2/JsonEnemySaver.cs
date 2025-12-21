using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EnemyEditor
{
    public class JsonEnemySaver : ISaveList<List<CEnemyTemplate>>
    {
        private readonly JsonSerializerOptions _options;

        public JsonEnemySaver()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new EnemyTemplateConverter() }
            };
        }

        public List<CEnemyTemplate> Load(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    return JsonSerializer.Deserialize<List<CEnemyTemplate>>(json, _options) ?? new List<CEnemyTemplate>();
                }
                return new List<CEnemyTemplate>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading enemies: {ex.Message}");
                return new List<CEnemyTemplate>();
            }
        }

        public void Save(List<CEnemyTemplate> data, string path)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, _options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving enemies: {ex.Message}");
                throw;
            }
        }
    }
}