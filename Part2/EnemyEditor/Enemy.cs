using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class Enemy
    {
        private string name;
        private BigNumber maxHitPoints;
        private BigNumber currentHitPoints;
        private BigNumber goldReward;
        private bool isDead;
        private string iconName;

        public string Name { get => name; private set => name = value; }
        public BigNumber MaxHitPoints { get => maxHitPoints; private set => maxHitPoints = value; }
        public BigNumber GoldReward { get => goldReward; private set => goldReward = value; }
        public BigNumber CurrentHitPoints { get => currentHitPoints; private set => currentHitPoints = value; }
        public bool IsDead { get => isDead; private set => isDead = value; }
        public string IconName { get => iconName; private set => iconName = value; }

        public Enemy(string name, BigNumber maxHp, BigNumber gold, string icon)
        {
            Name = name;
            MaxHitPoints = maxHp;
            CurrentHitPoints = maxHp;
            GoldReward = gold;
            IconName = icon;
            IsDead = false;
        }

        public Enemy(CEnemyTemplate template)
        {
            Name = template.Name;

            double health = template.Baselife() * template.LifeModifier();
            MaxHitPoints = new BigNumber(((int)health).ToString());

            CurrentHitPoints = MaxHitPoints;

            double gold = template.BaseGold() * template.GoldModifier();
            GoldReward = new BigNumber(((int)gold).ToString());

            IconName = template.IconName();
            IsDead = false;
        }

        public bool TakeDamage(BigNumber dmg, out BigNumber reward)
        {
            reward = new BigNumber("0");

            if (IsDead)
                return false;

            if (currentHitPoints > dmg)
            {
                currentHitPoints = currentHitPoints - dmg;
                return false;
            }
            else
            {
                reward = GoldReward;
                Die();
                return true;
            }
        }

        private void Die()
        {
            IsDead = true;
            currentHitPoints = new BigNumber("0");
        }

        public void Reset()
        {
            CurrentHitPoints = MaxHitPoints;
            IsDead = false;
        }
    }
}

