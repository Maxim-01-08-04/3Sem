using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Triangle
    {
        private Point2D p1;
        private Point2D p2;
        private Point2D p3;
        public Triangle(Point2D p1, Point2D p2, Point2D p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public Point2D getP1() { return p1; }
        public Point2D getP2() { return p2; }
        public Point2D getP3() { return p3; }

        public void addX(int X)
        {
            p1.addX(X);
            p2.addX(X);
            p3.addX(X);
        }

        public void addY(int Y)
        {
            p1.addY(Y);
            p2.addY(Y);
            p3.addY(Y);
        }

        public void setPosition(int x, int y)
        {
            int centerX = (p1.getX() + p2.getX() + p3.getX()) / 3;
            int centerY = (p1.getY() + p2.getY() + p3.getY()) / 3;

            int deltaX = x - centerX;
            int deltaY = y - centerY;

            addX(deltaX);
            addY(deltaY);
        }
    }
}
