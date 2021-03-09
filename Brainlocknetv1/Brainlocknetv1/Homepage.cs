using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using NeuroSky.ThinkGear;
using System.Drawing;
using System.Media;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Brainlocknetv1
{
    public partial class Homepage : Form
    {

        SoundPlayer player1k = new SoundPlayer(@"E:\\Brainlock\\audio\\1k.wav");
        SoundPlayer player440 = new SoundPlayer(@"E:\\Brainlock\\audio\\440.wav");
        SoundPlayer player225 = new SoundPlayer(@"E:\\Brainlock\\audio\\250.wav");
        SoundPlayer player100 = new SoundPlayer(@"E:\\Brainlock\\audio\100.wav");
        private Connector connector = new Connector();
        public string DevicePortName = "COM8" ;
        private string name;
        private string testtime;
        Boolean startClick = false;
        private string datalow = "";
        public int timing;
        Boolean checking = false;
        private string datahigh = "";
        private string dataraw = "";
        public Homepage()
        {
            InitializeComponent();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            
            connector.DeviceNotFound += new EventHandler(OnDeviceNotFound);
            connector.DeviceConnectFail += new EventHandler(OnDeviceNotFound);

        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (NameID.Text != "" && TestNum.Text != "")
            {
                if (checking == true)
                {
                    
                    startClick = true;
                    
                    audioplay();
                    
                }
                else
                {
                    MessageBox.Show("Click connect button to connect headset");
                }
                label3.Text = "Đang đo .........";

            }
            else

            {
                MessageBox.Show("Fill textbox");
            }
           

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (NameID.Text != "" && TestNum.Text != "")
            {
                name = NameID.Text;
                testtime = TestNum.Text;

                try
                {
                    connector.Connect("COM8");
                }
                catch
                {
                    MessageBox.Show("Cant connect to port COM8");

                }
            } else 
           
            {
                MessageBox.Show("Fill textbox");
            }
            
            
        }
        
        void OnDeviceNotFound(object sender, EventArgs e)
        {

            MessageBox.Show("Cant found any headset");
        }
        void OnDeviceConnected(object sender, EventArgs e)
        {

            Connector.DeviceEventArgs deviceEventArgs = (Connector.DeviceEventArgs)e;
            // refress data 
            
            MessageBox.Show("New Headset Created.");
            checking = true;
            deviceEventArgs.Device.DataReceived += new EventHandler(OnDataReceived);
            
           
        }
        
       
        private string high;
        private string low;
        private string raw;
        void OnDataReceived(object sender, EventArgs e)
        {
           
            /* Cast the event sender as a Device object, and e as the Device's DataEventArgs */
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            /* Create a TGParser to parse the Device's DataRowArray[] */
            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);

            /* Loop through parsed data TGParser for its parsed data... */
            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {

                // See the Data Types documentation for valid keys such
                // as "Raw", "PoorSignal", "Attention", etc.

                if (tgParser.ParsedData[i].ContainsKey("PoorSignal"))
                {
                    
                    Console.WriteLine("PQ Value:" + tgParser.ParsedData[i]["PoorSignal"]);
                    Console.WriteLine("LAN" + timing.ToString());
                    
                }

                if (tgParser.ParsedData[i].ContainsKey("Attention"))
                {

                    Console.WriteLine("Att Value:" + tgParser.ParsedData[i]["Attention"]);
                }
              
                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta1"))
                {
                    low = tgParser.ParsedData[i]["EegPowerBeta1"].ToString();
                    if (startClick == true)
                    {
                        dtlow(low.ToString(), 1);

                    }
                   
                    Console.WriteLine("Beta1:" + low);
                }
                if (tgParser.ParsedData[i].ContainsKey("Raw"))
                {
                   raw = tgParser.ParsedData[i]["Raw"].ToString();
                    if (startClick == true)
                    {
                        dtraw(raw.ToString(), 1);
                        //playsound random

                    }
                   
                   // Console.WriteLine("Raw:" + raw);
                }
                if (tgParser.ParsedData[i].ContainsKey("EegPowerBeta2"))
                {

                    high = tgParser.ParsedData[i]["EegPowerBeta2"].ToString();
                    if (startClick == true)
                    {
                        dthigh(high.ToString(), 1);
                        //playsound random

                    }
                    Console.WriteLine("Beta2:" + high);
                }


            }
        }
        void dthigh(string hi, int b)
        {
            if (b == 1)
            {
                datahigh += " " + hi;
                timing++;

                if (timing > 35)
                {
                    timing = 0;
                    
                    output();
                    dataraw = "";
                    datahigh = "";
                    datalow = "";
                    timing = 0;
                }
            } else
            {
                datahigh = "";
            }
        }
        void dtraw(string ra, int b)
        {
            if (b == 1)
            {
                dataraw += " " + ra;

            }
            else
            {
                dataraw = "";
            }
        }
        void dtlow(string lo, int b)
        {
            if (b == 1)
            {
                
                datalow += " " + lo;
               
            }
            else
            {
                datalow = "";
            }
        }
        void output()
        {
            
            audiostop();

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Append text to an existing file named "WriteLines.txt".


            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, NameID.Text + "_hightest.txt"), true))
            {
                outputFile.WriteLine(DateTime.Now.ToString("MM/dd/yyyy H:mm") + " " + NameID.Text + " Lan_" + TestNum.Text + " Datahigh: " + datahigh);
               
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, NameID.Text + "_lowtest.txt"), true))
            {
               
                outputFile.WriteLine(DateTime.Now.ToString("MM/dd/yyyy H:mm") + " " + NameID.Text + " Lan_" + TestNum.Text + " Datalow: " + datalow);
            }
            using (StreamWriter outputFile2 = new StreamWriter(Path.Combine(docPath, NameID.Text + "_rawtest.txt"), true))
            {
                
                outputFile2.WriteLine(DateTime.Now.ToString("MM/dd/yyyy H:mm") + "/" + NameID.Text + "_Lan" + TestNum.Text + "_Dataraw: " + dataraw);
            }
            
            
            startClick = false;
            MessageBox.Show("Finish");
         
        }
        int set;
        void audioplay()
        {
            if (checkBox1.Checked == false)
            {
                Random rnd = new Random();
                set = rnd.Next(1, 2);
                switch (set)
                {
                    case 1:
                        {
                            try
                            {
                                player1k.Play();
                            }
                            catch
                            {

                            }
                            break;
                        }
                    case 2:
                        try
                        {
                            player440.Play();
                        }
                        catch
                        {

                        }

                        break;
                    case 3:
                        try
                        {
                            player225.Play();
                        }
                        catch
                        {

                        }
                        break;
                    default:
                        try
                        {
                            player100.Play();
                        }
                        catch
                        {

                        }
                        break;
                }
            }
        }
        void audiostop()
        {
            if (checkBox1.Checked == false)
            {
                switch (set)
                {
                    case 1:
                        {
                            try
                            {
                                player1k.Stop();
                            }
                            catch
                            {

                            }
                            break;
                        }
                    case 2:
                        try
                        {
                            player440.Stop();
                        }
                        catch
                        {

                        }

                        break;
                    case 3:
                        try
                        {
                            player225.Stop();
                        }
                        catch
                        {

                        }
                        break;
                    default:
                        try
                        {
                            player100.Stop();
                        }
                        catch
                        {

                        }
                        break;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            audiostop();
            connector.Close();
            Application.Exit();
        }

        private void NameID_TextChanged(object sender, EventArgs e)
        {

           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            TestNum.Text = (int.Parse(TestNum.Text) + 1).ToString();   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TestNum.Text = (int.Parse(TestNum.Text) - 1).ToString();
        }
    }
}
