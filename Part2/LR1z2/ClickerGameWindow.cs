using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;

namespace EnemyEditor
{
    public partial class ClickerGameWindow : Window, INotifyPropertyChanged
    {
        private CPlayer player;
        private CEnemyManager enemyManager;
        private CEnemyTemplateList templateList;
        private CCollectableController collectableController;
        private int repeatCount = 5;
        private DispatcherTimer gameTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public int RepeatCount
        {
            get => repeatCount;
            set
            {
                repeatCount = value;
                OnPropertyChanged(nameof(RepeatCount));
            }
        }

        public ClickerGameWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeGame();
        }

        private void InitializeGame()
        {
            player = new CPlayer();
            enemyManager = new CEnemyManager();
            templateList = new CEnemyTemplateList();

            collectableController = new CCollectableController();

            // Загружаем врагов ТОЛЬКО из JSON
            LoadEnemiesFromJson();

            player.PropertyChanged += Player_PropertyChanged;

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            SpawnNewEnemy();
            UpdateUI();
        }

        private void LoadEnemiesFromJson()
        {
            string defaultPath = "enemies.json";
            if (File.Exists(defaultPath))
            {
                try
                {
                    templateList.LoadFromJson(defaultPath);
                    var templates = GetTemplatesFromList();

                    if (templates.Count > 0)
                    {
                        enemyManager.LoadTemplates(templates);
                        Console.WriteLine($"Загружено {templates.Count} врагов из JSON");
                    }
                    else
                    {
                        throw new Exception("JSON файл пустой");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки врагов: {ex.Message}");
                    Application.Current.Shutdown();
                }
            }
            else
            {
                MessageBox.Show("Файл enemies.json не найден! Создайте врагов в редакторе.");
                Application.Current.Shutdown();
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            collectableController.Update(0.1, player, enemyManager);
            UpdateCollectablesUI();
        }

        private void UpdateCollectablesUI()
        {
            CollectablesCanvas.Children.Clear();

            foreach (var collectable in collectableController.Collectables)
            {
                var sprite = collectable.GetSprite();
                CollectablesCanvas.Children.Add(sprite);
            }
        }

        private void CollectablesCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(CollectablesCanvas);
            var clickedCollectable = collectableController.HandleClick(mousePosition, player, enemyManager);

            if (clickedCollectable != null)
            {
                // Обновляем UI если кликнули по сфере
                UpdateCollectablesUI();
                UpdateUI();
            }
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI();
        }

        private void SpawnNewEnemy()
        {
            var enemy = enemyManager.GetNextEnemyWithModification(player.Level);
            if (enemy != null)
            {
                if (enemyManager.CurrentEnemy != null)
                {
                    enemyManager.CurrentEnemy.PropertyChanged -= Enemy_PropertyChanged;
                }

                enemy.PropertyChanged += Enemy_PropertyChanged;
                UpdateEnemyUI();
                LoadEnemyIcon(enemy.Name);
                RepeatCount = 5;
            }
        }

        private void Enemy_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateEnemyUI();
        }

        private void LoadEnemyIcon(string enemyName)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    string assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                    string iconPath = Path.Combine(assetsPath, enemyName + ".png");

                    if (!File.Exists(iconPath))
                    {
                        var template = templateList.GetEnemyByName(enemyName);
                        if (template != null)
                        {
                            iconPath = Path.Combine(assetsPath, template.IconName + ".png");
                        }
                    }

