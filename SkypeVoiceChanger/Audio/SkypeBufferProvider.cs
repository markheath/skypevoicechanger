using System;
using NAudio.Wave;

namespace SkypeVoiceChanger.Audio
{
    class SkypeBufferProvider : IWaveProvider
    {
        private byte[] latestInBuffer;
        private readonly WaveFormat waveFormat;

        public SkypeBufferProvider(int sampleRate)
        {
            waveFormat = new WaveFormat(sampleRate, 16, 1);
        }

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public void SetLatestInBuffer(byte[] buffer)
        {
            latestInBuffer = buffer;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (offset != 0)
                throw new ArgumentOutOfRangeException("offset");
            if (buffer != latestInBuffer)
                Array.Copy(latestInBuffer, buffer, count);
            return count;
        }
    }
}
