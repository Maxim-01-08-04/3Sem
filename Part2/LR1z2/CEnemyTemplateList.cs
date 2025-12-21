using System.Collections.Generic;
using System.Linq;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies = new List<CEnemyTemplate>();
        private readonly ISaveList<List<CEnemyTemplate>> _serializer = new JsonEnemySaver();

        public void AddEnemy(CEnemyTemplate enemy)
        {
            DeleteEnemyByName(enemy.Name);
            enemies.Add(enemy);
        }

        public void AddNormalEnemy(string name, string iconName, int baseline, double lifeModifier,
                                 int baseGold, double goldModifier, double spawnChance)
        {
            AddEnemy(new CNormalEnemyTemplate(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance));
        }

        public void AddArmoredEnemy(string name, string iconName, int baseline, double lifeModifier,
                                  int baseGold, double goldModifier, double spawnChance, double armor)
        {
            AddEnemy(new CArmoredEnemyTemplate(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance, armor));
        }

        public void AddShrinkingEnemy(string name, string iconName, int baseline, double lifeModifier,
                                    int baseGold, double goldModifier, double spawnChance, double shrinkFactor)
        {
            AddEnemy(new CShrinkingEnemyTemplate(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance, shrinkFactor));
        }

        public void AddHealingEnemy(string name, string iconName, int baseline, double lifeModifier,
                                  int baseGold, double goldModifier, double spawnChance, double healChance)
        {
            AddEnemy(new CHealingEnemyTemplate(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance, healChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
        {
            return enemies.Find(e => e.Name == name);
        }

        public void DeleteEnemyByName(string name)
        {
            enemies.RemoveAll(e => e.Name == name);
        }

        public void DeleteEnemyByIndex(int index)
        {
            if (index >= 0 && index < enemies.Count)
                enemies.RemoveAt(index);
        }

        public List<string> GetListOfEnemyNames()
        {
            return enemies.Select(e => e.Name).ToList();
        }

        public List<CEnemyTemplate> GetEnemies()
        {
            return new List<CEnemyTemplate>(enemies);
        }

        public void SaveToJson(string path)
        {
            _serializer.Save(enemies, path);
        }

        public void LoadFromJson(string path)
        {
            var loadedEnemies = _serializer.Load(path);
            enemies.Clear();
            enemies.AddRange(loadedEnemies);
        }

        public void Clear()
        {
            enemies.Clear();
        }

        public int Count => enemies.Count;
    }
}