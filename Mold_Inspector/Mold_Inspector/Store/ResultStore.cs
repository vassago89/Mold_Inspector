using Mold_Inspector.Model.Algorithm;
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
    class ResultStore :BindableBase
    {
        private Dictionary<int, IEnumerable<IAlgorithmResult>> _resultDictionary;
        private Dictionary<int, Action<IEnumerable<IAlgorithmResult>>> _inspectDone;
        private Action _refresh;
        
        public ResultStore()
        {
            _resultDictionary = new Dictionary<int, IEnumerable<IAlgorithmResult>>();
            _inspectDone = new Dictionary<int, Action<IEnumerable<IAlgorithmResult>>>();
        }

        public void InpectDone(IEnumerable<(int ID, IAlgorithmResult Result)> results)
        {
            var dictionary = new Dictionary<int, ICollection<IAlgorithmResult>>();
            foreach (var result in results)
            {
                if (dictionary.ContainsKey(result.ID) == false)
                    dictionary[result.ID] = new List<IAlgorithmResult>();

                dictionary[result.ID].Add(result.Result);
            }

            foreach (var pair in dictionary)
            {
                _resultDictionary[pair.Key] = pair.Value;

                if (_inspectDone.ContainsKey(pair.Key))
                    _inspectDone[pair.Key](pair.Value);
            }
        }

        public void Subscribe(int id, Action<IEnumerable<IAlgorithmResult>> action, Action refresh)
        {
            if (_inspectDone.ContainsKey(id))
                _inspectDone[id] += action;
            else
                _inspectDone.Add(id, action);

            if (_resultDictionary.ContainsKey(id))
                _inspectDone[id]?.Invoke(_resultDictionary[id]);

            _refresh += refresh;
        }

        public void Unsubscribe(int id, Action<IEnumerable<IAlgorithmResult>> action, Action refresh)
        {
            if (_inspectDone.ContainsKey(id))
                _inspectDone[id] -= action;

            _refresh -= refresh;
        }

        public void Refresh()
        {
            _resultDictionary.Clear();

            _refresh?.Invoke();
        }
    }
}
