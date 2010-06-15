using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNet;
using NAudio.Wave;

namespace SkypeFx
{
    class MainFormAudioGraph : IDisposable
    {
        WaveStream outStream;
        IWavePlayer player;
        EffectChain effects;
        EffectStream effectStream;
        MicInterceptor interceptor;
        ILog log;

        public MainFormAudioGraph(ILog log)
        {
            this.log = log;
            effects = new EffectChain();
        }

        public bool FileLoaded
        {
            get
            {
                return outStream != null;
            }
        }

        public void ConnectToSkpe()
        {
            Stop();
            DisconnectFromSkype();
            interceptor = new MicInterceptor(log, effects);
            effectStream = interceptor.OutputStream;
        }

        public void DisconnectFromSkype()
        {
            if (interceptor != null)
            {
                interceptor.Dispose();
                interceptor = null;
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
                outStream = WaveFormatConversionStream.CreatePcmStream(outStream);
                outStream = new BlockAlignReductionStream(outStream); // reduces choppiness
            }
        }

        public void Play(IntPtr handle)
        {
            effectStream = new EffectStream(effects, outStream);
            CreatePlayer();
            player.Init(effectStream);
            player.Play();
        }

        private void CreatePlayer()
        {
            if (player == null)
            {
                player = new WaveOut(0, 300, true);
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
            DisconnectFromSkype();
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

            RIAddEffect(effect.Name);
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

            RIRemoveEffect(effect.Name);
        }


        private void RIAddEffect(string effect) {
        }

        private void RIRemoveEffect(string effect) {
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
