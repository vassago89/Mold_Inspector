using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.BinaryCount
{
    [Serializable]
    class BinaryCountResult : IAlgorithmResult
    {
        public IAlgorithm Algorithm { get; }

        public bool Pass { get; }

        public System.Windows.Rect Region { get; }
        public System.Windows.Point Center { get; }

        public double Percentage { get; }

        public BinaryCountResult(
            BinaryCountAlgorithm algorithm,
            bool pass,
            double percentage,
            System.Windows.Rect region,
            System.Windows.Point center)
        {
            Algorithm = algorithm;
            Pass = pass;
            Percentage = percentage;
            Region = region;
            Center = center;
        }
    }
}
