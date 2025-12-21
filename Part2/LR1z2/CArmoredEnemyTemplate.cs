namespace EnemyEditor
{
    public class CArmoredEnemyTemplate : CEnemyTemplate
    {
        private double armor;
        public double Armor
        {
            get => armor;
            set => armor = value >= 0 ? value : 25;
        }

        public CArmoredEnemyTemplate() : base() { }

        public CArmoredEnemyTemplate(string name, string iconName, int baseline, double lifeModifier,
                                   int baseGold, double goldModifier, double spawnChance, double armor)
            : base(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance)
        {
            Armor = armor;
        }
    }
}