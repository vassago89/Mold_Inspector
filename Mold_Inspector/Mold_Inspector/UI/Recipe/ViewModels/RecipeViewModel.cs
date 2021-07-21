using FirstFloor.ModernUI.Windows.Controls;
using Mold_Inspector.Config;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mold_Inspector.Model;
using Mold_Inspector.Service;
using Mold_Inspector.Device;

namespace Mold_Inspector.UI.Recipe.ViewModels
{
    class RecipeViewModel : BindableBase
    {
        private string _registrationName;
        public string RegistrationName
        {
            get => _registrationName;
            set => SetProperty(ref _registrationName, value);
        }

        private Model.Recipe _currentRecipe;
        public Model.Recipe CurrentRecipe 
        {
            get => _currentRecipe;
            set => SetProperty(ref _currentRecipe, value);
        }

        public RecipeStore RecipeStore { get; }
        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Model.Recipe> RemoveRecipeCommand { get; }

        private StateStore _stateStore;
        private TeachingStore _teachingStore;
        private PageStore _pageStore;

        private ResultStore _resultStore;

        private CameraServiceMediator _mediator;
        private CameraStore _cameraStore;

        private IOService _ioService;
        private DefaultStore _defaultStore;
        private InspectStore _inspectStore;


        public RecipeViewModel(
            IOService ioService,
            CameraServiceMediator mediator,
            CameraStore cameraStore,
            StateStore stateStore,
            TeachingStore teachingStore,
            RecipeStore recipeStore,
            ResultStore resultStore,
            PageStore pageStore,
            InspectStore inspectStore,
            DefaultStore defaultStore)
        {
            _ioService = ioService;
            _stateStore = stateStore;
            _teachingStore = teachingStore;
            _pageStore = pageStore;
            _defaultStore = defaultStore;
            _inspectStore = inspectStore;

            RecipeStore = recipeStore;

            _resultStore = resultStore;

            _mediator = mediator;
            _cameraStore = cameraStore;

            AddRecipeCommand = new DelegateCommand(() =>
            {
                AddRecipe();
            });

            RemoveRecipeCommand = new DelegateCommand<Model.Recipe>((recipe) =>
            {
                RecipeStore.RemoveRecipe(recipe);
            });

            if (RecipeStore.Recipes.Count > 0)
            {
                CurrentRecipe = RecipeStore.Recipes.OrderByDescending(r => r.LastUsedDate).First();
                SelectRecipe();
            }
        }

        private void AddRecipe()
        {
            if (RecipeStore.AddRecipe(RegistrationName) == false)
                RegistrationName = null;
        }

        public void SelectRecipe()
        {
            if (RecipeStore.Selected != null)
                _mediator.Disconnect();

            _resultStore.Refresh();

            if (_currentRecipe != null)
                _currentRecipe.PostProcess(_cameraStore, _defaultStore);

            RecipeStore.SelectRecipe(_currentRecipe);

            RecipeStore.Save();

            _inspectStore.Reset();
            _stateStore.Initialize();
            _teachingStore.IsLock = false;
            _teachingStore.InspectMode = InspectMode.Production;
            
            _pageStore.SelectPage("Main");
            _ioService.StartManualMode();
        }
    }
}
