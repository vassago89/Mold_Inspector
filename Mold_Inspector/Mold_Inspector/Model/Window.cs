using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Algorithm.BinaryCount;
using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Model.Algorithm.Profile;
using Mold_Inspector.Model.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mold_Inspector.Model
{
    [Serializable]
    public class Window
    {
        public int CameraID { get; set; }
        public Rectangle Region { get; set; }
        public IAlgorithm Algorithm { get; set; }
        
        [field:NonSerialized]
        public object Color
        {   
            get
            {
                switch (Algorithm.AlgorithmType)
                {
                    
                    case AlgorithmType.Pattern:
                        return App.Current.Resources["PatternBrush"];
                    case AlgorithmType.Binary:
                        return App.Current.Resources["BinaryBrush"];
                    case AlgorithmType.Profile:
                        return App.Current.Resources["ProfileBrush"];
                }

                return null;
            }
        }

        private Window()
        {

        }

        public static Window CreateWindow<T>(int cameraID, Rectangle region, T @default) where T : IAlgorithm
        {
            var @new = new Window()
            {
                CameraID = cameraID,
                Region = region,
                Algorithm = @default.Clone()
            };

            return @new;
        }

        public SelectDirection CheckDirection(int x, int y, int capacity)
        {
            var inflate = Region;
            inflate.Inflate(capacity, capacity);
            if (inflate.Contains(x, y) == false)
                return SelectDirection.None;

            if (Math.Abs(x - Region.X) < capacity && Math.Abs(y - Region.Y) < capacity)
                return SelectDirection.LeftTop;

            if (Math.Abs(x - Region.X) < capacity && Math.Abs(y - Region.Bottom) < capacity)
                return SelectDirection.LeftBottom;

            if (Math.Abs(x - Region.Right) < capacity && Math.Abs(y - Region.Y) < capacity)
                return SelectDirection.RightTop;

            if (Math.Abs(x - Region.Right) < capacity && Math.Abs(y - Region.Bottom) < capacity)
                return SelectDirection.RightBottom;

            if (Math.Abs(x - Region.X) < capacity)
                return SelectDirection.Left;

            if (Math.Abs(x - Region.Right) < capacity)
                return SelectDirection.Right;

            if (Math.Abs(y - Region.Y) < capacity)
                return SelectDirection.Top;

            if (Math.Abs(y - Region.Bottom) < capacity)
                return SelectDirection.Bottom;

            return SelectDirection.Move;
        }

        public void PostProcess()
        {
            Algorithm.PostProcess();
        }
    }
}
