using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;


namespace ProjectGUI
{
   
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        double faceShift1; 
        double faceShift2; 
        double result;

        public Form1()
        {
            InitializeComponent();
            double result = PowerMeterDLL.GetValue();
            textBox1.Text = result.ToString();
            serialPort = new SerialPort("COM5", 115200);
            try
            {
                // Open the serial port
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening serial port: " + ex.Message);
            }
        }
        
        class PowerMeterDLL
        {
            [DllImport("D:\\New folder\\PowerMeterDLL\\x64\\Release\\PowerMeterDLL.dll")]
            public static extern double GetValue();
        }
        class MeasurementEvaluation
        {
            [DllImport("C:\\Users\\Mannan\\source\\repos\\MeasurementEvaluation\\x64\\Release\\MeasurementEvaluation.dll")]
            public static extern unsafe double CalculateFaceShift(double* Power_mw, int arrayLength);
        }
            private async void button3_Click(object sender, EventArgs e)
            {
                ResultData withBeamSplitter = new ResultData();
                result = PowerMeterDLL.GetValue();
                textBox1.Text = result.ToString();

                var series = new System.Windows.Forms.DataVisualization.Charting.Series("Reference");
                series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                try
                {
                    double anglevariable = 0.0;
                    double maxResultWithBeamSplitter = double.MinValue;
                    List<ResultData> withBeamSplitterData = new List<ResultData>(); // Create a list to store the data

               
                    for (int i = 0; i < 200; i++)
                    {
                        await Task.Delay(400);
                        result = PowerMeterDLL.GetValue();
                        textBox1.Text = result.ToString();
                        textBox2.Text = anglevariable.ToString();
                        withBeamSplitter.angle = anglevariable;
                        withBeamSplitter.result = result;
                  
                        series.Points.AddXY(withBeamSplitter.angle, withBeamSplitter.result);

                        anglevariable += 1.8;
                        if (serialPort.IsOpen)
                        {
                            serialPort.Write("1");
                        }
                        withBeamSplitterData.Add(withBeamSplitter); // Add the ResultData object to the list
                        if (result > maxResultWithBeamSplitter)
                        {
                            maxResultWithBeamSplitter = result;
                            textBox3.Text = maxResultWithBeamSplitter.ToString();

                        }
                        Console.WriteLine($"Angle: {withBeamSplitter.angle}, Result: {withBeamSplitter.result}");

                    }
                    chart1.Series.Add(series);
                    double[] dataArray = withBeamSplitterData.Select(rd => rd.result).ToArray();
                    unsafe
                    {
                        fixed (double* pData = dataArray)
                        {
                            faceShift2 = MeasurementEvaluation.CalculateFaceShift(pData, dataArray.Length);
                        }
                    }

                }
                catch (Exception ex) { 
                }

            }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Get the initial result and display it in textBox1
                result = PowerMeterDLL.GetValue();
                textBox1.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting initial value: " + ex.Message);
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            ResultData withoutBeamSplitter = new ResultData();
            result = PowerMeterDLL.GetValue();
            textBox1.Text = result.ToString();
            var series = new System.Windows.Forms.DataVisualization.Charting.Series("Without Beam Splitter");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            try
            {
                double anglevariable = 0.0;
                double maxResultWithoutBeamSplitter = double.MinValue;
                List<ResultData> withoutBeamSplitterData = new List<ResultData>();

                
                for (int i = 0; i < 200; i++)
                {
                    await Task.Delay(400);

                    result = PowerMeterDLL.GetValue();
                    textBox1.Text = result.ToString();
                    textBox2.Text = anglevariable.ToString();
                    withoutBeamSplitter.angle = anglevariable;
                    withoutBeamSplitter.result = result;
                    
                    series.Points.AddXY(withoutBeamSplitter.angle, withoutBeamSplitter.result);

                    anglevariable += 1.8;
                    if (serialPort.IsOpen)
                    {
                        serialPort.Write("1");
                    }
                    withoutBeamSplitterData.Add(withoutBeamSplitter); // Add the ResultData object to the list

                    if (result > maxResultWithoutBeamSplitter)
                    {
                        maxResultWithoutBeamSplitter = result;
                        textBox3.Text = maxResultWithoutBeamSplitter.ToString();

                    }
                    Console.WriteLine($"Angle: {withoutBeamSplitter.angle}, Result: {withoutBeamSplitter.result}");

                }
                chart1.Series.Add(series);

                double[] dataArray = withoutBeamSplitterData.Select(rd => rd.result).ToArray(); // Use the correct list
                unsafe
                {
                    fixed (double* pData = dataArray)
                    {
                        faceShift1 = MeasurementEvaluation.CalculateFaceShift(pData, dataArray.Length);
                    }
                }
                double faceShiftDifference = faceShift1 - faceShift2;
                textBox5.Text = faceShiftDifference.ToString();
            }
            catch (Exception ex)
            {
            }

        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
