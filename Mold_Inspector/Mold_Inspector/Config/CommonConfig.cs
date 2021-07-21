﻿using Mold_Inspector.Config.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mold_Inspector.Config
{
    enum ImageViewMode
    {
        Stretch, Vertical
    }

    class CommonConfig : JsonConfig<CommonConfig>
    {
        public uint DefaultTarget { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public Stretch Stretch { get; set; }
        public string Culture { get; set; }
        public string Theme { get; set; }
        public Color AccentColor { get; set; }
        public string AccentName { get; set; }

        public CommonConfig()
        {
            DefaultTarget = 1000;

            Stretch = Stretch.Uniform;
            Cols = 1;
            Rows = 1;
            Culture = "en-US";
            Theme = "Light";
            AccentColor = Colors.SteelBlue;
            AccentName = "SteelBlue";
        }
    }
}
