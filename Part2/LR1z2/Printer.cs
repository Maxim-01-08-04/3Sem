using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class Printer : ILogger, IWriter, IPrinter
    {
        public void Log(string message)
        {
            Console.WriteLine("LOG: " + message);
        }
        public void Error(string errorMessage)
        {
            Console.WriteLine("ERROR: " + errorMessage);
        }
        public string Name => "Printer";
        public void Write(string text)
        {
            Console.Write(text);
        }
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
        public void Print(string text)
        {
            Console.WriteLine(text);
        }
    }
}
