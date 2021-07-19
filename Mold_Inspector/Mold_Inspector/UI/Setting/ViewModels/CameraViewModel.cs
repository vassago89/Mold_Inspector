using Mold_Inspector.Device.Camera;
using Mold_Inspector.Store;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Setting.ViewModels
{
    class CameraViewModel
    {
        public CameraStore CameraStore { get; }

        public DelegateCommand<CameraInfo> AddCommand { get; }
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand<CameraInfoWrapper> RemoveCommand { get; }

        public CameraViewModel(CameraStore cameraStore)
        {
            CameraStore = cameraStore;

            AddCommand = new DelegateCommand<CameraInfo>(info => cameraStore.Add(info));
            RefreshCommand = new DelegateCommand(cameraStore.Refresh);
            RemoveCommand = new DelegateCommand<CameraInfoWrapper>(wrapper => CameraStore.Remove(wrapper));
        }
    }
}
