using Mold_Inspector.Device;
using Mold_Inspector.Device.Camera;
using Mold_Inspector.Device.IO;
using Mold_Inspector.Store;
using Mold_Inspector.UI.Controls.Views;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mold_Inspector.Service
{
    enum UserCommand
    {
        OK, Restart
    }

    class IOService : BindableBase
    {
        private bool _inOpen;
        public bool InOpen
        {
            get => _inOpen;
            set
            {
                if (_inDoor)
                    SetProperty(ref _inOpen, value);
            }
        }

        private bool _inEjected;
        public bool InEjected
        {
            get => _inEjected;
            set
            {
                if (_inDoor)
                    SetProperty(ref _inEjected, value);
            }
        }

        private bool _inOK;
        public bool InOK
        {
            get => _inOK;
            set
            {
                if (_inDoor)
                    SetProperty(ref _inOK, value);
            }
        }

        private bool _inNG;
        public bool InNG
        {
            get => _inNG;
            set
            {
                if (_inDoor)
                {
                    var prev = _inNG;
                    SetProperty(ref _inNG, value);
                    if (prev != _inNG && InNG)
                        OutAlram = false;
                }
            }
        }

        private bool _inSet;
        public bool InSet
        {
            get => _inSet;
            set
            {
                if (_inDoor)
                    SetProperty(ref _inSet, value);
            }
        }

        private bool _inRestart;
        public bool InRestart
        {
            get => _inRestart;
            set
            {
                if (_inDoor)
                    SetProperty(ref _inRestart, value);
            }
        }

        private bool _inDoor;
        public bool InDoor
        {
            get => _inDoor;
            set
            {
                var prev = _inDoor;
                SetProperty(ref _inDoor, value);
                if (prev != _inDoor && _inDoor == false)
                    OutAlram = false;
            }
        }

        private bool _outClose;
        public bool OutClose
        {
            get => _outClose;
            set
            {
                SetProperty(ref _outClose, value);
                if (_inDoor)
                    _controller.Write(_ioStore.IOPort.OutClose, value);
            }
        }

        private bool _outEject;
        public bool OutEject
        {
            get => _outEject;
            set
            {
                SetProperty(ref _outEject, value);
                if (_inDoor)
                    _controller.Write(_ioStore.IOPort.OutEject, value);
            }
        }

        private bool _outRobot;
        public bool OutRobot
        {
            get => _outRobot;
            set
            {
                SetProperty(ref _outRobot, value);
                if (_inDoor)
                    _controller.Write(_ioStore.IOPort.OutRobot, value);
            }
        }

        private bool _outReEject;
        public bool OutReEject
        {
            get => _outReEject;
            set
            {
                SetProperty(ref _outReEject, value);
                if (_inDoor)
                    _controller.Write(_ioStore.IOPort.OutReEject, value);
            }
        }

        private bool _outAlram;
        public bool OutAlram
        {
            get => _outAlram;
            set
            {
                SetProperty(ref _outAlram, value);
                _controller.Write(_ioStore.IOPort.OutAlram, value);
            }
        }

        private bool _outCS;
        public bool OutCS
        {
            get => _outCS;
            set
            {
                SetProperty(ref _outCS, value);
                if (_inDoor)
                    _controller.Write(_ioStore.IOPort.OutCS, value);
            }
        }

        private CameraStore _cameraStore;
        private StateStore _stateStore;
        private CameraServiceMediator _mediator;
        private InspectService _inspectService;

        private IOController _controller;
        private IOStore _ioStore;
        private PageStore _pageStore;
        private RecipeStore _recipeStore;
        private TeachingStore _teachingStore;
        private InspectStore _inspectStore;

        private bool _isManualRun;
        private bool _isAutoRun;

        public IOService(
            CameraServiceMediator mediator,
            InspectService inspectService,
            InspectStore inspectStore,
            CameraStore cameraStore,
            StateStore stateStore,
            TeachingStore teachingStore,
            IOStore ioStore,
            PageStore pageStore,
            RecipeStore recipeStore,
            IOController controller,
            CancellationToken token)
        {
            _recipeStore = recipeStore;
            _pageStore = pageStore;
            _cameraStore = cameraStore;
            _mediator = mediator;
            _inspectService = inspectService;
            _stateStore = stateStore;
            _controller = controller;
            _ioStore = ioStore;
            _teachingStore = teachingStore;
            _inspectStore = inspectStore;

            InDoor = true;
            InOpen = true;
            InEjected = true;

            Release();

            _controller.InChanged += InChanged;
        }

        private void InChanged(bool[] datas)
        {
            InOpen = !datas[_ioStore.IOPort.InOpen];
            InEjected = !datas[_ioStore.IOPort.InEjected];
            InOK = !datas[_ioStore.IOPort.InOK];
            InNG = !datas[_ioStore.IOPort.InNG];
            InSet = !datas[_ioStore.IOPort.InSet];
            InRestart = !datas[_ioStore.IOPort.InRestart];
            InDoor = !datas[_ioStore.IOPort.InDoor];
        }

        public void Release()
        {
            OutAlram = false;
            OutReEject = false;

            OutEject = true;
            OutRobot = true;

            Close();
        }

        public void StartAutoMode()
        {
            if (_isAutoRun)
                return;

            _stateStore.IsTeachEnable = false;
            _pageStore.SelectPage("Main");
            _teachingStore.IsLock = true;

            Task.Run(async () =>
            {
                _isAutoRun = true;

                Func<bool> check = () => _stateStore.TeachState == TeachState.Complate;

                while (check() && _inspectStore.Complate < _inspectStore.Target)
                {
                    var result = true;
                    _inspectStore.Production = InspectState.Busy;

                    Close();
                    
                    await WaitClosing(check);
                    Ejected();
                    await WaitOpen(check);

                    Closed();
                    await Grab();

                    _teachingStore.InspectMode = InspectMode.Production;
                    if (_inspectService.Inspect(InspectMode.Production) == false)
                    {
                        result = false;
                        OutAlram = true;
                        await WaitAlram(check);
                        await WaitOK(check);

                        if (check() == false)
                            continue;
                    }

                    _inspectStore.Production = InspectState.Ready;
                    _inspectStore.Mold = InspectState.Busy;

                    await Eject(check);

                    while (true) 
                    {
                        var tuple = await InspectMold(check);
                        result &= tuple.result;

                        if (tuple.Command == UserCommand.Restart)
                            break;
                    }

                    _inspectStore.Mold = InspectState.Ready;
                    _inspectStore.Complate++;
                    if (result)
                        _inspectStore.OK++;
                    else
                        _inspectStore.NG++;
                }

                _inspectStore.Production = InspectState.Ready;
                _inspectStore.Mold = InspectState.Ready;

                _isAutoRun = false;
                _stateStore.IsTeachEnable = true;
            });
        }

        private async Task<(UserCommand Command, bool result)> InspectMold(Func<bool> check)
        {
            bool result = true;
            await Grab();
            _teachingStore.InspectMode = InspectMode.Mold;
            if (_inspectService.Inspect(InspectMode.Mold) == false)
            {
                result = false;
                bool isPass = false;
                int count = 0;

                while (isPass == false && _ioStore.UseReEject && count < _ioStore.ReEjectCount)
                {
                    await ReEject(check);
                    await Grab();
                    isPass = _inspectService.Inspect(InspectMode.Mold);
                    count++;
                }

                if (isPass == false)
                {
                    OutAlram = true;
                    await WaitAlram(check);
                    return (await WaitOKOrRestart(check), result);
                }
            }

            return (UserCommand.Restart, result);
        }

        private async Task<bool> Grab()
        {
            var grabInfos = new List<GrabInfo?>();

            foreach (var camera in _cameraStore.Cameras)
                _mediator.Connect(camera.ID, camera.Info);

            foreach (var info in _cameraStore.CameraInfos)
                grabInfos.Add(await _mediator.Grab(info));

            return grabInfos.All(info => info != null);
        }

        public void StartManualMode()
        {
            if (_isManualRun)
                return;

            Task.Run(async () =>
            {
                _isManualRun = true;

                Func<bool> check = () => _stateStore.TeachState != TeachState.Complate;

                while (check())
                {
                    await WaitSet(check);

                    switch (_stateStore.TeachState)
                    {
                        case TeachState.Prepare:
                            Close();
                            await WaitClosing(check);
                            Ejected();

                            await WaitOpen(check);
                            Closed();
                            if (await Grab())
                            {
                                _teachingStore.InspectMode = InspectMode.Production;
                                _stateStore.TeachStateChanging();
                            }
                            break;
                        case TeachState.Production:
                            _inspectService.RefreshPattern(InspectMode.Production);
                            await Eject(check);
                            if (await Grab())
                            {
                                _teachingStore.InspectMode = InspectMode.Mold;
                                _stateStore.TeachStateChanging();
                            }
                            break;
                        case TeachState.Mold:
                            _inspectService.RefreshPattern(InspectMode.Mold);
                            _stateStore.TeachStateChanging();
                            break;
                        case TeachState.Complate:
                            _recipeStore.Save();
                            _stateStore.TeachStateChanging();
                            StartAutoMode();
                            break;
                    }
                }

                _isManualRun = false;
            });
        }



        private async Task Eject(Func<bool> check)
        {
            OutEject = true;
            OutRobot = true;

            
            if (_ioStore.UseEjectEnd)
            {
                await WaitEjecting(check);
                await WaitEjected(check);
            }
            
            await Task.Delay(_ioStore.EjectDelay);
        }

        private async Task ReEject(Func<bool> check)
        {
            OutReEject = true;

            if (_ioStore.UseEjectEnd)
            {
                await WaitEjecting(check);
                await WaitEjected(check);
            }
            
            await Task.Delay(_ioStore.EjectDelay);

            OutReEject = false;
        }

        private void Ejected()
        {
            OutEject = false;
            OutRobot = false;
        }

        private void Close()
        {
            OutClose = true;
            OutCS = true;
        }

        private void Closed()
        {
            OutClose = false;
            OutCS = false;
        }

        private async Task WaitOpen(Func<bool> check)
        {
            _stateStore.IsWaitOpen = true;

            while (_inOpen == false && check())
                await Task.Delay(100);

            _stateStore.IsWaitOpen = false;
        }

        private async Task WaitClosing(Func<bool> check)
        {
            _stateStore.IsWaitClosing = true;

            while (_inOpen && check())
                await Task.Delay(100);

            _stateStore.IsWaitClosing = false;
        }

        private async Task WaitEjected(Func<bool> check)
        {
            _stateStore.IsWaitEjected = true;

            while (_inEjected == false && check())
                await Task.Delay(100);

            _stateStore.IsWaitEjected = false;
        }

        private async Task WaitEjecting(Func<bool> check)
        {
            _stateStore.IsWaitEjecting = true;

            while (_inEjected && check())
                await Task.Delay(100);

            _stateStore.IsWaitEjecting = false;
        }

        private async Task WaitAlram(Func<bool> check)
        {
            _stateStore.IsWaitAlram = true;

            while (_outAlram && check())
                await Task.Delay(100);

            _stateStore.IsWaitAlram = false;
        }

        private async Task WaitOK(Func<bool> check)
        {
            _stateStore.IsWaitOK = true;

            _stateStore.IsTeachEnable = true;

            while (_inOK == false && check())
                await Task.Delay(100);

            if (check())
            {
                _stateStore.IsAutoMode = true;
                _stateStore.IsTeachEnable = false;
                _pageStore.SelectPage("Main");
            }
            
            _stateStore.IsWaitOK = false;
        }

        private async Task<UserCommand> WaitOKOrRestart(Func<bool> check)
        {
            _stateStore.IsWaitOK = true;
            _stateStore.IsWaitRestart = true;
            _stateStore.IsTeachEnable = true;

            while (true && check())
            {
                if (_inOK)
                {
                    _stateStore.IsWaitOK = false;
                    _stateStore.IsWaitRestart = false;
                    _stateStore.IsTeachEnable = false;
                    _stateStore.IsAutoMode = true;
                    _pageStore.SelectPage("Main");
                    return UserCommand.OK;
                }
                else if (_inRestart)
                {
                    _stateStore.IsWaitOK = false;
                    _stateStore.IsWaitRestart = false;
                    _stateStore.IsTeachEnable = false;
                    _stateStore.IsAutoMode = true;
                    _pageStore.SelectPage("Main");
                    return UserCommand.Restart;
                }

                await Task.Delay(100);
            }

            _stateStore.IsWaitOK = false;
            _stateStore.IsWaitRestart = false;

            return UserCommand.Restart;
        }

        private async Task WaitSet(Func<bool> check)
        {
            _stateStore.IsWaitSet = true;

            while (_inSet == false && check())
                await Task.Delay(100);

            _stateStore.IsWaitSet = false;
        }
    }
}
