using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CEnemyTemplate
    {
        [JsonInclude]
        private string name;
        [JsonInclude]
        private string iconName;
        [JsonInclude]
        private int baseline;
        [JsonInclude]
        private double lifeModifier;
        [JsonInclude]
        private int baseGold;
        [JsonInclude]
        private double goldModifier;
        [JsonInclude]
        private double spawnChance;

        public CEnemyTemplate(string name, string iconName, int baseline,
                            double lifeModifier, int baseGold, double goldModifier,
                            double spawnChance)
        {
            this.name = name;
            this.iconName = iconName;
            this.baseline = baseline;
            this.lifeModifier = lifeModifier;
            this.baseGold = baseGold;
            this.goldModifier = goldModifier;
            this.spawnChance = spawnChance;
        }

        public string Name() => name;
        public string IconName() => iconName;
        public int Baselife() => baseline;
        public double LifeModifier() => lifeModifier;
        public int BaseGold() => baseGold;
        public double GoldModifier() => goldModifier;
        public double SpawnChance() => spawnChance;
    }
}
