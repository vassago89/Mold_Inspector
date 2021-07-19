using Mold_Inspector.Config;
using Mold_Inspector.Device;
using Mold_Inspector.Device.Camera;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Mold_Inspector.UI.Controls.ViewModels
{
    class CameraViewModel : BindableBase
    {
        public ZoomService ZoomService { get; }
        public FrameworkElement Canvas { get; set; }

        private BitmapSource _source;
        public BitmapSource Source
        {
            get => _source;
            set
            {
                SetProperty(ref _source, value);
                ZoomFit();
            }
        }

        ImageStore _imageStore;

        private WindowStore _windowStore;
        public WindowStore WindowStore
        {
            get => _windowStore;
            set => SetProperty(ref _windowStore, value);
        }

        private int _id;

        public RecipeStore RecipeStore { get; }
        public TeachingStore TeachingStore { get; }

        public CameraViewModel(
            ImageStore imageStore,
            RecipeStore recipeStore,
            TeachingStore teachingStore)
        {
            _imageStore = imageStore;

            RecipeStore = recipeStore;
            TeachingStore = teachingStore;

            RecipeStore.RecipeChanged += RecipeChanged;

            ZoomService = new ZoomService();
        }

        ~CameraViewModel()
        {
            RecipeStore.RecipeChanged -= RecipeChanged;
        }

        public void RecipeChanged()
        {
            if (RecipeStore.Selected != null && RecipeStore.Selected.SizeDictionary.ContainsKey(_id))
            {
                ZoomService.ZoomFit(
                       Canvas.ActualWidth, 
                       Canvas.ActualHeight,
                       RecipeStore.Selected.SizeDictionary[_id].WIdth,
                       RecipeStore.Selected.SizeDictionary[_id].Height);
            }
        }

        public void Subscribe(int id)
        {
            _id = id;

            _imageStore.Subscribe(id, ImageCreated);
            Source = (BitmapSource)_imageStore.GetSouce(id);

            WindowStore = new WindowStore(id, TeachingStore, RecipeStore);
            RecipeChanged();
        }

        public void Unsubscribe(int id) => _imageStore.Unsubscribe(id, ImageCreated);

        private void ImageCreated(ImageSource source)
        {
            Source = (BitmapSource)source;

            if (Source != null && RecipeStore.Selected != null)
                RecipeStore.Selected.SizeDictionary[_id] = (Source.PixelWidth, Source.PixelHeight);
        }

        public void ZoomFit()
        {
            if (Source == null)
                return;

            if (ZoomService.CheckAccess())
            {
                ZoomService.ZoomFit(
                        Canvas.ActualWidth, Canvas.ActualHeight,
                        Source.PixelWidth, Source.PixelHeight);
            }
            else
            {
                ZoomService.Dispatcher.Invoke(() =>
                {
                    ZoomService.ZoomFit(
                        Canvas.ActualWidth, Canvas.ActualHeight,
                        Source.PixelWidth, Source.PixelHeight);
                });
            }
        }
    }
}
