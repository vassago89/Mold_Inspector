using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store.Process
{
    enum CommonProcess
    {
        Ready, Molding, WaitMold, Ejecting, WaitEject
    }

    abstract class CommomProcessStore : BindableBase
    {
        private CommonProcess _commonProcess;
        public CommonProcess CommonProcess
        {
            get => _commonProcess;
            set => SetProperty(ref _commonProcess, value);
        }

        public bool IsReady => _commonProcess == CommonProcess.Ready;
        public bool IsMolding => _commonProcess == CommonProcess.Molding;
        public bool IsWaitMold => _commonProcess == CommonProcess.WaitMold;
        public bool IsEjecting => _commonProcess == CommonProcess.Ejecting;
        public bool IsWaitEject => _commonProcess == CommonProcess.WaitEject;
    }
}
