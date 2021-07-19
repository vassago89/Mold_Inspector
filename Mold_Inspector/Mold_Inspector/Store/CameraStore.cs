using Mold_Inspector.Config;
using Mold_Inspector.Device;
using Mold_Inspector.Device.Camera;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Mold_Inspector.Store.Abstract;

namespace Mold_Inspector.Store
{
    public class CameraInfoWrapper : BindableBase
    {
        private int _id;
        public int ID
        {
            get => _id; 
            set => SetProperty(ref _id, value); 
        }

        public CameraInfo Info { get; }

        public CameraInfoWrapper(CameraInfo info)
        {
            Info = info;
        }

        public CameraInfoWrapper(int id, CameraInfo info)
        {
            _id = id;
            Info = info;
        }
    }

    class CameraStore : ConfigStore<CameraConfig>
    {
        public ObservableCollection<CameraInfoWrapper> Cameras { get; private set; }
        public ObservableCollection<CameraInfo> CameraInfos { get; private set; }

        public CameraStore() : base()
        {
            Refresh();
        }

        public void Add(CameraInfo info) => Cameras.Add(new CameraInfoWrapper(info));
        public void Remove(CameraInfoWrapper wrapper) => Cameras.Remove(wrapper);

        public void Refresh()
        {
            CameraInfos.Clear();
            CameraInfos.AddRange(CameraService.GetDeviceInfos());
        }

        protected override void Initialize()
        {
            Cameras = new ObservableCollection<CameraInfoWrapper>();
            CameraInfos = new ObservableCollection<CameraInfo>();

            BindingOperations.EnableCollectionSynchronization(Cameras, new object());
            BindingOperations.EnableCollectionSynchronization(CameraInfos, new object());
        }

        protected override void CopyFrom(CameraConfig config)
        {
            Cameras.Clear();
            foreach (var item in config.Cameras)
                Cameras.Add(new CameraInfoWrapper(item.Key, item.Value));
        }

        protected override void CopyTo(CameraConfig config)
        {
            config.Cameras.Clear();
            foreach (var camera in Cameras)
                config.Cameras.Add(camera.ID, camera.Info);
        }
    }
}
