using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using Mold_Inspector.Model.Algorithm.Profile;
using Mold_Inspector.Store;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.Controls.ViewModels
{
    class ProfileViewModel : BindableBase
    {
        private bool _isVertical;
        public bool IsVertical
        {
            get => _isVertical;
            set => SetProperty(ref _isVertical, value);
        }

        private bool _isHorizontal;
        public bool IsHorizontal
        {
            get => _isHorizontal;
            set => SetProperty(ref _isHorizontal, value);
        }

        private bool _isAbsolute;
        public bool IsAbsolute
        {
            get => _isAbsolute;
            set => SetProperty(ref _isAbsolute, value);
        }

        private bool _isRange;
        public bool IsRange
        {
            get => _isRange;
            set => SetProperty(ref _isRange, value);
        }

        private ImageStore _imageStore;

        private ChartValues<double> _chartValues;
        public ChartValues<double> ChartValues
        {
            get => _chartValues;
            set => SetProperty(ref _chartValues, value);
        }

        private ChartValues<double> _lowerValues;
        public ChartValues<double> LowerValues
        {
            get => _lowerValues;
            set => SetProperty(ref _lowerValues, value);
        }

        private ChartValues<double> _upperValues;
        public ChartValues<double> UpperValues
        {
            get => _upperValues;
            set => SetProperty(ref _upperValues, value);
        }

        public ProfileAlgorithm Algorithm { get; }
        private Window _window;

        public ProfileViewModel(
            TeachingStore teachingStore,
            ImageStore imageStore)
        {
            ChartValues = new ChartValues<double>();
            LowerValues = new ChartValues<double>();
            UpperValues = new ChartValues<double>();

            _imageStore = imageStore;

            _window = teachingStore.Window;

            if (_window == null)
                return;

            Algorithm = _window.Algorithm as ProfileAlgorithm;
            Algorithm.ParameterChanged += CalcProfile;

            CalcProfile();
        }

        ~ProfileViewModel()
        {
            if (Algorithm != null)
                Algorithm.ParameterChanged -= CalcProfile;
        }

        private void CalcProfile()
        {
            IsVertical = Algorithm.Orientation == ProfileOrientation.Vertical;
            IsHorizontal = Algorithm.Orientation == ProfileOrientation.Horizontal;

            IsRange = Algorithm.ProfileMode == ProfileMode.Range;
            IsAbsolute = Algorithm.ProfileMode == ProfileMode.Absolute;

            ChartValues.Clear();
            LowerValues.Clear();
            UpperValues.Clear();

            var mat = _imageStore.GetMat(_window.CameraID);
            if (mat == null)
                return;

            var whole = new System.Drawing.Rectangle(0, 0, mat.Width, mat.Height);
            var region = _window.Region;
            region.Intersect(whole);

            if (region.Width <= 0 || region.Height <= 0)
                return;
            
            var profile = Algorithm.GetProfile(mat, region);

            ChartValues.AddRange(profile);

            switch (Algorithm.ProfileMode)
            {
                case ProfileMode.Absolute:
                    LowerValues.AddRange(
                        Enumerable.Repeat(
                            (double)Algorithm.Lower, profile.Count()));
                    UpperValues.AddRange(
                        Enumerable.Repeat(
                            (double)Algorithm.Upper, profile.Count()));
                    break;
                case ProfileMode.Range:
                    var average = profile.Average();
                    var lower = Math.Max(0, average - Algorithm.Range);
                    var upper = Math.Min(255, average + Algorithm.Range);
                    LowerValues.AddRange(
                        Enumerable.Repeat(
                            lower, profile.Count()));
                    UpperValues.AddRange(
                        Enumerable.Repeat(
                            upper, profile.Count()));
                    break;
            }
        }
    }
}
