using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public interface IEnemy : INotifyPropertyChanged
    {
        string Name { get; }
        BigNumber BaseLife { get; }
        BigNumber CurrentLife { get; }
        BigNumber Gold { get; }
        string IconName { get; }
        double HealthPercentage { get; }

        void TakeDamage(BigNumber damage);
        bool IsDefeated();
        void ResetHealth();
        void ScaleWithPlayerLevel(double lifeMultiplier, double goldMultiplier);
    }
}
