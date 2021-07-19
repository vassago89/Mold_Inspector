using Mold_Inspector.Config.Abstract;
using Mold_Inspector.Model;
using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Algorithm.PatternMatching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mold_Inspector.Config
{
    [Serializable]
    class RecipeConfig : BinaryConfig<RecipeConfig>
    {
        private List<Recipe> _recipes;
        public IEnumerable<Recipe> Recipes
        { 
            get => _recipes;
            set => _recipes = value.ToList(); 
        }

        public RecipeConfig()
        {
            _recipes = new List<Recipe>();
        }
    }
}
