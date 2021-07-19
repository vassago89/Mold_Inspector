using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mold_Inspector.Model.Algorithm.Controls.Views
{
    class AlgorithmDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject d)
        {
            var algorithm = item as IAlgorithm;
            if (algorithm == null)
                return null;

            var fe = d as FrameworkElement;

            return fe.TryFindResource(algorithm.AlgorithmType.ToString()) as DataTemplate;
        }
    }
}
