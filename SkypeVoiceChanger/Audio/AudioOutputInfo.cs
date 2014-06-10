using System.IO;

namespace SkypeVoiceChanger.Audio
{
    class AudioOutputInfo
    {
        public Stream OutStream { get; set; }
        public string OutPath { get; set; }
    }
}