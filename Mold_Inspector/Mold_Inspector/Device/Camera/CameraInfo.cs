using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Device.Camera
{
    public enum ECameraType
    {
        USB, GIGE
    }

    public enum ECameraManufacturer
    {
        Hik, Basler, iDS
    }

    public class CameraInfo : IEquatable<CameraInfo> 
    {
        public ECameraManufacturer Manufacturer { get; set; }
        public ECameraType CameraType { get; set; }
        public string ModelName { get; set; }
        public string SerialNo { get; set; }

        public CameraInfo()
        {

        }

        public CameraInfo(ECameraManufacturer manufacturer, ECameraType cameraType, string modelName, string serialNo)
        {
            Manufacturer = manufacturer;
            CameraType = cameraType;
            ModelName = modelName;
            SerialNo = serialNo;
        }

        public override int GetHashCode()
        {
            return Manufacturer.GetHashCode()
                ^ CameraType.GetHashCode()
                ^ ModelName.GetHashCode()
                ^ SerialNo.GetHashCode();
        }

        public bool Equals(CameraInfo other)
        {
            if (Manufacturer == other.Manufacturer
                && CameraType == other.CameraType
                && ModelName == other.ModelName
                && SerialNo == other.SerialNo)
                return true;

            return false;
        }
    }
}
