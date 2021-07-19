using Mold_Inspector.Service;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    enum TeachState
    { 
        Prepare,
        Production,
        Mold,
        Complate
    }

    class StateStore : BindableBase
    {
        private TeachState _teachState;
        public TeachState TeachState
        {
            get => _teachState;
            private set
            {
                SetProperty(ref _teachState, value);
                switch (_teachState)
                {
                    case TeachState.Production:
                    case TeachState.Mold:
                        IsAutoEnable = false;
                        break;
                    case TeachState.Prepare:
                    case TeachState.Complate:
                        IsAutoEnable = true;
                        break;
                }
            }
        }

        private bool _isAutoEnable;
        public bool IsAutoEnable
        {
            get => _isAutoEnable;
            set => SetProperty(ref _isAutoEnable, value);
        }

        private bool _isTeachEnable;
        public bool IsTeachEnable
        {
            get => _isTeachEnable;
            set => SetProperty(ref _isTeachEnable, value);
        }

        private bool _isAutoMode;
        public bool IsAutoMode
        {
            get => _isAutoMode;
            set
            {
                SetProperty(ref _isAutoMode, value);
                if (_isAutoMode)
                    TeachState = TeachState.Complate;

                if (IsTeachMode == _isAutoMode)
                    IsTeachMode = !_isAutoMode;
            }
        }

        private bool _isTeachMode;
        public bool IsTeachMode
        {
            get => _isTeachMode;
            set
            {
                SetProperty(ref _isTeachMode, value);
                if (IsAutoMode == _isTeachMode)
                    IsAutoMode = !_isTeachMode;
            }
        }

        private bool _isWaitOpen;
        public bool IsWaitOpen
        {
            get => _isWaitOpen;
            set => SetProperty(ref _isWaitOpen, value);
        }

        private bool _isWaitClosing;
        public bool IsWaitClosing
        {
            get => _isWaitClosing;
            set => SetProperty(ref _isWaitClosing, value);
        }
        

        private bool _isWaitEjected;
        public bool IsWaitEjected
        {
            get => _isWaitEjected;
            set => SetProperty(ref _isWaitEjected, value);
        }

        private bool _isWaitEjecting;
        public bool IsWaitEjecting
        {
            get => _isWaitEjecting;
            set => SetProperty(ref _isWaitEjecting, value);
        }

        private bool _isWaitAlram;
        public bool IsWaitAlram
        {
            get => _isWaitAlram;
            set => SetProperty(ref _isWaitAlram, value);
        }

        private bool _isWaitOK;
        public bool IsWaitOK
        {
            get => _isWaitOK;
            set => SetProperty(ref _isWaitOK, value);
        }

        private bool _isWaitRestart;
        public bool IsWaitRestart
        {
            get => _isWaitRestart;
            set => SetProperty(ref _isWaitRestart, value);
        }

        private bool _isWaitSet;
        public bool IsWaitSet
        {
            get => _isWaitSet;
            set => SetProperty(ref _isWaitSet, value);
        }

        private Action<TeachState> _teachStateChanging;

        private PageStore _pageStore;

        public StateStore(PageStore pageStore)
        {
            _pageStore = pageStore;

            Initialize();
        }

        public void Initialize()
        {
            TeachState = TeachState.Prepare;

            _isTeachMode = true;
            _isAutoMode = false;
            _isAutoEnable = true;
            _isTeachEnable = true;
        }

        public void AddTeachStateChanging(Action<TeachState> stateChanging)
        {
            _teachStateChanging += stateChanging;
        }

        public void TeachStateChanging()
        {
            _teachStateChanging?.Invoke(_teachState);

            switch (_teachState)
            {
                case TeachState.Prepare:
                case TeachState.Production:
                case TeachState.Mold:
                    TeachState++;
                    break;
                case TeachState.Complate:
                    IsAutoMode = true;
                    IsTeachEnable = false;
                    break;
            }
        }
    }
}