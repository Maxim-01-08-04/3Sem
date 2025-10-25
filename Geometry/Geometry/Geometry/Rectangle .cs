using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Rectangle
    {
        private Point2D startPoint;
        private int width;
        private int height;

        public Rectangle(Point2D startPoint, int width, int height)
        {
            this.startPoint = startPoint;
            this.width = width;
            this.height = height;
        }

        public Rectangle(int x, int y, int width, int height)
        {
            this.startPoint = new Point2D(x, y);
            this.width = width;
            this.height = height;
        }

        public Point2D getStartPoint() { return startPoint; }
        public int getWidth() { return width; }
        public int getHeight() { return height; }

        public Point2D getP1() { return startPoint; }
        public Point2D getP2() { return new Point2D(startPoint.getX() + width, startPoint.getY()); }
        public Point2D getP3() { return new Point2D(startPoint.getX() + width, startPoint.getY() + height); }
        public Point2D getP4() { return new Point2D(startPoint.getX(), startPoint.getY() + height); }

        public void addX(int x)
        {
            startPoint.addX(x);
        }

        public void addY(int y)
        {
            startPoint.addY(y);
        }

        public void setPosition(int x, int y)
        {
            startPoint.setX(x);
            startPoint.setY(y);
        }
    }
}
