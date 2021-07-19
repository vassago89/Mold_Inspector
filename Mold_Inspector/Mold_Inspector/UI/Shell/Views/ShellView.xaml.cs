using FirstFloor.ModernUI.Windows.Controls;
using Mold_Inspector.UI.Shell.ViewModels;
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

namespace Mold_Inspector.UI.Shell.Views
{
    class AndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Cast<bool>().All(v => v);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShellView : ModernWindow
    {
        private ShellViewModel _viewModel => this.DataContext as ShellViewModel;

        public ShellView()
        {
            InitializeComponent();

            _viewModel.AddPage("null", null);

            _viewModel.AddPage("Main", Main);
            _viewModel.AddPage("Recipe", Recipe);
            _viewModel.AddPage("Setting", Setting);

            _viewModel.Initialize();
        }
    }
}
