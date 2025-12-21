using System;
using System.ComponentModel;

namespace EnemyEditor
{
    public abstract class CEnemy : IEnemy
    {
        protected string name;
        protected BigNumber baseLife;
        protected BigNumber currentLife;
        protected BigNumber gold;
        protected string iconName;

        public event PropertyChangedEventHandler PropertyChanged;

        public CEnemy(string name, BigNumber baseLife, BigNumber gold, string iconName)
        {
            this.name = name;
            this.baseLife = baseLife;
            this.currentLife = baseLife;
            this.gold = gold;
            this.iconName = iconName;
        }

        public string Name => name;
        public BigNumber BaseLife => baseLife;
        public BigNumber CurrentLife
        {
            get => currentLife;
            protected set
            {
                currentLife = value;
                OnPropertyChanged(nameof(CurrentLife));
                OnPropertyChanged(nameof(HealthPercentage));
            }
        }
        public BigNumber Gold => gold;
        public string IconName => iconName;

        public double HealthPercentage
        {
            get
            {
                if (baseLife.Equals(new BigNumber(0))) return 0;
                return (currentLife.ToDouble() / baseLife.ToDouble()) * 100.0;
            }
        }

        public virtual void TakeDamage(BigNumber damage)
        {
            if (IsDefeated()) return;

            CurrentLife = CurrentLife.Subtract(damage);
            if (CurrentLife.LessThan(new BigNumber(0)))
            {
                CurrentLife = new BigNumber(0);
            }
        }

        public bool IsDefeated()
        {
            return CurrentLife.LessThanOrEqual(new BigNumber(0));
        }

        public void ResetHealth()
        {
            CurrentLife = baseLife;
        }

        public virtual void ScaleWithPlayerLevel(double lifeMultiplier, double goldMultiplier)
        {
            double healthRatio = currentLife.ToDouble() / baseLife.ToDouble();

            baseLife = baseLife.Multiply(lifeMultiplier);
            currentLife = baseLife.Multiply(healthRatio);
            gold = gold.Multiply(goldMultiplier);

            OnPropertyChanged(nameof(BaseLife));
            OnPropertyChanged(nameof(CurrentLife));
            OnPropertyChanged(nameof(Gold));
            OnPropertyChanged(nameof(HealthPercentage));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}