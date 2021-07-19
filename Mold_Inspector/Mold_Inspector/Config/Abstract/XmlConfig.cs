using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Mold_Inspector.Config.Abstract
{
    [Serializable]
    [DataContract]
    public abstract class XmlConfig<T> where T : class, new()
    {
        public static string DefaultPath => Path.Combine(Environment.CurrentDirectory, $"{typeof(T).Name}.xml");

        public XmlConfig()
        {

        }

        public static T Deserialize(string filePath = null)
        {
            if (filePath == null)
                filePath = DefaultPath;

            //if (File.Exists(filePath) == false)
                return new T();

            //try
            //{
            //    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            //    {
            //        var serializer = new XmlSerializer(typeof(T));
            //        return (T)serializer.Deserialize(memStream);
            //    }    
            //}
            //catch (Exception e)
            //{
            //    return new T();
            //}
        }

        public void Serialize(string filePath = null)
        {
            var serializer = new DataContractSerializer(typeof(T));
            
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(filePath ?? DefaultPath, null))
                {
                    writer.Formatting = Formatting.Indented;
                    serializer.WriteObject(writer, this);
                    writer.Flush();
                }
            }
        }
    }
}
