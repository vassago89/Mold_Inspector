using Mold_Inspector.Config.Abstract;
using Mold_Inspector.Model;
using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Algorithm.PatternMatching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mold_Inspector.Config
{
    class RecipeConfig
    {
        private static BinaryFormatter _formatter = new BinaryFormatter();

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

        public void Save()
        {
            var recipesPath = Path.Combine(Environment.CurrentDirectory, "Recipes");
            if (Directory.Exists(recipesPath) == false)
                Directory.CreateDirectory(recipesPath);

            foreach (var recipe in _recipes)
            {
                using (var stream = File.Create(Path.Combine(recipesPath, $"{recipe.Name}.bin")))
                    _formatter.Serialize(stream, recipe);
            }
        }


        public void Load()
        {
            _recipes.Clear();

            var recipesPath = Path.Combine(Environment.CurrentDirectory, "Recipes");
            if (Directory.Exists(recipesPath) == false)
                return;

            var directoryInfo = new DirectoryInfo(recipesPath);
            var files =directoryInfo.GetFiles();

            foreach (var file in files)
            {
                using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        _recipes.Add(_formatter.Deserialize(stream) as Recipe);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
