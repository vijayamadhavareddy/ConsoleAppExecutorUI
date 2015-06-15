using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace ConsoleApplicationExecUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StreamWriter InputStream;
        Brush redBrush = null;
        Brush greehBrush = null;
        public MainWindow()
        {
            InitializeComponent();
            redBrush = new SolidColorBrush(Colors.Red);
            greehBrush = new SolidColorBrush(Colors.Green);
        }

        void StartProcess(string path)
        {
            ProcessStartInfo info = new ProcessStartInfo(path);
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;


            using (Process process = new Process())
            {
                process.StartInfo = info;
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += process_OutputDataReceived;
                process.ErrorDataReceived += process_ErrorDataReceived;
                try
                {
                    process.Start();
                    InputStream = process.StandardInput;
                    //StreamReader sr = process.StandardOutput;
                    //process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    // process.WaitForExit();;
                    UpdateApplicationOutput("Process Started", greehBrush);
                }
                catch(Exception ex)
                {
                    UpdateApplicationOutput(String.Format("\n Error occured in starting {0} \n", path), redBrush);
                }
            }
        }

        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            UpdateApplicationOutput(String.Format("\n Error occured in process \n {0}",e.Data), redBrush);
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            UpdateConsoleStreamOutput(e.Data);
        }

        void StartProcess_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ProcessPath.Text))
            {
                UpdateApplicationOutput("\n ProcessPath shoud not be Empty", redBrush);
                return;
            }
            if(String.IsNullOrWhiteSpace(ProcessPath.Text))
            {
                UpdateApplicationOutput("\n ProcessPath shoud not be Whitespace", redBrush);
                return;
            }
            StartProcess(ProcessPath.Text);
        }

        private void ExecuteCommand_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Input.Text))
            {
                UpdateApplicationOutput("\n Command shoud not be Empty", redBrush);
                return;
            }
            if (String.IsNullOrWhiteSpace(Input.Text))
            {
                UpdateApplicationOutput("\n Command shoud not be Whitespace", redBrush);
                return;
            }
            InputStream.WriteLine(Input.Text);
        }

        void UpdateApplicationOutput(String applicatonOuputText, Brush foreground)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ApplicationOutput.Foreground = foreground;
                ApplicationOutput.Text += applicatonOuputText;
            }));
        }

        void UpdateConsoleStreamOutput(String outputText)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ConsoleStreamOutput.Text += "\n";
                ConsoleStreamOutput.Text += outputText;
            }));
        }
    }
}
