
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace EnemyEditor
{
    public class GameController
    {
        private List<CCollectable> collectables;
        private Random random;
        private Size sceneSize;
        private DispatcherTimer spawnTimer;
        private DispatcherTimer updateTimer;

        private double damageMultiplier = 1.0;
        private double cooldownMultiplier = 1.0;
        private double lifetimeMultiplier = 1.0;

        private double damageBoostRemaining = 0;
        private double cooldownBoostRemaining = 0;
        private double lifetimeBoostRemaining = 0;

        private double spawnRate = 3.0; 
        private double timeSinceLastSpawn = 0;

        private double minSpriteSize = 20;
        private double maxSpriteSize = 40;
        private double minLifetime = 5;
        private double maxLifetime = 10;

        public List<CCollectable> Collectables => collectables;
        public double DamageMultiplier => damageMultiplier;
        public double CooldownMultiplier => cooldownMultiplier;
        public double LifetimeMultiplier => lifetimeMultiplier;

        public GameController(Size sceneSize)
        {
            this.sceneSize = sceneSize;
            collectables = new List<CCollectable>();
            random = new Random();

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            spawnTimer = new DispatcherTimer();
            spawnTimer.Interval = TimeSpan.FromSeconds(1);
            spawnTimer.Tick += SpawnTimer_Tick;

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            updateTimer.Tick += UpdateTimer_Tick;
        }

        public void StartGame()
        {
            spawnTimer.Start();
            updateTimer.Start();
        }

        public void StopGame()
        {
            spawnTimer.Stop();
            updateTimer.Stop();
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
        {
            timeSinceLastSpawn += 1;

            if (timeSinceLastSpawn >= spawnRate)
            {
                SpawnCollectable();
                timeSinceLastSpawn = 0;
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateCollectables(0.1);
            UpdateEffects(0.1);
        }

        private void SpawnCollectable()
        {
            double x = random.NextDouble() * (sceneSize.Width - maxSpriteSize);
            double y = random.NextDouble() * (sceneSize.Height - maxSpriteSize);
            Point position = new Point(x, y);

            double size = random.NextDouble() * (maxSpriteSize - minSpriteSize) + minSpriteSize;
            double lifetime = random.NextDouble() * (maxLifetime - minLifetime) + minLifetime;

            int type = random.Next(100);
            CCollectable collectable = null;

            if (type < 30) 
            {
                double goldValue = random.Next(1, 10);
                collectable = new CPointGiver(position, size, lifetime, goldValue);
            }
            else if (type < 55) 
            {
                double multiplier = 1.5 + random.NextDouble() * 0.5; 
                double duration = 10 + random.NextDouble() * 10; 
                collectable = new CDamageBooster(position, size, lifetime, multiplier, duration);
            }
            else if (type < 80) 
            {
                double reduction = 0.5 + random.NextDouble() * 0.3; 
                double duration = 8 + random.NextDouble() * 8; 
                collectable = new CCooldownReducer(position, size, lifetime, reduction, duration);
            }
            else 
            {
                double multiplier = 1.3 + random.NextDouble() * 0.4; 
                double duration = 12 + random.NextDouble() * 12; 
                collectable = new CTimeExtender(position, size, lifetime, multiplier, duration);
            }

            if (collectable != null)
            {
                collectables.Add(collectable);
            }
        }

        private void UpdateCollectables(double deltaTime)
        {
            for (int i = collectables.Count - 1; i >= 0; i--)
            {
                if (collectables[i].UpdateLifetime(deltaTime))
                {
                    collectables.RemoveAt(i);
                }
            }
        }

        private void UpdateEffects(double deltaTime)
        {
            if (damageBoostRemaining > 0)
            {
                damageBoostRemaining -= deltaTime;
                if (damageBoostRemaining <= 0)
                {
                    damageMultiplier = 1.0;
                }
            }

            if (cooldownBoostRemaining > 0)
            {
                cooldownBoostRemaining -= deltaTime;
                if (cooldownBoostRemaining <= 0)
                {
                    cooldownMultiplier = 1.0;
                }
            }

            if (lifetimeBoostRemaining > 0)
            {
                lifetimeBoostRemaining -= deltaTime;
                if (lifetimeBoostRemaining <= 0)
                {
                    lifetimeMultiplier = 1.0;
                }
            }
        }

        public void HandleClick(Point mousePosition, Player player)
        {
            for (int i = collectables.Count - 1; i >= 0; i--)
            {
                if (collectables[i].IsMouseOnObject(mousePosition))
                {
                    collectables[i].OnClick(player, this);
                    collectables.RemoveAt(i);
                    break; 
                }
            }
        }

        public void ActivateDamageBoost(double multiplier, double duration)
        {
            damageMultiplier = multiplier;
            damageBoostRemaining = duration;
        }

        public void ActivateCooldownReduction(double multiplier, double duration)
        {
            cooldownMultiplier = multiplier;
            cooldownBoostRemaining = duration;
        }

        public void ActivateLifetimeBoost(double multiplier, double duration)
        {
            lifetimeMultiplier = multiplier;
            lifetimeBoostRemaining = duration;
        }

        public void ClearCollectables()
        {
            collectables.Clear();
        }
    }
}