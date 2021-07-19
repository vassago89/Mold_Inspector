using Mold_Inspector.Config.Abstract;
using Mold_Inspector.Device.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Config
{
    class CameraConfig : JsonConfig<CameraConfig>
    {
        public Dictionary<int, CameraInfo> Cameras { get; set; }

        public CameraConfig()
        {
            Cameras = new Dictionary<int, CameraInfo>();
        }

        
    }
}
