using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
        public void Error(string errorMessage)
        {
            Console.WriteLine("ERROR: " + errorMessage);
        }
        public string Name => "Console Logger";

    }
}
