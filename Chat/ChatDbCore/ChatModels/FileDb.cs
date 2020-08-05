using System;
using System.Collections.Generic;
using System.Text;

namespace ChatDbCore.ChatModels
{
    public class FileDb : BaseDbModel
    {
        public string FilePath { get; set; }
        public bool IsImage { get; set; }
        public bool IsAudio { get; set; }
        public bool IsVideo { get; set; }
    }
}
