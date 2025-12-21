using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace EnemyEditor
{
    public abstract class CCollectable
    {
        protected Point position;
        protected Size size;
        protected double lifetime;
        protected Shape sprite;
        protected double currentLifetime;

        public CCollectable(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = new Size(size, size);
            this.lifetime = lifetime;
            this.currentLifetime = lifetime;
        }

        public bool IsMouseOnObject(Point mousePosition)
        {
            return (mousePosition.X >= position.X && mousePosition.X <= position.X + size.Width &&
                    mousePosition.Y >= position.Y && mousePosition.Y <= position.Y + size.Height);
        }

        public Shape GetSprite() => sprite;

        public bool UpdateLifetime(double delta)
        {
            currentLifetime -= delta;
            return currentLifetime <= 0;
        }

        public abstract bool OnClick(CPlayer player, CEnemyManager enemyManager, Point mousePosition);
    }
}