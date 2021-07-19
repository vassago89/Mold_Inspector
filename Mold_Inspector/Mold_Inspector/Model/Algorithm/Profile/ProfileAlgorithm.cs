using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.Profile
{
    [Serializable]
    enum ProfileOrientation
    {
        Horizontal, Vertical
    }

    [Serializable]
    enum ProfileMode
    {
        Absolute, Range
    }

    [Serializable]
    class ProfileAlgorithm : IAlgorithm
    {
        public static IEnumerable<ProfileOrientation> Orientations
            => Enum.GetValues(typeof(ProfileOrientation)).Cast<ProfileOrientation>();

        public static IEnumerable<ProfileMode> ProfileModes
            => Enum.GetValues(typeof(ProfileMode)).Cast<ProfileMode>();

        private ProfileMode _profileMode;
        public ProfileMode ProfileMode
        {
            get => _profileMode;
            set
            {
                _profileMode = value;
                ParameterChanged?.Invoke();
            }
        }

        private ProfileOrientation _orientation;
        public ProfileOrientation Orientation 
        {
            get => _orientation;
            set
            {
                _orientation = value;
                ParameterChanged?.Invoke();
            }
        }

        private byte _lower;
        public byte Lower
        {
            get => _lower;
            set
            {
                _lower = value;
                ParameterChanged?.Invoke();
            }
        }

        private byte _upper;
        public byte Upper 
        { 
            get => _upper;
            set
            {
                _upper = value;
                ParameterChanged?.Invoke();
            } 
        }

        private byte _range;
        public byte Range
        {
            get => _range;
            set
            {
                _range = value;
                ParameterChanged?.Invoke();
            }
        }

        public AlgorithmType AlgorithmType => AlgorithmType.Profile;

        public Action ParameterChanged { get; set; }

        public ProfileAlgorithm()
        {

        }


        public IAlgorithmResult Inspect(Mat source, Window window)
        {
            var whole = new System.Drawing.Rectangle(0, 0, source.Width, source.Height);
            var region = window.Region;
            region.Intersect(whole);

            if (region.Width <= 0 || region.Height <= 0)
                return new ProfileResult(
                    this,
                    false,
                    new System.Windows.Rect(region.X, region.Y, region.Width, region.Height),
                    new System.Windows.Point(region.X + region.Width / 2, region.Y + region.Height / 2));

            var profile = GetProfile(source, region);
            var min = profile.Min();
            var max = profile.Max();
            var average = profile.Average();

            var result = true;
            switch (ProfileMode)
            {
                case ProfileMode.Absolute:
                    if (min < Lower || max > Upper)
                        result = false;
                    break;
                case ProfileMode.Range:
                    if (min < average - Range || max > average + Range)
                        result = false;
                    break;
            }

            return new ProfileResult(
                    this,
                    result,
                    min,
                    max,
                    average,
                    new System.Windows.Rect(region.X, region.Y, region.Width, region.Height),
                    new System.Windows.Point(region.X + region.Width / 2, region.Y + region.Height / 2));
        }

        public IEnumerable<double> GetProfile(Mat source, System.Drawing.Rectangle region)
        {
            var clone = source.Clone(new OpenCvSharp.Rect(region.X, region.Y, region.Width, region.Height));

            if (clone.Channels() == 3)
                Cv2.CvtColor(clone, clone, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            if (Orientation == ProfileOrientation.Horizontal)
                clone = clone.Transpose();

            var data = new byte[clone.Width * clone.Height];
            Marshal.Copy(clone.Data, data, 0, clone.Width * clone.Height);

            var profile = new double[clone.Height];
            for (int i = 0, y = 0; y < clone.Height; y++)
            {
                double sum = 0;
                for (int x = 0; x < clone.Width; x++, i++)
                    sum += data[i];

                profile[y] = sum / clone.Width;
            }
            return profile;
        }

        public void PostProcess()
        {

        }

        public IAlgorithm Clone()
        {
            var @new = new ProfileAlgorithm();
            
            @new.Range = Range;
            @new.Upper = Upper;
            @new.Lower = Lower;
            @new.Orientation = Orientation;
            @new.ProfileMode = ProfileMode;

            return @new;
        }
    }
}

