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
    class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CraeteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (Store.CommandState)value;

            if (state == Store.CommandState.Create)
                return Visibility.Visible;
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (Store.CommandState)value;

            if (state == Store.CommandState.Selection)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// WindowView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowView : UserControl
    {
        public static readonly DependencyProperty IDProperty =
           DependencyProperty.Register(
               "ID",
               typeof(int),
               typeof(WindowView));

        public int ID
        {
            get => (int)GetValue(IDProperty);
            set => SetValue(IDProperty, value);
        }

        public static readonly DependencyProperty ZoomServiceProperty =
            DependencyProperty.Register(
                "ZoomService",
                typeof(ZoomService),
                typeof(WindowView),
                new PropertyMetadata((d, e) =>
                {
                    var view = d as WindowView;
                    var viewModel = view.DataContext as WindowViewModel;
                    viewModel.ZoomService = e.NewValue as ZoomService;
                }));

        public ZoomService ZoomService
        {
            get => (ZoomService)GetValue(ZoomServiceProperty);
            set => SetValue(ZoomServiceProperty, value);
        }

        public static readonly DependencyProperty WindowStoreProperty =
            DependencyProperty.Register(
                "WindowStore",
                typeof(WindowStore),
                typeof(WindowView),
                new PropertyMetadata((d, e) =>
                {
                    var view = d as WindowView;
                    var viewModel = view.DataContext as WindowViewModel;
                    viewModel.WindowStore = e.NewValue as WindowStore;
                }));

        internal WindowStore WindowStore
        {
            get => (WindowStore)GetValue(WindowStoreProperty);
            set => SetValue(WindowStoreProperty, value);
        }

        public WindowView()
        {
            InitializeComponent();
        }
    }
}
