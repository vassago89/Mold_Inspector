using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Main.ViewModels
{
    class AutoModeViewModel : BindableBase
    {
        private bool _isEditable;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

        public InspectStore InspectStore { get; }
        public StateStore StateStore { get; }
        public DelegateCommand RefreshCommand { get; }


        public AutoModeViewModel(
            InspectStore inspectStore,
            StateStore stateStore)
        {
            InspectStore = inspectStore;
            StateStore = stateStore;

            RefreshCommand = new DelegateCommand(() =>
            {
                InspectStore.Complate = 0;
                InspectStore.OK = 0;
                InspectStore.NG = 0;
            });
        }
    }
}
