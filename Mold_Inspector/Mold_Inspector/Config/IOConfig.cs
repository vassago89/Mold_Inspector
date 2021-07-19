using Mold_Inspector.Config.Abstract;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Config
{
    class IOPort
    {
        public int InOpen { get; set; }
        public int InEjected { get; set; }
        public int InOK { get; set; }
        public int InNG { get; set; }
        public int InSet { get; set; }
        public int InRestart { get; set; }
        public int InDoor { get; set; }

        public int OutClose { get; set; }
        public int OutEject { get; set; }
        public int OutRobot { get; set; }
        public int OutReEject { get; set; }
        public int OutAlram { get; set; }
        public int OutCS { get; set; }
    }

    class IOConfig : JsonConfig<IOConfig>
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        public IOPort IOPort { get; set; }

        public bool UseEjectEnd { get; set; }
        public bool UseReEject { get; set; }
        public int ReEjectCount { get; set; }
        public int EjectDelay { get; set; }

        public IOConfig()
        {
            BaudRate = 9600;
            DataBits = 8;
            Parity = Parity.None;
            StopBits = StopBits.Two;
            IOPort = new IOPort();
            UseEjectEnd = true;
            UseReEject = true;
            EjectDelay = 1000;
            ReEjectCount = 5;
        }
    }
}
