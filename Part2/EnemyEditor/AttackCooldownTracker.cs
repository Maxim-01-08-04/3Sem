using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class AttackCooldownTracker
    {
        private double baseCooldown;
        private double currentCooldown;
        private bool isReady;

        public AttackCooldownTracker(double cooldown)
        {
            baseCooldown = cooldown;
            currentCooldown = 0;
            isReady = true;
        }

        public bool IsReady => isReady;
        public double Progress => (baseCooldown - currentCooldown) / baseCooldown * 100;

        public void StartCooldown()
        {
            isReady = false;
            currentCooldown = baseCooldown;
        }

        public void Update(double deltaTime)
        {
            if (!isReady)
            {
                currentCooldown -= deltaTime;
                if (currentCooldown <= 0)
                {
                    currentCooldown = 0;
                    isReady = true;
                }
            }
        }

        public void ApplyMultiplier(double multiplier)
        {
            baseCooldown *= multiplier;
            if (currentCooldown > 0)
            {
                currentCooldown *= multiplier;
            }
        }
    }
}
