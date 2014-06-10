using System;
using NAudio.Wave;
using SkypeFx;
using SkypeVoiceChanger.Audio;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger
{
    /// <summary>
    /// just for the playback part
    /// </summary>
    class AudioPlaybackGraph : IDisposable
    {
        WaveStream outStream;
        IWavePlayer player;
        EffectStream effectStream;
        ILog log;
        private readonly EffectChain effects;

        public AudioPlaybackGraph(ILog log, EffectChain effectChain)
        {
            this.log = log;
            this.effects = effectChain;
        }

        public bool FileLoaded
        {
            get
            {
                return outStream != null;
            }
        }



        public void LoadFile(string fileName)
        {
            if (outStream != null)
            {
                outStream.Dispose();
            }

            if (fileName.EndsWith(".mp3"))
            {
                outStream = new Mp3FileReader(fileName);
            }
            else if (fileName.EndsWith(".wav"))
            {
                outStream = new WaveFileReader(fileName);
            }
            else
            {
                throw new InvalidOperationException("Can't open this type of file");
            }

            if (outStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            {
                // no longer needed for MP3, but this should let us support mu-law etc
                outStream = WaveFormatConversionStream.CreatePcmStream(outStream);
            }
        }

        public void Play()
        {
            effectStream = new EffectStream(effects, outStream.ToSampleProvider());
            CreatePlayer();
            player.Init(effectStream);
            player.Play();
        }

        private void CreatePlayer()
        {
            if (player == null)
            {
                var waveOut = new WaveOut();
                waveOut.DesiredLatency = 200; // 200ms
                waveOut.NumberOfBuffers = 2;
                waveOut.DeviceNumber = 0; // default device
                player = waveOut;               
            }
        }

        public void OnTimer()
        {
            if (player != null && player.PlaybackState == PlaybackState.Playing)
            {
                if (outStream.Position >= outStream.Length)
                {
                    player.Stop();
                }
            }
        }

        public void Pause()
        {
            if (player != null)
            {
                player.Pause();
            }
        }

        public void Dispose()
        {
            if (player != null)
            {
                player.Dispose();
                player = null;
            }
            if (outStream != null)
            {
                outStream.Dispose();
                outStream = null;
            }
        }

        public void Stop()
        {
            if (player != null)
            {
                player.Stop();
            }
        }

        public void Rewind()
        {
            if (outStream != null)
            {
                outStream.Position = 0;
            }
        }

        public void AddEffect(Effect effect)
        {
            if (effectStream != null)
            {
                effectStream.AddEffect(effect);
            }
            else
            {
                effects.Add(effect);
            }

        }

        public void RemoveEffect(Effect effect)
        {
            if (effectStream != null)
            {
                effectStream.RemoveEffect(effect);
            }
            else
            {
                effects.Remove(effect);
            }

        }


        public bool MoveUp(Effect effect)
        {
            if (effectStream != null)
            {
                return effectStream.MoveUp(effect);
            }
            else
            {
                return effects.MoveUp(effect);
            }
        }

        public bool MoveDown(Effect effect)
        {
            if (effectStream != null)
            {
                return effectStream.MoveDown(effect);
            }
            else
            {
                return effects.MoveDown(effect);
            }
        }
    }
}
