﻿using System;
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
    /// PatternMatchingResultView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BinaryCountResultView : UserControl
    {
        //public static readonly DependencyProperty ResultProperty =
        //   DependencyProperty.Register(
        //       "Result",
        //       typeof(IAlgorithmResult),
        //       typeof(BinaryCountResultView));

        //public IAlgorithmResult Result
        //{
        //    get => (IAlgorithmResult)GetValue(ResultProperty);
        //    set => SetValue(ResultProperty, value);
        //}

        public BinaryCountResultView()
        {
            InitializeComponent();
        }
    }
}