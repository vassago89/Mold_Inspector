using Mold_Inspector.Config;
using Mold_Inspector.Model;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store.Abstract;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mold_Inspector.Store
{
    class RecipeStore : ConfigStore<RecipeConfig>
    {
        private Recipe _selected;
        public Recipe Selected
        { 
            get => _selected;
            private set
            {
                SetProperty(ref _selected, value);
                RecipeSelected = _selected != null;
            }
        }

        private bool _recipeSelected;
        public bool RecipeSelected
        {
            get => _recipeSelected;
            set => SetProperty(ref _recipeSelected, value);
        }

        public ObservableCollection<Recipe> Recipes { get; set; }
        public Action RecipeChanged { get; set; }


        public RecipeStore() : base()
        {

        }

        public bool AddRecipe(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (Recipes.Any(r => r.Name == name))
                return false;

            Recipes.Add(new Recipe(name));

            return true;
        }

        public void RemoveRecipe(Recipe recipe) => Recipes.Remove(recipe);
        public void SelectRecipe(Recipe recipe)
        {
            if (recipe != null)
                recipe.PostProcess();
            
            Selected = recipe;
            Selected.LastUsedDate = DateTime.Now;
            Save();
            RecipeChanged?.Invoke();
        }

        protected override void Initialize()
        {
            Recipes = new ObservableCollection<Recipe>();
            BindingOperations.EnableCollectionSynchronization(Recipes, new object());
        }

        protected override void CopyFrom(RecipeConfig config)
        {
            Recipes.Clear();
            Recipes.AddRange(config.Recipes);
        }

        protected override void CopyTo(RecipeConfig config)
        {
            config.Recipes = Recipes;
        }

        public (SelectDirection Direction, Window Window) SelectWindow(WindowStore windowStore, int x, int y)
        {
            if (Selected == null)
                return (SelectDirection.None, null);
                
            //foreach (var window in Selected.Current)
            //{
            //    var direction = window.CheckDirection(x, y, 15);
            //    if (direction != SelectDirection.None)
            //        return (direction, window);
            //}

            windowStore.SetSelected(null);
            return (SelectDirection.None, null);
        }

        public bool SelectWindows(WindowStore windowStore, int x, int y)
        {
            if (Selected == null)
            {
                windowStore.SetSelected(null);
                return false;
            }


            foreach (var window in windowStore.SelectedWindows)
            {
                if (window.CheckDirection(x, y, 0) != SelectDirection.None)
                    return true;
            }

            return false;
        }
    }
}
