using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Mold_Inspector.UI.Controls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mold_Inspector.UI.Controls.Views
{
    /// <summary>
    /// SelectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ControlView : UserControl
    {
        public static readonly DependencyProperty ZoomServiceProperty =
            DependencyProperty.Register(
                "ZoomService",
                typeof(ZoomService),
                typeof(ControlView),
                new PropertyMetadata((d, e) =>
                {
                    var view = d as ControlView;
                    var viewModel = view.DataContext as ControlViewModel;
                    viewModel.ZoomService = e.NewValue as ZoomService;
                }));

        public ZoomService ZoomService
        {
            get => (ZoomService)GetValue(ZoomServiceProperty);
            set => SetValue(ZoomServiceProperty, value);
        }

        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register(
                "Canvas",
                typeof(FrameworkElement),
                typeof(ControlView), 
                new PropertyMetadata((d, e) =>
                {
                    var view = d as ControlView;
                    var viewModel = view.DataContext as ControlViewModel;
                    viewModel.Canvas = e.NewValue as FrameworkElement;
                }));

        public FrameworkElement Canvas
        {
            get => (FrameworkElement)GetValue(CanvasProperty);
            set => SetValue(CanvasProperty, value);
        }

        public static readonly DependencyProperty WindowStoreProperty =
           DependencyProperty.Register(
               "WindowStore",
               typeof(WindowStore),
               typeof(ControlView),
               new PropertyMetadata((d, e) =>
               {
                   var view = d as ControlView;
                   var viewModel = view.DataContext as ControlViewModel;
                   viewModel.WindowStore = e.NewValue as WindowStore;
               }));

        internal WindowStore WindowStore
        {
            get => (WindowStore)GetValue(WindowStoreProperty);
            set => SetValue(WindowStoreProperty, value);
        }

        public static readonly DependencyProperty IDProperty =
           DependencyProperty.Register(
               "ID",
               typeof(int),
               typeof(ControlView),
               new PropertyMetadata(-1, (d, e) =>
               {
                   var view = d as ControlView;
                   var viewModel = view.DataContext as ControlViewModel;
                   viewModel.ID = (int)e.NewValue;
               }));

        public int ID
        {
            get => (int)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        ControlViewModel _viewModel => DataContext as ControlViewModel;

        public ControlView()
        {
            InitializeComponent();

            _viewModel.Control = Control;
        }

        private void MouseDownHandle(object sender, MouseButtonEventArgs e)
        {
            Focus();
            
            if (e.ClickCount > 1)
            {
                _viewModel.CommandRelease();
                return;
            }

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _viewModel.MouseHandle(MouseAction.LeftClick, e);
                    break;
                case MouseButton.Middle:
                    _viewModel.MouseHandle(MouseAction.WheelClick, e);
                    break;
                case MouseButton.Right:
                    _viewModel.MouseHandle(MouseAction.RightClick, e);
                    break;
            }
        }

        private void MouseUpHandle(object sender, MouseButtonEventArgs e)
        {
            _viewModel.Release();
        }

        private void MouseWheelHandle(object sender, MouseWheelEventArgs e)
        {
            _viewModel.Zoom(e);
        }

        private void MouseMoveHandle(object sender, MouseEventArgs e)
        {
            var direction = _viewModel.Move(e);

            switch (direction)
            {
                case SelectDirection.None:
                    Cursor = Cursors.Arrow;
                    break;
                case SelectDirection.Move:
                    Cursor = Cursors.Hand;
                    break;
                case SelectDirection.Left:
                case SelectDirection.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case SelectDirection.Top:
                case SelectDirection.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
                case SelectDirection.LeftTop:
                case SelectDirection.RightBottom:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case SelectDirection.RightTop:
                case SelectDirection.LeftBottom:
                    Cursor = Cursors.SizeNESW;
                    break;
                case SelectDirection.Select:
                    Cursor = Cursors.Hand;
                    break;
            }
        }

        private void MouseLeaveHandle(object sender, MouseEventArgs e)
        {
            _viewModel.Release();
        }

        private void KeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.RemoveWindows();
            }
            else if (e.Key == Key.Escape)
            {
                _viewModel.CommandRelease();
            }
        }
    }
}