using Mold_Inspector.Store;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mold_Inspector.Model.Algorithm.PatternMatching
{
    [Serializable]
    class PatternMatchingImage
    {
        [field: NonSerialized]
        public Mat Mat { get; set; }

        [field: NonSerialized]
        public ImageSource Source { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int Channels { get; set; }

        public byte[] Data { get; set; }

        public PatternMatchingImage()
        {

        }

        public PatternMatchingImage(Mat mat)
        {
            Mat = mat;

            Width = Mat.Width;
            Height = Mat.Height;
            Channels = Mat.Channels();

            Data = new byte[Width * Height * Channels];
            Marshal.Copy(mat.Data, Data, 0, Data.Length);
            Source = mat.ToImageSource();
        }

        public void PostProcess()
        {
            switch (Channels)
            {
                case 1:
                    Mat = new Mat(Height, Width, MatType.CV_8UC1, Data);
                    break;
                case 3:
                    Mat = new Mat(Height, Width, MatType.CV_8UC3, Data);
                    break;
            }

            Source = Mat.ToImageSource();
        }
    }
}
