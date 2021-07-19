using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mold_Inspector.Model.Algorithm.Controls.Views
{
    class AlgorithmResultDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Pattern { get; set; }
        public DataTemplate Binary { get; set; }
        public DataTemplate Profile { get; set; }

        public AlgorithmResultDataTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject d)
        {
            var result = item as IAlgorithmResult;
            if (result == null || result.Algorithm == null)
                return null;

            switch (result.Algorithm.AlgorithmType)
            {
                case AlgorithmType.Pattern:
                    return Pattern;
                case AlgorithmType.Binary:
                    return Binary;
                case AlgorithmType.Profile:
                    return Profile;
            }

            return null;
        }
    }
}
