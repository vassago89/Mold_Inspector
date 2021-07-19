using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Controls.ViewModels
{
    class WindowViewModel : BindableBase
    {
        public MouseStore MouseStore { get; }
        public RecipeStore RecipeStore { get; }

        private ZoomService _zoomService;
        public ZoomService ZoomService
        {
            get => _zoomService; 
            set => SetProperty(ref _zoomService, value); 
        }

        private WindowStore _windowStore;
        public WindowStore WindowStore
        {
            get => _windowStore;
            set => SetProperty(ref _windowStore, value);
        }

        public WindowViewModel(
            MouseStore mouseStore,
            RecipeStore recipeStore)
        {
            MouseStore = mouseStore;
            RecipeStore = recipeStore;
        }
    }
}
