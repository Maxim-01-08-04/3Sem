using System;

namespace EnemyEditor
{
    public class CShrinkingEnemy : CEnemy
    {
        public double DodgeChance { get; private set; }
        private Random random;

        public CShrinkingEnemy(string name, BigNumber baseLife, BigNumber gold, string iconName, double dodgeChance)
            : base(name, baseLife, gold, iconName)
        {
            DodgeChance = dodgeChance;
            random = new Random();
        }

        public override void TakeDamage(BigNumber damage)
        {
            if (random.NextDouble() < DodgeChance)
            {
                return;
            }

            base.TakeDamage(damage);
        }

        public override void ScaleWithPlayerLevel(double lifeMultiplier, double goldMultiplier)
        {
            double healthRatio = currentLife.ToDouble() / baseLife.ToDouble();

            baseLife = baseLife.Multiply(lifeMultiplier);
            currentLife = baseLife.Multiply(healthRatio);
            gold = gold.Multiply(goldMultiplier);

            DodgeChance = Math.Min(0.8, DodgeChance * 1.05);

            OnPropertyChanged(nameof(BaseLife));
            OnPropertyChanged(nameof(CurrentLife));
            OnPropertyChanged(nameof(Gold));
            OnPropertyChanged(nameof(HealthPercentage));
        }
    }
}