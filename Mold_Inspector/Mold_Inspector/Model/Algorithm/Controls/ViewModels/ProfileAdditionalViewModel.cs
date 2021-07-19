using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Model.Algorithm.Profile;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.Controls.ViewModels
{
    class ProfileAdditionalViewModel : BindableBase
    {
        private ZoomService _zoomService;
        public ZoomService ZoomService 
        { 
            get => _zoomService;
            set => SetProperty(ref _zoomService, value); 
        }

        private ProfileAlgorithm _algorithm;
        private TeachingStore _teachingStore;
        private Window _window;

        private Point _start;
        public Point Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        private Point _end;
        public Point End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        public ProfileAdditionalViewModel(TeachingStore teachingStore)
        {
            _teachingStore = teachingStore;
            _window = teachingStore.Window;
            _algorithm = teachingStore.Window.Algorithm as ProfileAlgorithm;

            teachingStore.WindowChanged += Refresh;
            _algorithm.ParameterChanged += Refresh;

            Refresh();
        }

        ~ProfileAdditionalViewModel()
        {
            _algorithm.ParameterChanged -= Refresh;
            _teachingStore.WindowChanged -= Refresh;
        }

        private void Refresh()
        {
            switch (_algorithm.Orientation)
            {
                case ProfileOrientation.Horizontal:
                    Start = new Point(
                        _window.Region.X,
                        _window.Region.Y + _window.Region.Height / 2);
                    End = new Point(
                        _window.Region.X + _window.Region.Width,
                        _window.Region.Y + _window.Region.Height / 2);
                    break;
                case ProfileOrientation.Vertical:
                    Start = new Point(
                        _window.Region.X + _window.Region.Width / 2,
                        _window.Region.Y);
                    End = new Point(
                        _window.Region.X + _window.Region.Width / 2,
                        _window.Region.Y + _window.Region.Height);
                    break;
            }
        }
    }
}
