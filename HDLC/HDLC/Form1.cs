using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
namespace HDLC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive_Handler);
        }
        const byte Slave1 = 1, Slave2 = 2, Slave3 = 3;
        public void MyApp()
        {
            this.connection2.ConnectBtHandler(new EventHandler(OpenPort_Click));
            this.connection2.DisconnectBtHandler(new EventHandler(ClosePort_Click));
            this.primarystation1.StartBtHandler(new EventHandler(StartTransmit_Click));
            this.primarystation1.StopBtHandler(new EventHandler(StopTransmit_Click));
            this.primarystation1.CbTextIndexChanged(new EventHandler(CbTextIndexChanged));
            this.secondarystation1.CbbSelectedChanged(new EventHandler(CbTextIndexChanged2));
        }
        
        private void OpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = connection2.comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(connection2.comboBox2.Text);
                if (connection2.comboBox3.Text == "1") serialPort1.StopBits = System.IO.Ports.StopBits.One;
                else if (connection2.comboBox3.Text == "1.5") serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                else serialPort1.StopBits = System.IO.Ports.StopBits.Two;
                if (connection2.comboBox4.Text == "none") serialPort1.Parity = System.IO.Ports.Parity.None;
                else if (connection2.comboBox4.Text == "even") serialPort1.Parity = System.IO.Ports.Parity.Even;
                else serialPort1.Parity = System.IO.Ports.Parity.Odd;
                serialPort1.Handshake = System.IO.Ports.Handshake.None;
                serialPort1.Open();
                label1.Text = "Connected: "+serialPort1.PortName.ToString()+","+serialPort1.BaudRate.ToString()+","+serialPort1.StopBits.ToString()+","+serialPort1.Parity.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Failture", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClosePort_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                label1.Text = "Disconnected";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Failture", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ToBinary(int input)
        {
            string result = string.Empty;
            int mask = 1, temp;
            for(int i = 0; i < 8; i++)
            {
                temp = input & (mask << i);
                if (temp == 0)
                    result += "0";
                else
                    result += "1";
            }
            char[] chararray = result.ToCharArray();
            Array.Reverse(chararray);
            result = new string(chararray);
            return result;
        }
        private int ToByte(string input)
        {
            int result = 0, temp = 1 << 7; 
            for(int i = 0; i < 8; i++)
            {
                result += Convert.ToInt16(input[i] - 48) * temp;
                temp >>= 1;
            }
            return result;
        }

        int NbSendFrame = 0, NbReceiveFrame = 0;
        int CtrlField = 0;
        int count = 0;
        private void StartTransmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    if (primarystation1.textBox3.Text == string.Empty)
                        throw new Exception("Cannot empty messages");
                    if (primarystation1.comboBox1.Text == "none")
                        throw new Exception("Choose slave address");

                    CtrlField = NbSendFrame * 16 + 8 + NbReceiveFrame;
                    serialPort1.WriteLine(primarystation1.comboBox1.Text + ToBinary(CtrlField) + primarystation1.textBox3.Text);
                    primarystation1.textBox1.Text += primarystation1.comboBox1.Text+ " " + ToBinary(CtrlField) + " " + primarystation1.textBox3.Text + Environment.NewLine;
                    Thread.Sleep(500);
                    primarystation1.dataGridView1.Rows.Add();
                    primarystation1.dataGridView1.Rows[count].Cells[0].Value = count + 1;
                    primarystation1.dataGridView1.Rows[count].Cells[1].Value = "Command";
                    primarystation1.dataGridView1.Rows[count].Cells[2].Value = primarystation1.comboBox1.Text;
                    primarystation1.dataGridView1.Rows[count].Cells[3].Value = "I";
                    primarystation1.dataGridView1.Rows[count].Cells[4].Value = NbSendFrame;
                    primarystation1.dataGridView1.Rows[count].Cells[5].Value = 1;
                    primarystation1.dataGridView1.Rows[count].Cells[6].Value = NbReceiveFrame;
                    primarystation1.dataGridView1.Rows[count].Cells[7].Value = primarystation1.textBox3.Text;
                    count++;
                    if (NbSendFrame > 7)
                    {
                        NbSendFrame = 0;
                    }
                    if(serialPort1.BytesToRead != 0)
                    {
               
                        string temp = string.Empty;
                        temp = serialPort1.ReadLine();
                        primarystation1.textBox2.Text += temp[0].ToString() + " " + temp.Substring(1,8) + " " + temp.Substring(9) + Environment.NewLine;
                        NbReceiveFrame = ToByte(temp.Substring(1, 8)) & 0x07;
                        primarystation1.dataGridView1.Rows.Add();
                        primarystation1.dataGridView1.Rows[count].Cells[0].Value = count + 1;
                        primarystation1.dataGridView1.Rows[count].Cells[1].Value = "Response";
                        primarystation1.dataGridView1.Rows[count].Cells[2].Value = temp[0].ToString();
                        primarystation1.dataGridView1.Rows[count].Cells[3].Value = "I";
                        primarystation1.dataGridView1.Rows[count].Cells[4].Value = (ToByte(temp.Substring(1, 8)) & 0x70) / 16;
                        primarystation1.dataGridView1.Rows[count].Cells[5].Value = 1;
                        primarystation1.dataGridView1.Rows[count].Cells[6].Value = ToByte(temp.Substring(1, 8)) & 0x07;
                        primarystation1.dataGridView1.Rows[count].Cells[7].Value = temp.Substring(9);
                        NbSendFrame++;
                    }
                    else
                    {
                        primarystation1.dataGridView1.Rows.Add();
                        primarystation1.dataGridView1.Rows[count].Cells[0].Value = count + 1;
                        primarystation1.dataGridView1.Rows[count].Cells[1].Value = "Failture";
                    }
                    count++;
                    
                }
                else
                    throw new Exception("Com port was closed");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: "+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void StopTransmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    NbSendFrame = 0;
                    NbReceiveFrame = 0;
                    CtrlField = CtrlField + 1 << 7;
                    serialPort1.WriteLine(primarystation1.comboBox1.Text + ToBinary(CtrlField));
                    primarystation1.dataGridView1.Rows.Add();
                    primarystation1.dataGridView1.Rows[count].Cells[0].Value = count + 1;
                    primarystation1.dataGridView1.Rows[count].Cells[1].Value = "Command";
                    primarystation1.dataGridView1.Rows[count].Cells[2].Value = primarystation1.comboBox1.Text;
                    primarystation1.dataGridView1.Rows[count].Cells[3].Value = "DISC";
                }
                else
                    throw new Exception("Com port was closed");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool mouseDown, MasSla_Flag = false; // 0 - Master, 1 - Slave
        private Point lastLocation;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            MyApp();
            label1.Text = "Disconnected";
            label3.Text = "Select mode";
            timer1.Enabled = false;
            textBox1.Text = "HDLC Simulator" + Environment.NewLine + "Written by: Group 6";
            primarystation1.textBox3.Text = "Hello";
            secondarystation1.textBox3.Text = "Hi There";
            connection2.comboBox1.Text = "COM1";
            connection2.comboBox2.Text = "9600";
            connection2.comboBox3.Text = "1";
            connection2.comboBox4.Text = "none";
            connection2.comboBox5.Text = "none";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (MasSla_Flag == true)
                {
                    try
                    {
                        if (serialPort1.BytesToRead != 0)
                        {
                            string Rtemp = serialPort1.ReadLine();
                            if (Rtemp[0].ToString() == secondarystation1.comboBox1.Text)
                            {
                                if(Rtemp[1].ToString() == "0")
                                {
                                    secondarystation1.textBox2.Text += Rtemp[0].ToString() + " " + Rtemp.Substring(1, 8) + " " + Rtemp.Substring(9) + Environment.NewLine;
                                    int temp1 = ((ToByte(Rtemp.Substring(1, 8)) & 0x07) + 1) & 0x07;
                                    int temp2 = ToByte(Rtemp.Substring(1, 8)) & 0xF8;
                                    CtrlField = temp1 | temp2;
                                    serialPort1.WriteLine(secondarystation1.comboBox1.Text + ToBinary(CtrlField) + secondarystation1.textBox3.Text);
                                    secondarystation1.textBox1.Text += secondarystation1.comboBox1.Text + " " + ToBinary(CtrlField) + " " + secondarystation1.textBox3.Text + Environment.NewLine;
                                }
                                else
                                {
                                    secondarystation1.textBox2.Text += Rtemp[0].ToString() + " " + Rtemp.Substring(1, 8) + " " + Rtemp.Substring(9) + Environment.NewLine;

                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            if(this.WindowState.ToString() == "Normal")
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState.ToString() == "Normal")
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            connection2.BringToFront();
            label3.Text = "Serial Port";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            secondarystation1.BringToFront();
            label3.Text = "Slave: " + secondarystation1.comboBox1.Text;
            MasSla_Flag = true;
            timer1.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            primarystation1.BringToFront();
            label3.Text = "Master";
            MasSla_Flag = false;
            timer1.Enabled = false;
        }
        private void CbTextIndexChanged(object sender, EventArgs e)
        {
            NbReceiveFrame = 0;
            NbSendFrame = 0;
        }
        private void CbTextIndexChanged2(object sender, EventArgs e)
        {
            label3.Text = "Slave: " + secondarystation1.comboBox1.Text;
        }
    }
}
