using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace EnemyEditor
{
    public class CIconList
    {
        private List<CIcon> icons = new List<CIcon>();
        private int iconWidth;
        private int iconHeight;
        private Canvas parentCanvas;

        public CIconList(Canvas canvas, int iconWidth, int iconHeight)
        {
            parentCanvas = canvas;
            this.iconWidth = iconWidth;
            this.iconHeight = iconHeight;
        }

        public void Load(string folderPath)
        {
            icons.Clear();
            parentCanvas.Children.Clear();

            if (!Directory.Exists(folderPath)) return;

            string filter = "*.png";
            string[] files = Directory.GetFiles(folderPath, filter);

            int x = 20;
            int y = 20;
            int xStep = iconWidth + 20;
            int yStep = iconHeight + 20;
            int iconsPerRow = 6; 

            for (int i = 0; i < files.Length; i++)
            {
                CIcon newIcon = new CIcon(iconWidth, iconHeight, files[i]);

                int row = i / iconsPerRow;
                int col = i % iconsPerRow;

                int posX = 20 + col * xStep;
                int posY = 20 + row * yStep;

                newIcon.SetPosition(new Point(posX, posY));
                icons.Add(newIcon);
                parentCanvas.Children.Add(newIcon.GetIcon());
            }

            int totalRows = (files.Length + iconsPerRow - 1) / iconsPerRow;
            parentCanvas.Height = 20 + totalRows * yStep;
            parentCanvas.Width = 20 + iconsPerRow * xStep;
        }

        public List<CIcon> GetIcons() => icons;

        public CIcon IsMouseOver(Point mousePosition)
        {
            foreach (var icon in icons)
            {
                if (icon.IsMouseOver(mousePosition))
                {
                    return icon;
                }
            }
            return null;
        }
    }
}