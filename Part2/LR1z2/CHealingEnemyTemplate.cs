namespace EnemyEditor
{
    public class CHealingEnemyTemplate : CEnemyTemplate
    {
        private double healChance;
        public double HealChance
        {
            get => healChance;
            set => healChance = value >= 0 && value <= 1 ? value : 0.3;
        }

        public CHealingEnemyTemplate() : base() { }

        public CHealingEnemyTemplate(string name, string iconName, int baseline, double lifeModifier,
                                   int baseGold, double goldModifier, double spawnChance, double healChance)
            : base(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance)
        {
            HealChance = healChance;
        }
    }
}