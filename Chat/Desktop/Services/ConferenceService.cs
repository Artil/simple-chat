using Accord.Audio;
using Accord.Audio.Formats;
using Accord.DirectSound;
using Accord.Video;
using Accord.Video.DirectShow;
using AviFile;
using ChatCore.Enums;
using ChatDesktop.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;

namespace ChatDesktop.Services
{
    public class ConferenceService : IConference
    {
        private VideoCaptureDevice VideoSource;
        private ScreenCaptureStream ScreenCapture;
        private AudioCaptureDevice AudioCapture;
        private FileStream _audioStream;
        private WaveEncoder _waveEncoder;
        private AviManager AviManager;
        private VideoStream _aviStream;
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private Timer _stopTimer;
        private string TempFileName;
        private string TempVoice = "tempVoice.wav";
        private string TempVideo = "tempVideo.avi";
        private CaptureTypeEnum CaptureType;
        private delegate void StopAll();
        private StopAll stopAll;
        private int BitmapsCount;
        private double FrameRate;

        public event Action<string> Stop;

        public void StartCapture(string fileName, CaptureTypeEnum captureType = CaptureTypeEnum.VoiceCapture)
        {
            TempFileName = fileName;
            CaptureType = captureType;

            if (captureType.Equals(CaptureTypeEnum.VoiceCapture) || captureType.Equals(CaptureTypeEnum.VideoCaptureWithVoice))
            {
                StartVoiceCapture(captureType.Equals(CaptureTypeEnum.VideoCaptureWithVoice) ? TempVoice : fileName);
                stopAll += StopVoiceCapture;
            }

            if (captureType.Equals(CaptureTypeEnum.VideoCapture) || captureType.Equals(CaptureTypeEnum.VideoCaptureWithVoice))
            {
                StartVideoCapture(captureType.Equals(CaptureTypeEnum.VideoCaptureWithVoice) ? TempVideo : fileName);
                stopAll += StopVideoCapture;
            }

            if (!String.IsNullOrEmpty(TempFileName))
            {
                var tm = new TimerCallback(TimeOut); // set callback
                _stopTimer = new Timer(tm, 0, 1000 * 600, 1000 * 60); // 1 min
            }
        }

        public void StopAllCapture()
        {
            if(!ReferenceEquals(_stopTimer, null))
                _stopTimer.Dispose();

            stopAll();

            if (CaptureType.Equals(CaptureTypeEnum.VideoCaptureWithVoice) || CaptureType.Equals(CaptureTypeEnum.ScreenCaptureWithVoice))
                MergeAudioVideo();

            Stop?.Invoke(TempFileName);
            TempFileName = null;
        }

        private void TimeOut(object obj)
        {
            stopAll();
        }

        //private void CreateVideo()
        //{
        //    var frameRate = (double)Bitmaps.Count / _waveEncoder.Duration * 1000;
        //    foreach (var item in Bitmaps.ToList())
        //    {
        //        if (_aviStream is null)
        //            _aviStream = AviManager.AddVideoStream(false, frameRate, item);
        //        else
        //            _aviStream.AddFrame(item);
        //    }
        //}

        private void MergeAudioVideo()
        {
            string Path_FFMPEG = "ffmpeg/ffmpeg.exe";
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try
            {
                proc.StartInfo.FileName = Path_FFMPEG;
                proc.StartInfo.Arguments = $"-i {"AppFiles/TempFiles/" + TempVoice} -r {FrameRate} -i {"AppFiles/TempFiles/" + TempVideo} -acodec copy -vcodec msmpeg4v2 {"AppFiles/TempFiles/" + TempFileName}";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;

                proc.Start();

                string StdOutVideo = proc.StandardOutput.ReadToEnd();
                string StdErrVideo = proc.StandardError.ReadToEnd();
            }
            catch(Exception ex) { }
            finally
            {
                 //   proc.WaitForExit();
                proc.Close();
                FrameRate = 0;

                if (File.Exists(baseDirectory + "AppFiles/TempFiles/" + TempVoice))
                    File.Delete(baseDirectory + "AppFiles/TempFiles/" + TempVoice);

                if (File.Exists(baseDirectory + "AppFiles/TempFiles/" + TempVideo))
                    File.Delete(baseDirectory + "AppFiles/TempFiles/" + TempVideo);
            }
        }

