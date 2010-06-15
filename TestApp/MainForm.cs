// n.b. icons used are Milky Icons, courtesy of Min Tran - http://min.frexy.com/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using JSNet;
using System.ComponentModel.Composition;

namespace SkypeFx
{
    [Export(typeof(MainForm))]
    public partial class MainForm : Form
    {
        MainFormAudioGraph audioGraph;
        List<ToolStripItem> playbackButtons;
        CustomerFeedbackForm feedbackForm;
        
        [Import]
        public ICollection<Effect> Effects { get; set; }

        public MainForm()
        {
            InitializeComponent();
            timer1.Interval = 500;
            timer1.Start();
            var log = new RichTextLogger(this.richTextBox1);
            audioGraph = new MainFormAudioGraph(log);
            playbackButtons = new List<ToolStripItem>();
            playbackButtons.Add(buttonPlay);
            playbackButtons.Add(buttonPause);
            playbackButtons.Add(buttonOpen);
            playbackButtons.Add(buttonStop);
            playbackButtons.Add(buttonRewind);           
        }

        private void passThroughEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.wav|*.wav";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (WaveFileReader reader = new WaveFileReader(openFileDialog.FileName))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "*.wav|*.wav";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = saveFileDialog.FileName;
                        EffectStream effectStream = new EffectStream(new PitchDown(), reader);
                        WaveFileWriter.CreateWaveFile(fileName, effectStream);
                        System.Diagnostics.Process.Start(fileName);
                    }
                }
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Files (*.mp3;*.wav)|*.mp3;*.wav";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                audioGraph.LoadFile(fileName);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if(!audioGraph.FileLoaded)
            {
                buttonOpen_Click(sender, e);
            }
            if (audioGraph.FileLoaded)
            {
                audioGraph.Play(this.Handle);
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            audioGraph.Pause();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            audioGraph.Stop();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            audioGraph.Dispose();
        }

        private void buttonRewind_Click(object sender, EventArgs e)
        {
            audioGraph.Rewind();
        }

        private void buttonAddEffect_Click(object sender, EventArgs e)
        {
            EffectSelectorForm effectSelectorForm = new EffectSelectorForm(Effects);
            if (effectSelectorForm.ShowDialog(this) == DialogResult.OK)
            {
                // create a new instance of the selected effect as we may want multiple copies of one effect
                Effect effect = (Effect)Activator.CreateInstance(effectSelectorForm.SelectedEffect.GetType());
                audioGraph.AddEffect(effect);
                int index = checkedListBox1.Items.Add(effect, true);
                checkedListBox1.SelectedIndex = index;
            }
            //MessageBox.Show(String.Format("I have {0} effects", Effects.Count));
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            audioGraph.OnTimer();
        }

        private void buttonRemoveEffect_Click(object sender, EventArgs e)
        {
            Effect selectedEffect = (Effect)checkedListBox1.SelectedItem;
            if (selectedEffect != null)
            {
                int index = checkedListBox1.SelectedIndex;
                checkedListBox1.Items.Remove(selectedEffect);
                audioGraph.RemoveEffect(selectedEffect);
                if(index < checkedListBox1.Items.Count)
                    checkedListBox1.SelectedIndex = index;
                else
                    checkedListBox1.SelectedIndex = index-1;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Effect effect = (Effect)checkedListBox1.SelectedItem;
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
            Effect effect = (Effect)checkedListBox1.Items[e.Index];
            effect.Enabled = e.NewValue == CheckState.Checked;
        }

        private void buttonMoveEffectUp_Click(object sender, EventArgs e)
        {
            Effect selectedEffect = (Effect)checkedListBox1.SelectedItem;
            if (selectedEffect != null)
            {
                if (audioGraph.MoveUp(selectedEffect))
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
            Effect selectedEffect = (Effect)checkedListBox1.SelectedItem;
            if (selectedEffect != null)
            {
                if (audioGraph.MoveDown(selectedEffect))
                {
                    int index = checkedListBox1.SelectedIndex;
                    MoveEffect(selectedEffect, index, index + 1);
                }
            }
        }

        private void toolStripButtonSkype_Click(object sender, EventArgs e)
        {
            SkypeMode = !SkypeMode;
        }

        private bool SkypeMode
        {
            get
            {
                return toolStripButtonSkype.Checked;
            }
            set
            {
                if (SkypeMode != value)
                {
                    toolStripButtonSkype.Checked = value;
                    if (value)
                    {
                        audioGraph.ConnectToSkpe();
                    }
                    else
                    {
                        audioGraph.DisconnectFromSkype();
                    }
                    EnablePlaybackButtons(!value);
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

        private void buttonSave_Click(object sender, EventArgs e)
        {

        }


        private void linkCustomerFeedbackOptions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (feedbackForm == null || !feedbackForm.Visible)
            {
                feedbackForm = new CustomerFeedbackForm();
            }

            feedbackForm.Show();
        }
    }
}
