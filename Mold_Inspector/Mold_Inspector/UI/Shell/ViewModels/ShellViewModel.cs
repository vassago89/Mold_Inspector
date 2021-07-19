using Mold_Inspector.Service;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mold_Inspector.UI.Shell.ViewModels
{
    class ShellViewModel : BindableBase
    {
        public StateStore StateStore { get; }
        public RecipeStore RecipeStore { get; }

        public PageStore PageStore { get; }

        public DelegateCommand AutoCommand { get; }
        public DelegateCommand TeachCommand { get; }
        public IOStore IOStore { get; }

        public ShellViewModel(
            IOService ioService,
            StateStore stateStore,
            PageStore pageStore,
            RecipeStore recipeStore,
            IOStore ioStore)
        {
            StateStore = stateStore;
            PageStore = pageStore;
            RecipeStore = recipeStore;
            IOStore = ioStore;

            AutoCommand = new DelegateCommand(() =>
            {
                ioService.StartAutoMode();
            });

            TeachCommand = new DelegateCommand(() =>
            {
                ioService.StartManualMode();
            });
        }

        public void AddPage(string name, object page)
        {
            PageStore.AddPage(name, page);
        }

        public void Initialize()
        {
            if (RecipeStore.Recipes.Count == 0)
            {
                PageStore.SelectPage("Recipe");
                return;
            }

            PageStore.SelectPage("Main");
        }
    }
}
