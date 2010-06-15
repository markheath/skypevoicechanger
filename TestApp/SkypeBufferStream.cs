using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace SkypeFx
{
    class SkypeBufferStream : WaveStream
    {
        byte[] latestInBuffer;
        WaveFormat waveFormat;

        public SkypeBufferStream(int sampleRate)
        {
            waveFormat = new WaveFormat(sampleRate, 16, 1);
        }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetLatestInBuffer(byte[] buffer)
        {
            latestInBuffer = buffer;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset != 0)
                throw new ArgumentOutOfRangeException("offset");
            if (buffer != latestInBuffer)
                Array.Copy(latestInBuffer, buffer, count);
            return count;
        }
    }
}
