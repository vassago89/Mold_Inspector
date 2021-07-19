using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Mold_Inspector.UI.Controls.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    class AlgorithmResultColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Green" : "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// ResultView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ResultView : UserControl
    {
        private ResultViewModel _viewModel => DataContext as ResultViewModel;

        public static readonly DependencyProperty IDProperty =
           DependencyProperty.Register(
               "ID",
               typeof(int),
               typeof(ResultView),
               new PropertyMetadata(-1, (d, e) =>
               {
                   var view = d as ResultView;
                   var viewModel = view.DataContext as ResultViewModel;
                   viewModel.Unsubscribe((int)e.OldValue);
                   viewModel.Subscribe((int)e.NewValue);
               }));

        public int ID
        {
            get => (int)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        public static readonly DependencyProperty ZoomServiceProperty =
           DependencyProperty.Register(
               "ZoomService",
               typeof(ZoomService),
               typeof(ResultView),
               new PropertyMetadata((d, e) =>
               {
                   var view = d as ResultView;
                   var viewModel = view.DataContext as ResultViewModel;
                   viewModel.ZoomService = view.ZoomService;
               }));

        public ZoomService ZoomService
        {
            get => (ZoomService)GetValue(ZoomServiceProperty);
            set => SetValue(ZoomServiceProperty, value);
        }

        public ResultView()
        {
            InitializeComponent();
        }
    }
}
