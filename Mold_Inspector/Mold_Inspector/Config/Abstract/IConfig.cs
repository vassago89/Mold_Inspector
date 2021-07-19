using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Config.Abstract
{
    interface IConfig<T>
    {
        T Deserialize(string filePath = null);
        void Serialize(string filePath = null);
    }
}
