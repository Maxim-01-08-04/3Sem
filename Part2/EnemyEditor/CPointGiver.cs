using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace EnemyEditor
{
    public class CPointGiver : CCollectable
    {
        private double pointValue;

        public CPointGiver(Point position, double size, double lifetime, double pointValue)
            : base(position, size, lifetime)
        {
            this.pointValue = pointValue;
            sprite.Fill = Brushes.Gold;
        }

        public override void OnClick(Player player, GameController controller)
        {
            player.AddGold(new BigNumber(pointValue.ToString()));
        }

        public override double GetPointsValue()
        {
            return pointValue;
        }
    }
}
