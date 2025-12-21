using System.Text.Json.Serialization;

namespace EnemyEditor
{
    public abstract class CEnemyTemplate
    {
        public string Name { get; set; } = "Unknown";
        public string IconName { get; set; } = "default";
        public int Baselife { get; set; } = 50;
        public double LifeModifier { get; set; } = 1.1;
        public int BaseGold { get; set; } = 25;
        public double GoldModifier { get; set; } = 1.1;
        public double SpawnChance { get; set; } = 0.5;

        [JsonConstructor]
        protected CEnemyTemplate() { }

        protected CEnemyTemplate(string name, string iconName, int baseline, double lifeModifier,
                               int baseGold, double goldModifier, double spawnChance)
        {
            Name = name;
            IconName = iconName;
            Baselife = baseline;
            LifeModifier = lifeModifier;
            BaseGold = baseGold;
            GoldModifier = goldModifier;
            SpawnChance = spawnChance;
        }
    }
}