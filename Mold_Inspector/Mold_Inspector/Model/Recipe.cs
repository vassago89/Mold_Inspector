using Mold_Inspector.Service;
using Mold_Inspector.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mold_Inspector.Model
{
    [Serializable]
    class Recipe
    {
        public bool IsMoldTeached { get; set; }
        public bool IsProductTeached { get; set; }

        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastUsedDate { get; set; }

        public Dictionary<int, (int WIdth, int Height)> SizeDictionary { get; }
        public Dictionary<InspectMode, Dictionary<int, ObservableCollection<Window>>> Windows { get; }

        public Dictionary<int, double> ExposureDictionary { get; }
        public Dictionary<int, double> GainDictionary { get; }

        public Recipe(string name)
        {
            Name = name;
            SizeDictionary = new Dictionary<int, (int WIdth, int Height)>();
            Windows = new Dictionary<InspectMode, Dictionary<int, ObservableCollection<Window>>>();
            foreach (var mode in Enum.GetValues(typeof(InspectMode)).Cast<InspectMode>())
                Windows[mode] = new Dictionary<int, ObservableCollection<Window>>();

            ExposureDictionary = new Dictionary<int, double>();
            GainDictionary = new Dictionary<int, double>();

            RegistrationDate = DateTime.Now;
            LastUsedDate = DateTime.Now;
        }

        public ICollection<Window> GetWindows(InspectMode inspectMode, int id)
        {
            if (Windows[inspectMode].ContainsKey(id))
                return Windows[inspectMode][id];
            
            var windows = new ObservableCollection<Window>();
            BindingOperations.EnableCollectionSynchronization(windows, new object());
            Windows[inspectMode][id] = windows;
            return windows;
        }

        public void AddWindow(InspectMode inspectMode, int id, Window window)
        {
            if (Windows[inspectMode].ContainsKey(id))
                Windows[inspectMode][id].Add(window);

            var windows = new ObservableCollection<Window>();
            BindingOperations.EnableCollectionSynchronization(windows, new object());
            Windows[inspectMode][id] = windows;
        }

        public void PostProcess(CameraStore cameraStore, DefaultStore defaultStore)
        {
            foreach (var camera in cameraStore.Cameras)
            {
                if (ExposureDictionary.ContainsKey(camera.ID) == false)
                    ExposureDictionary[camera.ID] = defaultStore.Exposure;

                if (GainDictionary.ContainsKey(camera.ID) == false)
                    GainDictionary[camera.ID] = defaultStore.Gain;
            }

            foreach (var mode in Enum.GetValues(typeof(InspectMode)).Cast<InspectMode>())
            {
                foreach(var windows in Windows[mode].Values)
                {
                    BindingOperations.EnableCollectionSynchronization(windows, new object());
                    foreach (var window in windows)
                        window.PostProcess();
                }
            }
        }

        public void Save()
        {

        }
    }
}
