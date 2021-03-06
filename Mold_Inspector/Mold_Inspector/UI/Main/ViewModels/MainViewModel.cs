using Mold_Inspector.Device;
using Mold_Inspector.Service;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.UI.Main.ViewModels
{
    class MainViewModel : BindableBase
    {
        private bool _onLive;
        public bool OnLive
        {
            get => _onLive;
            set
            {
                SetProperty(ref _onLive, value);
                if (_onLive)
                {
                    foreach (var camera in CameraStore.Cameras)
                        _cameraServiceMediator.Connect(camera.ID, camera.Info);

                    foreach (var info in CameraStore.CameraInfos)
                        _cameraServiceMediator.Live(info);
                }
                else
                {
                    foreach (var info in CameraStore.CameraInfos)
                        _cameraServiceMediator.Stop(info);
                }
            }
        }

        public CameraStore CameraStore { get; }
        public CommonStore CommonStore { get; }
        public StateStore StateStore { get; }

        public DelegateCommand ProductionCommand { get; }
        public DelegateCommand MoldCommand { get; }

        public DelegateCommand GrabCommand { get; }

        public DelegateCommand InspectCommand { get; }

        public DelegateCommand RemoveCommand { get; }

        public DelegateCommand NoneCommand { get; }
        public DelegateCommand PatternCommand { get; }
        public DelegateCommand BinaryCommand { get; }
        public DelegateCommand ProfileCommand { get; }
        public DelegateCommand ClearCommand { get; set; }

        private CameraServiceMediator _cameraServiceMediator;
        private MouseStore _mouseStore;
        public TeachingStore TeachingStore { get; }
        public RecipeStore RecipeStore { get; }

        public MainViewModel(
            CameraServiceMediator cameraServiceMediator,
            InspectService inspectService,
            CameraStore cameraStore, 
            CommonStore commonStore,
            StateStore stateStore,
            MouseStore mouseStore,
            RecipeStore recipeStore,
            ResultStore resultStore,
            TeachingStore teachingStore)
        {
            _cameraServiceMediator = cameraServiceMediator;
            _mouseStore = mouseStore;
            TeachingStore = teachingStore;

            CameraStore = cameraStore;
            CommonStore = commonStore;
            StateStore = stateStore;

            RecipeStore = recipeStore;


            ProductionCommand = new DelegateCommand(() =>
            {
                TeachingStore.InspectMode = InspectMode.Production;
                resultStore.Refresh();
            });
            MoldCommand = new DelegateCommand(() =>
            {
                TeachingStore.InspectMode = InspectMode.Mold;
                resultStore.Refresh();
            });

            GrabCommand = new DelegateCommand(async () =>
            {
                foreach (var camera in CameraStore.Cameras)
                    _cameraServiceMediator.Connect(camera.ID, camera.Info);
                 
                foreach (var camera in CameraStore.Cameras)
                {
                    if (RecipeStore.Selected.ExposureDictionary.ContainsKey(camera.ID))
                        _cameraServiceMediator.SetExposure(camera.Info, RecipeStore.Selected.ExposureDictionary[camera.ID]);

                    if (RecipeStore.Selected.GainDictionary.ContainsKey(camera.ID))
                        _cameraServiceMediator.SetGain(camera.Info, RecipeStore.Selected.GainDictionary[camera.ID]);

                    await _cameraServiceMediator.Grab(camera.Info);
                }
            });

            InspectCommand = new DelegateCommand(() =>
            {
                inspectService.Inspect(TeachingStore.InspectMode);
            });

            RemoveCommand = new DelegateCommand(() =>
            {
                WindowStoreMediator.Instacne.RemoveWindows();
                TeachingStore.Window = null;
            });

            NoneCommand = new DelegateCommand(() =>
            {
                _mouseStore.Command = CommandState.None;
            });

            PatternCommand = new DelegateCommand(() =>
            {
                _mouseStore.Command = CommandState.Create;
                WindowStoreMediator.Instacne.SelectAlgorithm(Model.Algorithm.AlgorithmType.Pattern);
            });

            BinaryCommand = new DelegateCommand(() =>
            {
                _mouseStore.Command = CommandState.Create;
                WindowStoreMediator.Instacne.SelectAlgorithm(Model.Algorithm.AlgorithmType.Binary);
            });

            ProfileCommand = new DelegateCommand(() =>
            {
                _mouseStore.Command = CommandState.Create;
                WindowStoreMediator.Instacne.SelectAlgorithm(Model.Algorithm.AlgorithmType.Profile);
            });

            ClearCommand = new DelegateCommand(() =>
            {
                resultStore.Refresh();
            });
        }
    }
}
