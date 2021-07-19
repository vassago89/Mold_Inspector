using Mold_Inspector.Service;
using Mold_Inspector.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Shell.ViewModels
{
    class StatusViewModel
    {
        public IOService IOService { get; }
        public IOStore IOStore { get; }

        public bool IsReleaseMode { get; }

        public StatusViewModel(
            IOService ioService,
            IOStore ioStore)
        {
            IOService = ioService;
            IOStore = ioStore;

#if !DEBUG
            IsReleaseMode = true;
#endif
        }
    }
}
