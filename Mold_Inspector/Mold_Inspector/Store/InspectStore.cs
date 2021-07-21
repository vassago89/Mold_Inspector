using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    enum InspectState
    {
        Ready, Busy, Complate
    }

    class InspectStore : BindableBase
    {
        private InspectState _production;
        public InspectState Production
        {
            get => _production;
            set => SetProperty(ref _production, value);
        }

        private InspectState _mold;
        public InspectState Mold
        {
            get => _mold;
            set => SetProperty(ref _mold, value);
        }

        private uint _target;
        public uint Target
        {
            get => _target;
            set => SetProperty(ref _target, value);
        }

        private uint _complate;
        public uint Complate
        {
            get => _complate;
            set => SetProperty(ref _complate, value);
        }

        private uint _ok;
        public uint OK
        {
            get => _ok;
            set => SetProperty(ref _ok, value);
        }

        private uint _ng;
        public uint NG
        {
            get => _ng;
            set => SetProperty(ref _ng, value);
        }

        CommonStore _commonStore;

        public InspectStore(CommonStore commonStore)
        {
            _commonStore = commonStore;

            Reset();
        }

        public void Reset()
        {
            Target = _commonStore.DefaultTarget;
            Complate = 0;
            OK = 0;
            NG = 0;
        }
    }
}
