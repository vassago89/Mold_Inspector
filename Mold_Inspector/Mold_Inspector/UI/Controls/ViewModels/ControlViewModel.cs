using Mold_Inspector.Device;
using Mold_Inspector.Device.Camera;
using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Service;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mold_Inspector.UI.Controls.ViewModels
{
    class ControlViewModel : BindableBase
    {
        private Point _initTranslate;
        private Point _initPosition;
        
        private int _id;
        public int ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private MouseStore _mouseStore;
        private RecipeStore _recipeStore;
        private TeachingStore _teachingStore;
        private DefaultStore _defaultStore;

        public ZoomService ZoomService { get; set; }
        public FrameworkElement Control { get; set; }
        public FrameworkElement Canvas { get; set; }

        public DelegateCommand ZoomInCommand { get; set; }
        public DelegateCommand ZoomOutCommand { get; set; }
        public DelegateCommand ZoomFitCommand { get; set; }

        private WindowStore _windowStore;
        public WindowStore WindowStore
        {
            get => _windowStore;
            set => SetProperty(ref _windowStore, value);
        }

        public StateStore StateStore { get; }

        private CameraStore _cameraStore;
        private CameraServiceMediator _mediator;

        public ControlViewModel(
            TeachingStore teachingStore,
            MouseStore mouseStore,
            RecipeStore recipeStore,
            CameraStore cameraStore,
            StateStore stateStore,
            CameraServiceMediator mediator,
            DefaultStore defaultStore)
        {
            StateStore = stateStore;

            _teachingStore = teachingStore;
            _mouseStore = mouseStore;
            _recipeStore = recipeStore;
            _defaultStore = defaultStore;

            _cameraStore = cameraStore;
            _mediator = mediator;

            ZoomInCommand = new DelegateCommand(() =>
            {
                ZoomService.ZoomIn(Canvas.ActualWidth, Canvas.ActualHeight);
            });

            ZoomOutCommand = new DelegateCommand(() =>
            {
                ZoomService.ZoomOut(Canvas.ActualWidth, Canvas.ActualHeight);
            });

            ZoomFitCommand = new DelegateCommand(() =>
            {
                if (recipeStore.Selected == null)
                    return;

                if (recipeStore.Selected.SizeDictionary.ContainsKey(_id) == false)
                    return;

                ZoomService.ZoomFit(Canvas.ActualWidth, Canvas.ActualHeight, recipeStore.Selected.SizeDictionary[_id].WIdth, recipeStore.Selected.SizeDictionary[_id].Height);
            });
        }

        public void MouseHandle(MouseAction action, MouseButtonEventArgs e)
        {
            var canvasPos = e.GetPosition(Canvas);
            var controlPos = e.GetPosition(Control);
            switch (action)
            {
                case MouseAction.LeftClick:
                case MouseAction.WheelClick:
                    _mouseStore.ClickedX = canvasPos.X;
                    _mouseStore.ClickedY = canvasPos.Y;
                    break;
                case MouseAction.RightClick:
                    _initTranslate = 
                        new Point(
                            ZoomService.TranslateX,
                            ZoomService.TranslateY);
                    _initPosition =
                        new Point(controlPos.X, controlPos.Y);
                    break;
            }

            switch (action)
            {
                case MouseAction.LeftClick:
                    _mouseStore.Mouse = MouseState.Left;
                    _mouseStore.SelectDirection = SelectDirection.None;
                    SelectWindow(null);
                    if (_mouseStore.Command == CommandState.Create)
                        return;

                    _windowStore.SetSelected(null);
                    if (_recipeStore.SelectWindows(_windowStore, (int)canvasPos.X, (int)canvasPos.Y))
                    {
                        if (_teachingStore.IsLock == false)
                            _mouseStore.Command = CommandState.Windows;
                    }
                    else
                    {
                        _windowStore.ClearSelected();
                        foreach (var window in _windowStore.CurrentWindows)
                        {
                            var direction = window.CheckDirection((int)canvasPos.X, (int)canvasPos.Y, 15);
                            if (direction != SelectDirection.None)
                            {   
                                _mouseStore.SelectDirection = direction;
                                SelectWindow(window);
                                _windowStore.SetSelected(window);
                                break;
                            }
                        }

                        if (_mouseStore.SelectDirection != SelectDirection.None)
                            _mouseStore.Command = CommandState.Modify;
                        else
                            _mouseStore.Command = CommandState.Selection;
                    }
                    
                    break;
                case MouseAction.RightClick:
                    _mouseStore.Mouse = MouseState.Right;
                    break;
                case MouseAction.WheelClick:
                    _mouseStore.Mouse = MouseState.Wheel;
                    break;
            }
        }

        public SelectDirection Move(MouseEventArgs e)
        {
            var canvasPos = e.GetPosition(Canvas);
            _mouseStore.CurrentX = canvasPos.X;
            _mouseStore.CurrentY = canvasPos.Y;

            switch (_mouseStore.Mouse)
            {
                case MouseState.None:
                case MouseState.Wheel:
                    break;
                case MouseState.Left:
                    _windowStore.SetRegion(new System.Drawing.Rectangle(
                        (int)Math.Round(Math.Min(_mouseStore.ClickedX, _mouseStore.CurrentX)),
                        (int)Math.Round(Math.Min(_mouseStore.ClickedY, _mouseStore.CurrentY)),
                        (int)Math.Round(Math.Abs(_mouseStore.ClickedX - _mouseStore.CurrentX)),
                        (int)Math.Round(Math.Abs(_mouseStore.ClickedY - _mouseStore.CurrentY))));
                    break;
                case MouseState.Right:
                    var controlPos = e.GetPosition(Control);
                    ZoomService.TranslateX =
                        _initTranslate.X + (controlPos.X - _initPosition.X);
                    ZoomService.TranslateY =
                        _initTranslate.Y + (controlPos.Y - _initPosition.Y);
                    break;
            }

            if (_recipeStore.Selected == null)
                return SelectDirection.None;

            switch (_mouseStore.Command)
            {
                case CommandState.Selection:
                    return SelectDirection.None;
                case CommandState.Modify:
                    if (_teachingStore.IsLock == false)
                        _windowStore.Offset(
                            _mouseStore.SelectDirection,
                            (int)(_mouseStore.CurrentX - _mouseStore.ClickedX),
                            (int)(_mouseStore.CurrentY - _mouseStore.ClickedY));
                    return _mouseStore.SelectDirection;
                case CommandState.Windows:
                    _windowStore.Offset(
                        (int)(_mouseStore.CurrentX - _mouseStore.ClickedX),
                        (int)(_mouseStore.CurrentY - _mouseStore.ClickedY));
                    return SelectDirection.Move;
                case CommandState.Create:
                    return SelectDirection.None;
            }

            if (_windowStore.SelectedWindows.Count() > 0)
            {
                if (_windowStore.SelectedWindows.Any(
                    w => w.Region.Contains((int)canvasPos.X, (int)canvasPos.Y)))
                    return SelectDirection.Move;

                return SelectDirection.None;                
            }

            foreach (var window in _windowStore.CurrentWindows)
            {
                var direction = window.CheckDirection((int)canvasPos.X, (int)canvasPos.Y, 15);
                if (direction != SelectDirection.None)
                    return direction;
            }

            return SelectDirection.None;
        }

        public void Zoom(MouseWheelEventArgs e)
        {
            var pos = e.GetPosition(Control);
            if (e.Delta > 0)
                ZoomService.ExecuteZoom(pos.X, pos.Y, 1.1);
            else
                ZoomService.ExecuteZoom(pos.X, pos.Y, 0.9);
        }

        public void Release()
        {
            if (_recipeStore.Selected != null)
            {
                switch (_mouseStore.Command)
                {
                    case CommandState.Selection:
                        var windows = _windowStore.CurrentWindows.Where(w => _windowStore.Region.Contains(w.Region));
                        if (windows.Count() == 1)
                        {
                            _windowStore.SetSelected(windows.First());
                            SelectWindow(windows.First());
                        }
                        else
                        {
                            foreach (var window in windows)
                                _windowStore.AddSelected(window);
                        }
                        break;
                    case CommandState.Modify:
                    case CommandState.Windows:
                        _windowStore.Refresh();
                        if (_teachingStore.Window != null)
                        {
                            var temp = _teachingStore.Window;
                            SelectWindow(null);
                            SelectWindow(temp);
                            _teachingStore.Window.Algorithm.ParameterChanged?.Invoke();
                        }
                        break;
                    case CommandState.Create:
                        if (_windowStore.Region.Width > 15 || _windowStore.Region.Height > 15)
                        {
                            _windowStore.CurrentWindows.Add(
                                Model.Window.CreateWindow(
                                    _id,
                                    _windowStore.Region,
                                    _defaultStore.GetDefault(
                                        WindowStoreMediator.Instacne.Algorithm)));
                        }
                        _mouseStore.Mouse = MouseState.None;
                        _windowStore.SetRegion(System.Drawing.Rectangle.Empty);
                        return;
                }
            }

            _windowStore.SetRegion(System.Drawing.Rectangle.Empty);
            _mouseStore.Mouse = MouseState.None;
            _mouseStore.Command = CommandState.None;
        }

        private void SelectWindow(Model.Window selected)
        {
            _teachingStore.Window = selected;

            if (selected != null && _cameraStore.Cameras.Any(c => c.ID == selected.CameraID))
            {   
                var wrapper = _cameraStore.Cameras.First(c => c.ID == selected.CameraID);

                _teachingStore.Exposure = new ParameterInfoWrapper(_recipeStore.Selected, ECameraParameter.Exposure, _mediator, wrapper);
                _teachingStore.Gain = new ParameterInfoWrapper(_recipeStore.Selected, ECameraParameter.Gain, _mediator, wrapper);
                return;
            }

            _teachingStore.Exposure = null;
            _teachingStore.Gain = null;
        }
    }
}