using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace EnemyEditor
{
    public class CDamageBooster : CCollectable
    {
        private double damageMultiplier;
        private double effectDuration;

        public CDamageBooster(Point position, double size, double lifetime, double multiplier, double duration)
            : base(position, size, lifetime)
        {
            this.damageMultiplier = multiplier;
            this.effectDuration = duration;
            sprite.Fill = Brushes.Red;
        }

        public override void OnClick(Player player, GameController controller)
        {
            controller.ActivateDamageBoost(damageMultiplier, effectDuration);
        }
    }
}
