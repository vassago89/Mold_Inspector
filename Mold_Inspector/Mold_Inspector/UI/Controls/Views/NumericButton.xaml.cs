using Prism.Commands;
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
    /// NumericButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumericButton : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register(
               "Value",
               typeof(decimal),
               typeof(NumericButton),
               new FrameworkPropertyMetadata((decimal)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, Math.Round(value, 2));
        }

        public DelegateCommand UpCommand { get; }
        public DelegateCommand DownCommand { get; }

        public NumericButton()
        {
            UpCommand = new DelegateCommand(() =>
            {
                Value++;
            });

            DownCommand = new DelegateCommand(() =>
            {
                Value--;
            });

            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (decimal.TryParse(textBox.Text, out decimal value))
                Value = value;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (e.Key == Key.Enter)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
