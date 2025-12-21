using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CArmoredEnemy : CEnemy
    {
        public double Armor { get; private set; }

        public CArmoredEnemy(string name, BigNumber baseLife, BigNumber gold, string iconName, double armor)
            : base(name, baseLife, gold, iconName)
        {
            Armor = armor;
        }

        public override void TakeDamage(BigNumber damage)
        {
            BigNumber actualDamage = damage;

            if (Armor > 0)
            {
                double reduction = Armor / (Armor + 100);
                actualDamage = damage.Multiply(1 - reduction);
            }

            base.TakeDamage(actualDamage);
        }
    }
}
