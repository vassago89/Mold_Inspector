using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    class PageStore : BindableBase
    {
        private object _selectedControl;
        public object SelectedControl
        {
            get => _selectedControl;
            set => SetProperty(ref _selectedControl, value);
        }

        private IDictionary<string, object> _constols;

        public PageStore()
        {
            _constols = new Dictionary<string, object>();
        }

        public void AddPage(string name, object page)
        {
            _constols[name] = page;
        }

        public void SelectPage(string name)
        {
            if (_constols.ContainsKey(name))
                SelectedControl = _constols[name];
        }
    }
}
