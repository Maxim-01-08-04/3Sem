using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;



namespace EnemyEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CIconList iconList;
        private CEnemyTemplateList enemyList;
        private string selectedIconName = "";

        public MainWindow()
        {
            InitializeComponent();
            iconList = new CIconList(IconsCanvasMain);
            enemyList = new CEnemyTemplateList();
            UpdateEnemiesList();

            string iconsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "icons");
            if (Directory.Exists(iconsPath))
            {
                iconList.LoadIcons(iconsPath);
            }

            UpdateEnemiesList(); 
        }

           


        

        private void btnLoadIcons_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                iconList.LoadIcons(dialog.SelectedPath);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(IconsCanvasMain);
            selectedIconName = iconList.GetIconNameAtPoint(mousePosition);
            if (selectedIconName != null)
            {
                txtIconName.Text = selectedIconName;
            }
        }

        private void btnAddEnemy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEnemyName.Text) || string.IsNullOrWhiteSpace(txtIconName.Text))
            {
                System.Windows.MessageBox.Show("Заполните название и выберите иконку!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBaseLife.Text) || string.IsNullOrWhiteSpace(txtBaseGold.Text) ||
                string.IsNullOrWhiteSpace(txtLifeModifier.Text) || string.IsNullOrWhiteSpace(txtGoldModifier.Text) ||
                string.IsNullOrWhiteSpace(txtSpawnChance.Text))
            {
                MessageBox.Show("Заполните все числовые поля!");
                return;
            }

            try
            {
                CEnemyTemplate enemy = new CEnemyTemplate(
                    txtEnemyName.Text,
                    txtIconName.Text,
                    int.Parse(txtBaseLife.Text),
                    double.Parse(txtLifeModifier.Text),
                    int.Parse(txtBaseGold.Text),
                    double.Parse(txtGoldModifier.Text),
                    double.Parse(txtSpawnChance.Text)
                );

                enemyList.AddEnemy(enemy);
                UpdateEnemiesList();
                ClearForm();
            }
            catch
            {
                System.Windows.MessageBox.Show("Ошибка в формате данных!");
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lstEnemies.SelectedIndex != -1)
            {
                enemyList.RemoveEnemyAt(lstEnemies.SelectedIndex);
                UpdateEnemiesList();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "enemies";
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON files (.json)|*.json";

            if (dialog.ShowDialog() == true)
            {
                enemyList.SaveToFile(dialog.FileName);
                System.Windows.MessageBox.Show("Список сохранен!");
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "enemies";
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON files (.json)|*.json";

            if (dialog.ShowDialog() == true)
            {
                enemyList.LoadFromFile(dialog.FileName);
                UpdateEnemiesList();
                System.Windows.MessageBox.Show("Список загружен!");
            }
        }

        private void UpdateEnemiesList()
        {
            lstEnemies.Items.Clear();
            foreach (var enemy in enemyList.GetEnemies())
            {
                lstEnemies.Items.Add($"{enemy.GetName()} (Иконка: {enemy.GetIconName()})");
            }
        }

        private void ClearForm()
        {
            txtEnemyName.Clear();
            txtIconName.Clear();
            txtBaseLife.Clear();
            txtLifeModifier.Clear();
            txtBaseGold.Clear();
            txtGoldModifier.Clear();
            txtSpawnChance.Clear();
            selectedIconName = "";
        }
    }
}
