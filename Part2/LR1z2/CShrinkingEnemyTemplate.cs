namespace EnemyEditor
{
    public class CShrinkingEnemyTemplate : CEnemyTemplate
    {
        private double shrinkFactor;
        public double ShrinkFactor
        {
            get => shrinkFactor;
            set => shrinkFactor = value >= 0 && value <= 1 ? value : 0.8;
        }

        public CShrinkingEnemyTemplate() : base() { }

        public CShrinkingEnemyTemplate(string name, string iconName, int baseline, double lifeModifier,
                                     int baseGold, double goldModifier, double spawnChance, double shrinkFactor)
            : base(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance)
        {
            ShrinkFactor = shrinkFactor;
        }
    }
}