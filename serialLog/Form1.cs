using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;


namespace serialLog
{
    public partial class Form1 : Form
    {
        public string[] comPorts;
        public bool shouldLog = false;
        public BinaryWriter logFile;
        public string logFileName = "";
        public int bytesRcvd = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadComList();
            textBox1.Text = DateTime.Now.ToFileTime().ToString();
        }

        public void loadComList()
        {
            comPorts = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            int numFound = 0;
            foreach (string cm in comPorts)
            {
                comboBox1.Items.Add(cm);
                numFound++;
            }
            if (numFound == 0)
            {
                comboBox1.Items.Add("NO PORTS");
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadComList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.BaudRate = Convert.ToInt32(textBox2.Text);
            serialPort1.DataBits = Convert.ToInt32(textBox3.Text);
            serialPort1.PortName = comPorts[comboBox1.SelectedIndex];
            bytesRcvd = 0;
            try
            {
                serialPort1.Open();
                toolStripStatusLabel1.Text = "Port Open";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            serialPort1.DiscardNull = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            serialPort1.DtrEnable = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            serialPort1.RtsEnable = checkBox3.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            toolStripStatusLabel1.Text = "Port Closed";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                logFile = new BinaryWriter(new FileStream(logFileName, FileMode.Create));
                shouldLog = true;
                toolStripStatusLabel3.Text = "Logging On";
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace(" ", "_");
            if (textBox1.Text.Length > 0)
            {
                textBox1.SelectionStart = textBox1.TextLength;
            }
            logFileName = "c:\\roverlogs\\" + textBox1.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            logFile.Close();
            shouldLog = false;
            toolStripStatusLabel3.Text = "Logging Off";
            rcMenuItm_Click(sender, e);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();
            bytesRcvd = bytesRcvd + data.Length;
            toolStripStatusLabel2.Text = bytesRcvd.ToString() + " bytes received";

            //

            if (shouldLog)
            {
                logFile.Write(data);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
            button1_Click(sender, e);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {

        }

        private void rcMenuItm_Click(object sender, EventArgs e)
        {
            textBox1.Text = DateTime.Now.ToFileTime().ToString();
            textBox1_TextChanged(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BinaryReader br = new BinaryReader(new FileStream(openFileDialog1.FileName, FileMode.Open));
                    string data = br.ReadString();
                    serialPort1.Write(data);
                    br.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
            rcMenuItm_Click(sender, e);
            button4_Click(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
            button5_Click(sender, e);
        }
    }
}
