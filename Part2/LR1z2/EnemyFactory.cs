using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class EnemyFactory
    {
        public static IEnemy CreateEnemy(string typeName, params object[] args)
        {
            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName && typeof(IEnemy).IsAssignableFrom(t));

            if (type == null)
                throw new ArgumentException($"Unknown enemy type: {typeName}");

            return (IEnemy)Activator.CreateInstance(type, args);
        }

        public static IEnemy CreateFromTemplate(CEnemyTemplate template)
        {
            var baseLife = new BigNumber(template.Baselife);
            var gold = new BigNumber(template.BaseGold);

            return template switch
            {
                CArmoredEnemyTemplate armored => new CArmoredEnemy(
                    template.Name, baseLife, gold, template.IconName, armored.Armor),

                CShrinkingEnemyTemplate shrinking => new CShrinkingEnemy(
                    template.Name, baseLife, gold, template.IconName, shrinking.ShrinkFactor),

                CHealingEnemyTemplate healing => new CHealingEnemy(
                    template.Name, baseLife, gold, template.IconName, healing.HealChance),

                _ => new CNormalEnemy(template.Name, baseLife, gold, template.IconName)
            };
        }
    }
}