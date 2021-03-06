using Mold_Inspector.Config;
using Mold_Inspector.Store.Abstract;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mold_Inspector.Store
{
    class CommonStore : ConfigStore<CommonConfig>
    {
        private uint _defaultTarget;
        public uint DefaultTarget
        {
            get => _defaultTarget;
            set => SetProperty(ref _defaultTarget, value);
        }

        private int _cols;
        public int Cols 
        {
            get => _cols;
            set => SetProperty(ref _cols, value);
        }

        private int _rows;
        public int Rows 
        {
            get => _rows;
            set => SetProperty(ref _rows, value); 
        }

        private Stretch _stretch;
        public Stretch Stretch 
        { 
            get => _stretch; 
            set => SetProperty(ref _stretch, value); 
        }

        private string _culture;
        public string Culture
        {
            get => _culture;
            set => SetProperty(ref _culture, value);
        }

        private string _theme;
        public string Theme
        {
            get => _theme;
            set => SetProperty(ref _theme, value);
        }

        private Color _accentColor;
        public Color AccentColor
        {
            get => _accentColor;
            set => SetProperty(ref _accentColor, value);
        }

        private string _accentName;
        public string AccentName
        {
            get => _accentName;
            set => SetProperty(ref _accentName, value);
        }

        private Color _patternColor;
        public Color PatternColor
        {
            get => _patternColor;
            set => SetProperty(ref _patternColor, value);
        }

        private string _patternName;
        public string PatternName
        {
            get => _patternName;
            set => SetProperty(ref _patternName, value);
        }

        private Color _binaryColor;
        public Color BinaryColor
        {
            get => _binaryColor;
            set => SetProperty(ref _binaryColor, value);
        }

        private string _binaryName;
        public string BinaryName
        {
            get => _binaryName;
            set => SetProperty(ref _binaryName, value);
        }

        private Color _profileColor;
        public Color ProfileColor
        {
            get => _profileColor;
            set => SetProperty(ref _profileColor, value);
        }

        private string _profileName;
        public string ProfileName
        {
            get => _profileName;
            set => SetProperty(ref _profileName, value);
        }

        public CommonStore() : base()
        {
            
        }

        protected override void Initialize()
        {
            
        }

        protected override void CopyFrom(CommonConfig config)
        {
            Cols = config.Cols;
            Rows = config.Rows;
            Stretch = config.Stretch;
            Culture = config.Culture;
            Theme = config.Theme;
            AccentColor = config.AccentColor;
            AccentName = config.AccentName;
            DefaultTarget = config.DefaultTarget;

            PatternColor = config.PatternColor;
            PatternName = config.PatternName;
            ProfileColor = config.ProfileColor;
            ProfileName = config.ProfileName;
            BinaryColor = config.BinaryColor;
            BinaryName = config.BinaryName;
        }

        protected override void CopyTo(CommonConfig config)
        {
            config.Cols = Cols;
            config.Rows = Rows;
            config.Stretch = Stretch;
            config.Culture = Culture;
            config.Theme = Theme;
            config.AccentColor = AccentColor;
            config.AccentName = AccentName;
            config.DefaultTarget = DefaultTarget;

            config.PatternColor = PatternColor;
            config.PatternName = PatternName;
            config.ProfileColor = ProfileColor;
            config.ProfileName = ProfileName;
            config.BinaryColor = BinaryColor;
            config.BinaryName = BinaryName;
        }
    }
}
