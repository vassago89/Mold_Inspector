using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Device.IO
{
    class VirtualIOController : IOController
    {
        public Action<bool[]> ValueChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<bool[]> InChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<bool[]> OutChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Read(int channel)
        {
            return false;
        }

        public IEnumerable<bool> Read()
        {
            return new List<bool>();
        }

        public void Write(int channel, bool value)
        {

        }

        public void Write(params bool[] values)
        {

        }
    }
}
