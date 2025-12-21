using System;

namespace EnemyEditor
{
    public class CHealingEnemy : CEnemy
    {
        public double HealChance { get; private set; }
        private Random random = new Random();

        public CHealingEnemy(string name, BigNumber baseLife, BigNumber gold, string iconName, double healChance)
            : base(name, baseLife, gold, iconName)
        {
            HealChance = healChance;
        }

        public override void TakeDamage(BigNumber damage)
        {
            var healthBefore = CurrentLife;

            base.TakeDamage(damage);

            if (!IsDefeated() && random.NextDouble() < HealChance && healthBefore.GreaterThan(CurrentLife))
            {
                BigNumber healAmount = baseLife.Multiply(0.2); 
                var healBefore = CurrentLife;
                CurrentLife = CurrentLife.Add(healAmount);
                if (CurrentLife.GreaterThan(baseLife))
                    CurrentLife = baseLife;

                if (CurrentLife.GreaterThan(healBefore))
                {
                    OnHealed(CurrentLife.Subtract(healBefore), healBefore);
                }
            }
        }

        public event EventHandler<HealingEventArgs> Healed;

        protected virtual void OnHealed(BigNumber healAmount, BigNumber healthBefore)
        {
            Healed?.Invoke(this, new HealingEventArgs(healAmount, healthBefore, CurrentLife));
        }

        public override void ScaleWithPlayerLevel(double lifeMultiplier, double goldMultiplier)
        {
            double healthRatio = currentLife.ToDouble() / baseLife.ToDouble();

            baseLife = baseLife.Multiply(lifeMultiplier);
            currentLife = baseLife.Multiply(healthRatio);
            gold = gold.Multiply(goldMultiplier);

            HealChance = Math.Min(0.5, HealChance * 1.03);

            OnPropertyChanged(nameof(BaseLife));
            OnPropertyChanged(nameof(CurrentLife));
            OnPropertyChanged(nameof(Gold));
            OnPropertyChanged(nameof(HealthPercentage));
        }
    }

    public class HealingEventArgs : EventArgs
    {
        public BigNumber HealAmount { get; }
        public BigNumber HealthBefore { get; }
        public BigNumber HealthAfter { get; }

        public HealingEventArgs(BigNumber healAmount, BigNumber healthBefore, BigNumber healthAfter)
        {
            HealAmount = healAmount;
            HealthBefore = healthBefore;
            HealthAfter = healthAfter;
        }
    }
}