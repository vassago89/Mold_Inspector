using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace Mold_Inspector.Device.IO.Serial
{
    class KM6050IOController : IOController
    {
        private SerialPort _port;

        public Action<bool[]> InChanged { get; set; }
        public Action<bool[]> OutChanged { get; set; }

        CancellationTokenSource _cts;

        private byte _prevIn;
        private byte _prevOut;
        private ManualResetEvent _resetEvent;

        private string _data;

        public KM6050IOController()
        {
            _data = string.Empty;
        }

        public void StartMonitoring(int timeout)
        {
            if (_port == null)
                return;

            Task.Run(() =>
            {
                var sw = new Stopwatch();
                while (_cts.IsCancellationRequested == false)
                {
                    try
                    {
                        _port?.Write("$016\r\n");
                        sw.Restart();

                        while (sw.ElapsedMilliseconds < timeout)
                        {
                            var data = _port?.ReadExisting();
                            if (data == string.Empty)
                                continue;

                            var index = data.LastIndexOf('!');
                            if (index >= 0)
                                _data = data.Substring(index, data.Length - index);
                            else
                                _data += data;

                            if (_data.First() == '!' && _data.Last() == '\r')
                            {
                                if (_data.Length == 8)
                                {
                                    var inValue = Convert.ToByte(_data.Substring(3, 2), 16);
                                    if (inValue != _prevIn)
                                    {
                                        var values = new bool[8];
                                        values[0] = (inValue & 0x01) > 0;
                                        values[1] = (inValue & 0x02) > 0;
                                        values[2] = (inValue & 0x04) > 0;
                                        values[3] = (inValue & 0x08) > 0;
                                        values[4] = (inValue & 0x10) > 0;
                                        values[5] = (inValue & 0x20) > 0;
                                        values[6] = (inValue & 0x40) > 0;
                                        values[7] = (inValue & 0x80) > 0;
                                        InChanged?.Invoke(values);
                                    }

                                    var outValue = Convert.ToByte(_data.Substring(1, 2), 16);
                                    if (outValue != _prevOut)
                                    {
                                        var values = new bool[8];
                                        values[0] = (outValue & 0x01) > 0;
                                        values[1] = (outValue & 0x02) > 0;
                                        values[2] = (outValue & 0x04) > 0;
                                        values[3] = (outValue & 0x08) > 0;
                                        values[4] = (outValue & 0x10) > 0;
                                        values[5] = (outValue & 0x20) > 0;
                                        values[6] = (outValue & 0x40) > 0;
                                        values[7] = (outValue & 0x80) > 0;
                                        OutChanged?.Invoke(values);
                                    }

                                    _prevIn = inValue;
                                    _prevOut = outValue;
                                }
                                break;
                            }
                        }

                        _data = string.Empty;
                    }
                    catch (Exception e)
                    {

                    }
                }

                _resetEvent.Set();
            });
        }

        public void Connect(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                _cts?.Cancel();
                if (_cts != null)
                {
                    _resetEvent = new ManualResetEvent(false);
                    _cts.Cancel();
                    _resetEvent.WaitOne();
                }

                _cts = new CancellationTokenSource();

                _data = string.Empty;

                if (_port != null && _port.IsOpen)
                    _port.Close();
                    

                _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                _port.Open();
            }
            catch (Exception e)
            {
                _data = string.Empty;
                _resetEvent = null;
                _cts = null;
                _port = null;
            }
        }


        public void Write(int channel, bool value)
        {
            _port?.Write($"#011{channel}0{(value ? 1 : 0)}\r\n");
        }

        public void Write(params bool[] values)
        {
            throw new NotImplementedException();
        }
    }
}
