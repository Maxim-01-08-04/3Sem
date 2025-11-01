using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Geometry
{
    public class Rectangle
    {
        private Point2D startPoint;
        private int width;
        private int height;
        private Point2D p1;
        private Point2D p2;
        private Point2D p3;
        private Point2D p4;

        public Rectangle(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;  
        }

        //public Rectangle(int x, int y, int width, int height)
        //{
        //    this.startPoint = new Point2D(x, y);
        //    this.width = width;
        //    this.height = height;
        //}

        public Point2D getStartPoint() { return startPoint; }
        public int getWidth() { return width; }
        public int getHeight() { return height; }

        //public Point2D getP1() { return startPoint; }
        //public Point2D getP2() { return new Point2D(startPoint.getX(), startPoint.getY()); }
        //public Point2D getP3() { return new Point2D(startPoint.getX(), startPoint.getY()); }
        //public Point2D getP4() { return new Point2D(startPoint.getX(), startPoint.getY()); }
        public Point2D getP1() { return p1; }
        public Point2D getP2() { return p2; }
        public Point2D getP3() { return p3; }
        public Point2D getP4() { return p4; }

        public void addX(int X)
        {
            p1.addX(X);
            p2.addX(X);
            p3.addX(X);
            p4.addX(X);
        }

        public void addY(int Y)
        {
            p1.addY(Y);
            p2.addY(Y);
            p3.addY(Y);
            p4.addY(Y);
        }

        public void setPosition(int x, int y)
        {
            startPoint.setX(x);
            startPoint.setY(y);
        }
    }
}
