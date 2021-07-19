using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.BinaryCount
{
    [Serializable]
    enum AutoThresholdType
    {
        Otsu, Triangle
    }

    [Serializable]
    class BinaryCountAlgorithm : IAlgorithm
    {
        public AlgorithmType AlgorithmType => AlgorithmType.Binary;

        private AutoThresholdType _autoThresholdType;
        public AutoThresholdType AutoThresholdType 
        { 
            get => _autoThresholdType;
            set
            {
                _autoThresholdType = value;
                ParameterChanged?.Invoke();
            }
        }

        public static IEnumerable<AutoThresholdType> AutoThresholdTypes
            => Enum.GetValues(typeof(AutoThresholdType)).Cast<AutoThresholdType>();

        [field: NonSerialized]
        public Action ParameterChanged { get; set; }

        private bool _isInvert;
        public bool IsInvert 
        { 
            get => _isInvert;
            set
            {
                _isInvert = value;
                ParameterChanged?.Invoke();
            }
        }

        private bool _isAutoMode;
        public bool IsAutoMode 
        { 
            get => _isAutoMode;
            set
            {
                _isAutoMode = value;
                ParameterChanged?.Invoke();
            }
        }

        private byte _threshold;
        public byte Threshold 
        { 
            get => _threshold;
            set
            {
                _threshold = value;
                ParameterChanged?.Invoke();
            }
        }

        public double Percentage { get; set; }

        public BinaryCountAlgorithm()
        {
            
        }

        public IAlgorithmResult Inspect(Mat source, Window window)
        {
            var sourceRect = new Rect(0, 0, source.Width, source.Height);
            var windowRect = new Rect(
                window.Region.X,
                window.Region.Y,
                window.Region.Width,
                window.Region.Height);

            windowRect = windowRect.Intersect(sourceRect);

            if (windowRect.Width == 0 || windowRect.Height == 0)
                return new BinaryCountResult(
                    this, 
                    false, 
                    0,
                    new System.Windows.Rect(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height), 
                    new System.Windows.Point(windowRect.X + windowRect.Width / 2, windowRect.Y + windowRect.Height / 2));

            var roi = source.SubMat(windowRect);
            if (roi.Channels() == 3)
                Cv2.CvtColor(roi, roi, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            var result = new Mat();
            if (IsAutoMode)
            {
                switch (AutoThresholdType)
                {
                    case AutoThresholdType.Otsu:
                        result = roi.Threshold(Threshold, 255, ThresholdTypes.Otsu);
                        break;
                    case AutoThresholdType.Triangle:
                        result = roi.Threshold(Threshold, 255, ThresholdTypes.Triangle);
                        break;
                }
            }
            else
            {
                result = roi.Threshold(Threshold, 255, ThresholdTypes.Binary);
            }

            var total = result.Width * result.Height;
            var nonZero = Cv2.CountNonZero(result);

            if (IsInvert)
                nonZero = total - nonZero;

            var percentage = (double)nonZero / (double)total * 100.0;

            return new BinaryCountResult(
                    this,
                    percentage > Percentage,
                    percentage,
                    new System.Windows.Rect(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height),
                    new System.Windows.Point(windowRect.X + windowRect.Width / 2, windowRect.Y + windowRect.Height / 2));
        }

        public void PostProcess()
        {
            
        }

        public IAlgorithm Clone()
        {
            var @new = new BinaryCountAlgorithm();

            @new.Percentage = Percentage;
            @new.Threshold = Threshold;
            @new.IsInvert = IsInvert;
            @new.IsAutoMode = IsAutoMode;
            @new.AutoThresholdType = AutoThresholdType;

            return @new;
        }
    }
}
