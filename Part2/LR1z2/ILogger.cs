using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public interface ILogger
    {
        void Log(string message);
        void Error(string errorMessage);
        string Name { get; }
    }

}
