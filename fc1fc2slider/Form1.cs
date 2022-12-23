using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Memory;


namespace fc1fc2slider
{
    public partial class Form1 : Form
    {

        public Mem m = new Mem();

        public Form1()
        {
            InitializeComponent();
        }

        bool ProcOpen = false;
        bool ProcOpen2 = false;
        bool backwork1 = true;
        bool backwork2 = true;
        int fc1pid = 0;
        int fc2pid = 0;
        int fc1dec = 0;
        int fc2dec = 0;
        long fc1SingleAoBScanResult;
        long fc2SingleAoBScanResult;
        string fc1memory = null;
        string fc2memory = null;
        string fc1hex = null;
        string fc2hex = null;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Using_TrackBar2_Load(object sender, EventArgs e)
        {
            trackBar2.Value = 10;
        }

//**********************************************************************************************************//
// 백그라운드 작업 - 프로세스 id 검색 및 attach // fc1

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (backwork1 == true)
                {
                    fc1pid = m.GetProcIdFromName("ggpofba-ng");

                    if (fc1pid != 0)
                    {
                        ProcOpen = true;
                        m.OpenProcess("ggpofba-ng");
                        backgroundWorker1.ReportProgress(0);
                        Thread.Sleep(1000);
                        return;
                    }

                    else if (fc1pid == 0)
                    {
                        ProcOpen = false;
                        backgroundWorker1.ReportProgress(0);
                        Thread.Sleep(1000);
                        return;
                    }

                }

                else if (backwork1 == false)
                {
                    backgroundWorker1.ReportProgress(0);
                    Thread.Sleep(1000);
                    return;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backwork1 == true)
            {
                if (!m.OpenProcess("ggpofba-ng"))
                {
                    ProcOpen = false;
                    fc1status.Text = "The Emulator is Not Working";
                    fc1dec = 0;
                    fc1memory = null;
                    fc1hex = null;
                }

                if (m.OpenProcess("ggpofba-ng"))
                {
                    ProcOpen = true;
                    backwork1 = false;
                    fc1status.Text = "The Emulator is Working Now";
                    fc1scan.PerformClick();
                }
            }

            if (backwork1 == false)
            {
                if (!m.OpenProcess("ggpofba-ng"))
                {
                    ProcOpen = false;
                    backwork1 = true;
                }
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

//**********************************************************************************************************//
// 백그라운드 작업 - 프로세스 id 검색 및 attach // fc2

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (backwork2 == true)
            {
                fc2pid = m.GetProcIdFromName("fcadefbneo");

                if (fc2pid != 0)
                {
                    ProcOpen2 = true;
                    m.OpenProcess("fcadefbneo");
                    backgroundWorker2.ReportProgress(0);
                    Thread.Sleep(1000);
                    return;
                }

                else if (fc2pid == 0)
                {
                    ProcOpen2 = false;
                    backgroundWorker2.ReportProgress(0);
                    Thread.Sleep(1000);
                    return;
                }

            }

            else if (backwork2 == false)
            {
                backgroundWorker2.ReportProgress(0);
                Thread.Sleep(1000);
                return;
            }
        }
    

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (backwork2 == true) { 

                if (!m.OpenProcess("fcadefbneo"))
            {
                ProcOpen2 = false;
                fc2status.Text = "The Emulator is Not Working";
                fc2dec = 0;
                fc2memory = null;
                fc2hex = null;
            }

            if (m.OpenProcess("fcadefbneo"))
            {
                ProcOpen2 = true;
                backwork2 = false;
                fc2status.Text = "The Emulator is Working Now";
                fc2scanbutton.PerformClick();
            }

            }

