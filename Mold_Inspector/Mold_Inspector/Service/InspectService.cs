using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Store;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Service
{
    public enum InspectMode
    {
        Production, Mold
    }

    class InspectService
    {
        private ImageStore _imageStore;
        private RecipeStore _recipeStore;
        private CameraStore _cameraStore;

        private ResultStore _resultStore;

        public InspectService(
            CameraStore cameraStore,
            ImageStore imageStore,
            RecipeStore recipeStore,
            ResultStore resultStore)
        {
            _cameraStore = cameraStore;
            _imageStore = imageStore;
            _recipeStore = recipeStore;
            _resultStore = resultStore;
        }

        public bool RefreshPattern(InspectMode mode)
        {
            if (_recipeStore.Selected == null)
                return false;

            foreach (var windows in _recipeStore.Selected.Windows[mode].Values)
            {
                foreach (var window in windows)
                {
                    var patternMatching = window.Algorithm as PatternMatchingAlgorithm;

                    if (patternMatching == null)
                        continue;

                    var mat = _imageStore.GetMat(window.CameraID);
                    if (mat == null)
                        return false;

                    var whole = new Rectangle(0, 0, mat.Width, mat.Height);

                    var region = window.Region;
                    region.Intersect(whole);

                    if (region.Width < 0 || region.Height < 0)
                        return false;

                    patternMatching.PatternMatchingImages.Clear();

                    var clone = mat.Clone(new OpenCvSharp.Rect(region.X, region.Y, region.Width, region.Height));
                    patternMatching.PatternMatchingImages.Add(new PatternMatchingImage(clone));
                }
            }

            return true;
        }

        public bool Inspect(InspectMode mode)
        {
            var results = new List<(int ID, IAlgorithmResult Result)>();

            foreach (var camera in _cameraStore.Cameras)
            {
                var mat = _imageStore.GetMat(camera.ID);
                if (mat == null || _recipeStore.Selected == null)
                    return false;

                results.AddRange(Inspect(mode, mat, camera));
            }

            _resultStore.Refresh();
            _resultStore.InpectDone(results);

            return results.All(r => r.Result.Pass);
        }

        private IEnumerable<(int ID, IAlgorithmResult Result)> Inspect(InspectMode mode, Mat mat, CameraInfoWrapper camera)
        {
            var results = new List<(int ID, IAlgorithmResult Result)>();

            if (_recipeStore.Selected == null)
                return results;

            if (_recipeStore.Selected.Windows.ContainsKey(mode) == false)
                return results;

            if (_recipeStore.Selected.Windows[mode].ContainsKey(camera.ID) == false)
                return results;

            foreach (var window in _recipeStore.Selected.Windows[mode][camera.ID])
                results.Add((camera.ID, window.Algorithm.Inspect(mat, window)));

            return results;
        }

        private void InspectProduction(List<(int ID, IAlgorithmResult Result)> results, CameraInfoWrapper camera, Mat mat)
        {
            //foreach (var window in _recipeStore.Selected.Production.Where(w => w.CameraID == camera.ID))
            //{
            //    results.Add(
            //        (window.CameraID, window.Algorithm.Inspect(mat, window)));
            //}
        }
    }
}
