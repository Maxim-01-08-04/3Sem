using System;
using System.Collections.Generic;
using System.Linq;

namespace EnemyEditor
{
    public class CEnemyManager
    {
        public delegate void EnemyEvent(object sender, CControllerEventArgs e);

        public event EnemyEvent EnemySpawned;
        public event EnemyEvent EnemyDefeated;
        public event EnemyEvent EnemyReplaced;
        public event EnemyEvent EnemyDamaged;
        public event EnemyEvent PointsEarned;

        private List<CEnemyTemplate> enemyTemplates;
        private IEnemy currentEnemy;
        private Random random;
        private int playerLevel = 1;
        private int currentEnemyIndex = 0;

        public CEnemyManager()
        {
            enemyTemplates = new List<CEnemyTemplate>();
            random = new Random();
        }

        public IEnemy CurrentEnemy => currentEnemy;

        public void LoadTemplates(List<CEnemyTemplate> templates)
        {
            enemyTemplates.Clear();
            if (templates != null && templates.Count > 0)
            {
                enemyTemplates.AddRange(templates);
                currentEnemyIndex = 0;
            }
            else
            {
                CreateDefaultTemplate();
            }
        }

        public void LoadTemplatesFromList(CEnemyTemplateList templateList)
        {
            enemyTemplates.Clear();
            var names = templateList.GetListOfEnemyNames();

            foreach (var name in names)
            {
                var template = templateList.GetEnemyByName(name);
                if (template != null)
                {
                    enemyTemplates.Add(template);
                }
            }

            if (enemyTemplates.Count == 0)
            {
                CreateDefaultTemplate();
            }

            currentEnemyIndex = 0;
        }

        private void CreateDefaultTemplate()
        {
            enemyTemplates.Add(new CNormalEnemyTemplate("Default Enemy", "default", 50, 1.1, 25, 1.1, 0.5));
        }

        public IEnemy GetNextEnemyWithModification(int playerLevel)
        {
            this.playerLevel = playerLevel;

            if (enemyTemplates.Count == 0)
            {
                CreateDefaultTemplate();
            }

            CEnemyTemplate template = enemyTemplates[currentEnemyIndex];
            currentEnemyIndex = (currentEnemyIndex + 1) % enemyTemplates.Count;

            currentEnemy = EnemyFactory.CreateFromTemplate(template);

            if (playerLevel > 1)
            {
                double lifeModifier = Math.Pow(1.1, playerLevel - 1);
                double goldModifier = Math.Pow(1.05, playerLevel - 1);

                currentEnemy.ScaleWithPlayerLevel(lifeModifier, goldModifier);
            }

            EnemySpawned?.Invoke(this, new CControllerEventArgs($"Появился новый противник: {currentEnemy.Name}"));

            return currentEnemy;
        }

        public void OnEnemyDamaged(BigNumber damage)
        {
            EnemyDamaged?.Invoke(this, new CControllerEventArgs($"Нанесен урон: {damage}"));
        }

        public void OnEnemyDefeated(BigNumber goldReward)
        {
            EnemyDefeated?.Invoke(this, new CControllerEventArgs(goldReward)
            {
                Message = $"Противник побежден! Награда: {goldReward}"
            });

            PointsEarned?.Invoke(this, new CControllerEventArgs(goldReward)
            {
                Message = $"Получено золота: {goldReward} за победу"
            });
        }

        public void OnEnemyReplaced(string oldEnemyName, string newEnemyName)
        {
            EnemyReplaced?.Invoke(this, new CControllerEventArgs($"Противник заменен: {oldEnemyName} -> {newEnemyName}"));
        }

        public void UpdatePlayerLevel(int level)
        {
            playerLevel = level;
        }

        public int GetEnemyCount()
        {
            return enemyTemplates.Count;
        }

        public List<string> GetEnemyNames()
        {
            return enemyTemplates.Select(t => t.Name).ToList();
        }
    }
}