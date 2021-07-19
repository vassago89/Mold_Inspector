using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm
{
    public interface IAlgorithm
    {
        AlgorithmType AlgorithmType { get; }

        Action ParameterChanged { get; set; }

        IAlgorithmResult Inspect(Mat source, Window window);
        void PostProcess();

        IAlgorithm Clone();
    }
}