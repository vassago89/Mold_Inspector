using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Algorithm.Controls.ViewModels
{
    class PatternMatchingAdditionalViewModel : BindableBase
    {
        private ZoomService _zoomService;
        public ZoomService ZoomService 
        { 
            get => _zoomService;
            set => SetProperty(ref _zoomService, value); 
        }

        private Rectangle _inflate;
        public Rectangle Inflate
        {
            get => _inflate;
            set => SetProperty(ref _inflate, value);
        }

        TeachingStore _teachingStore;

        public PatternMatchingAdditionalViewModel(TeachingStore teachingStore)
        {
            _teachingStore = teachingStore;

            _teachingStore.WindowChanged += Refresh;
            _teachingStore.Window.Algorithm.ParameterChanged += Refresh;

            Refresh();
        }

        ~PatternMatchingAdditionalViewModel()
        {
            if (_teachingStore.Window != null)
            {
                _teachingStore.WindowChanged -= Refresh;
                _teachingStore.Window.Algorithm.ParameterChanged -= Refresh;
            }
        }

        private void Refresh()
        {
            if (_teachingStore.Window == null)
                return;

            var patternMatching = _teachingStore.Window.Algorithm as PatternMatchingAlgorithm;
            if (patternMatching == null)
                return;

            var region = _teachingStore.Window.Region;
            region.Inflate(patternMatching.InflateWidth, patternMatching.InflateHeight);

            Inflate = region;
        }
    }
}
