using Mold_Inspector.Model.Algorithm.Controls.ViewModels;
using Mold_Inspector.Model.Selection;
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

namespace Mold_Inspector.Model.Algorithm.Controls.Views
{
    /// <summary>
    /// PatternMatchingAdditionalDataTemplate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PatternMatchingAdditionalView : UserControl
    {
        public static readonly DependencyProperty ZoomServiceProperty =
            DependencyProperty.Register(
                "ZoomService",
                typeof(ZoomService),
                typeof(PatternMatchingAdditionalView),
                new PropertyMetadata((d, e) =>
                {
                    var view = d as PatternMatchingAdditionalView;
                    var viewModel = view.DataContext as PatternMatchingAdditionalViewModel;
                    viewModel.ZoomService = e.NewValue as ZoomService;
                }));

        public ZoomService ZoomService
        {
            get => (ZoomService)GetValue(ZoomServiceProperty);
            set => SetValue(ZoomServiceProperty, value);
        }

        public PatternMatchingAdditionalView()
        {
            InitializeComponent();
        }
    }
}
