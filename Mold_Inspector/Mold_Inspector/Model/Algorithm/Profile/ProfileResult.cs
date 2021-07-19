using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.Profile
{
    [Serializable]
    class ProfileResult : IAlgorithmResult
    {
        public IAlgorithm Algorithm { get; }

        public bool Pass { get; }

        public System.Windows.Rect Region { get; }

        public System.Windows.Point Center { get; }

        public double Min { get; }
        public double Max { get; }
        public double Average { get; }

        public ProfileResult(
            IAlgorithm algorithm,
            bool pass,
            System.Windows.Rect region,
            System.Windows.Point center)
        {
            Algorithm = algorithm;
            Pass = pass;
            Region = region;
            Center = center;
        }

        public ProfileResult(
            IAlgorithm algorithm,
            bool pass,
            double min,
            double max,
            double average,
            System.Windows.Rect region,
            System.Windows.Point center)
        {
            Algorithm = algorithm;
            Pass = pass;
            Min = min;
            Max = max;
            Average = average;
            Region = region;
            Center = center;
        }
    }
}
