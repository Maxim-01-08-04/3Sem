using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace EnemyEditor
{
    public class CTimeExtender : CCollectable
    {
        private double lifetimeMultiplier;
        private double effectDuration;

        public CTimeExtender(Point position, double size, double lifetime, double multiplier, double duration)
            : base(position, size, lifetime)
        {
            this.lifetimeMultiplier = multiplier;
            this.effectDuration = duration;
            sprite.Fill = Brushes.Green;
        }

        public override void OnClick(Player player, GameController controller)
        {
            controller.ActivateLifetimeBoost(lifetimeMultiplier, effectDuration);
        }
    }
}
