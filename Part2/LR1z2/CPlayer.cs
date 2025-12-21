using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Threading;

namespace EnemyEditor
{
    public class CPlayer : INotifyPropertyChanged
    {
        private int lvl;
        private BigNumber gold;
        private BigNumber baseDamage;
        private BigNumber upgradeCost;
        private int cooldownLevel = 0;
        private BigNumber cooldownUpgradeCost;
        private double baseCooldown = 0.5;
        private double cooldownReductionPerLevel = 0.05;

        private double attackCooldown;
        private double currentCooldown;
        private double damageBoost = 1.0;
        private double cooldownReduction = 1.0;
        private double damageBoostEndTime = 0;
        private double cooldownReductionEndTime = 0;


        private DispatcherTimer gameTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonConstructor]
        public CPlayer()
        {
            Initialize();
        }

        public CPlayer(bool initialize = true)
        {
            if (initialize)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            lvl = 1;
            gold = new BigNumber(0);
            baseDamage = new BigNumber(8);
            upgradeCost = new BigNumber(30);
            cooldownUpgradeCost = new BigNumber(50);
            attackCooldown = baseCooldown;
            currentCooldown = 0;

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = System.TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Elapsed;
            gameTimer.Start();
        }

        private void GameTimer_Elapsed(object sender, System.EventArgs e)
        {
            UpdateEffects(0.1);
            UpdateCooldown(0.1);
        }

        [JsonInclude]
        public int Level
        {
            get => lvl;
            private set
            {
                lvl = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        [JsonInclude]
        public BigNumber Gold
        {
            get => gold;
            private set
            {
                gold = value;
                OnPropertyChanged(nameof(Gold));
            }
        }

        [JsonIgnore]
        public BigNumber Damage => baseDamage.Multiply(damageBoost).Multiply(1 + (Level - 1) * 0.1);

       
       
        [JsonIgnore]
        public double CurrentCooldown => currentCooldown;

        [JsonIgnore]
        public double AttackCooldown => attackCooldown * cooldownReduction;

        [JsonIgnore]
        public bool CanAttack => currentCooldown <= 0;

        [JsonIgnore]
        public double CooldownReductionPercent => CooldownLevel * cooldownReductionPerLevel * 100;

        public BigNumber UpgradeCost
        {
            get => upgradeCost;
            private set
            {
                upgradeCost = value;
                OnPropertyChanged(nameof(UpgradeCost));
            }
        }

        [JsonInclude]
        public int CooldownLevel
        {
            get => cooldownLevel;
            private set
            {
                cooldownLevel = value;
                OnPropertyChanged(nameof(CooldownLevel));
            }
        }

        [JsonInclude]
        public BigNumber BaseDamage
        {
            get => baseDamage;
            private set
            {
                baseDamage = value;
                OnPropertyChanged(nameof(BaseDamage));
            }
        }

        [JsonInclude]
        public BigNumber CooldownUpgradeCost
        {
            get => cooldownUpgradeCost;
            private set
            {
                cooldownUpgradeCost = value;
                OnPropertyChanged(nameof(CooldownUpgradeCost));
            }
        }

        

        public void AddGold(BigNumber amount)
        {
            Gold = Gold.Add(amount);
        }

       public bool DealDamage(IEnemy enemy)
        {
            if (!CanAttack) return false;

            enemy.TakeDamage(Damage);
            currentCooldown = AttackCooldown;
            return enemy.IsDefeated();
        }

        public bool BuyUpgrade()
        {
            if (Gold.GreaterThanOrEqual(UpgradeCost))
            {
                Gold = Gold.Subtract(UpgradeCost);
                Level++;
                baseDamage = baseDamage.Multiply(1.15);
                UpgradeCost = UpgradeCost.Multiply(1.4);

                OnPropertyChanged(nameof(Damage));
                return true;
            }
            return false;
        }

        public bool BuyCooldownUpgrade()
        {
            if (Gold.GreaterThanOrEqual(CooldownUpgradeCost))
            {
                Gold = Gold.Subtract(CooldownUpgradeCost);
                CooldownLevel++;

                attackCooldown = baseCooldown * (1 - cooldownLevel * cooldownReductionPerLevel);
                if (attackCooldown < 0.1) attackCooldown = 0.1; 

                CooldownUpgradeCost = CooldownUpgradeCost.Multiply(1.6);

                OnPropertyChanged(nameof(AttackCooldown));
                OnPropertyChanged(nameof(CooldownReductionPercent));
                return true;
            }
            return false;
        }

        public void ApplyDamageBoost(double multiplier, double duration)
        {
            damageBoost = multiplier;
            damageBoostEndTime = duration;
            OnPropertyChanged(nameof(Damage));
        }

        public void ApplyCooldownReduction(double multiplier, double duration)
        {
            cooldownReduction = multiplier;
            cooldownReductionEndTime = duration;
            OnPropertyChanged(nameof(AttackCooldown));
        }

        private void UpdateEffects(double deltaTime)
        {
            if (damageBoostEndTime > 0)
            {
                damageBoostEndTime -= deltaTime;
                if (damageBoostEndTime <= 0)
                {
                    damageBoost = 1.0;
                    OnPropertyChanged(nameof(Damage));
                }
            }

            if (cooldownReductionEndTime > 0)
            {
                cooldownReductionEndTime -= deltaTime;
                if (cooldownReductionEndTime <= 0)
                {
                    cooldownReduction = 1.0;
                    OnPropertyChanged(nameof(AttackCooldown));
                }
            }
        }

        private void UpdateCooldown(double deltaTime)
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= deltaTime;
                if (currentCooldown < 0) currentCooldown = 0;
                OnPropertyChanged(nameof(CurrentCooldown));
                OnPropertyChanged(nameof(CanAttack));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}