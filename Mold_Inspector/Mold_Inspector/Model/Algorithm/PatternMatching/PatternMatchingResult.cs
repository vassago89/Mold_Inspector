using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.PatternMatching
{
    [Serializable]
    class PatternMatchingResult : IAlgorithmResult
    {
        public IAlgorithm Algorithm { get; set; }

        public bool Pass { get; set; }

        public double Score { get; set; }

        public System.Windows.Rect Region { get; set; }
        public System.Windows.Point Center { get; set; }

        public System.Windows.Rect ResultRegion { get; set; }
        public System.Windows.Point ResultCenter { get; set; }

        public System.Windows.Rect InflateRegion { get; set; }

        public PatternMatchingResult()
        {

        }

        public PatternMatchingResult(
            PatternMatchingAlgorithm algorithm,
            bool pass,
            double score,
            Rect region,
            Point center,
            Rect resultRegion,
            Point resultCenter,
            Rect inflateRegion)
        {
            Algorithm = algorithm;
            Pass = pass;
            Score = score;
            Region = new System.Windows.Rect(
                region.X, region.Y, region.Width, region.Height);
            Center = new System.Windows.Point(
                center.X, center.Y);

            ResultRegion = new System.Windows.Rect(
                resultRegion.X, resultRegion.Y, resultRegion.Width, resultRegion.Height);
            ResultCenter = new System.Windows.Point(
                resultCenter.X, resultCenter.Y);
            InflateRegion = new System.Windows.Rect(
                inflateRegion.X, inflateRegion.Y, inflateRegion.Width, inflateRegion.Height);
        }
    }
}
