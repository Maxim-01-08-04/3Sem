using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EnemyEditor
{
    public class CCollectableController
    {
        public delegate void CollectableEvent(object sender, CControllerEventArgs e);

        public event CollectableEvent BonusSpawned;
        public event CollectableEvent BonusCollected;
        public event CollectableEvent BonusExpired;
        public event CollectableEvent PointsEarned;

        private List<CCollectable> collectables;
        private Random random;
        private double spawnTimer;
        private double spawnInterval = 3.0;

        public CCollectableController()
        {
            collectables = new List<CCollectable>();
            random = new Random();
            spawnTimer = 0;
        }

        public List<CCollectable> Collectables => collectables;

        public void Update(double deltaTime, CPlayer player, CEnemyManager enemyManager)
        {
            spawnTimer += deltaTime;
            if (spawnTimer >= spawnInterval && collectables.Count < 3)
            {
                SpawnRandomCollectable(player);
                spawnTimer = 0;
            }

            for (int i = collectables.Count - 1; i >= 0; i--)
            {
                if (collectables[i].UpdateLifetime(deltaTime))
                {
                    BonusExpired?.Invoke(this, new CControllerEventArgs($"Бонус исчез!"));
                    collectables.RemoveAt(i);
                }
            }
        }

        public CCollectable HandleClick(Point mousePosition, CPlayer player, CEnemyManager enemyManager)
        {
            for (int i = collectables.Count - 1; i >= 0; i--)
            {
                if (collectables[i].IsMouseOnObject(mousePosition))
                {
                    if (collectables[i].OnClick(player, enemyManager, mousePosition))
                    {
                        var clicked = collectables[i];

                        BonusCollected?.Invoke(this, new CControllerEventArgs($"Бонус собран: {clicked.GetType().Name}"));

                        collectables.RemoveAt(i);
                        return clicked;
                    }
                }
            }
            return null;
        }

        private void SpawnRandomCollectable(CPlayer player)
        {
            double x = 280 + random.NextDouble() * 80;
            double y = 150 + random.NextDouble() * 150;

            Point spawnPosition = new Point(x, y);
            double size = 30;
            double lifetime = 8;

            CCollectable collectable = null;

            int effectType = random.Next(3);
            switch (effectType)
            {
                case 0:
                    collectable = new CDamageBooster(spawnPosition, size, lifetime, 2.5, 6.0);
                    break;
                case 1:
                    collectable = new CCooldownReducer(spawnPosition, size, lifetime, 0.3, 5.0);
                    break;
                case 2:
                    BigNumber goldAmount = new BigNumber(30 + player.Level * 10);
                    collectable = new CGoldGiver(spawnPosition, size, lifetime, goldAmount);
                    break;
            }

            if (collectable != null)
            {
                collectables.Add(collectable);

                BonusSpawned?.Invoke(this, new CControllerEventArgs($"Появился новый бонус!"));
            }
        }

        public void OnPointsEarned(BigNumber points, string source)
        {
            PointsEarned?.Invoke(this, new CControllerEventArgs(points)
            {
                Message = $"Получено очков: {points} ({source})"
            });
        }

        public void Clear()
        {
            collectables.Clear();
        }
    }
}