using Mold_Inspector.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Main.ViewModels
{
    class TeachModeViewModel
    {
        public TeachingStore TeachingStore { get; }
        public StateStore StateStore { get; }
        
        public TeachModeViewModel(
            TeachingStore teachingStore,
            StateStore stateStore)
        {
            TeachingStore = teachingStore;
            StateStore = stateStore;
        }
    }
}
