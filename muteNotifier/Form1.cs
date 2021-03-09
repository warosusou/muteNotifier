using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace muteNotifier
{
    public partial class Form1 : Form
    {
        private double muteDuration;
        private bool isMute;
        
        public Form1()
        {
            InitializeComponent();
            muteDuration = 0;
            isMute = false;
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render,DeviceState.Active);
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render,Role.Communications);
            comboBox1.Items.AddRange(devices.ToArray());
            comboBox1.SelectedItem = comboBox1.Items.Cast<MMDevice>().First(x => x.ID == defaultDevice.ID);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            endButton.Enabled = false;
        }

        private void endButton_Click(object sender, EventArgs e)
        {
            muteDuration = 0;
            isMute = false;
            startButton.Enabled = true;
            endButton.Enabled = false;
            comboBox1.Enabled = true;
            textBox1.Enabled = true;
            updatingTimer.Enabled = false;
            volumeLabel.Text = "0";
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && textBox1.Text != "" && Double.TryParse(textBox1.Text,out var maxDuration))
            {
                startButton.Enabled = false;
                endButton.Enabled = true;
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                updatingTimer = new Timer { Interval = 100, Enabled = true };
                updatingTimer.Tick += (se, ev) =>
                {
                    updatingTimer.Enabled = false;
                    if (comboBox1.SelectedItem != null)
                    {
                        var device = (MMDevice)comboBox1.SelectedItem;
                        var volume = device.AudioMeterInformation.MasterPeakValue / device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                        volumeLabel.Text = volume.ToString();
                        if(volume <= 0 && !isMute)
                        {
                            isMute = true;
                        }
                        else if(volume <= 0 && isMute)
                        {
                            muteDuration += ((double)updatingTimer.Interval / 1000);//milliSeconds
                            if(muteDuration >= maxDuration)
                            {
                                //通知
                                MessageBox.Show("ミュート期間が設定値に達しました","",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                                muteDuration = 0;
                            }
                        }
                        else
                        {
                            muteDuration = 0;
                            isMute = false;
                        }
                        updatingTimer.Enabled = true;
                    }
                };
            }
        }
    }
}
