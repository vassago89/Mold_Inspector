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
    class RecipeStore : BindableBase
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

        private RecipeConfig _recipeConfig;

        public RecipeStore(RecipeConfig recipeConfig) : base()
        {
            _recipeConfig = recipeConfig;
            _recipeConfig.Load();
            Recipes = new ObservableCollection<Recipe>();
            BindingOperations.EnableCollectionSynchronization(Recipes, new object());

            CopyFrom(_recipeConfig);
        }

        ~RecipeStore()
        {
            Save();
        }

        public void Save()
        {
            CopyTo(_recipeConfig);
            _recipeConfig.Save();
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
            Selected = recipe;
            Selected.LastUsedDate = DateTime.Now;
            _recipeConfig.Save();
            RecipeChanged?.Invoke();
        }

        protected void CopyFrom(RecipeConfig config)
        {
            Recipes.Clear();
            Recipes.AddRange(config.Recipes);
        }

        protected void CopyTo(RecipeConfig config)
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
