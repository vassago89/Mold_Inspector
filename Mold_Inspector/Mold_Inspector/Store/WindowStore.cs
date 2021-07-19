using Mold_Inspector.Device.Camera;
using Mold_Inspector.Model;
using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Service;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mold_Inspector.Store
{
    public class WindowStoreMediator
    {
        private static WindowStoreMediator _instance;

        public static WindowStoreMediator Instacne => _instance ?? (_instance = new WindowStoreMediator());

        private ICollection<WindowStore> _stores;

        public AlgorithmType Algorithm { get; private set; }

        private WindowStoreMediator()
        {
            _stores = new List<WindowStore>();
        }

        public void Register(WindowStore store)
        {
            _stores.Add(store);
        }

        public void Selected(WindowStore current)
        {
            foreach (var store in _stores)
            {
                if (store == current)
                    continue;

                store.Release();
            }
        }

        public void SelectAlgorithm(AlgorithmType algorithm)
        {
            Algorithm = algorithm;
        }

        public void RemoveWindows()
        {
            foreach (var store in _stores)
                store.Remove();
        }

        public void Refresh()
        {
            foreach (var store in _stores)
            {
                store.Release();
                store.Refresh();
            }
        }
    }

    public class NextWindow : BindableBase
    {
        private Rectangle _region;
        public Rectangle Region 
        { 
            get => _region;
            set => SetProperty(ref _region, value);
        }

        public Window Window { get; }

        public NextWindow(Rectangle region , Window window)
        {
            Region = region;
            Window = window;
        }

        public void Modify()
        {
            Window.Region = Region;
        }
    }

    public class WindowStore : BindableBase
    {
        private Rectangle _region;
        public Rectangle Region
        {
            get => _region;
            private set => SetProperty(ref _region, value);
        }

        private Window _selected;
        public Window Selected
        {
            get => _selected;
            private set => SetProperty(ref _selected, value);
        }

        private NextWindow _nextSelected;
        public NextWindow NextSelected
        {
            get => _nextSelected;
            private set => SetProperty(ref _nextSelected, value);
        }

        private ObservableCollection<Window> _selectedWindows;
        public IEnumerable<Window> SelectedWindows => _selectedWindows;

        private IEnumerable<Window> _backgroundWindows;
        public IEnumerable<Window> BackgroundWindows
        {
            get => _backgroundWindows;
            private set => SetProperty(ref _backgroundWindows, value);
        }

        private IEnumerable<NextWindow> _nextWindows;
        public IEnumerable<NextWindow> NextWindows
        {
            get => _nextWindows;
            private set => SetProperty(ref _nextWindows, value);
        }

        private ICollection<Window> _currentWindows;
        public ICollection<Window> CurrentWindows 
        { 
            get => _currentWindows;
            private set => SetProperty(ref _currentWindows, value);
        }

        private int _id;
        private RecipeStore _recipeStore;
        private TeachingStore _teachingStore;
        
        internal WindowStore(
            int id,
            TeachingStore teachingStore,
            RecipeStore recipeStore)
        {
            _selectedWindows = new ObservableCollection<Window>();
            BindingOperations.EnableCollectionSynchronization(_selectedWindows, new object());

            _id = id;
            _recipeStore = recipeStore;
            _teachingStore = teachingStore;

            _teachingStore.ModeChanged += Refresh;
            _recipeStore.RecipeChanged += Refresh;

            Refresh();

            WindowStoreMediator.Instacne.Register(this);
        }

        ~WindowStore()
        {
            _teachingStore.ModeChanged -= Refresh;
            _recipeStore.RecipeChanged -= Refresh;
        }

        public void Refresh()
        {
            if (NextWindows != null)
            {
                _selectedWindows.Clear();
                foreach (var nw in NextWindows)
                {
                    nw.Modify();
                    _selectedWindows.Add(nw.Window);
                }

                NextWindows = null;
            }

            if (NextSelected != null)
            {
                NextSelected.Modify();
                Selected = null;
                Selected = NextSelected.Window;
                NextSelected = null;
            }

            CurrentWindows = null;
            CurrentWindows = _recipeStore.Selected.GetWindows(_teachingStore.InspectMode, _id);

            switch (_teachingStore.InspectMode)
            {
                case InspectMode.Production:
                    BackgroundWindows = _recipeStore.Selected.GetWindows(InspectMode.Mold, _id);
                    break;
                case InspectMode.Mold:
                    BackgroundWindows = _recipeStore.Selected.GetWindows(InspectMode.Production, _id);
                    break;
            }
        }

        public void Release()
        {
            NextSelected = null;
            Selected = null;
            _selectedWindows.Clear();
            NextWindows = null;
        }

        public void Offset(SelectDirection direction, int x, int y)
        {
            if (Selected == null)
                return;

            var regionX = Selected.Region.X;
            var regionY = Selected.Region.Y;
            var width = Selected.Region.Width;
            var height = Selected.Region.Height;

            switch (direction)
            {
                case SelectDirection.Move:
                    regionX += x;
                    regionY += y;
                    break;
                case SelectDirection.Left:
                    regionX += x;
                    width -= x;
                    break;
                case SelectDirection.Right:
                    width += x;
                    break;
                case SelectDirection.Top:
                    regionY += y;
                    height -= y;
                    break;
                case SelectDirection.Bottom:
                    height += y;
                    break;
                case SelectDirection.LeftTop:
                    regionX += x;
                    width -= x;
                    regionY += y;
                    height -= y;
                    break;
                case SelectDirection.RightTop:
                    width += x;
                    regionY += y;
                    height -= y;
                    break;
                case SelectDirection.LeftBottom:
                    regionX += x;
                    width -= x;
                    height += y;
                    break;
                case SelectDirection.RightBottom:
                    width += x;
                    height += y;
                    break;
            }

            var region = new Rectangle(regionX, regionY, width, height);
            if (region.Width < 15
                || region.Height < 15)
            {
                return;
            }

            NextSelected = new NextWindow(region, Selected);
        }

        public void Offset(int x, int y)
        {
            var windows = new List<NextWindow>();

            foreach (var window in SelectedWindows)
            {
                windows.Add(new NextWindow(
                    new Rectangle(
                        window.Region.X + x,
                        window.Region.Y + y,
                        window.Region.Width,
                        window.Region.Height), window));
            }
            
            NextWindows = windows;
        }

        public void AddSelected(Window selected)
        {
            _selectedWindows.Add(selected);
            WindowStoreMediator.Instacne.Selected(this);
        }

        public void ClearSelected()
        {
            _selectedWindows.Clear();
        }

        public void SetRegion(Rectangle region)
        {
            Region = region;
        }

        public void SetSelected(Window selected)
        {
            Selected = selected;
            WindowStoreMediator.Instacne.Selected(this);
        }

        public void Remove()
        {
            foreach (var windows in _recipeStore.Selected.Windows.Values)
            {
                if (windows.ContainsKey(_id) == false)
                    continue;

                foreach (var window in _selectedWindows)
                {
                    if (windows.ContainsKey(_id))
                    {
                        windows[_id].Remove(window);
                    }
                }
                if (windows.ContainsKey(_id))
                    windows[_id].Remove(_selected);
            }

            Release();
            Refresh();
        }
    }
}