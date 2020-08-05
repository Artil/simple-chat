using ChatCore.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDesktop.Interfaces
{
    public interface IConference
    {
        event Action<string> Stop;
        void StartCapture(string fileName, CaptureTypeEnum captureType = CaptureTypeEnum.VoiceCapture);
        void StopAllCapture();
    }
}
