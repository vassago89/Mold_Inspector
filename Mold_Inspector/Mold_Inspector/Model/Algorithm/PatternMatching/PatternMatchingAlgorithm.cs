using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Mold_Inspector.Store;
using Newtonsoft.Json;
using OpenCvSharp;
using Prism.Mvvm;

namespace Mold_Inspector.Model.Algorithm.PatternMatching
{
    enum PatternMatchingAlgorithmType
    {
        SDD, ZNCC
    }

    [Serializable]
    class PatternMatchingAlgorithm : IAlgorithm
    {
        public double Score { get; set; }

        public AlgorithmType AlgorithmType => AlgorithmType.Pattern;

        public ObservableCollection<PatternMatchingImage> PatternMatchingImages { get; }

        private int _inflateWidth;
        public int InflateWidth 
        {
            get => _inflateWidth;
            set
            {
                _inflateWidth = value;
                ParameterChanged?.Invoke();
            }
        }

        private int _inflateHeight;
        public int InflateHeight 
        { 
            get => _inflateHeight;
            set
            {
                _inflateHeight = value;
                ParameterChanged?.Invoke();
            }
        }

        public PatternMatchingAlgorithmType PatternMatchingAlgorithmType { get; set; }
        public static IEnumerable<PatternMatchingAlgorithmType> PatternMatchingAlgorithmTypes
            => Enum.GetValues(typeof(PatternMatchingAlgorithmType)).Cast<PatternMatchingAlgorithmType>();

        [field:NonSerialized]
        public Action ParameterChanged { get; set; }

        public PatternMatchingAlgorithm()
        {
            PatternMatchingImages = new ObservableCollection<PatternMatchingImage>();
            
            Score = 50;
            _inflateWidth = 100;
            _inflateHeight = 100;
        }

        private (double MaxVal, Point MaxLocation) Inspect(
            Mat source,
            Mat template)
        {
            var result = new Mat(
                    source.Rows - template.Rows,
                    source.Cols - template.Cols,
                    MatType.CV_32FC1);

            switch (PatternMatchingAlgorithmType)
            {
                case PatternMatchingAlgorithmType.SDD:
                    Cv2.MatchTemplate(source, template, result, TemplateMatchModes.SqDiffNormed);
                    break;
                case PatternMatchingAlgorithmType.ZNCC:
                    Cv2.MatchTemplate(source, template, result, TemplateMatchModes.CCoeffNormed);
                    break;
            }

            Cv2.MinMaxLoc(
                result,
                out double minVal,
                out double maxVal,
                out Point minLoc,
                out Point maxLoc);

            return PatternMatchingAlgorithmType == PatternMatchingAlgorithmType.SDD ? 
                (1 - minVal, minLoc) : (maxVal, maxLoc);
        }

        public IAlgorithmResult Inspect(Mat source, Window window)
        {
            var sourceRect = new Rect(0, 0, source.Width, source.Height);
            var windowRect = new Rect(
                window.Region.X, 
                window.Region.Y, 
                window.Region.Width,
                window.Region.Height);

            var inflateRect = windowRect;

            inflateRect.Inflate(InflateWidth, InflateHeight);

            inflateRect = inflateRect.Intersect(sourceRect);

            if (inflateRect.Width == 0 || inflateRect.Height == 0)
                return new PatternMatchingResult();

            var roi = source.SubMat(inflateRect);

            double maxValue = 0;
            var maxLocation = new Point();
            var imageSize = new Size(window.Region.Width, window.Region.Height);

            foreach (var image in PatternMatchingImages)
            {
                var imageResult = Inspect(roi, image.Mat);
                if (maxValue < imageResult.MaxVal)
                {
                    maxValue = imageResult.MaxVal;
                    maxLocation = imageResult.MaxLocation;
                    imageSize = new Size(image.Width, image.Height);
                }
            }
            //roi.SaveImage("roi.bmp");
            var boundingRect = new Rect(maxLocation.X + inflateRect.X, maxLocation.Y + inflateRect.Y, imageSize.Width, imageSize.Height);
            //var boundingRect = new Rect(maxLocation.X + windowRect.X, maxLocation.Y + windowRect.Y, 0, 0);
            //boundingRect.Inflate(window.Region.Width / 2, window.Region.Height / 2);

            boundingRect = boundingRect.Intersect(sourceRect);

            return new PatternMatchingResult(
                this,
                Score <= maxValue * 100.0,
                maxValue * 100.0,
                windowRect,
                new Point(
                    windowRect.X + windowRect.Width / 2,
                    windowRect.Y + windowRect.Height / 2),
                new Rect(
                    boundingRect.X,
                    boundingRect.Y,
                    boundingRect.Width,
                    boundingRect.Height),
                new Point(
                    maxLocation.X + inflateRect.X + imageSize.Width / 2,
                    maxLocation.Y + inflateRect.Y + imageSize.Height / 2),
                inflateRect);
        }

        public void PostProcess()
        {
            BindingOperations.EnableCollectionSynchronization(PatternMatchingImages, new object());

            foreach (var image in PatternMatchingImages)
                image.PostProcess();
        }

        public IAlgorithm Clone()
        {
            var @new = new PatternMatchingAlgorithm();
            
            @new.Score = Score;
            @new.InflateWidth = InflateWidth;
            @new.InflateHeight = InflateHeight;
            @new.PatternMatchingAlgorithmType = PatternMatchingAlgorithmType;

            return @new;
        }
    }
}