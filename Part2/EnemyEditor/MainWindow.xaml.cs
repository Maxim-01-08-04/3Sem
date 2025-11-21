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
    public partial class MainWindow : Window
    {
        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private List<EnemyIcon> enemyIcons = new List<EnemyIcon>();
        private string selectedIconName = "";

        public MainWindow()
        {
            InitializeComponent();
            UpdateEnemiesList();
        }

        private void LoadIconsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Выберите папку с иконками";
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = "Выберите папку";

            if (dialog.ShowDialog() == true)
            {
                string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                LoadIconsFromFolder(folderPath);
            }
        }

        public void LoadIconsFromFolder(string path)
        {
            try
            {
                enemyIcons.Clear();
                IconsListBox.Items.Clear();

                string filter = "*.png";
                string[] files = System.IO.Directory.GetFiles(path, filter);

                foreach (string file in files)
                {
                    var icon = new EnemyIcon
                    {
                        Name = System.IO.Path.GetFileName(file),
                        ImagePath = file
                    };
                    enemyIcons.Add(icon);

                    // Создаем Image для ListBox
                    var image = new Image()
                    {
                        Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri(icon.ImagePath)),
                        Height = 50,
                        Tag = icon.Name // Сохраняем имя иконки в Tag
                    };
                    IconsListBox.Items.Add(image);
                }

                MessageBox.Show($"Загружено {files.Length} иконок", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки иконок: {ex.Message}", "Ошибка");
            }
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IconsListBox.SelectedItem is Image selectedImage && selectedImage != null)
            {
                // Обновляем главную иконку
                MainEnemyIcon.Source = selectedImage.Source;

                // Получаем имя иконки из Tag
                selectedIconName = selectedImage.Tag as string;
                IconNameTextBlock.Text = selectedIconName;
            }
        }

        private void AddEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Введите название противника", "Ошибка");
                    return;
                }

                if (string.IsNullOrEmpty(selectedIconName))
                {
                    MessageBox.Show("Выберите иконку", "Ошибка");
                    return;
                }

                // Проверяем и устанавливаем значения по умолчанию для пустых полей
                int health = string.IsNullOrWhiteSpace(HealthTextBox.Text) ? 100 : int.Parse(HealthTextBox.Text);
                double healthMod = string.IsNullOrWhiteSpace(HealthModTextBox.Text) ? 1.0 : double.Parse(HealthModTextBox.Text);
                int gold = string.IsNullOrWhiteSpace(GoldTextBox.Text) ? 10 : int.Parse(GoldTextBox.Text);
                double goldMod = string.IsNullOrWhiteSpace(GoldModTextBox.Text) ? 1.0 : double.Parse(GoldModTextBox.Text);
                double spawnChance = string.IsNullOrWhiteSpace(SpawnChanceTextBox.Text) ? 0.5 : double.Parse(SpawnChanceTextBox.Text);

                enemyList.AddEnemy(
                    NameTextBox.Text,
                    selectedIconName,
                    health,
                    healthMod,
                    gold,
                    goldMod,
                    spawnChance
                );

                UpdateEnemiesList();
                ClearForm();
                MessageBox.Show("Противник добавлен", "Успех");
            }
            catch (FormatException)
            {
                MessageBox.Show("Проверьте правильность введенных числовых значений", "Ошибка формата");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog();
                dialog.FileName = "enemies";
                dialog.DefaultExt = ".json";
                dialog.Filter = "JSON files (.json)|*.json";

                if (dialog.ShowDialog() == true)
                {
                    enemyList.SaveToJson(dialog.FileName);
                    MessageBox.Show("Список сохранен", "Успех");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.DefaultExt = ".json";
                dialog.Filter = "JSON files (.json)|*.json";

                if (dialog.ShowDialog() == true)
                {
                    enemyList.LoadFromJson(dialog.FileName);
                    UpdateEnemiesList();
                    MessageBox.Show("Список загружен", "Успех");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
            }
        }

        private void DeleteEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedIndex != -1)
            {
                enemyList.DeleteEnemyByIndex(EnemiesListBox.SelectedIndex);
                UpdateEnemiesList();
            }
            else
            {
                MessageBox.Show("Выберите противника для удаления", "Ошибка");
            }
        }

        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemiesListBox.SelectedIndex != -1)
            {
                var enemy = enemyList.GetEnemyByIndex(EnemiesListBox.SelectedIndex);
                if (enemy != null)
                {
                    // Можно добавить функционал просмотра деталей выбранного противника
                }
            }
        }

        private void UpdateEnemiesList()
        {
            EnemiesListBox.ItemsSource = null;
            EnemiesListBox.ItemsSource = enemyList.GetEnemies();
        }

        private void ClearForm()
        {
            NameTextBox.Clear();
            HealthTextBox.Clear();
            HealthModTextBox.Clear();
            GoldTextBox.Clear();
            GoldModTextBox.Clear();
            SpawnChanceTextBox.Clear();
            IconNameTextBlock.Text = "";
            selectedIconName = "";
            MainEnemyIcon.Source = null;
        }
    }
}