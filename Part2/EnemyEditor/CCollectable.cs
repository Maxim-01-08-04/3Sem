using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace EnemyEditor
{
    public abstract class CCollectable
    {
        protected Point position;
        protected Size size;
        protected double lifetime;
        protected double currentLifetime;
        protected Ellipse sprite;
        protected bool isActive;

        public Point Position => position;
        public Ellipse Sprite => sprite;
        public bool IsActive => isActive;
        public double CurrentLifetime => currentLifetime;

        public CCollectable(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = new Size(size, size);
            this.lifetime = lifetime;
            this.currentLifetime = lifetime;
            this.isActive = true;

            CreateSprite();
        }

        protected virtual void CreateSprite()
        {
            sprite = new Ellipse();
            sprite.Fill = Brushes.BlueViolet;
            sprite.StrokeThickness = 2;
            sprite.Stroke = Brushes.Black;
            sprite.HorizontalAlignment = HorizontalAlignment.Center;
            sprite.VerticalAlignment = VerticalAlignment.Center;
            sprite.Width = this.size.Width;
            sprite.Height = this.size.Height;
        }

        public bool IsMouseOnObject(Point mousePosition)
        {
            double distance = Point.Subtract(position, mousePosition).Length;
            return distance <= size.Width / 2;
        }

        public bool UpdateLifetime(double deltaTime)
        {
            if (!isActive) return false;

            currentLifetime -= deltaTime;
            if (currentLifetime <= 0)
            {
                isActive = false;
                return true; 
            }
            return false;
        }

        public abstract void OnClick(Player player, GameController controller);

        public virtual double GetPointsValue()
        {
            return 0;
        }
    }
}
