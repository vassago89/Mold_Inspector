using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mold_Inspector.Device.Camera.iDS
{
    class iDSCamera : ICamera
    {
        public Action<GrabInfo> ImageGrabbed { get; set; }
        public Action GrabStarted { get; set; }
        public Action GrabDone { get; set; }

        uEye.Camera _camera;
        int _bufferNum;

        int _grabCount;
        int _count;

        public iDSCamera(CameraInfo cameraInfo, int bufferNum = 3)
        {
            _bufferNum = bufferNum;

            InitCamera(cameraInfo);
        }

        public bool Disconnect()
        {
            var result = _camera?.Exit() == uEye.Defines.Status.Success;
            _camera = null;

            return result;
        }

        public CameraParameterInfo GetParameterInfo()
        {
            if (_camera == null)
                return null;

            _camera.Trigger.Get(out var triggerMode);

            return new CameraParameterInfo(
                triggerMode != uEye.Defines.TriggerMode.Off,
                GetCameraParameter(),
                null);   
        }

        private Dictionary<ECameraParameter, CameraParameter> GetCameraParameter()
        {
            var dictionary = new Dictionary<ECameraParameter, CameraParameter>();

            _camera.Timing.Exposure.GetRange(out var exposureMin, out var exposureMax, out var exposureInc);
            _camera.Timing.Exposure.Get(out var exposureCur);

            dictionary[ECameraParameter.Exposure] = new CameraParameter(exposureCur, exposureMin, exposureMax);

            _camera.Timing.Framerate.GetFrameRateRange(out var frameRateRange);
            _camera.Timing.Framerate.Get(out var frameRateCur);
            dictionary[ECameraParameter.FrameRate] = new CameraParameter(frameRateCur, frameRateRange.Minimum, frameRateRange.Maximum);

            _camera.Gain.Hardware.Scaled.GetMaster(out int gainCur);
            dictionary[ECameraParameter.Gain] = new CameraParameter(gainCur, 0, 100);

            //나머진 필요 시

            return dictionary;
        }

        public bool IsConnected()
        {
            if (_camera == null)
                return false;

            return _camera.IsOpened;
        }

        public bool SetAuto(ECameraAutoType type, ECameraAutoValue value)
        {
            throw new NotImplementedException();
        }

        public bool SetParameter(ECameraParameter parameter, double value)
        {
            uEye.Defines.Status statusRet = 0;
            switch (parameter)
            {
                case ECameraParameter.OffsetX:
                case ECameraParameter.OffsetY:
                case ECameraParameter.Width:
                case ECameraParameter.Height:
                case ECameraParameter.FrameRate:
                case ECameraParameter.TriggerDelay:
                    throw new NotImplementedException();
                case ECameraParameter.Exposure:
                    statusRet = _camera.Timing.Exposure.Set(value);
                    break;
                case ECameraParameter.Gain:
                    statusRet = _camera.Gain.Hardware.Scaled.SetMaster((int)value);
                    break;
            }

            return statusRet == uEye.Defines.Status.SUCCESS;
        }

        public bool SetROI(uint x, uint y, uint width, uint height)
        {
            throw new NotImplementedException();
        }

        public bool SetTriggerMode(bool isTriggerMode)
        {
            throw new NotImplementedException();
        }

        public bool StartGrab(int grabCount = -1)
        {
            GrabStarted?.Invoke();

            _grabCount = grabCount;
            _count = 0;

            return _camera.Acquisition.Capture() == uEye.Defines.Status.SUCCESS;
        }

        public bool Stop()
        {
            return _camera.Acquisition.Stop() == uEye.Defines.Status.SUCCESS;
        }

        private void InitCamera(CameraInfo cameraInfo)
        {
            uEye.Info.Camera.GetCameraList(out var cameraInfomations);
            _camera = new uEye.Camera(cameraInfomations.First(info => info.SerialNumber == cameraInfo.SerialNo).CameraID);

            uEye.Defines.Status statusRet = 0;

            // Allocate Memory
            statusRet = AllocImageMems();
            if (statusRet != uEye.Defines.Status.Success)
                throw new NotImplementedException();

            statusRet = InitSequence();
            if (statusRet != uEye.Defines.Status.Success)
                throw new NotImplementedException();

            _camera.EventFrame += OnEventFrame;
        }

        private void OnEventFrame(object sender, EventArgs e)
        {
            if (_grabCount > 0)
            {
                _count++;

                if (_count > _grabCount)
                    return;

                if (_count >= _grabCount)
                {
                    Stop();
                    if (GrabDone != null)
                        GrabDone();
                }
            }

            var statusRet = _camera.Memory.GetLast(out Int32 s32MemID);

            if ((uEye.Defines.Status.SUCCESS == statusRet) && (0 < s32MemID))
            {
                if (uEye.Defines.Status.SUCCESS == _camera.Memory.Lock(s32MemID))
                {
                    _camera.Memory.CopyToArray(s32MemID, out byte[] buffer);
                    _camera.Memory.GetBitsPerPixel(s32MemID, out int bpp);
                    _camera.Memory.GetWidth(s32MemID, out int width);
                    _camera.Memory.GetHeight(s32MemID, out int height);
                    _camera.Memory.Unlock(s32MemID);

                    ImageGrabbed?.Invoke(new GrabInfo(EGrabResult.Success, width, height, bpp / 8, buffer));
                }
            }
        }

        private uEye.Defines.Status AllocImageMems()
        {
            uEye.Defines.Status statusRet = uEye.Defines.Status.SUCCESS;

            for (int i = 0; i < _bufferNum; i++)
            {
                statusRet = _camera.Memory.Allocate();

                if (statusRet != uEye.Defines.Status.SUCCESS)
                {
                    FreeImageMems();
                }
            }

            return statusRet;
        }

        private uEye.Defines.Status FreeImageMems()
        {
            int[] idList;
            uEye.Defines.Status statusRet = _camera.Memory.GetList(out idList);

            if (uEye.Defines.Status.SUCCESS == statusRet)
            {
                foreach (int nMemID in idList)
                {
                    do
                    {
                        statusRet = _camera.Memory.Free(nMemID);

                        if (uEye.Defines.Status.SEQ_BUFFER_IS_LOCKED == statusRet)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        break;
                    }
                    while (true);
                }
            }

            return statusRet;
        }

        private uEye.Defines.Status InitSequence()
        {
            int[] idList;
            uEye.Defines.Status statusRet = _camera.Memory.GetList(out idList);

            if (uEye.Defines.Status.SUCCESS == statusRet)
            {
                statusRet = _camera.Memory.Sequence.Add(idList);

                if (uEye.Defines.Status.SUCCESS != statusRet)
                {
                    ClearSequence();
                }
            }

            return statusRet;
        }

        private uEye.Defines.Status ClearSequence()
        {
            return _camera.Memory.Sequence.Clear();
        }
    }
}
