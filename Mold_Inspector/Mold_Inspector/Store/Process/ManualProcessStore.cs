using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store.Process
{
    enum ManualProcess
    {
        Initialize, Production, Mold
    }

    class ManualProcessStore : CommomProcessStore
    {
        private ManualProcess _manualProcess;
        public ManualProcess ManualProcess
        {
            get => _manualProcess;
            set => SetProperty(ref _manualProcess, value);
        }

        public bool IsInitialize => _manualProcess == ManualProcess.Initialize;
        public bool IsProduction => _manualProcess == ManualProcess.Production;
        public bool IsMold => _manualProcess == ManualProcess.Mold;
    }
}