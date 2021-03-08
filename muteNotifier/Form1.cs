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
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All,DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            endButton.Enabled = false;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && textBox1.Text != "" && Double.TryParse(textBox1.Text,out var maxDuration))
            {
                startButton.Enabled = false;
                endButton.Enabled = true;
                updatingTimer = new Timer { Interval = 100, Enabled = true };
                updatingTimer.Tick += (se, ev) =>
                {
                    updatingTimer.Enabled = false;
                    if (comboBox1.SelectedItem != null)
                    {
                        var device = (MMDevice)comboBox1.SelectedItem;
                        var volume = device.AudioMeterInformation.MasterPeakValue * 100;
                        volumeLabel.Text = volume.ToString();
                        if(volume <= 12.5 && !isMute)
                        {
                            isMute = true;
                        }
                        else if(volume <= 12.5 && isMute)
                        {
                            muteDuration += ((double)updatingTimer.Interval / 1000);//milliSeconds
                            if(muteDuration >= maxDuration)
                            {
                                //通知
                                MessageBox.Show("ミュート期間が設定値に達しました");
                                muteDuration = 0;
                            }
                        }
                        updatingTimer.Enabled = true;
                    }
                };
            }
        }
    }
}
