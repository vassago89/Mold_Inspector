using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Selection;
using Mold_Inspector.Store;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mold_Inspector.UI.Controls.ViewModels
{
    class ResultViewModel : BindableBase
    {
        private int _id;
        public int ID
        {
            get => _id;
            set
            {
                Results.Clear();
                Unsubscribe(_id);
                SetProperty(ref _id, value);
                Subscribe(_id);
            }
        }

        public ResultStore ResultStore { get; }
        public ObservableCollection<IAlgorithmResult> Results { get; }

        private ZoomService _zoomService;
        public ZoomService ZoomService
        {
            get => _zoomService;
            set => SetProperty(ref _zoomService, value);
        }

        public ResultViewModel(
            ResultStore resultStore)
        {
            ResultStore = resultStore;
            Results = new ObservableCollection<IAlgorithmResult>();
            BindingOperations.EnableCollectionSynchronization(Results, new object());
        }

        public void Subscribe(int id)
        {
            ResultStore.Subscribe(id, InspectDone, Refresh);
        }

        public void Unsubscribe(int id)
        {
            Results.Clear();
            ResultStore.Unsubscribe(id, InspectDone, Refresh);
        }

        private void InspectDone(IEnumerable<IAlgorithmResult> results)
        {
            Results.Clear();
            Results.AddRange(results);
        }

        private void Refresh() => Results.Clear();
    }
}