            if (backwork2 == false)
            {
                if (!m.OpenProcess("fcadefbneo"))
                {
                    ProcOpen2 = false;
                    backwork2 = true;
                }
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

//**********************************************************************************************************//
// Array Of Byte 스캔 - 범용으로 만들기 위해서는 필수
// this function is async, which means it does not block other code
//reference : https://www.delftstack.com/ko/howto/csharp/integer-to-hexadecimal-in-csharp/

        public async void fc1Scan()
        {
            if (!m.OpenProcess("ggpofba-ng"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IEnumerable<long> fc1AoBScanResults = await m.AoBScan("?? ?? 00 00 01 00 00 00 FF FF 01 01", true, true, true);
            fc1SingleAoBScanResult = fc1AoBScanResults.FirstOrDefault();

            fc1memory = fc1SingleAoBScanResult.ToString("X"); //16진수 메모리주소
            fc1dec = Int32.Parse(m.Read2Byte(fc1SingleAoBScanResult.ToString("X")).ToString()); //10진수256(100%)
            fc1hex = fc1dec.ToString("X"); // 16진수 100(256)
        }

        public async void fc2Scan()
        {
            if (!m.OpenProcess("fcadefbneo"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IEnumerable<long> fc2AoBScanResults = await m.AoBScan("?? ?? 00 00 44 97 02", true, true, true);
            fc2SingleAoBScanResult = fc2AoBScanResults.FirstOrDefault();

            fc2memory = fc2SingleAoBScanResult.ToString("X"); //16진수 메모리주소
            fc2dec = Int32.Parse(m.Read2Byte(fc2SingleAoBScanResult.ToString("X")).ToString()); //10진수256(100%)
            fc2hex = fc2dec.ToString("X"); // 16진수 100(256)
        }

//**********************************************************************************************************//
// 스캔버튼 및 UI 기능(fc1)

        private void fc1scan_Click(object sender, EventArgs e)
        {
            Task taskA = new Task(() => fc1Scan());
            taskA.Start();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = "" + trackBar2.Value;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "0";
            }
            if (Convert.ToInt32(textBox1.Text) > 533)
            {
                textBox1.Text = "533";
            }
            trackBar2.Value = Convert.ToInt32(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ProcOpen)
            {
                fc1status.Text = "Emulator is not opened!";
                fc1value.Text = "N/A";
                return;
            }

            String fc1percent = Convert.ToString(textBox1.Text);
            int fc1value2 = Convert.ToInt32(fc1percent, 16);
            string cpuvaluefc1 = Convert.ToString(fc1value2);
            fc1dec = fc1value2;

            m.WriteMemory(fc1memory, "int", cpuvaluefc1);
            fc1status.Text = "Overclocked with " + fc1percent + " %";
            fc1value.Text = fc1percent + " %";
            return;
        }
//**********************************************************************************************************//
// 스캔버튼 및 UI 기능(fc2)

        private void fc2scanbutton_Click(object sender, EventArgs e)
        {
            Task taskB = new Task(() => fc2Scan());
            taskB.Start();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = "" + trackBar3.Value;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.Text = "0";
            }
            if (Convert.ToInt32(textBox2.Text) > 533)
            {
                textBox2.Text = "533";
            }
            trackBar3.Value = Convert.ToInt32(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ProcOpen)
            {
                fc2status.Text = "Emulator is not opened!";
                fc2value.Text = "N/A";
                return;
            }

            String fc2percent = Convert.ToString(textBox2.Text);
            int fc2value2 = Convert.ToInt32(fc2percent, 16);
            string cpuvaluefc2 = Convert.ToString(fc2value2);
            fc2dec = fc2value2;

            m.WriteMemory(fc1memory, "int", cpuvaluefc2);
            fc2status.Text = "Overclocked with " + fc2percent + " %";
            fc2value.Text = fc2percent + " %";
            return;
        }

        private void Form1_Shown_1(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //blogspot
        {
            System.Diagnostics.Process.Start("https://github.com/Zansword/FC-Overclocker-slider/releases");
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //blogspot
        {
            System.Diagnostics.Process.Start("https://chamcham425.blogspot.com/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //discord 
        {
            System.Diagnostics.Process.Start("https://discord.gg/rxjcur9mTT");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // fc2official
        {
            System.Diagnostics.Process.Start("https://www.fightcade.com/");
        }
    }

}
