using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace EnemyEditor
{
    public class CCooldownReducer : CCollectable
    {
        private double cooldownReduction;
        private double effectDuration;

        public CCooldownReducer(Point position, double size, double lifetime, double reduction, double duration)
            : base(position, size, lifetime)
        {
            this.cooldownReduction = reduction;
            this.effectDuration = duration;
            sprite.Fill = Brushes.Blue;
        }

        public override void OnClick(Player player, GameController controller)
        {
            controller.ActivateCooldownReduction(cooldownReduction, effectDuration);
        }
    }
}
