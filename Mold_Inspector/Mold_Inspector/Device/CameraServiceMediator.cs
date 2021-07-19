using Mold_Inspector.Device.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Device
{
    class CameraServiceMediator
    {
        class DelegateDictionary
        {
            private IDictionary<int, Action<CameraInfo, GrabInfo>> _grabbedDictionary;
            private IDictionary<int, Action<CameraInfo>> _donedDictionary;

            public DelegateDictionary()
            {
                _grabbedDictionary = new Dictionary<int, Action<CameraInfo, GrabInfo>>();
                _donedDictionary = new Dictionary<int, Action<CameraInfo>>();
            }

            public Action<CameraInfo, GrabInfo> GetDelegate(int id, Action<CameraInfo, GrabInfo> action)
            {
                if (_grabbedDictionary.ContainsKey(id))
                    return _grabbedDictionary[id];

                _grabbedDictionary[id] = action;
                return action;
            }

            public Action<CameraInfo> GetDelegate(int id, Action<CameraInfo> action)
            {
                if (_donedDictionary.ContainsKey(id))
                    return _donedDictionary[id];

                _donedDictionary[id] = action;
                return action;
            }
        }

        Dictionary<CameraInfo, CameraService> _serviceDictionary;

        public Action<int, GrabInfo> ImageGrabbed { get; set; }
        public Action<int> GrabDone { get; set; }

        DelegateDictionary _delegateDictionary;

        public CameraServiceMediator()
        {
            _serviceDictionary = new Dictionary<CameraInfo, CameraService>();
            _delegateDictionary = new DelegateDictionary();
        }

        public void Connect(int id, CameraInfo info)
        {
            if (_serviceDictionary.ContainsKey(info) == false)
                _serviceDictionary[info] = new CameraService();

            if (_serviceDictionary[info].IsConnected() == false)
            {
                if (_serviceDictionary[info].Connect(info) == false)
                    return;
            }

            if (_serviceDictionary.ContainsKey(info))
            {
                var grabbed = _delegateDictionary.GetDelegate(id, (cameraInfo, grabInfo) => ImageGrabbed?.Invoke(id, grabInfo));
                if (_serviceDictionary[info].ImageGrabbed == null)
                    _serviceDictionary[info].ImageGrabbed += grabbed;
                else if (_serviceDictionary[info].ImageGrabbed.GetInvocationList().Contains(grabbed) == false)
                    _serviceDictionary[info].ImageGrabbed += grabbed;

                var grabDone = _delegateDictionary.GetDelegate(id, (cameraInfo) => GrabDone?.Invoke(id));
                if (_serviceDictionary[info].GrabDone == null)
                    _serviceDictionary[info].GrabDone += grabDone;
                if (_serviceDictionary[info].GrabDone.GetInvocationList().Contains(grabDone) == false)
                    _serviceDictionary[info].GrabDone += grabDone;
            }
        }

        public void Disconnect()
        {
            foreach (var service in _serviceDictionary.Values)
                service.Disconnect();

            _serviceDictionary.Clear();
        }

        public async Task<GrabInfo?> Grab(CameraInfo info)
        {
            if (_serviceDictionary.ContainsKey(info) == false)
                return null;
            
            return await _serviceDictionary[info].Grab();
        }

        public void Stop(CameraInfo info)
        {
            if (_serviceDictionary.ContainsKey(info) == false)
                return;

            _serviceDictionary[info].Stop();
        }

        public void Live(CameraInfo info)
        {
            if (_serviceDictionary.ContainsKey(info) == false)
                return;

            _serviceDictionary[info].StartGrab();
        }
    }
}
