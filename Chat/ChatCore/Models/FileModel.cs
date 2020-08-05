using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ChatCore.Models
{
    public class FileModel : ViewModelBase
    {
        public string FileName { get => Path.GetFileName(FileFullName); }
        public string FileFullName { get; set; }
        public string TempFilePath { get; set; }
        private byte[] _fileContent;
        public byte[] FileContent 
        { 
            get => _fileContent;
            set
            {
                _fileContent = value;
                OnPropertyChanged();
            }
        }

        private int _downloadProgress;
        public int DownloadProgress 
        {
            get => _downloadProgress;
            set
            {
                if (_downloadProgress.Equals(value))
                    return;

                _downloadProgress = value;
                OnPropertyChanged();
            }
        }

        public bool IsImage { get; set; }
        public bool IsVoice { get; set; }
        public bool IsVideo { get; set; }
        public CancellationTokenSource Cancellation { get; set; }
    }
}
