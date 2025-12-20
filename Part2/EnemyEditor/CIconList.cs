using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace EnemyEditor
{
    public class CIconList
    {
        private List<CIcon> icons;
        private Canvas canvas;
        private int iconWidth;
        private int iconHeight;

        public CIconList(Canvas targetCanvas, int width = 64, int height = 64)
        {
            icons = new List<CIcon>();
            canvas = targetCanvas;
            iconWidth = width;
            iconHeight = height;

            canvas.Height = 150;
        }

        public void LoadIcons(string folderPath)
        {
            ClearIcons();

            if (!Directory.Exists(folderPath)) return;

            string[] pngFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

            int x = 10;
            int y = 10;
            int iconsPerRow = 5;

            for (int i = 0; i < pngFiles.Length; i++)
            {
                string iconName = Path.GetFileNameWithoutExtension(pngFiles[i]);
                CIcon icon = new CIcon(iconWidth, iconHeight, pngFiles[i], iconName);

                icon.SetPosition(x, y);
                icons.Add(icon);
                canvas.Children.Add(icon.GetRectangle());

                x += iconWidth + 10;
                if ((i + 1) % iconsPerRow == 0)
                {
                    x = 10;
                    y += iconHeight + 10;
                }
            }
        }

        public void ClearIcons()
        {
            foreach (var icon in icons)
            {
                canvas.Children.Remove(icon.GetRectangle());
            }
            icons.Clear();
        }

        public string GetIconNameAtPoint(Point point)
        {
            foreach (var icon in icons)
            {
                if (icon.ContainsPoint(point))
                {
                    return icon.GetName();
                }
            }
            return null;
        }

        public List<CIcon> GetIcons() => icons;
    }
}
