using Mold_Inspector.Config.Abstract;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store.Abstract
{
    abstract class ConfigStore<T> : BindableBase where T : IConfig<T>, new()
    {
        private T _config;

        protected ConfigStore()
        {
            Initialize();
            Load();
        }

        ~ConfigStore()
        {
            Save();
        }

        public void Load()
        {
            _config = new T().Deserialize();
            CopyFrom(_config);
        }

        public void Save()
        {
            CopyTo(_config);
            _config.Serialize();
        }

        abstract protected void Initialize();
        abstract protected void CopyFrom(T config);
        abstract protected void CopyTo(T config);
    }
}
