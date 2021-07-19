using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm
{
    public interface IAlgorithmResult
    {
        IAlgorithm Algorithm { get; }

        bool Pass { get; }

        System.Windows.Rect Region { get; }
        System.Windows.Point Center { get; }
    }
}
