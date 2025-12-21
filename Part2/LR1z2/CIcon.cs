using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EnemyEditor
{
    public class CIcon
    {
        private string name;
        private int iconWidth;
        private int iconHeight;
        private Point position;
        private Rectangle icon;
        public event MouseButtonEventHandler MouseDown;

        public CIcon(int iconWidth, int iconHeight, string imagePath)
        {
            icon.MouseDown += (sender, e) => MouseDown?.Invoke(sender, e);
            this.iconWidth = iconWidth;
            this.iconHeight = iconHeight;
            name = System.IO.Path.GetFileNameWithoutExtension(imagePath);
            position = new Point(0, 0);

            icon = new Rectangle();
            icon.Stroke = Brushes.Black;
            icon.StrokeThickness = 1;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(imagePath));
            imageBrush.Stretch = Stretch.Uniform;

            icon.Fill = imageBrush;
            icon.Width = iconWidth;
            icon.Height = iconHeight;

            UpdatePosition(position);
        }

        public string Name() => name;
        public double X() => position.X;
        public double Y() => position.Y;
        public int IconWidth() => iconWidth;
        public int IconHeight() => iconHeight;
        public Rectangle GetIcon() => icon;

        public void SetPosition(Point newPosition)
        {
            position = newPosition;
            UpdatePosition(position);
        }

        private void UpdatePosition(Point newPosition)
        {
            icon.RenderTransform = new TranslateTransform(newPosition.X, newPosition.Y);
        }

        public bool IsMouseOver(Point mousePosition)
        {
            return (mousePosition.X >= position.X && mousePosition.X <= position.X + iconWidth &&
                    mousePosition.Y >= position.Y && mousePosition.Y <= position.Y + iconHeight);
        }
    }
}