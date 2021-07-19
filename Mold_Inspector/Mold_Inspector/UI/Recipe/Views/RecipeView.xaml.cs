using Mold_Inspector.UI.Recipe.ViewModels;
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

namespace Mold_Inspector.UI.Recipe.Views
{
    /// <summary>
    /// RecipeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecipeView : UserControl
    {
        private RecipeViewModel _viewModel => this.DataContext as RecipeViewModel;

        public RecipeView()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                _viewModel.SelectRecipe();
        }
    }
}
