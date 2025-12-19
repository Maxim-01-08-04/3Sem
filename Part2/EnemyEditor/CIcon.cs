using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Shapes;

namespace EnemyEditor
{
    public class CIcon
    {
        private Point position;
        private string name;
        private Rectangle icon;
        private int width;
        private int height;

        public CIcon(int iconWidth, int iconHeight, string imagePath, string iconName)
        {
            width = iconWidth;
            height = iconHeight;
            name = iconName;
            position = new Point(0, 0);
            CreateIcon(imagePath);
        }

        private void CreateIcon(string imagePath)
        {
            icon = new Rectangle();
            icon.Stroke = Brushes.Black;
            icon.StrokeThickness = 1;

            ImageBrush ib = new ImageBrush();
            ib.AlignmentX = AlignmentX.Left;
            ib.AlignmentY = AlignmentY.Top;
            ib.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            icon.RenderTransform = new TranslateTransform(position.X, position.Y);
            icon.Fill = ib;

            icon.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            icon.VerticalAlignment = VerticalAlignment.Top;
            icon.Height = height;
            icon.Width = width;
        }

        public void SetPosition(double x, double y)
        {
            position = new Point(x, y);
            icon.RenderTransform = new TranslateTransform(x, y);
        }

        public bool ContainsPoint(Point point)
        {
            return point.X >= position.X && point.X <= position.X + width &&
                   point.Y >= position.Y && point.Y <= position.Y + height;
        }

        public Rectangle GetRectangle() => icon;
        public string GetName() => name;
        public Point GetPosition() => position;
        public int GetWidth() => width;
        public int GetHeight() => height;
    }
}
