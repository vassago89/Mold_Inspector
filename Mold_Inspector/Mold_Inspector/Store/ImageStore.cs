using Mold_Inspector.Device;
using Mold_Inspector.Device.Camera;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mold_Inspector.Store
{
    public static class GrabInfoExtensions
    {
        public static ImageSource ToImageSource(this GrabInfo grabInfo)
        {
            ImageSource source = null;
            switch (grabInfo.Channels)
            {
                case 1:
                    source = BitmapSource.Create(grabInfo.Width, grabInfo.Height, 96, 96, PixelFormats.Gray8, null, grabInfo.Data, grabInfo.Width);
                    break;
                case 3:
                    source = BitmapSource.Create(grabInfo.Width, grabInfo.Height, 96, 96, PixelFormats.Rgb24, null, grabInfo.Data, grabInfo.Width * 3);
                    break;
            }

            if (source != null)
                source.Freeze();

            return source;
        }

        public static Mat ToMat(this GrabInfo grabInfo)
        {
            switch (grabInfo.Channels)
            {
                case 1:
                    return new Mat(grabInfo.Height, grabInfo.Width, MatType.CV_8UC1, grabInfo.Data);
                case 3:
                    return new Mat(grabInfo.Height, grabInfo.Width, MatType.CV_8UC3, grabInfo.Data);
            }

            throw new NotImplementedException();
        }
    }

    public static class MatExtensions
    {
        public static ImageSource ToImageSource(this Mat mat)
        {
            ImageSource source = null;
            var size = mat.Rows * mat.Cols * mat.Channels();
            
            switch (mat.Channels())
            {
                case 1:
                    source = BitmapSource.Create(mat.Cols, mat.Rows, 96, 96, PixelFormats.Gray8, null, mat.Data, size, mat.Cols);
                    break;
                case 3:
                    source = BitmapSource.Create(mat.Cols, mat.Rows, 96, 96, PixelFormats.Rgb24, null, mat.Data, size, mat.Cols * 3);
                    break;
            }

            source?.Freeze();

            return source;
        }
    }

    class ImageStore
    {
        private Dictionary<int, ImageSource> _sourceDictionary;
        private Dictionary<int, GrabInfo> _grabInfoDictionary;
        private Dictionary<int, Mat> _matDictionary;

        private Dictionary<int, Action<ImageSource>> _sourceCreated;

        private CameraStore _cameraStore;

        public ImageStore(
            CameraServiceMediator mediator, 
            CameraStore cameraStore)
        {
            _cameraStore = cameraStore;

            _sourceDictionary = new Dictionary<int, ImageSource>();
            _grabInfoDictionary = new Dictionary<int, GrabInfo>();
            _matDictionary = new Dictionary<int, Mat>();

            _sourceCreated = new Dictionary<int, Action<ImageSource>>();

            mediator.ImageGrabbed += ImageGrabbed;
            mediator.GrabDone += GrabDone;
        }

        private void ImageGrabbed(int id, GrabInfo grabInfo)
        {
            _grabInfoDictionary[id] = grabInfo;
            _sourceDictionary[id] = grabInfo.ToImageSource();
            _matDictionary[id] = grabInfo.ToMat();

            if (_sourceCreated.ContainsKey(id))
                _sourceCreated[id]?.Invoke(_sourceDictionary[id]);
        }

        private void GrabDone(int id)
        {
            //if (_cameraStore.Cameras.Any(c => c.Info == cameraInfo) == false)
            //    return;

            //var id = _cameraStore.Cameras.First(c => c.Info == cameraInfo).ID;

            //_sourceDictionary[id] = grabInfo.ToImageSource();
        }

        public void Subscribe(int id, Action<ImageSource> action)
        {
            if (_sourceCreated.ContainsKey(id))
                _sourceCreated[id] += action;
            else
                _sourceCreated.Add(id, action);
        }

        public void Unsubscribe(int id, Action<ImageSource> action)
        {
            if (_sourceCreated.ContainsKey(id))
                _sourceCreated[id] -= action;
        }

        public ImageSource GetSouce(int id)
        {
            if (_sourceDictionary.ContainsKey(id))
                return _sourceDictionary[id];

            return null;
        }

        public GrabInfo GetGrabInfo(int id)
        {
            if (_grabInfoDictionary.ContainsKey(id))
                return _grabInfoDictionary[id];

            return new GrabInfo();
        }

        public Mat GetMat(int id)
        {
            if (_matDictionary.ContainsKey(id))
                return _matDictionary[id];

            return null;
        }
    }
}
