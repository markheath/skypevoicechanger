// n.b. icons used are Milky Icons, courtesy of Min Tran - http://min.frexy.com/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using MetroFramework.Forms;
using SKYPE4COMLib;
using SkypeVoiceChanger.Audio;
using SkypeVoiceChanger.Effects;
using SkypeVoiceChanger.Views;

namespace SkypeVoiceChanger
{
    [Export(typeof(MainForm))]
    public partial class MainForm : MetroForm
    {
        readonly AudioPipeline audioPipeline;
        private readonly EffectChain effectChain;
        private readonly AudioPlaybackGraph audioPlaybackGraph;
        private SkypeAudioInterceptor audioInterceptor;
        private readonly ILog log;
        private readonly ConnectionStatusPage connectionStatusPage;
        private readonly EffectsPage effectsPage;

        [ImportingConstructor]
        public MainForm(ICollection<Effect> effects)
        {
            InitializeComponent();
            effectChain = new EffectChain();
            audioPlaybackGraph = new AudioPlaybackGraph(effectChain);
            tabPageRecord.Controls.Add(new RecordingPage() { Dock = DockStyle.Fill });
            tabPageAbout.Controls.Add(new AboutPage() { Dock = DockStyle.Fill });
            connectionStatusPage = new ConnectionStatusPage() {Dock = DockStyle.Fill};
            effectsPage = new EffectsPage(effectChain, effects, audioPlaybackGraph) { Dock = DockStyle.Fill };
            tabPage1.Controls.Add(connectionStatusPage);
            tabPage2.Controls.Add(effectsPage);
            log = connectionStatusPage.Log;
            audioPipeline = new AudioPipeline(effectChain);

        }

        public void ConnectToSkype()
        {
            audioPlaybackGraph.Stop();
            DisconnectFromSkype();
            if (audioInterceptor == null)
            {
                var skype = new Skype();

                audioInterceptor = new SkypeAudioInterceptor(skype, skype, log, audioPipeline);

                audioInterceptor.SkypeStatusChanged += AudioInterceptorOnSkypeStatusChanged;
                AudioInterceptorOnSkypeStatusChanged(this, EventArgs.Empty);// get initial state set up
            }
            audioInterceptor.Attach();
        }

        public void DisconnectFromSkype()
        {
            if (audioInterceptor != null)
            {
                audioInterceptor.SkypeStatusChanged -= AudioInterceptorOnSkypeStatusChanged;
                audioInterceptor.Dispose();
                audioInterceptor = null;
            }
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            audioPlaybackGraph.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e) {

            Properties.Settings appSettings = Properties.Settings.Default;

            ConnectToSkype();

            if (appSettings.FirstRun) {

                appSettings.FirstRun = false;
                appSettings.Save();
            }
        }

        private void AudioInterceptorOnSkypeStatusChanged(object sender, EventArgs eventArgs)
        {
            connectionStatusPage.ConnectionStatus = audioInterceptor.SkypeStatus;
            effectsPage.ConnectionStatus = audioInterceptor.SkypeStatus;
        }
    }
}
