using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Config.Abstract
{
    [Serializable]
    public abstract class BinaryConfig<T> : IConfig<T> where T : class, new()
    {
        public string DefualtPath => Path.Combine(Environment.CurrentDirectory, $"{GetType().Name}.bin");

        private static BinaryFormatter _formatter = new BinaryFormatter();

        public BinaryConfig()
        {

        }

        public T Deserialize(string filePath = null)
        {
            if (filePath == null)
                filePath = DefualtPath;

            if (File.Exists(filePath) == false)
                return new T();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return _formatter.Deserialize(stream) as T;
                }
                catch (Exception e)
                {
                    return new T();
                }
            }
        }

        public void Serialize(string filePath = null)
        {
            using (var stream = File.Create(filePath ?? DefualtPath))
            {
                _formatter.Serialize(stream, this);
            }
        }
    }
}
