using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace EnemyEditor
{
    
    public delegate void SceneEvent(object sender, CControllerEventArgs e);

    
    public class CController
    {
        public event SceneEvent AddObject;
        public event SceneEvent RemoveObject;
        public event SceneEvent ObjectSpawned;
        public event SceneEvent ObjectDestroyed;
        public event SceneEvent GameEvent;

        private CPlayer player;
        private CEnemyManager enemyManager;
        private CCollectableController collectableController;
        private CEnemyTemplateList templateList;
        private Random random;
        private Size sceneSize;

        private double spawnTimer;
        private double spawnInterval;
        private int maxObjectsOnScene;

        public CController(Size sceneSize, CEnemyTemplateList templateList, double spawnInterval = 3.0, int maxObjectsOnScene = 5)
        {
            this.sceneSize = sceneSize;
            this.templateList = templateList;
            this.spawnInterval = spawnInterval;
            this.maxObjectsOnScene = maxObjectsOnScene;
            this.random = new Random();
            this.spawnTimer = 0;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            player = new CPlayer();
            enemyManager = new CEnemyManager();
            collectableController = new CCollectableController();

            LoadEnemies();

            enemyManager.EnemySpawned += OnEnemySpawned;
            enemyManager.EnemyDefeated += OnEnemyDefeated;
            enemyManager.EnemyReplaced += OnEnemyReplaced;
            enemyManager.EnemyDamaged += OnEnemyDamaged;
            enemyManager.PointsEarned += OnPointsEarned;

            collectableController.BonusSpawned += OnBonusSpawned;
            collectableController.BonusCollected += OnBonusCollected;
            collectableController.BonusExpired += OnBonusExpired;
            collectableController.PointsEarned += OnPointsEarned;
        }

        private void LoadEnemies()
        {
            try
            {
                if (templateList.GetListOfEnemyNames().Count > 0)
                {
                    enemyManager.LoadTemplatesFromList(templateList);
                    return;
                }

                string defaultPath = "GOBLACHI.json";
                if (System.IO.File.Exists(defaultPath))
                {
                    templateList.LoadFromJson(defaultPath);
                    enemyManager.LoadTemplatesFromList(templateList);

                    if (enemyManager.GetEnemyCount() == 0)
                    {
                        CreateDefaultEnemies();
                    }
                }
                else
                {
                    CreateDefaultEnemies();
                }
            }
            catch (Exception ex)
            {
                CreateDefaultEnemies();
            }
        }

        private void CreateDefaultEnemies()
        {
            templateList.AddNormalEnemy("Goblin", "goblin", 50, 1.1, 25, 1.1, 0.5);
            templateList.AddArmoredEnemy("Knight", "knight", 100, 1.2, 50, 1.1, 0.3, 30.0);
            templateList.AddShrinkingEnemy("Ninja", "ninja", 60, 1.1, 40, 1.1, 0.4, 0.3);
            templateList.AddHealingEnemy("Priest", "priest", 80, 1.15, 45, 1.1, 0.3, 0.3);
            templateList.AddNormalEnemy("Orc", "orc", 75, 1.15, 35, 1.1, 0.4);

            enemyManager.LoadTemplatesFromList(templateList);
            templateList.SaveToJson("enemies.json");
        }

        #region Публичные методы управления игрой

        public void StartGame()
        {
            GameEvent?.Invoke(this, new CControllerEventArgs("Игра началась!"));

            SpawnEnemy();
        }

        public void Update(double deltaTime)
        {
            spawnTimer += deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                TrySpawnCollectable();
                spawnTimer = 0;
            }

            collectableController.Update(deltaTime, player, enemyManager);
        }

        public void HandleEnemyClick(Point mousePosition)
        {
            if (enemyManager.CurrentEnemy == null || !player.CanAttack) return;

            GameEvent?.Invoke(this, new CControllerEventArgs($"Игрок атакует {enemyManager.CurrentEnemy.Name}"));

            bool isDefeated = false;
            var healthBefore = enemyManager.CurrentEnemy.CurrentLife;

            if (enemyManager.CurrentEnemy is CShrinkingEnemy shrinkingEnemy)
            {
                shrinkingEnemy.TakeDamage(player.Damage);
                isDefeated = shrinkingEnemy.IsDefeated();

                if (!isDefeated && shrinkingEnemy.CurrentLife.Equals(healthBefore))
                {
                    GameEvent?.Invoke(this, new CControllerEventArgs($"{shrinkingEnemy.Name} увернулся от атаки!"));
                }
                else if (!shrinkingEnemy.CurrentLife.Equals(healthBefore))
                {
                    GameEvent?.Invoke(this, new CControllerEventArgs($"Вы нанесли {player.Damage} урона {shrinkingEnemy.Name}!"));
                }
            }
            else
            {
                isDefeated = player.DealDamage(enemyManager.CurrentEnemy);
                if (!enemyManager.CurrentEnemy.CurrentLife.Equals(healthBefore))
                {
                    GameEvent?.Invoke(this, new CControllerEventArgs($"Вы нанесли {player.Damage} урона {enemyManager.CurrentEnemy.Name}!"));
                }
            }

            if (isDefeated)
            {
                player.AddGold(enemyManager.CurrentEnemy.Gold);
                SpawnEnemy();
                collectableController.Clear();
            }
        }

        public void HandleCollectableClick(Point mousePosition)
        {
            var clickedCollectable = collectableController.HandleClick(mousePosition, player, enemyManager);
            if (clickedCollectable != null)
            {
            }
        }

        public bool BuyDamageUpgrade()
        {
            if (player.BuyUpgrade())
            {
                GameEvent?.Invoke(this, new CControllerEventArgs($"Улучшение урона! Уровень: {player.Level}"));
                return true;
            }
            return false;
        }

        public bool BuyCooldownUpgrade()
        {
            if (player.BuyCooldownUpgrade())
            {
                GameEvent?.Invoke(this, new CControllerEventArgs($"Улучшение перезарядки! Уровень: {player.CooldownLevel}"));
                return true;
            }
            return false;
        }

        #endregion

        #region Внутренние методы

        private void SpawnEnemy()
        {
            var enemy = enemyManager.GetNextEnemyWithModification(player.Level);
            if (enemy != null)
            {
                if (enemy is CHealingEnemy healingEnemy)
                {
                    healingEnemy.Healed += OnEnemyHealed;
                }
            }
        }

        private void TrySpawnCollectable()
        {
            if (collectableController.Collectables.Count >= maxObjectsOnScene)
                return;

            
        }

        #endregion

        #region Обработчики событий от компонентов

        private void OnEnemySpawned(object sender, CControllerEventArgs e)
        {
            ObjectSpawned?.Invoke(this, e);
            GameEvent?.Invoke(this, e);
        }

        private void OnEnemyDefeated(object sender, CControllerEventArgs e)
        {
            ObjectDestroyed?.Invoke(this, e);
            GameEvent?.Invoke(this, e);
        }

        private void OnEnemyReplaced(object sender, CControllerEventArgs e)
        {
            GameEvent?.Invoke(this, e);
        }

        private void OnEnemyDamaged(object sender, CControllerEventArgs e)
        {
            GameEvent?.Invoke(this, e);
        }

        private void OnPointsEarned(object sender, CControllerEventArgs e)
        {
            GameEvent?.Invoke(this, e);
        }

        private void OnBonusSpawned(object sender, CControllerEventArgs e)
        {
            ObjectSpawned?.Invoke(this, e);
            GameEvent?.Invoke(this, e);
        }

        private void OnBonusCollected(object sender, CControllerEventArgs e)
        {
            ObjectDestroyed?.Invoke(this, e);
            GameEvent?.Invoke(this, e);
        }

        private void OnBonusExpired(object sender, CControllerEventArgs e)
        {
            ObjectDestroyed?.Invoke(this, e);
            GameEvent?.Invoke(this, e);
        }

        private void OnEnemyHealed(object sender, HealingEventArgs e)
        {
            if (sender is CHealingEnemy healingEnemy)
            {
                GameEvent?.Invoke(this, new CControllerEventArgs(
                    $"{healingEnemy.Name} восстановил {e.HealAmount} здоровья!")
                );
            }
        }

        #endregion

        #region Публичные свойства для доступа к компонентам

        public CPlayer Player => player;
        public CEnemyManager EnemyManager => enemyManager;
        public CCollectableController CollectableController => collectableController;
        public IEnemy CurrentEnemy => enemyManager.CurrentEnemy;

        #endregion
    }
}