using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace EnemyEditor
{
    public class CDamageBooster : CCollectable
    {
        private double damageMultiplier;
        private double effectDuration;

        public CDamageBooster(Point position, double size, double lifetime,
                            double damageMultiplier = 2.0, double effectDuration = 10.0)
                            : base(position, size, lifetime)
        {
            this.damageMultiplier = damageMultiplier;
            this.effectDuration = effectDuration;

            sprite = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new RadialGradientBrush(Colors.Red, Colors.DarkRed),
                Stroke = Brushes.DarkRed,
                StrokeThickness = 2,
                RenderTransform = new TranslateTransform(position.X, position.Y)
            };
        }

        public override bool OnClick(CPlayer player, CEnemyManager enemyManager, Point mousePosition)
        {
            if (!IsMouseOnObject(mousePosition)) return false;

            player.ApplyDamageBoost(damageMultiplier, effectDuration);
            return true;
        }
    }

    public class CCooldownReducer : CCollectable
    {
        private double cooldownMultiplier;
        private double effectDuration;

        public CCooldownReducer(Point position, double size, double lifetime,
                              double cooldownMultiplier = 0.5, double effectDuration = 8.0)
                              : base(position, size, lifetime)
        {
            this.cooldownMultiplier = cooldownMultiplier;
            this.effectDuration = effectDuration;

            sprite = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new RadialGradientBrush(Colors.Blue, Colors.DarkBlue),
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 2,
                RenderTransform = new TranslateTransform(position.X, position.Y)
            };
        }

        public override bool OnClick(CPlayer player, CEnemyManager enemyManager, Point mousePosition)
        {
            if (!IsMouseOnObject(mousePosition)) return false;

            player.ApplyCooldownReduction(cooldownMultiplier, effectDuration);
            return true;
        }
    }

    public class CGoldGiver : CCollectable
    {
        private BigNumber goldAmount;

        public CGoldGiver(Point position, double size, double lifetime, BigNumber goldAmount)
                        : base(position, size, lifetime)
        {
            this.goldAmount = goldAmount;

            sprite = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new RadialGradientBrush(Colors.Gold, Colors.Orange),
                Stroke = Brushes.Orange,
                StrokeThickness = 2,
                RenderTransform = new TranslateTransform(position.X, position.Y)
            };
        }

        public override bool OnClick(CPlayer player, CEnemyManager enemyManager, Point mousePosition)
        {
            if (!IsMouseOnObject(mousePosition)) return false;

            player.AddGold(goldAmount);
            return true;
        }
    }
}