                    if (File.Exists(iconPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(iconPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        EnemyIconImage.Source = bitmap;
                        EnemyNameText.Text = enemyName;
                    }
                    else
                    {
                        EnemyIconImage.Source = CreateDefaultIcon();
                        EnemyNameText.Text = enemyName;
                    }
                }
                catch (Exception ex)
                {
                    EnemyIconImage.Source = CreateDefaultIcon();
                    EnemyNameText.Text = enemyName;
                }
            });
        }

        private BitmapImage CreateDefaultIcon()
        {
            // Создаем простую иконку по умолчанию
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.Black, 2), new Rect(0, 0, 100, 100));
                drawingContext.DrawText(
                    new FormattedText("?",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        36, Brushes.Black),
                    new Point(30, 25));
            }

            var renderTarget = new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void UpdateUI()
        {
            Dispatcher.Invoke(() =>
            {
                LevelText.Text = player.Level.ToString();
                GoldText.Text = player.Gold.ToString();
                DamageText.Text = player.Damage.ToString();
                UpgradeCostText.Text = $"Cost: {player.UpgradeCost}";

                // Обновляем прогресс перезарядки
                if (player.AttackCooldown > 0)
                {
                    double cooldownPercent = player.CurrentCooldown / player.AttackCooldown;
                    CooldownProgress.Width = Math.Max(0, (1 - cooldownPercent) * 250);
                }
                else
                {
                    CooldownProgress.Width = 250;
                }

                CooldownText.Text = player.CanAttack ? "Ready!" : $"Cooldown: {player.CurrentCooldown:F1}s";

                // Обновляем информацию об улучшении перезарядки
                CooldownLevelText.Text = player.CooldownLevel.ToString();
                CooldownReductionText.Text = $"Reduction: {player.CooldownReductionPercent:F1}%";
                CooldownUpgradeCostText.Text = $"Cost: {player.CooldownUpgradeCost}";

                AttackHintText.Text = $"Click to attack! Level: {player.Level}";
                UpgradeButton.IsEnabled = player.Gold.GreaterThanOrEqual(player.UpgradeCost);
                CooldownUpgradeButton.IsEnabled = player.Gold.GreaterThanOrEqual(player.CooldownUpgradeCost);

                UpgradeButton.Background = UpgradeButton.IsEnabled ?
                    new SolidColorBrush(Color.FromRgb(76, 175, 80)) :
                    new SolidColorBrush(Color.FromRgb(200, 200, 200));

                CooldownUpgradeButton.Background = CooldownUpgradeButton.IsEnabled ?
                    new SolidColorBrush(Color.FromRgb(33, 150, 243)) :
                    new SolidColorBrush(Color.FromRgb(200, 200, 200));

                RepeatCountText.Text = $"Repeat Next: {RepeatCount}";
            });
        }

        private void UpdateEnemyUI()
        {
            var enemy = enemyManager.CurrentEnemy;
            if (enemy != null)
            {
                Dispatcher.Invoke(() =>
                {
                    EnemyNameText.Text = enemy.Name;
                    HealthBar.Value = enemy.HealthPercentage;
                    HealthText.Text = $"{enemy.CurrentHitPoints}/{enemy.MaxHitPoints}";
                    GoldRewardText.Text = enemy.GoldReward.ToString();

                    double damageRatio = player.Damage.ToDouble() / enemy.MaxHitPoints.ToDouble();
                    if (damageRatio >= 1.0)
                    {
                        AttackHintText.Text = $"ONE HIT! Level: {player.Level}";
                    }
                    else
                    {
                        int hitsNeeded = (int)Math.Ceiling(1.0 / damageRatio);
                        AttackHintText.Text = $"Hits needed: {hitsNeeded} Level: {player.Level}";
                    }
                });
            }
        }

        private void EnemyIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (enemyManager.CurrentEnemy == null) return;

            if (!player.CanAttack) return;

            bool isDefeated = player.DealDamage(enemyManager.CurrentEnemy);

            if (isDefeated)
            {
                player.AddGold(enemyManager.CurrentEnemy.GoldReward);
                SpawnNewEnemy();
                collectableController.Clear();
            }
            else
            {
                RepeatCount--;
                if (RepeatCount <= 0)
                {
                    RepeatCount = 5;
                }
            }

            UpdateUI();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.BuyUpgrade())
            {
                UpdateUI();
                if (enemyManager.CurrentEnemy != null)
                {
                    enemyManager.UpdatePlayerLevel(player.Level);
                    UpdateEnemyUI();
                }
            }
        }

        private void CooldownUpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.BuyCooldownUpgrade())
            {
                UpdateUI();
            }
        }

        private List<CEnemyTemplate> GetTemplatesFromList()
        {
            var templates = new List<CEnemyTemplate>();
            var names = templateList.GetListOfEnemyNames();

            foreach (var name in names)
            {
                var template = templateList.GetEnemyByName(name);
                if (template != null)
                    templates.Add(template);
            }

            return templates;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}