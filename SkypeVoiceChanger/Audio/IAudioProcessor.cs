namespace SkypeVoiceChanger.Audio
{
    interface IAudioProcessor
    {
        void ProcessOutgoing(byte[] buffer, int count);
        void ProcessIncoming(byte[] buffer, int count);
        void QueueForPlayback(string path);
    }
}
