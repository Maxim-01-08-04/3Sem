using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Point2D
    {   
        //делаем поля для храннеия координат
        private int X;
        private int Y;

        public Point2D(int x, int y)  //передаем координаты
        {
            this.X = x;
            this.Y = y;
        }

        public int getX()
        {
            return X;
        }

        public int getY()
        {
            return Y;
        }

        public void addX(int x)
        {
            X += x;  //изменяем координаты ч на указанное значение
        }

        public void addY(int y)
        {
            Y += y;
        }

        public void setX(int x)
        {
            X = x; //устанвока абсолют значения координат
        }

        public void setY(int y)
        {
            Y = y;
        }
    }
}

