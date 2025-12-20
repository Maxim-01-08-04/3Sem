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

        private Player player;
        private Enemy currentEnemy;
        private EnemyManager enemyManager;
        private GameController gameController;
        private DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            InitializeGame();
            UpdateEnemiesList();
            InitializeCollectables();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCollectables();
        }

        private void InitializeGame()
        {
            player = new Player();
            enemyManager = new EnemyManager();

            if (enemyList.GetEnemies().Count > 0)
            {
                enemyManager.LoadTemplates(enemyList.GetEnemies());
                currentEnemy = enemyManager.GetRandomEnemy();
            }
            else
            {
                currentEnemy = new Enemy("Слайм", new BigNumber("10"), new BigNumber("5"), "default.png");
            }

            UpdateGameUI();
        }

        private void InitializeCollectables()
        {
            try
            {
                if (EnemyArea != null)
                {
                    double areaWidth = EnemyArea.ActualWidth;
                    double areaHeight = EnemyArea.ActualHeight;

                    if (areaWidth <= 0) areaWidth = 400;
                    if (areaHeight <= 0) areaHeight = 400;

                    Size sceneSize = new Size(areaWidth - 50, areaHeight - 50);
                    gameController = new GameController(sceneSize);
                    gameController.StartGame();
                }

                gameTimer = new DispatcherTimer();
                gameTimer.Interval = TimeSpan.FromMilliseconds(100);
                gameTimer.Tick += GameTimer_Tick;
                gameTimer.Start();

                if (GameCanvas != null)
                {
                    GameCanvas.MouseDown += GameCanvas_MouseDown;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации бонусов: {ex.Message}");
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            player.UpdateCooldown(0.1);

            UpdateCooldownUI();

            UpdateActiveEffects();

            DrawCollectables();
        }

        private void UpdateCooldownUI()
        {
            if (CooldownProgressBar != null && CooldownText != null)
            {
                CooldownProgressBar.Value = player.CooldownProgress;
                CooldownText.Text = player.CanAttack ? "Готово!" :
                    $"Перезарядка: {player.CurrentCooldown:F1}с";
                AttackButton.IsEnabled = player.CanAttack && !currentEnemy.IsDead;
            }
        }

        private void UpdateActiveEffects()
        {
            if (ActiveEffectsPanel == null) return;

            ActiveEffectsPanel.Children.Clear();

            if (gameController != null)
            {
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
            if (GameCanvas == null || gameController == null) return;

            GameCanvas.Children.Clear();

            foreach (var collectable in gameController.Collectables)
            {
                var ellipse = collectable.Sprite;
                Canvas.SetLeft(ellipse, collectable.Position.X);
                Canvas.SetTop(ellipse, collectable.Position.Y);
                GameCanvas.Children.Add(ellipse);
            }
        }
        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gameController == null) return;

            var position = e.GetPosition(GameCanvas);

            gameController.HandleClick(position, player);

            UpdateGameUI();
            UpdateActiveEffects();
        }

        private void EnemyArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(EnemyImage.Parent as IInputElement);

            gameController.HandleClick(position, player);

            if (player.CanAttack && !currentEnemy.IsDead)
            {
                AttackButton_Click(sender, null);
            }
        }

        private void UpgradeCooldownButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigNumber cost = new BigNumber("100");
                if (player.Gold >= cost)
                {
                    player.AddGold(new BigNumber("-100")); 

                    

                    player.ApplyCooldownMultiplier(0.9); 
                    MessageBox.Show($"Перезарядка улучшена!\nНовая перезарядка: {player.AttackCooldown:F2}с", "Успех");
                    UpdateGameUI();
                }
                else
                {
                    MessageBox.Show($"Недостаточно золота!\nНужно: {cost}, есть: {player.Gold}", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка улучшения: {ex.Message}", "Ошибка");
            }
        }

        private void UpdateGameUI()
        {
            try
            {
                PlayerLevelText.Text = player.Lvl.ToString();
                PlayerGoldText.Text = player.Gold.ToString();
                PlayerDamageText.Text = player.Damage.ToString();
                UpgradeCostText.Text = player.UpgradeCost.ToString();
                UpgradeInfoText.Text = $"Стоимость улучшения: {player.UpgradeCost} золота";

                EnemyNameText.Text = currentEnemy.Name;
                EnemyHealthText.Text = $"{currentEnemy.CurrentHitPoints}/{currentEnemy.MaxHitPoints}";

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

                UpgradeButton.IsEnabled = player.Gold >= player.UpgradeCost;
                AttackButton.IsEnabled = !currentEnemy.IsDead && player.CanAttack;
                UpgradeCooldownButton.IsEnabled = player.Gold >= new BigNumber("100");

                if (currentEnemy.IsDead)
                {
                    AttackButton.Content = "Противник побежден!";
                    AttackButton.Background = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    AttackButton.Content = "Атаковать!";
                    AttackButton.Background = new SolidColorBrush(Colors.Red);
                }

                UpdateCooldownUI();
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
                BigNumber finalDamage;

                if (gameController != null)
                {
                    finalDamage = baseDamage * gameController.DamageMultiplier;
                }
                else
                {
                    finalDamage = baseDamage;
                }

                bool isDefeated = currentEnemy.TakeDamage(finalDamage, out BigNumber reward);

                if (isDefeated)
                {
                    player.AddGold(reward);
                    MessageBox.Show($"Противник '{currentEnemy.Name}' побежден!\nПолучено {reward} золота.", "Победа!");

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
                BigNumber oldDamage = player.Damage.Clone();

                if (player.TryUpgrade())
                {
                    BigNumber newDamage = player.Damage;

                    string debugInfo = $"Уровень повышен до {player.Lvl}!\n" +
                                      $"Старый урон: {oldDamage}\n" +
                                      $"Новый урон: {newDamage}\n" +
                                      $"Разница: {(newDamage > oldDamage ? "Увеличен" : "Не изменился")}";

                    MessageBox.Show(debugInfo, "Улучшение");

                    UpdateGameUI();
                }
                else
                {
                    MessageBox.Show($"Недостаточно золота для улучшения!\nНужно: {player.UpgradeCost}, есть: {player.Gold}", "Ошибка");
                }
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

                    try
                    {
                        var image = new Image()
                        {
                            Source = new BitmapImage(new Uri(icon.ImagePath)),
                            Height = 50,
                            Tag = icon.Name 
                        };
                        IconsListBox.Items.Add(image);
                    }
                    catch
                    {
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
                MainEnemyIcon.Source = selectedImage.Source;

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
        }

        private void UpdateEnemiesList()
        {
            EnemiesListBox.ItemsSource = null;

            var enemies = enemyList.GetEnemies();
            var enemyDisplayList = enemies.Select(e => new
            {
                Name = e.Name, 
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