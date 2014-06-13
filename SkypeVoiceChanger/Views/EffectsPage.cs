using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Controls;
using SkypeVoiceChanger.Audio;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger.Views
{
    public partial class EffectsPage : MetroUserControl
    {
        private readonly EffectChain effectsChain;
        private readonly ICollection<Effect> availableEffects;
        private readonly AudioPlaybackGraph audioPlaybackGraph;

        readonly List<ToolStripItem> playbackButtons;

        public EffectsPage(EffectChain effectsChain, ICollection<Effect> availableEffects, AudioPlaybackGraph audioPlaybackGraph)
        {
            this.effectsChain = effectsChain;
            this.availableEffects = availableEffects;
            this.audioPlaybackGraph = audioPlaybackGraph;
            InitializeComponent();
            playbackButtons = new List<ToolStripItem> {buttonPlay, buttonPause, buttonOpen, buttonStop, buttonRewind};
        }

        // TODO: use to enable / disable playback buttons
        public SkypeStatus ConnectionStatus { get; set; }

        private void buttonAddEffect_Click(object sender, EventArgs e)
        {
            var effectSelectorForm = new EffectSelectorForm(availableEffects);
            if (effectSelectorForm.ShowDialog(this) == DialogResult.OK)
            {
                // create a new instance of the selected effect as we may want multiple copies of one effect
                var effect = (Effect)Activator.CreateInstance(effectSelectorForm.SelectedEffect.GetType());
                effectsChain.Add(effect);
                int index = checkedListBox1.Items.Add(effect, true);
                checkedListBox1.SelectedIndex = index;
            }
        }

        private void buttonRemoveEffect_Click(object sender, EventArgs e)
        {
            var selectedEffect = (Effect)checkedListBox1.SelectedItem;
            if (selectedEffect != null)
            {
                int index = checkedListBox1.SelectedIndex;
                checkedListBox1.Items.Remove(selectedEffect);
                effectsChain.Remove(selectedEffect);
                if (index < checkedListBox1.Items.Count)
                    checkedListBox1.SelectedIndex = index;
                else
                    checkedListBox1.SelectedIndex = index - 1;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var effect = (Effect)checkedListBox1.SelectedItem;
            if (effect != null)
            {
                effectPanel1.Initialize(effect);
            }
            else
            {
                effectPanel1.Clear();
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var effect = (Effect)checkedListBox1.Items[e.Index];
            effect.Enabled = e.NewValue == CheckState.Checked;
        }

        private void buttonMoveEffectUp_Click(object sender, EventArgs e)
        {
            var selectedEffect = (Effect)checkedListBox1.SelectedItem;
            if (selectedEffect != null)
            {
                if (effectsChain.MoveUp(selectedEffect))
                {
                    int index = checkedListBox1.SelectedIndex;
                    MoveEffect(selectedEffect, index, index - 1);
                }
            }
        }

        private void MoveEffect(Effect selectedEffect, int index, int newIndex)
        {
            bool isChecked = checkedListBox1.GetItemChecked(index);
            checkedListBox1.Items.Remove(selectedEffect);
            checkedListBox1.Items.Insert(newIndex, selectedEffect);
            checkedListBox1.SetItemChecked(newIndex, isChecked);
            checkedListBox1.SelectedIndex = newIndex;
        }

        private void buttonMoveEffectDown_Click(object sender, EventArgs e)
        {
            var selectedEffect = (Effect)checkedListBox1.SelectedItem;
            if (selectedEffect != null)
            {
                if (effectsChain.MoveDown(selectedEffect))
                {
                    int index = checkedListBox1.SelectedIndex;
                    MoveEffect(selectedEffect, index, index + 1);
                }
            }
        }


        private void EnablePlaybackButtons(bool enable)
        {
            foreach (ToolStripItem button in toolStrip1.Items)
            {
                if (playbackButtons.Contains(button))
                {
                    button.Enabled = enable;
                }
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Supported Files (*.mp3;*.wav)|*.mp3;*.wav";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                audioPlaybackGraph.LoadFile(fileName);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (!audioPlaybackGraph.FileLoaded)
            {
                buttonOpen_Click(sender, e);
            }
            if (audioPlaybackGraph.FileLoaded)
            {
                audioPlaybackGraph.Play();
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            audioPlaybackGraph.Pause();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            audioPlaybackGraph.Stop();
        }

        private void buttonRewind_Click(object sender, EventArgs e)
        {
            audioPlaybackGraph.Rewind();
        }
    }
}
