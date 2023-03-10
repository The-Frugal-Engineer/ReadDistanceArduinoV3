using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts.Charts;
using LiveCharts.WinForms;
using CartesianChart = LiveCharts.Wpf.CartesianChart;
using LiveCharts.Definitions.Charts;
using System.IO.Ports;
using System.Threading;

namespace ReadDistanceArduinoV3
{
    public partial class Form1 : Form
    {
        private const int V = 0;
        SerialPort port;

        LiveCharts.WinForms.CartesianChart cartesianChart2;
        public Form1()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            if (port == null)
            {
                port = new SerialPort("COM8", 9600);//Set your board COM
                port.Open();
            }
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
            progressBar1.Step = 1;
            resetChart();
        }
        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
        }
        private void CartesianChart1OnDataClick(object sender, ChartPoint chartPoint)
        {
            MessageBox.Show("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")");
        }

        public void resetChart()
        {
            LiveCharts.WinForms.CartesianChart cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            cartesianChart1.Width = 600;
            cartesianChart1.Height = 300;
            cartesianChart1.Visible = false;
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> {}
                }

            };

            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Measure",
                
                //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
            });
            

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Distance CM",
                MinValue = 0,
                MaxValue= 50,
                //,LabelFormatter = value => value.ToString("C")
            });

            cartesianChart1.LegendLocation = LegendLocation.Right;


            cartesianChart1.DataClick += CartesianChart1OnDataClick;

            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(cartesianChart1);
            cartesianChart1.Visible = true;
            this.cartesianChart2 = cartesianChart1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resetChart();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //String a = textBox1.Text;
            int numberOfReadings = 0;
            Int32.TryParse(textBox1.Text, out numberOfReadings);
            CleanBuffer();
            for (int i = 0; i < numberOfReadings; i++)
            {
                
                Task <String> valueSerial = ReadSerial();
                String finalValueSerial = await valueSerial;
                Double d = Convert.ToDouble(finalValueSerial);
                this.cartesianChart2.Series[0].Values.Add(d);
                int barValue = Convert.ToInt32(d);
                if(barValue >100)
                {
                    barValue = 100;
                }
                progressBar1.Value = barValue;
                label4.Text =  d.ToString();
            }
           
          
        }
        public void CleanBuffer()
        {
            port.DiscardInBuffer();
        }
        public async Task<String> ReadSerial()
        {
            String stringSerial = "";

            await Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting Read");
                
                stringSerial = port.ReadLine();
            });


            return stringSerial;
        }
    }
}
