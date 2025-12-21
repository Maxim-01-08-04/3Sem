namespace EnemyEditor
{
    public class CNormalEnemyTemplate : CEnemyTemplate
    {
        public CNormalEnemyTemplate() : base() { }

        public CNormalEnemyTemplate(string name, string iconName, int baseline, double lifeModifier,
                                  int baseGold, double goldModifier, double spawnChance)
            : base(name, iconName, baseline, lifeModifier, baseGold, goldModifier, spawnChance)
        {
        }
    }
}