using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EnemyEditor
{
    public partial class Window2 : Window, INotifyPropertyChanged
    {
        private CController gameController;
        private CEnemyTemplateList templateList;
        private int repeatCount = 5;
        private DispatcherTimer gameTimer;
        private readonly ISaveList<CPlayer> playerSaver = new PlayerSaver();
        private string currentSavePath = "player_save.json";

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

        public Window2(CEnemyTemplateList enemyTemplateList)
        {
            InitializeComponent();
            DataContext = this;

            templateList = enemyTemplateList ?? new CEnemyTemplateList();
            InitializeGame();
        }

        private void InitializeGame()
        {
            var savedPlayer = playerSaver.Load(currentSavePath);

            Size sceneSize = new Size(CollectablesCanvas.ActualWidth, CollectablesCanvas.ActualHeight);
            gameController = new CController(sceneSize, templateList, 3.0, 5);

            gameController.AddObject += AddObjectToScene;
            gameController.RemoveObject += RemoveObjectFromScene;
            gameController.ObjectSpawned += OnObjectSpawned;
            gameController.ObjectDestroyed += OnObjectDestroyed;
            gameController.GameEvent += OnGameEvent;

            gameController.Player.PropertyChanged += OnPlayerPropertyChanged;

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            gameController.StartGame();
            UpdateUI();
            UpdateSaveStatus("Игра загружена");

            AddMessageToLog("Игра началась! Добро пожаловать!");
        }

        #region Обработчики событий контроллера

        private void AddObjectToScene(object sender, CControllerEventArgs e)
        {
            if (e.Sprite != null)
            {
                Dispatcher.Invoke(() =>
                {
                    CollectablesCanvas.Children.Add(e.Sprite);
                });
            }
        }

        private void RemoveObjectFromScene(object sender, CControllerEventArgs e)
        {
            if (e.Sprite != null)
            {
                Dispatcher.Invoke(() =>
                {
                    CollectablesCanvas.Children.Remove(e.Sprite);
                });
            }
        }

        private void OnObjectSpawned(object sender, CControllerEventArgs e)
        {
            AddMessageToLog(e.Message);
        }

        private void OnObjectDestroyed(object sender, CControllerEventArgs e)
        {
            AddMessageToLog(e.Message);
        }

        private void OnGameEvent(object sender, CControllerEventArgs e)
        {
            AddMessageToLog(e.Message);
        }

        private void OnPlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI();
        }

        #endregion

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameController.Update(0.1);
            UpdateEnemyUI();
        }

        private void AddMessageToLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                MessageLog.Items.Insert(0, $"[{timestamp}] {message}");

                if (MessageLog.Items.Count > 50)
                {
                    MessageLog.Items.RemoveAt(MessageLog.Items.Count - 1);
                }
            });
        }

        private void UpdateSaveStatus(string status)
        {
            Dispatcher.Invoke(() =>
            {
                SaveStatusText.Text = status;
            });
        }

        private void EnemyIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(EnemyIconBorder);
            gameController.HandleEnemyClick(mousePosition);
            UpdateUI();
        }

        private void CollectablesCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(CollectablesCanvas);
            gameController.HandleCollectableClick(mousePosition);
            UpdateUI();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameController.BuyDamageUpgrade())
            {
                UpdateUI();
            }
            else
            {
                AddMessageToLog($"Недостаточно золота для улучшения! Нужно: {gameController.Player.UpgradeCost}");
            }
        }

        private void CooldownUpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameController.BuyCooldownUpgrade())
            {
                UpdateUI();
            }
            else
            {
                AddMessageToLog($"Недостаточно золота для улучшения перезарядки! Нужно: {gameController.Player.CooldownUpgradeCost}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "JSON files (*.json)|*.json";
                saveDialog.DefaultExt = ".json";
                saveDialog.FileName = "player_progress.json";

                if (saveDialog.ShowDialog() == true)
                {
                    playerSaver.Save(gameController.Player, saveDialog.FileName);
                    currentSavePath = saveDialog.FileName;
                    UpdateSaveStatus($"Прогресс сохранен в: {Path.GetFileName(saveDialog.FileName)}");
                    AddMessageToLog("Прогресс сохранен!");
                }
            }
            catch (Exception ex)
            {
                UpdateSaveStatus("Ошибка сохранения прогресса");
                AddMessageToLog($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void LoadProgressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "JSON files (*.json)|*.json";
                openDialog.DefaultExt = ".json";

                if (openDialog.ShowDialog() == true)
                {
                    var loadedPlayer = playerSaver.Load(openDialog.FileName);
                    if (loadedPlayer != null && loadedPlayer.Level > 0)
                    {
                        UpdateSaveStatus($"Прогресс загружен из: {Path.GetFileName(openDialog.FileName)}");
                        AddMessageToLog("Прогресс загружен! Перезапустите игру для применения.");
                    }
                    else
                    {
                        UpdateSaveStatus("Недопустимый файл сохранения");
                        AddMessageToLog("Ошибка: неверный файл сохранения");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateSaveStatus("Ошибка загрузки прогресса");
                AddMessageToLog($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void UpdateUI()
        {
            Dispatcher.Invoke(() =>
            {
                var player = gameController.Player;
                var enemy = gameController.CurrentEnemy;

                LevelText.Text = player.Level.ToString();
                GoldText.Text = player.Gold.ToString();
                DamageText.Text = player.Damage.ToString();
                UpgradeCostText.Text = $"Стоимость: {player.UpgradeCost}";

                if (player.AttackCooldown > 0)
                {
                    double cooldownPercent = player.CurrentCooldown / player.AttackCooldown;
                    CooldownProgress.Width = Math.Max(0, (1 - cooldownPercent) * 250);
                }
                else
                {
                    CooldownProgress.Width = 250;
                }

                CooldownText.Text = player.CanAttack ? "Готово!" : $"Кулдаун: {player.CurrentCooldown:F1}s";

                CooldownLevelText.Text = player.CooldownLevel.ToString();
                CooldownReductionText.Text = $"Снижение: {player.CooldownReductionPercent:F1}%";
                CooldownUpgradeCostText.Text = $"Стоимость: {player.CooldownUpgradeCost}";

                UpdateEnemyUI();

                UpgradeButton.IsEnabled = player.Gold.GreaterThanOrEqual(player.UpgradeCost);
                CooldownUpgradeButton.IsEnabled = player.Gold.GreaterThanOrEqual(player.CooldownUpgradeCost);

                UpgradeButton.Background = UpgradeButton.IsEnabled ?
                    new SolidColorBrush(Color.FromRgb(76, 175, 80)) :
                    new SolidColorBrush(Color.FromRgb(200, 200, 200));

                CooldownUpgradeButton.Background = CooldownUpgradeButton.IsEnabled ?
                    new SolidColorBrush(Color.FromRgb(33, 150, 243)) :
                    new SolidColorBrush(Color.FromRgb(200, 200, 200));

                RepeatCountText.Text = $"Повторите следующее: {RepeatCount}";
            });
        }

        private void UpdateEnemyUI()
        {
            var enemy = gameController.CurrentEnemy;
            if (enemy != null)
            {
                Dispatcher.Invoke(() =>
                {
                    EnemyNameText.Text = enemy.Name;
                    HealthBar.Value = enemy.HealthPercentage;
                    HealthText.Text = $"{enemy.CurrentLife}/{enemy.BaseLife}";
                    GoldRewardText.Text = enemy.Gold.ToString();

                    LoadEnemyIcon(enemy);

                    if (enemy is CShrinkingEnemy shrinkingEnemy)
                    {
                        AttackHintText.Text = $"Шанс уворота: {shrinkingEnemy.DodgeChance:P0}";
                    }
                    else if (enemy is CHealingEnemy healingEnemy)
                    {
                        AttackHintText.Text = $"Шанс лечения: {healingEnemy.HealChance:P0}";
                    }
                    else
                    {
                        AttackHintText.Text = $"Нажми для атаки! Уровень: {gameController.Player.Level}";
                    }
                });
            }
        }

        private void LoadEnemyIcon(IEnemy enemy)
        {
            try
            {
                string assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                string iconFileName = enemy.IconName;

                string[] possiblePaths = {
                    Path.Combine(assetsPath, iconFileName + ".png"),
                    Path.Combine(assetsPath, iconFileName + ".jpg"),
                    Path.Combine(assetsPath, iconFileName + ".jpeg"),
                    Path.Combine(assetsPath, enemy.Name + ".png"),
                    Path.Combine(assetsPath, enemy.Name + ".jpg"),
                    Path.Combine(assetsPath, "default.png"),
                    Path.Combine(assetsPath, "Default.png"),
                    Path.Combine(assetsPath, "default.jpg")
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(foundPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    EnemyIconImage.Source = bitmap;
                }
                else
                {
                    EnemyIconImage.Source = CreateColorIcon(enemy.Name);
                }
            }
            catch (Exception ex)
            {
                EnemyIconImage.Source = CreateColorIcon(enemy.Name);
            }
        }

        private BitmapImage CreateColorIcon(string enemyName)
        {
            try
            {
                var colors = new[] {
                    Colors.Red, Colors.Blue, Colors.Green, Colors.Orange,
                    Colors.Purple, Colors.Teal, Colors.Brown, Colors.Pink,
                    Colors.Cyan, Colors.Magenta, Colors.Lime, Colors.Gold
                };

                int colorIndex = Math.Abs(enemyName.GetHashCode()) % colors.Length;
                Color bgColor = colors[colorIndex];

                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(new SolidColorBrush(bgColor), null, new Rect(0, 0, 100, 100));
                    drawingContext.DrawRectangle(null, new Pen(Brushes.Black, 2), new Rect(0, 0, 100, 100));

                    string firstLetter = enemyName.Length > 0 ? enemyName.Substring(0, 1).ToUpper() : "?";
                    var formattedText = new FormattedText(
                        firstLetter,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        36,
                        Brushes.White,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    drawingContext.DrawText(formattedText, new Point(35, 25));
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
            catch
            {
                return null;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnClosed(EventArgs e)
        {
            if (gameController != null)
            {
                gameController.AddObject -= AddObjectToScene;
                gameController.RemoveObject -= RemoveObjectFromScene;
                gameController.ObjectSpawned -= OnObjectSpawned;
                gameController.ObjectDestroyed -= OnObjectDestroyed;
                gameController.GameEvent -= OnGameEvent;

                gameController.Player.PropertyChanged -= OnPlayerPropertyChanged;
            }

            gameTimer?.Stop();
            base.OnClosed(e);
        }
    }
}