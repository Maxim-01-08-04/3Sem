using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private string currentEnemyName = "";
        private List<string> availableIcons = new List<string>();
        private string currentEnemyType = "Нормальный";

        public MainWindow()
        {
            InitializeComponent();
            UpdateEnemiesListBox();
            InitializeEnemyTypeComboBox();
            UpdateSpecialPropertyVisibility();
        }

        private void InitializeEnemyTypeComboBox()
        {
            EnemyTypeComboBox.Items.Add("Нормальный");
            EnemyTypeComboBox.Items.Add("Бронированный");
            EnemyTypeComboBox.Items.Add("Уворачивающийся");
            EnemyTypeComboBox.Items.Add("Исцеляющийся");
            EnemyTypeComboBox.SelectedIndex = 0;
        }

        private void EnemyTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemyTypeComboBox.SelectedItem != null)
            {
                currentEnemyType = EnemyTypeComboBox.SelectedItem.ToString();
                UpdateSpecialPropertyVisibility();
            }
        }

        private void UpdateSpecialPropertyVisibility()
        {
            switch (currentEnemyType)
            {
                case "Бронированный":
                    SpecialPropertyTextBlock.Text = "Броня:";
                    SpecialPropertyTextBox.Text = "25";
                    SpecialPropertyPanel.Visibility = Visibility.Visible;
                    break;
                case "Уворачивающийся":
                    SpecialPropertyTextBlock.Text = "Шанс уворота:";
                    SpecialPropertyTextBox.Text = "0.8";
                    SpecialPropertyPanel.Visibility = Visibility.Visible;
                    break;
                case "Исцеляющийся":
                    SpecialPropertyTextBlock.Text = "Шанс исцеления:";
                    SpecialPropertyTextBox.Text = "0.3";
                    SpecialPropertyPanel.Visibility = Visibility.Visible;
                    break;
                default:
                    SpecialPropertyPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void LoadIconsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                if (!Directory.Exists(assetsPath))
                {
                    MessageBox.Show("Папка 'Assets' не найдена! Создайте ее и добавьте файлы в формате PNG.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                availableIcons.Clear();
                string[] pngFiles = Directory.GetFiles(assetsPath, "*.png");
                foreach (string file in pngFiles)
                    availableIcons.Add(Path.GetFileNameWithoutExtension(file));

                if (availableIcons.Count == 0)
                {
                    MessageBox.Show("В папке Assets не найдено файлов формата PNG!", "Инфо",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var iconDialog = new IconSelectionDialog(availableIcons);
                if (iconDialog.ShowDialog() == true)
                {
                    IconNameTextBox.Text = iconDialog.SelectedIcon;
                    UpdateEnemyIconPreview();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки иконки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateEnemyIconPreview()
        {
            if (!string.IsNullOrEmpty(IconNameTextBox.Text))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", IconNameTextBox.Text + ".png");
                if (File.Exists(imagePath))
                {
                    try
                    {
                        EnemyIconImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath));
                    }
                    catch
                    {
                        EnemyIconImage.Source = null;
                    }
                }
                else
                {
                    EnemyIconImage.Source = null;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EnemyNameTextBox.Text) || EnemyNameTextBox.Text == "Враг")
            {
                MessageBox.Show("Введите действительное имя врага!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(IconNameTextBox.Text))
            {
                MessageBox.Show("Сначала выберите значок!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (enemyList.GetEnemyByName(EnemyNameTextBox.Text) != null)
            {
                MessageBox.Show("Враг с таким именем уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TryGetCurrentValues(out int baseLife, out int baseGold, out double lifeMod,
                                   out double goldMod, out double spawnChance, out double specialValue))
            {
                switch (currentEnemyType)
                {
                    case "Нормальный":
                        enemyList.AddNormalEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0);
                        break;
                    case "Бронированный":
                        enemyList.AddArmoredEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0, specialValue);
                        break;
                    case "Уворачивающийся":
                        enemyList.AddShrinkingEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0, specialValue);
                        break;
                    case "Исцеляющийся":
                        enemyList.AddHealingEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0, specialValue);
                        break;
                }

                UpdateEnemiesListBox();
                MessageBox.Show($"Враг '{EnemyNameTextBox.Text}' добавлено успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentEnemyName))
            {
                MessageBox.Show("Сначала выбери врага!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TryGetCurrentValues(out int baseLife, out int baseGold, out double lifeMod,
                                   out double goldMod, out double spawnChance, out double specialValue))
            {
                var enemy = enemyList.GetEnemyByName(currentEnemyName);
                if (enemy != null)
                {
                    enemyList.DeleteEnemyByName(currentEnemyName);

                    switch (currentEnemyType)
                    {
                        case "Нормальный":
                            enemyList.AddNormalEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0);
                            break;
                        case "Бронированный":
                            enemyList.AddArmoredEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0, specialValue);
                            break;
                        case "Уворачивающийся":
                            enemyList.AddShrinkingEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0, specialValue);
                            break;
                        case "Исцеляющийся":
                            enemyList.AddHealingEnemy(EnemyNameTextBox.Text, IconNameTextBox.Text, baseLife, lifeMod, baseGold, goldMod, spawnChance / 100.0, specialValue);
                            break;
                    }

                    UpdateEnemiesListBox();
                    if (currentEnemyName != EnemyNameTextBox.Text)
                        currentEnemyName = EnemyNameTextBox.Text;

                    MessageBox.Show("Изменения успешно применены!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private bool TryGetCurrentValues(out int baseLife, out int baseGold, out double lifeMod,
                                        out double goldMod, out double spawnChance, out double specialValue)
        {
            baseLife = 0; baseGold = 0; lifeMod = 0; goldMod = 0; spawnChance = 0; specialValue = 0;

            if (!int.TryParse(BaseLifeTextBox.Text, out baseLife) || baseLife <= 0)
            {
                MessageBox.Show("Базовое здоровье должно быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(BaseGoldTextBox.Text, out baseGold) || baseGold <= 0)
            {
                MessageBox.Show("Базовое золото должно быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(LifeModifierTextBox.Text, out lifeMod) || lifeMod <= 0)
            {
                MessageBox.Show("Модификатор здоровья должен быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(GoldModifierTextBox.Text, out goldMod) || goldMod <= 0)
            {
                MessageBox.Show("Модификатор золота должен быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(SpawnChanceTextBox.Text, out spawnChance) || spawnChance < 1 || spawnChance > 100)
            {
                MessageBox.Show("Шанс появления должен быть от 1 до 100!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (SpecialPropertyPanel.Visibility == Visibility.Visible)
            {
                if (!double.TryParse(SpecialPropertyTextBox.Text, out specialValue))
                {
                    MessageBox.Show($"{SpecialPropertyTextBlock.Text.Replace(":", "")} должен быть действительный номер!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemiesListBox.SelectedIndex != -1 && EnemiesListBox.SelectedItem is string enemyName)
            {
                currentEnemyName = enemyName;
                var enemy = enemyList.GetEnemyByName(enemyName);

                if (enemy != null)
                {
                    EnemyNameTextBox.Text = enemy.Name;
                    IconNameTextBox.Text = enemy.IconName;
                    BaseLifeTextBox.Text = enemy.Baselife.ToString();
                    BaseGoldTextBox.Text = enemy.BaseGold.ToString();
                    LifeModifierTextBox.Text = enemy.LifeModifier.ToString("F1");
                    GoldModifierTextBox.Text = enemy.GoldModifier.ToString("F1");
                    SpawnChanceTextBox.Text = (enemy.SpawnChance * 100).ToString("F0");

                    if (enemy is CArmoredEnemyTemplate armored)
                    {
                        EnemyTypeComboBox.SelectedItem = "Бронированный";
                        SpecialPropertyTextBox.Text = armored.Armor.ToString("F1");
                        SpecialPropertyPanel.Visibility = Visibility.Visible;
                    }
                    else if (enemy is CShrinkingEnemyTemplate shrinking)
                    {
                        EnemyTypeComboBox.SelectedItem = "Уворачивающийся";
                        SpecialPropertyTextBox.Text = shrinking.ShrinkFactor.ToString("F1");
                        SpecialPropertyPanel.Visibility = Visibility.Visible;
                    }
                    else if (enemy is CHealingEnemyTemplate healing)
                    {
                        EnemyTypeComboBox.SelectedItem = "Исцеляющийся";
                        SpecialPropertyTextBox.Text = healing.HealChance.ToString("F1");
                        SpecialPropertyPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        EnemyTypeComboBox.SelectedItem = "Нормальный";
                        SpecialPropertyPanel.Visibility = Visibility.Collapsed;
                    }

                    UpdateEnemyIconPreview();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (enemyList.GetListOfEnemyNames().Count == 0)
            {
                MessageBox.Show("Нет врагов для сохранения!", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "JSON files (*.json)|*.json";
            saveDialog.DefaultExt = ".json";
            saveDialog.FileName = "enemies.json";

            if (saveDialog.ShowDialog() == true)
            {
                enemyList.SaveToJson(saveDialog.FileName); 
                MessageBox.Show($"Враг сохранен в: {saveDialog.FileName}", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "JSON files (*.json)|*.json";
            openDialog.DefaultExt = ".json";

            if (openDialog.ShowDialog() == true)
            {
                enemyList.LoadFromJson(openDialog.FileName); 
                UpdateEnemiesListBox();
                MessageBox.Show($"Враг загружен из: {openDialog.FileName}", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedIndex != -1 && EnemiesListBox.SelectedItem is string enemyName)
            {
                var result = MessageBox.Show($"Удалить врага '{enemyName}'?", "Подтвердить",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    enemyList.DeleteEnemyByName(enemyName);
                    UpdateEnemiesListBox();
                    ClearFields();
                    currentEnemyName = "";
                }
            }
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new Window2(enemyList);
            gameWindow.Show();
            this.Hide();
            gameWindow.Closed += (s, args) => this.Show();
        }

        private void UpdateEnemiesListBox()
        {
            EnemiesListBox.ItemsSource = null;
            EnemiesListBox.ItemsSource = enemyList.GetListOfEnemyNames();
        }

        private void ClearFields()
        {
            EnemyNameTextBox.Text = "Enemy";
            IconNameTextBox.Text = "";
            BaseLifeTextBox.Text = "100";
            BaseGoldTextBox.Text = "50";
            LifeModifierTextBox.Text = "1";
            GoldModifierTextBox.Text = "1";
            SpawnChanceTextBox.Text = "50";
            EnemyIconImage.Source = null;
            EnemyTypeComboBox.SelectedIndex = 0;
            SpecialPropertyTextBox.Text = "0";
        }
    }
}