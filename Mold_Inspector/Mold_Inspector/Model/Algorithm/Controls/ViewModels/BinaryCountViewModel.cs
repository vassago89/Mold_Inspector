using LiveCharts;
using Mold_Inspector.Model.Algorithm.BinaryCount;
using Mold_Inspector.Store;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mold_Inspector.Model.Algorithm.Controls.ViewModels
{
    class BinaryCountViewModel : BindableBase
    {
        private ImageSource _binary;
        public ImageSource Binary
        {
            get => _binary;
            set => SetProperty(ref _binary, value);
        }

        private double _threshold;
        public double Threshold
        {
            get => _threshold;
            set => SetProperty(ref _threshold, value);
        }

        private double _percentage;
        public double Percentage
        {
            get => _percentage;
            set => SetProperty(ref _percentage, value);
        }

        private bool _isAutoMode;
        public bool IsAutoMode
        {
            get => _isAutoMode;
            set
            {
                SetProperty(ref _isAutoMode, value);
                if (_algorithm == null)
                    return;

                var algorithm = _algorithm as BinaryCountAlgorithm;
                algorithm.IsAutoMode = _isAutoMode;
            }
        }

        private ChartValues<double> _chartValues;
        public ChartValues<double> ChartValues
        {
            get => _chartValues;
            set => SetProperty(ref _chartValues, value);
        }

        private ChartValues<double> _thresholdValue;
        public ChartValues<double> ThresholdValue
        {
            get => _thresholdValue;
            set => SetProperty(ref _thresholdValue, value);
        }

        private double _maxValue;
        public double MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }

        public TeachingStore TeachingStore { get; }
        private ImageStore _imageStore;

        private IAlgorithm _algorithm;

        public BinaryCountViewModel(
            TeachingStore teachingStore,
            ImageStore imageStore)
        {
            TeachingStore = teachingStore;
            _imageStore = imageStore;

            ChartValues = new ChartValues<double>();
            ThresholdValue = new ChartValues<double>();
            MaxValue = 1;

            if (TeachingStore.Window == null)
                return;

            var algorithm = TeachingStore.Window.Algorithm as BinaryCountAlgorithm;
            IsAutoMode = algorithm.IsAutoMode;
            algorithm.ParameterChanged += CalcHistogram;

            _algorithm = algorithm;

            CalcHistogram();
        }

        ~BinaryCountViewModel()
        {
            if (_algorithm != null)
                _algorithm.ParameterChanged -= CalcHistogram;
        }

        private void CalcHistogram()
        {
            var mat = _imageStore.GetMat(TeachingStore.Window.CameraID);
            if (mat == null)
                return;

            var region = TeachingStore.Window.Region;
            var whole = new System.Drawing.Rectangle(0, 0, mat.Width, mat.Height);
            region.Intersect(whole);

            if (region.Width <= 0 || region.Height <= 0)
                return;

            var clone = mat.Clone(new OpenCvSharp.Rect(region.X, region.Y, region.Width, region.Height));
            if (clone.Channels() == 3)
                Cv2.CvtColor(clone, clone, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            var data = new byte[clone.Width * clone.Height];
            Marshal.Copy(clone.Data, data, 0, clone.Width * clone.Height);

            var hist = new double[256];
            foreach (var value in data)
                hist[value]++;

            MaxValue = hist.Max();
            
            ChartValues.Clear();
            ChartValues.AddRange(hist);

            var algorithm = TeachingStore.Window.Algorithm as BinaryCountAlgorithm;

            if (IsAutoMode)
            {
                switch (algorithm.AutoThresholdType)
                {
                    case AutoThresholdType.Otsu:
                        Threshold = Cv2.Threshold(clone, clone, 255, 255, ThresholdTypes.Otsu);
                        break;
                    case AutoThresholdType.Triangle:
                        Threshold = Cv2.Threshold(clone, clone, 255, 255, ThresholdTypes.Triangle);
                        break;
                }
            }
            else
            {
                Cv2.Threshold(clone, clone, algorithm.Threshold, 255, ThresholdTypes.Binary);
                Threshold = algorithm.Threshold;
            }

            ThresholdValue.Clear();
            for (int i = 0; i < 256; i++)
            {
                if (i == Threshold)
                    ThresholdValue.Add(MaxValue);
                else
                    ThresholdValue.Add(-10000);
            }

            if (algorithm.IsInvert)
                Cv2.BitwiseNot(clone, clone);

            Binary = clone.ToImageSource();

            var nonZero = Cv2.CountNonZero(clone);
            Percentage = (double)nonZero / (double)(clone.Width * clone.Height) * 100.0;
        }
    }
}
