using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class Player
    {
        private int lvl;
        private BigNumber gold;
        private BigNumber damage;
        private double damageModifier;
        private BigNumber upgradeCost;
        private double upgradeModifier;
        private double attackCooldown = 1.0;
        private double currentCooldown = 0;
        private bool canAttack = true;

        public bool CanAttack => canAttack;
        public double AttackCooldown => attackCooldown;
        public double CurrentCooldown => currentCooldown;
        public double CooldownProgress => currentCooldown / attackCooldown * 100;



        public int Lvl { get => lvl; private set => lvl = value; }
        public BigNumber Gold { get => gold; private set => gold = value; }
        public BigNumber Damage { get => damage; private set => damage = value; }
        public double DamageModifier { get => damageModifier; private set => damageModifier = value; }
        public BigNumber UpgradeCost { get => upgradeCost; private set => upgradeCost = value; }
        public double UpgradeModifier { get => upgradeModifier; private set => upgradeModifier = value; }

        public Player()
        {
            Lvl = 1;
            Gold = new BigNumber("0");
            Damage = new BigNumber("10");
            DamageModifier = 1.2;
            UpgradeCost = new BigNumber("10");
            UpgradeModifier = 1.1;
        }



        public BigNumber DealDamage()
        {
            if (!canAttack)
                return new BigNumber("0");

            StartCooldown();
            return damage;
        }

        private void StartCooldown()
        {
            canAttack = false;
            currentCooldown = attackCooldown;
        }

        public void UpdateCooldown(double deltaTime)
        {
            if (!canAttack)
            {
                currentCooldown -= deltaTime;
                if (currentCooldown <= 0)
                {
                    currentCooldown = 0;
                    canAttack = true;
                }
            }
        }

        public void AddGold(BigNumber amount)
        {
            gold = gold + amount;
        }

        public bool TryUpgrade()
        {
            if (gold >= upgradeCost)
            {
                gold = gold - upgradeCost;
                lvl++;
                RecalculateStats();
                return true;
            }
            return false;
        }

        //public BigNumber DealDamage()
        //{
        //    return damage;
        //}

        public void ApplyCooldownMultiplier(double multiplier)
        {
            attackCooldown *= multiplier;
            if (currentCooldown > 0)
            {
                currentCooldown *= multiplier;
            }
        }

        public bool TryUpgradeCooldown()
        {
            BigNumber cost = new BigNumber("100"); 
            if (gold >= cost)
            {
                gold = gold - cost;
                attackCooldown *= 0.9; 
                return true;
            }
            return false;
        }

        private void RecalculateStats()
        {
            if (damage == null)
            {
                damage = new BigNumber("1");
            }

            damage = damage * damageModifier;

            upgradeCost = CalculateNextUpgradeCost();

            Console.WriteLine($"Уровень: {lvl}, Урон: {damage}, Модификатор: {damageModifier}");
        }

        private BigNumber CalculateNextUpgradeCost()
        {
            double multiplier = upgradeModifier * lvl;

            if (upgradeCost == null)
            {
                upgradeCost = new BigNumber("10");
            }

            return upgradeCost * multiplier;
        }

        private bool TrySpendGold(BigNumber amount)
        {
            if (gold >= amount)
            {
                gold = gold - amount;
                return true;
            }
            return false;
        }


    }
}

