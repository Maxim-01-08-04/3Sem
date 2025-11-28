using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class EnemyManager
    {
        private List<CEnemyTemplate> enemyTemplates;
        private Random random;

        public EnemyManager()
        {
            enemyTemplates = new List<CEnemyTemplate>();
            random = new Random();
        }

        public void LoadTemplates(List<CEnemyTemplate> templates)
        {
            enemyTemplates = templates;
            NormalizeChances();
        }

        public void AddTemplate(CEnemyTemplate template)
        {
            enemyTemplates.Add(template);
            NormalizeChances();
        }

        private void NormalizeChances()
        {
            if (enemyTemplates.Count == 0) return;

            double sum = enemyTemplates.Sum(t => t.SpawnChance());

            if (sum == 0) return;

            
        }

        public Enemy GetRandomEnemy()
        {
            if (enemyTemplates.Count == 0)
                return CreateDefaultEnemy();

            double totalChance = enemyTemplates.Sum(t => t.SpawnChance());
            double randomValue = random.NextDouble() * totalChance;

            double currentSum = 0;
            foreach (var template in enemyTemplates)
            {
                currentSum += template.SpawnChance();
                if (currentSum >= randomValue)
                {
                    return new Enemy(template);
                }
            }

            return new Enemy(enemyTemplates[0]);
        }

        private Enemy CreateDefaultEnemy()
        {
            return new Enemy("Default Enemy", new BigNumber("100"), new BigNumber("10"), "default.png");
        }

        public List<string> GetEnemyNames()
        {
            return enemyTemplates.Select(t => t.Name).ToList();
        }
    }
}
