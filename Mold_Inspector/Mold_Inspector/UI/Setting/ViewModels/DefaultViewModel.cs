using Mold_Inspector.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Setting.ViewModels
{
    class DefaultViewModel
    {
        public DefaultStore DefaultStore { get; }

        public DefaultViewModel(DefaultStore defaultStore)
        {
            DefaultStore = defaultStore;
        }
    }
}
