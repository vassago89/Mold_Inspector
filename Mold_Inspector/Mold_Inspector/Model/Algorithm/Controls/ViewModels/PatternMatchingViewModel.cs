using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.Controls.ViewModels
{
    class PatternMatchingViewModel : BindableBase
    {
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<PatternMatchingImage> RemoveCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        public TeachingStore TeachingStore { get; }
        private ImageStore _imageStore;

        private IAlgorithm _algorithm;

        public PatternMatchingViewModel(
            TeachingStore teachingStore,
            ImageStore imageStore)
        {
            _imageStore = imageStore;
            TeachingStore = teachingStore;

            AddCommand = new DelegateCommand(() =>
            {
                AddImage(TeachingStore.Window.Algorithm as PatternMatchingAlgorithm);
            });


            RemoveCommand = new DelegateCommand<PatternMatchingImage>(image =>
            {
                var patternMatching = TeachingStore.Window.Algorithm as PatternMatchingAlgorithm;
                patternMatching.PatternMatchingImages.Remove(image);
            });

            RefreshCommand = new DelegateCommand(() =>
            {
                var patternMatching = TeachingStore.Window.Algorithm as PatternMatchingAlgorithm;
                patternMatching.PatternMatchingImages.Clear();

                AddImage(patternMatching);
            });
        }

        private void AddImage(PatternMatchingAlgorithm patternMatching)
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
            patternMatching.PatternMatchingImages.Add(new PatternMatchingImage(clone));
        }
    }
}