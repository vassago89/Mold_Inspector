using Mold_Inspector.Device;
using Mold_Inspector.Device.Camera;
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
    /// ImageControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CameraView : UserControl
    {
        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register(
                "ID",
                typeof(int),
                typeof(CameraView), 
                new PropertyMetadata(-1, (d, e) =>
                {
                    var view = d as CameraView;
                    var viewModel = view.DataContext as CameraViewModel;
                    viewModel.Unsubscribe((int)e.OldValue);
                    viewModel.Subscribe((int)e.NewValue);
                }));

        public int ID
        {
            get => (int)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        public CameraView()
        {
            InitializeComponent();

            var viewModel = DataContext as CameraViewModel;
            viewModel.Canvas = Canvas;
        }

        private void SizeChangedHandle(object sender, SizeChangedEventArgs e)
        {
            var viewModel = DataContext as CameraViewModel;
            viewModel.ZoomFit();
        }
    }
}
