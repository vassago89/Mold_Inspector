using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Device.Camera.iDS
{
    class iDSCameraFactory : ICameraFactory
    {
        public iDSCameraFactory()
        {

        }

        public ICamera Connect(CameraInfo cameraInfo)
        {
            return new iDSCamera(cameraInfo);
        }

        public IEnumerable<CameraInfo> GetDevices()
        {
            var cameraInfos = new List<CameraInfo>();
            uEye.Info.Camera.GetCameraList(out var cameraInfomations);
            
            foreach (var cameraInfomation in cameraInfomations)
            {
                var camera = new uEye.Camera(cameraInfomation.CameraID);
                camera.Information.GetCameraInfo(out var info);
                switch (info.BoardType)
                {
                    case uEye.Defines.BoardType.Usb:
                    case uEye.Defines.BoardType.Usb_ME:
                    case uEye.Defines.BoardType.Usb_LE:
                    case uEye.Defines.BoardType.Usb_XS:
                    case uEye.Defines.BoardType.Usb_ML:
                    case uEye.Defines.BoardType.Usb3_SE:
                    case uEye.Defines.BoardType.Usb3_LE:
                    case uEye.Defines.BoardType.Usb3_XC:
                    case uEye.Defines.BoardType.Usb3_CP:
                    case uEye.Defines.BoardType.Usb3_ML:
                        cameraInfos.Add(
                            new CameraInfo(
                                ECameraManufacturer.iDS,
                                ECameraType.USB,
                                cameraInfomation.Model,
                                cameraInfomation.SerialNumber));
                        break;
                    case uEye.Defines.BoardType.Eth:
                    case uEye.Defines.BoardType.Eth_SE:
                    case uEye.Defines.BoardType.Eth_CP:
                    case uEye.Defines.BoardType.Eth_SEP:
                    case uEye.Defines.BoardType.Eth_LEET:
                    case uEye.Defines.BoardType.Eth_TE:
                    case uEye.Defines.BoardType.Eth_FA:
                    case uEye.Defines.BoardType.Eth_SE_R4:
                    case uEye.Defines.BoardType.Eth_CP_R2:
                        cameraInfos.Add(
                            new CameraInfo(
                                ECameraManufacturer.iDS,
                                ECameraType.GIGE,
                                cameraInfomation.Model,
                                cameraInfomation.SerialNumber));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                camera.Exit();
            }

            return cameraInfos;
        }

        public bool IsExist(CameraInfo cameraInfo)
        {
            uEye.Info.Camera.GetCameraList(out var cameraInfomations);
            return cameraInfomations.Any(info => info.SerialNumber == cameraInfo.SerialNo);
        }
    }
}
