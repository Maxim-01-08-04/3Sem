using System.Collections.Generic;

namespace EnemyEditor
{
    public interface ISaveList<T>
    {
        T Load(string path);
        void Save(T data, string path);
    }
}