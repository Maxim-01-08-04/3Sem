using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CNormalEnemy : CEnemy
    {
        public CNormalEnemy(string name, BigNumber baseLife, BigNumber gold, string iconName)
            : base(name, baseLife, gold, iconName) { }
    }
}
