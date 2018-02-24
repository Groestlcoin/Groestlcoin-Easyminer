using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GroestlCoin_EasyMiner_2018.Business_Logic;
using GroestlCoin_EasyMiner_2018.Properties;

namespace GroestlCoin_EasyMiner_2018
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer
    {
        private bool _ShowCpu;
        private bool _ShowGpu;


        public bool ShowCpu
        {
            get
            {
                return _ShowCpu;
            }
            set
            {
                _ShowCpu = value;
                uxCpuMiningLogGroup.Visibility = _ShowCpu ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public bool ShowGpu
        {
            get
            {
                return _ShowGpu;
            }
            set
            {
                _ShowGpu = value;
                uxGpuMiningLog.Visibility = _ShowGpu ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public LogViewer()
        {
            InitializeComponent();
            ShowCpu = false;
            ShowGpu = false;
        }




        private const int _MaxLines = 30;


        public void WriteToCpuLog(string Text)
        {
            WriteToLog(ref uxCpuLog, Text);
        }
        public void WriteToGpuLog(string Text)
        {
            WriteToLog(ref uxGpuLog, Text);
        }

        private void WriteToLog(ref TextBox TxtBox, string Text)
        {
            if (TxtBox != null)
            {
                TxtBox.AppendText(Environment.NewLine + Text);

                while (TxtBox.LineCount > _MaxLines)
                {
                    TxtBox.Text = TxtBox.Text.Remove(0, TxtBox.GetLineLength(0));
                }
                TxtBox.ScrollToEnd();
            }

        }
    }
}
