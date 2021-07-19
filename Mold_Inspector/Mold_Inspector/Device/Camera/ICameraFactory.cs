using Mold_Inspector.Device.Camera.Baslers;
using Mold_Inspector.Device.Camera.Hiks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Device.Camera
{
    public class CameraFactory
    {
        private static CameraFactory _instance;
        public static CameraFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CameraFactory();

                return _instance;
            }
        }

        public ICameraFactory Create(ECameraManufacturer manufacturer)
        {
            switch (manufacturer)
            {
                case ECameraManufacturer.Hik:
                    return new HikCameraFactory();
                case ECameraManufacturer.Basler:
                    return new BaslerCameraFactory();
            }

            return null;
        }
    }

    public interface ICameraFactory
    {
        IEnumerable<CameraInfo> GetDevices();
        ICamera Connect(CameraInfo cameraInfo);
        bool IsExist(CameraInfo cameraInfo);
    }
}