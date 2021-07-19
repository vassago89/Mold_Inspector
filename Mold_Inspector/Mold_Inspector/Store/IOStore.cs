using Mold_Inspector.Config;
using Mold_Inspector.Store.Abstract;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    class IOStore : ConfigStore<IOConfig>
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        public IOPort IOPort { get; set; }

        private bool _useEjectEnd;
        public bool UseEjectEnd
        {
            get => _useEjectEnd;
            set => SetProperty(ref _useEjectEnd, value);
        }

        private bool _useReEject;
        public bool UseReEject 
        {
            get => _useReEject;
            set => SetProperty(ref _useReEject, value);
        }

        private int _reEjectCount;
        public int ReEjectCount
        {
            get => _reEjectCount;
            set => SetProperty(ref _reEjectCount, value);
        }

        private int _ejectDelay;
        public int EjectDelay
        {
            get => _ejectDelay;
            set => SetProperty(ref _ejectDelay, value);
        }

        protected override void CopyFrom(IOConfig config)
        {
            PortName = config.PortName;
            BaudRate = config.BaudRate;
            Parity = config.Parity;
            DataBits = config.DataBits;
            StopBits = config.StopBits;
            IOPort = config.IOPort;
            EjectDelay = config.EjectDelay;
            UseEjectEnd = config.UseEjectEnd;
            UseReEject = config.UseReEject;
            ReEjectCount = config.ReEjectCount;
        }

        protected override void CopyTo(IOConfig config)
        {
            config.PortName = PortName;
            config.BaudRate = BaudRate;
            config.Parity = Parity;
            config.DataBits = DataBits;
            config.StopBits = StopBits;
            config.IOPort = IOPort;
            config.EjectDelay = EjectDelay;
            config.UseEjectEnd = UseEjectEnd;
            config.UseReEject = UseReEject;
            config.ReEjectCount = ReEjectCount;
        }

        protected override void Initialize()
        {
            
        }
    }
}
