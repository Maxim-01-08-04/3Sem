using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Geometry
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Triangle triangle;
        private Rectangle rectangle;
        private Random rnd;

        public MainWindow()
        {
            InitializeComponent();
            rnd = new Random();
        }

        public void DrawLine(Point2D p1, Point2D p2, Brush color)
        {  //создаем новый объект и указываем цвет и толщину
            Line line = new Line();
            line.Stroke = color;
            line.StrokeThickness = 2;
            line.X1 = p1.getX();
            line.Y1 = p1.getY();
            line.X2 = p2.getX();
            line.Y2 = p2.getY();
            Scene.Children.Add(line);
        }

        // ффункция рисования треугольника
        public void DrawTriangle(Triangle tr)
        {
            DrawLine(tr.getP1(), tr.getP2(), Brushes.Red);
            DrawLine(tr.getP2(), tr.getP3(), Brushes.Red);
            DrawLine(tr.getP3(), tr.getP1(), Brushes.Red);
        }

        // функция рисования прямоугольника
        public void DrawRectangle(Rectangle rect)
        {
            DrawLine(rect.getP1(), rect.getP2(), Brushes.Blue);
            DrawLine(rect.getP2(), rect.getP3(), Brushes.Blue);
            DrawLine(rect.getP3(), rect.getP4(), Brushes.Blue);
            DrawLine(rect.getP4(), rect.getP1(), Brushes.Blue);
        }

        
        public void ClearScene()
        {
            Scene.Children.Clear();
        }

        //  фигуры со случайными параметрами
        private void btnCreateRandom_Click(object sender, RoutedEventArgs e)
        {
            ClearScene();

            
            Point2D p1 = new Point2D(rnd.Next(50, (int)Scene.ActualWidth - 50),
                                    rnd.Next(50, (int)Scene.ActualHeight - 50));
            Point2D p2 = new Point2D(rnd.Next(50, (int)Scene.ActualWidth - 50),
                                    rnd.Next(50, (int)Scene.ActualHeight - 50));
            Point2D p3 = new Point2D(rnd.Next(50, (int)Scene.ActualWidth - 50),
                                    rnd.Next(50, (int)Scene.ActualHeight - 50));
            triangle = new Triangle(p1, p2, p3);

            
            Point2D startPoint = new Point2D(rnd.Next(50, (int)Scene.ActualWidth - 150),
                                           rnd.Next(50, (int)Scene.ActualHeight - 150));
            int width = rnd.Next(50, 150);
            int height = rnd.Next(50, 150);
            rectangle = new Rectangle(startPoint, width, height);

            // отрисовка фигур
            DrawTriangle(triangle);
            DrawRectangle(rectangle);
        }

        // создание фигур с пользовательскими параметрами
        private void btnCreateCustom_Click(object sender, RoutedEventArgs e)
        {
            
            var triangleDialog = new InputDialog();
            if (triangleDialog.ShowDialog() == true)
            {
                try
                {
                    string[] coords = triangleDialog.Answer.Split(',');
                    Point2D p1 = new Point2D(int.Parse(coords[0]), int.Parse(coords[1]));
                    Point2D p2 = new Point2D(int.Parse(coords[2]), int.Parse(coords[3]));
                    Point2D p3 = new Point2D(int.Parse(coords[4]), int.Parse(coords[5]));
                    triangle = new Triangle(p1, p2, p3);
                }
                catch
                {
                    MessageBox.Show("Ошибка в формате данных для треугольника!");
                    return;
                }
            }

            
            var rectDialog = new InputDialog();
            if (rectDialog.ShowDialog() == true)
            {
                try
                {
                    string[] paramsRect = rectDialog.Answer.Split(',');
                    rectangle = new Rectangle(int.Parse(paramsRect[0]), int.Parse(paramsRect[1]),
                                            int.Parse(paramsRect[2]), int.Parse(paramsRect[3]));
                }
                catch
                {
                    MessageBox.Show("Ошибка в формате данных для прямоугольника!");
                    return;
                }
            }

            ClearScene();
            DrawTriangle(triangle);
            DrawRectangle(rectangle);
        }

        
        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            if (triangle == null || rectangle == null)
            {
                MessageBox.Show("Сначала создайте фигуры!");
                return;
            }

            var moveDialog = new InputDialog();
            if (moveDialog.ShowDialog() == true)
            {
                //try
                //{
                //    string[] move = moveDialog.Answer.Split(',');
                //    int deltaX = int.Parse(move[0]);
                //    int deltaY = int.Parse(move[1]);

                //    triangle.addX(deltaX);
                //    triangle.addY(deltaY);
                //    rectangle.addX(deltaX);
                //    rectangle.addY(deltaY);

                //    ClearScene();
                //    DrawTriangle(triangle);
                //    DrawRectangle(rectangle);
                //}
                //catch
                //{
                //    MessageBox.Show("Ошибка в формате данных для перемещения!");
                //}
            }
        }

        
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearScene();
            triangle = null;
            rectangle = null;
        }

        private void x_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scene.Children.Clear();
            triangle.addX((int)e.OldValue - (int)e.NewValue * -1);
            rectangle.addX((int)e.OldValue - (int)e.NewValue * -1);
            DrawTriangle(triangle);
            DrawRectangle(rectangle);
        }

        private void y_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scene.Children.Clear();
            triangle.addY((int)e.OldValue - (int)e.NewValue * -1);
            rectangle.addY((int)e.OldValue - (int)e.NewValue * -1);
            DrawTriangle(triangle);
            DrawRectangle(rectangle);
        }
    }
}

