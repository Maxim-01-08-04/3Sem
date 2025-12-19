using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Microsoft.Win32;



namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private List<EnemyIcon> enemyIcons = new List<EnemyIcon>();
        private string selectedIconName = "";

        // Игровые объекты
        private Player player;
        private Enemy currentEnemy;
        private EnemyManager enemyManager;
        private GameController gameController;
        private DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            UpdateEnemiesList();
        }

        private void InitializeGame()
        {
            player = new Player();
            enemyManager = new EnemyManager();
            InitializeCollectables();

            // Загружаем существующие шаблоны если они есть
            if (enemyList.GetEnemies().Count > 0)
            {
                enemyManager.LoadTemplates(enemyList.GetEnemies());
                currentEnemy = enemyManager.GetRandomEnemy();
            }
            else
            {
                // Создаем противника по умолчанию
                currentEnemy = new Enemy("Слайм", new BigNumber("50"), new BigNumber("5"), "default.png");
            }

            UpdateGameUI();
        }
        private void InitializeCollectables()
        {
            // Получаем размер области противника для спавна бонусов
            var enemyArea = EnemyImage.Parent as Border;
            if (enemyArea != null)
            {
                Size sceneSize = new Size(enemyArea.ActualWidth, enemyArea.ActualHeight);
                gameController = new GameController(sceneSize);
                gameController.StartGame();
            }
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Обновляем перезарядку игрока
            player.UpdateCooldown(0.1);

            // Обновляем UI перезарядки
            UpdateCooldownUI();

            // Обновляем активные эффекты
            UpdateActiveEffects();

            // Отрисовываем собираемые объекты
            DrawCollectables();
        }

        private void UpdateCooldownUI()
        {
            CooldownProgressBar.Value = player.CooldownProgress;
            CooldownText.Text = player.CanAttack ? "Готово!" :
                $"Перезарядка: {player.CurrentCooldown:F1}с";
            AttackButton.IsEnabled = player.CanAttack && !currentEnemy.IsDead;
        }

        private void UpdateActiveEffects()
        {
            ActiveEffectsPanel.Children.Clear();

            // Создаем визуальные элементы для активных эффектов
            if (gameController.DamageMultiplier > 1.0)
            {
                var effect = CreateEffectBadge("Урон x" + gameController.DamageMultiplier.ToString("F1"), Brushes.Red);
                ActiveEffectsPanel.Children.Add(effect);
            }

            if (gameController.CooldownMultiplier < 1.0)
            {
                var effect = CreateEffectBadge("Перезар. x" + gameController.CooldownMultiplier.ToString("F1"), Brushes.Blue);
                ActiveEffectsPanel.Children.Add(effect);
            }

            if (gameController.LifetimeMultiplier > 1.0)
            {
                var effect = CreateEffectBadge("Время x" + gameController.LifetimeMultiplier.ToString("F1"), Brushes.Green);
                ActiveEffectsPanel.Children.Add(effect);
            }
        }

        private Border CreateEffectBadge(string text, Brush color)
        {
            return new Border
            {
                Background = color,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(5, 2, 5, 2),
                Margin = new Thickness(2),
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = Brushes.White,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                }
            };
        }

        private void DrawCollectables()
        {
            // Очищаем старые объекты
            var enemyArea = EnemyImage.Parent as Border;
            var canvas = enemyArea?.Child as Canvas;

            if (canvas == null)
            {
                canvas = new Canvas();
                if (enemyArea != null)
                {
                    enemyArea.Child = canvas;
                    var stackPanel = new StackPanel();
                    stackPanel.Children.Add(EnemyImage);
                    stackPanel.Children.Add(canvas);
                    enemyArea.Child = stackPanel;
                }
            }

            canvas.Children.Clear();

            // Добавляем все собираемые объекты
            foreach (var collectable in gameController.Collectables)
            {
                var ellipse = collectable.Sprite;
                Canvas.SetLeft(ellipse, collectable.Position.X);
                Canvas.SetTop(ellipse, collectable.Position.Y);
                canvas.Children.Add(ellipse);
            }

            // Добавляем EnemyImage поверх всего
            canvas.Children.Add(EnemyImage);
        }

        // Обработчик клика по области противника
        private void EnemyArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(EnemyImage.Parent as IInputElement);

            // Проверяем клик по собираемым объектам
            gameController.HandleClick(position, player);

            // Если кликнули по самому противнику
            if (player.CanAttack && !currentEnemy.IsDead)
            {
                AttackButton_Click(sender, null);
            }
        }

        // Обработчик улучшения перезарядки
        private void UpgradeCooldownButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgradeCooldown())
            {
                MessageBox.Show($"Перезарядка улучшена!\nНовая перезарядка: {player.AttackCooldown:F2}с", "Успех");
                UpdateGameUI();
            }
            else
            {
                MessageBox.Show("Недостаточно золота!", "Ошибка");
            }
        }

        private void UpdateGameUI()
        {
            try
            {
                // Обновляем статистику игрока
                PlayerLevelText.Text = player.Lvl.ToString();
                PlayerGoldText.Text = player.Gold.ToString();
                PlayerDamageText.Text = player.Damage.ToString();
                UpgradeCostText.Text = player.UpgradeCost.ToString();
                UpgradeInfoText.Text = $"Стоимость улучшения: {player.UpgradeCost} золота";

                // Обновляем информацию о противнике
                EnemyNameText.Text = currentEnemy.Name;
                EnemyHealthText.Text = $"{currentEnemy.CurrentHitPoints}/{currentEnemy.MaxHitPoints}";

                // Обновляем прогресс бар
                if (!currentEnemy.MaxHitPoints.IsZero())
                {
                    double maxHp = double.Parse(currentEnemy.MaxHitPoints.ToString());
                    double currentHp = double.Parse(currentEnemy.CurrentHitPoints.ToString());
                    double healthPercentage = (currentHp / maxHp) * 100;
                    EnemyHealthBar.Value = healthPercentage;
                }
                else
                {
                    EnemyHealthBar.Value = 0;
                }

                // Обновляем кнопки
                UpgradeButton.IsEnabled = player.Gold >= player.UpgradeCost;
                AttackButton.IsEnabled = !currentEnemy.IsDead;

                // Визуальная обратная связь
                if (currentEnemy.IsDead)
                {
                    AttackButton.Content = "Противник побежден!";
                    AttackButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                }
                else
                {
                    AttackButton.Content = "Атаковать!";
                    AttackButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления интерфейса: {ex.Message}", "Ошибка");
            }
        }

        #region Игровые методы

        private void AttackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentEnemy.IsDead)
                {
                    currentEnemy = enemyManager.GetRandomEnemy();
                    UpdateGameUI();
                    return;
                }

                BigNumber baseDamage = player.DealDamage();
                BigNumber finalDamage = baseDamage * gameController.DamageMultiplier;

                bool isDefeated = currentEnemy.TakeDamage(finalDamage, out BigNumber reward);

                if (isDefeated)
                {
                    player.AddGold(reward);
                    MessageBox.Show($"Противник '{currentEnemy.Name}' побежден!\nПолучено {reward} золота.", "Победа!");

                    // Создаем нового противника
                    currentEnemy = enemyManager.GetRandomEnemy();
                }

                UpdateGameUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка атаки: {ex.Message}", "Ошибка");
            }
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (player.TryUpgrade())
                {
                    MessageBox.Show($"Уровень повышен до {player.Lvl}!\nНовый урон: {player.Damage}", "Улучшение");
                }
                else
                {
                    MessageBox.Show($"Недостаточно золота для улучшения!\nНужно: {player.UpgradeCost}, есть: {player.Gold}", "Ошибка");
                }

                UpdateGameUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка улучшения: {ex.Message}", "Ошибка");
            }
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (enemyList.GetEnemies().Count > 0)
                {
                    enemyManager.LoadTemplates(enemyList.GetEnemies());
                    currentEnemy = enemyManager.GetRandomEnemy();
                    UpdateGameUI();
                    MessageBox.Show($"Загружено {enemyList.GetEnemies().Count} противников в игру!", "Успех");
                }
                else
                {
                    MessageBox.Show("Сначала создайте противников в редакторе!", "Информация");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки в игру: {ex.Message}", "Ошибка");
            }
        }

        private void ResetGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите сбросить прогресс игры?", "Сброс игры",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    player = new Player();
                    currentEnemy = enemyManager.GetRandomEnemy();
                    UpdateGameUI();
                    MessageBox.Show("Игра сброшена!", "Успех");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сброса игры: {ex.Message}", "Ошибка");
            }
        }

        #endregion

        #region Методы редактора противников

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

                string[] supportedFormats = { "*.png", "*.jpg", "*.jpeg", "*.bmp" };
                List<string> files = new List<string>();

                foreach (string format in supportedFormats)
                {
                    files.AddRange(System.IO.Directory.GetFiles(path, format));
                }

                foreach (string file in files)
                {
                    var icon = new EnemyIcon
                    {
                        Name = System.IO.Path.GetFileName(file),
                        ImagePath = file
                    };
                    enemyIcons.Add(icon);

                    // Создаем Image для ListBox
                    try
                    {
                        var image = new Image()
                        {
                            Source = new BitmapImage(new Uri(icon.ImagePath)),
                            Height = 50,
                            Tag = icon.Name // Сохраняем имя иконки в Tag
                        };
                        IconsListBox.Items.Add(image);
                    }
                    catch
                    {
                        // Пропускаем изображения, которые не удалось загрузить
                        continue;
                    }
                }

                MessageBox.Show($"Загружено {files.Count} иконок", "Успех");
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

                    // Обновляем менеджер врагов в игре
                    enemyManager.LoadTemplates(enemyList.GetEnemies());
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
            // Можно добавить функционал просмотра деталей выбранного противника
        }

        private void UpdateEnemiesList()
        {
            EnemiesListBox.ItemsSource = null;

            // Используем свойство вместо метода
            var enemies = enemyList.GetEnemies();
            var enemyDisplayList = enemies.Select(e => new
            {
                Name = e.Name, // Используем свойство вместо метода
                Health = e.Baselife(),
                Gold = e.BaseGold(),
                SpawnChance = e.SpawnChance()
            }).ToList();

            EnemiesListBox.ItemsSource = enemyDisplayList;
            EnemiesListBox.DisplayMemberPath = "Name";
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

        #endregion
    }
}