        #region video capture
        private void StartVideoCapture(string fileName)
        {
            var path = baseDirectory + "AppFiles/TempFiles/" + fileName;


            //if (!File.Exists(path))
            //    File.Create(path);

            VideoSource = new VideoCaptureDevice(Properties.UserSettings.Default.WebCamera);
            VideoSource.SetVideoProcAmpProperty(VideoProcAmpProperty.Brightness, 0, VideoProcAmpFlags.Manual);

            VideoSource.NewFrame += NewVideoFrame;
            VideoSource.VideoSourceError += VideoError;

            AviManager = new AviManager(path, false);

            VideoSource.Start();
        }

        private void StopVideoCapture()
        {
            VideoSource.SignalToStop();
            VideoSource.NewFrame -= NewVideoFrame;
            VideoSource.VideoSourceError -= VideoError;
            VideoSource = null;
            AviManager.Close();
            AviManager = null;
            _aviStream = null;
            stopAll -= StopVideoCapture;
        }

        public void VideoError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            if (File.Exists(baseDirectory + "AppFiles/TempFiles/" + TempFileName))
                File.Delete(baseDirectory + "AppFiles/TempFiles/" + TempFileName);

            stopAll();
        }

        private void NewVideoFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            if (_aviStream is null)
                _aviStream = AviManager.AddVideoStream(false, 5, eventArgs.Frame);
            else
                _aviStream.AddFrame(eventArgs.Frame);

            if (CaptureType.Equals(CaptureTypeEnum.VideoCaptureWithVoice) || CaptureType.Equals(CaptureTypeEnum.ScreenCaptureWithVoice))
                BitmapsCount++;
        }
        #endregion

        #region screen capture 
        private void StartScreenCapture()
        {
            var screenArea = new Rectangle(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);

            // create screen capture video source
            ScreenCapture = new ScreenCaptureStream(screenArea);

            // set NewFrame event handler
            ScreenCapture.NewFrame += new NewFrameEventHandler(NewVideoFrame);

            // start the video source
            ScreenCapture.Start();
        }

        private void StopScreenCapture()
        {
            // signal to stop
            ScreenCapture.SignalToStop();
            ScreenCapture.NewFrame -= NewVideoFrame;
        }
        #endregion

        #region audio capture
        private void StartVoiceCapture(string fileName)
        {
            AudioCapture = new AudioCaptureDevice(new Guid(Properties.UserSettings.Default.Microphone));
            // Specify capturing options
            AudioCapture.DesiredFrameSize = 4096;
            AudioCapture.SampleRate = 44100;

            AudioCapture.NewFrame += NewAudioFrame;
            AudioCapture.AudioSourceError += AudioError;

            _audioStream = new FileStream(baseDirectory + "AppFiles/TempFiles/" + fileName, FileMode.Create);
            _waveEncoder = new WaveEncoder(_audioStream);

            AudioCapture.Start();
        }

        private void StopVoiceCapture()
        {
            AudioCapture.SignalToStop();

            FrameRate = (double)BitmapsCount / _waveEncoder.Duration * 1000;
            BitmapsCount = 0;

            AudioCapture.NewFrame -= NewAudioFrame;
            AudioCapture.AudioSourceError -= AudioError;
            AudioCapture = null;
            _waveEncoder = null;
            _audioStream.Close();
            _audioStream = null;
            stopAll -= StopVoiceCapture;
        }

        public void AudioError(object sender, AudioSourceErrorEventArgs eventArgs)
        {
            if (File.Exists(baseDirectory + "AppFiles/TempFiles/" + TempFileName))
                File.Delete(baseDirectory + "AppFiles/TempFiles/" + TempFileName);

            stopAll();
        }

        private void NewAudioFrame(object sender, Accord.Audio.NewFrameEventArgs eventArgs)
        {
            _waveEncoder.Encode(eventArgs.Signal);
        }
        #endregion
    }
}
