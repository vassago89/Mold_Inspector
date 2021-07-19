using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mold_Inspector.Config.Abstract
{
    abstract class JsonConfig<T> : IConfig<T> where T : class, new()
    {
        private static string _defaultPath => Path.Combine(Environment.CurrentDirectory, $"{typeof(T).Name}.json");

        public JsonConfig()
        {

        }

        public T Deserialize(string filePath = null)
        {
            if (filePath == null)
                filePath = _defaultPath;

            if (File.Exists(filePath) == false)
                return new T();

            try
            {
                return JsonConvert.DeserializeObject<T>(
                    File.ReadAllText(filePath), new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Serialize(string filePath = null)
        {
            try
            {
                File.WriteAllText(
                    filePath ?? _defaultPath, 
                    JsonConvert.SerializeObject(this, Formatting.Indented,
                     new JsonSerializerSettings
                     {
                         TypeNameHandling = TypeNameHandling.All
                     }));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
