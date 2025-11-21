using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies = new List<CEnemyTemplate>();

        public void AddEnemy(string name, string iconName, int baseline,
                           double lifeModifier, int baseGold, double goldModifier,
                           double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(name, iconName, baseline, lifeModifier,
                                         baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
        {
            return enemies.Find(e => e.Name() == name);
        }

        public CEnemyTemplate GetEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
                return enemies[id];
            return null;
        }

        public void DeleteEnemyByName(string name)
        {
            enemies.RemoveAll(e => e.Name() == name);
        }

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
                enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            List<string> names = new List<string>();
            foreach (var enemy in enemies)
            {
                names.Add(enemy.Name());
            }
            return names;
        }

        public List<CEnemyTemplate> GetEnemies() => enemies;

        public void SaveToJson(string path)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(enemies, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                System.IO.File.WriteAllText(path, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения: {ex.Message}");
            }
        }

        public void LoadFromJson(string path)
        {
            try
            {
                string jsonFromFile = System.IO.File.ReadAllText(path);
                using JsonDocument doc = JsonDocument.Parse(jsonFromFile);

                enemies.Clear();

                foreach (JsonElement element in doc.RootElement.EnumerateArray())
                {
                    string name = element.GetProperty("name").GetString();
                    string iconName = element.GetProperty("iconName").GetString();
                    int baseline = element.GetProperty("baseline").GetInt32();
                    double lifeModifier = element.GetProperty("lifeModifier").GetDouble();
                    int baseGold = element.GetProperty("baseGold").GetInt32();
                    double goldModifier = element.GetProperty("goldModifier").GetDouble();
                    double spawnChance = element.GetProperty("spawnChance").GetDouble();

                    enemies.Add(new CEnemyTemplate(name, iconName, baseline, lifeModifier,
                                                 baseGold, goldModifier, spawnChance));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}