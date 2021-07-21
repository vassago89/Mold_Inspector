using Mold_Inspector.Model;
using Mold_Inspector.Service;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    class TeachingStore : BindableBase
    {
        public Action ModeChanged { get; set; }
        public Action WindowChanged { get; set; }

        private bool _isLock;
        public bool IsLock
        {
            get => _isLock;
            set
            {
                SetProperty(ref _isLock, value);
                if (_isLock)
                    IsNone = true;
            }
        }

        private bool _isNone;
        public bool IsNone
        {
            get => _isNone;
            set => SetProperty(ref _isNone, value);
        }

        private InspectMode _inspectMode;
        public InspectMode InspectMode
        {
            get => _inspectMode;
            set
            {
                SetProperty(ref _inspectMode, value);
                WindowStoreMediator.Instacne.Refresh();
                ModeChanged?.Invoke();

                IsProductionMode = InspectMode == InspectMode.Production;
                IsMoldMode = InspectMode == InspectMode.Mold;
            }
        }

        private bool _isProductionMode;
        public bool IsProductionMode
        {
            get => _isProductionMode;
            set => SetProperty(ref _isProductionMode, value);
        }

        private bool _isMoldMode;
        public bool IsMoldMode
        {
            get => _isMoldMode;
            set => SetProperty(ref _isMoldMode, value);
        }

        private Window _window;
        public Window Window
        {
            get => _window;
            set
            {
                SetProperty(ref _window, value);
                WindowChanged?.Invoke();
            }
        }

        private ParameterInfoWrapper _exposure;
        public ParameterInfoWrapper Exposure
        {
            get => _exposure;
            set => SetProperty(ref _exposure, value);
        }

        private ParameterInfoWrapper _gain;
        public ParameterInfoWrapper Gain
        {
            get => _gain;
            set => SetProperty(ref _gain, value);
        }

        public TeachingStore()
        {
            IsLock = true;
            InspectMode = InspectMode.Production;
        }
    }
}
