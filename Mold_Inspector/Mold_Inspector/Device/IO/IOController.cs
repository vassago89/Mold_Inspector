using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Device.IO
{
    interface IOController
    {
        Action<bool[]> InChanged { get; set; }
        Action<bool[]> OutChanged { get; set; }

        void Write(int channel, bool value);
        void Write(params bool[] values);
    }
}
