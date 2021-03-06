using Mold_Inspector.Device.IO;
using Mold_Inspector.Device.IO.Serial;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Setting.ViewModels
{
    class IOViewModel : BindableBase
    {
        public IEnumerable<Parity> Parities => Enum.GetValues(typeof(Parity)).Cast<Parity>();
        public IEnumerable<StopBits> StopBits => Enum.GetValues(typeof(StopBits)).Cast<StopBits>();
        public IEnumerable<string> Ports => SerialPort.GetPortNames();
        public IEnumerable<int> BaudRates => new int[] { 9600, 14400, 19200, 28800, 38400, 57600, 115200 };
        public IEnumerable<int> DataBits => new int[] { 5, 6, 7, 8 };

        public DelegateCommand ConnectCommand { get; }

        public IOStore IOStore { get; }

        public IOViewModel(
            IOStore ioStore,
            IOController ioController)
        {
            IOStore = ioStore;

            ConnectCommand = new DelegateCommand(() =>
            {
                var controller = ioController as KM6050IOController;
                if (controller == null)
                    return;

                controller.Connect(
                    ioStore.PortName,
                    ioStore.BaudRate,
                    ioStore.Parity,
                    ioStore.DataBits,
                    ioStore.StopBits);

                controller.StartMonitoring(1000);
            });
        }
    }
}
