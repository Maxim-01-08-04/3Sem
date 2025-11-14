using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        [JsonInclude]
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        public void AddEnemy(CEnemyTemplate enemy) => enemies.Add(enemy);
        public void RemoveEnemy(CEnemyTemplate enemy) => enemies.Remove(enemy);
        public void RemoveEnemyAt(int index) => enemies.RemoveAt(index);
        public List<CEnemyTemplate> GetEnemies() => enemies;
        public int Count => enemies.Count;
        public CEnemyTemplate this[int index] => enemies[index];

        public void SaveToFile(string filename)
        {
            string jsonString = JsonSerializer.Serialize(enemies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, jsonString);
        }

        public void LoadFromFile(string filename)
        {
            if (!File.Exists(filename)) return;

            string jsonFromFile = File.ReadAllText(filename);
            JsonDocument doc = JsonDocument.Parse(jsonFromFile);

            enemies.Clear();
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string name = element.GetProperty("name").GetString();
                string iconName = element.GetProperty("iconName").GetString();
                int baseLife = element.GetProperty("baseLife").GetInt32();
                double lifeModifier = element.GetProperty("lifeModifier").GetDouble();
                int baseGold = element.GetProperty("baseGold").GetInt32();
                double goldModifier = element.GetProperty("goldModifier").GetDouble();
                double spawnChance = element.GetProperty("spawnChance").GetDouble();

                CEnemyTemplate enemy = new CEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance);
                enemies.Add(enemy);
            }
        }
    }
}
