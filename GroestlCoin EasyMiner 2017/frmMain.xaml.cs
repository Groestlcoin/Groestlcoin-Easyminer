using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BL_EasyMiner;
using BL_EasyMiner.Helper;
using GroestlCoin_EasyMiner_2017.Properties;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace GroestlCoin_EasyMiner_2017 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>d
    public partial class MainWindow : Window {
        private const string ProgramTitle = "GroestlCoin Easyminer 2017";
        private bool _minerStarted = false;
        public DialogResult Result;
        private Timer timer;
        private Timer ProgressTimer = new Timer();

        private readonly BackgroundWorker _cpuBg = new BackgroundWorker();
        private readonly BackgroundWorker _amdBg = new BackgroundWorker();
        private readonly BackgroundWorker _nVidiaBg = new BackgroundWorker();

        public MainWindow() {
            InitializeComponent();
            var executingAssembly = System.IO.Directory.GetCurrentDirectory();
            MiningOperation.MinerExePath = executingAssembly + @"\Scripts\";
            MiningOperation.MiningBatch = executingAssembly + @"\Scripts\StartMining.bat";
            MiningOperation.ElectrumPath = executingAssembly + @"\Prerequisites\Electrum-GRS\electrum-grs.exe";
            MiningOperation.LogFileLocation = executingAssembly + @"\scripts\minelog.txt";

            ConfigureBackgroundWorkers();

            try {
                if (HelperLogic.RegAddress.Length > 0) {
                    RbCustomPool.IsChecked = true;
                    TxtAddress.Text = HelperLogic.RegAddress;
                }
                else {
                    RbUseElectrum.IsChecked = true;
                }
                if (HelperLogic.RegPool.Length > 0) {
                    RbCustomPool.IsChecked = true;
                    TxtPool.Text = HelperLogic.RegPool;
                    TxtUsername.Text = HelperLogic.RegUserName;
                    TxtPassword.Text = HelperLogic.RegPassword;
                }
                else {
                    RbUsedwarfPool.IsChecked = true;
                }
            }
            catch (Exception ex) {
                //Todo: Do something here
            }
        }

        private void ConfigureBackgroundWorkers() {
            var executingAssembly = System.IO.Directory.GetCurrentDirectory();

            _cpuBg.DoWork += (sender, args) => {
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = executingAssembly + @"\Resources\Miners\CPU Miner\minerd.exe";
                    info.Arguments =
                        $@"-a groestl -o stratum+tcp://moria.dwarfpool.com:3345 -u {args.Argument.ToString()} -p x >log.txt" ;
                    process.StartInfo = info;
                    process.Start();
                }
            };

            _amdBg.DoWork += (sender, args) => {
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = executingAssembly + @"\Resources\Miners\AMD Miner\sgminer.exe",
                        Arguments =
                            $"-I 19 -g 4 -w 64 -k groestlcoin --no-submit-stale -o stratum+tcp://moria.dwarfpool.com:3345 -u {args.Argument.ToString()} -p x >log.txt"
                    };
                    process.StartInfo = info;
                    process.Start();
                }
            };

            _nVidiaBg.DoWork += (sender, args) => {
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = executingAssembly + @"\Resources\Miners\nVidia Miner\ccminer.exe",
                        Arguments = $@"-a groestl -o stratum+tcp://moria.dwarfpool.com:3345 -u {args.Argument.ToString()} -p x",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    };
                    process.StartInfo = info;
                    process.OutputDataReceived += ProcessOnOutputDataReceived;
                    process.Start();

                    
                }
            };
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
                var text = dataReceivedEventArgs.ToString();
        }

        private void UpdateOutputWindow() {
            var location = "UpdateOutputWindow";
            //Read in the last 10 lines
            List<string> text = File.ReadLines(MiningOperation.LogFileLocation).Reverse().Take(10).ToList();

        }


        private void BtnStart_OnClick(object sender, RoutedEventArgs e) {
            _minerStarted = !_minerStarted;
            if (_minerStarted) {
                BtnStart.Content = "Stop Mining";
                var addr = string.IsNullOrEmpty(TxtAddress.Text) ? "Fjp6rPKmdhM3vhJ6nFm5LQPed7kyn62wPY" : TxtAddress.Text;

                if (UxCpuTgl.IsChecked == true) {
                    _cpuBg.RunWorkerAsync(addr);
                }
                if (uxnAMDRb.IsChecked == true) {
                    _amdBg.RunWorkerAsync(addr);
                }
                if (uxnVidiaRb.IsChecked == true) {
                    _nVidiaBg.RunWorkerAsync(addr);
                }
            ProgressBar.IsIndeterminate = true;
            }
            else {
                BtnStart.Content = "Start Mining";
                ProgressBar.IsIndeterminate = false;
            }
        }

        protected void Timer_Tick(object sender, EventArgs e) {
            string location = "Timer_Tick";
            try {
                UpdateOutputWindow();
                timer.Start();
            }
            catch (Exception ex) {
                //ToDO: Build error message
            }
        }

        private void AddressRadioButtons_OnChecked(object sender, RoutedEventArgs e) {
            try {
                if (RbUseElectrum.IsChecked == true) {
                    //Check if wallet is set up
                    Process.Start(".\address.vbs");
                    while (MiningOperation.WalletAddress == "") {
                        Thread.Sleep(500);
                    }
                    if (MiningOperation.IsWalletSetup) {
                        TxtAddress.Text = MiningOperation.WalletAddress;
                        TxtAddress.IsReadOnly = true;
                    }
                }
                if (RbCustomAddress.IsChecked != true) return;
                TxtAddress.IsReadOnly = false;
                TxtAddress.Text = HelperLogic.GetSetting("Easyminer", "Settings", "CustomAddress", "");
                TxtUsername.Text = HelperLogic.GetSetting("Easyminer", "Settings", "CustomUsername", "");
                TxtPassword.Text = HelperLogic.GetSetting("Easyminer", "Settings", "CustomPassword", "");
            }
            catch (Exception ex) {
                //ToDo: Throw exception
            }
        }


        private static void Watch() {
            var watch = new FileSystemWatcher();
            watch.Path = @"D:\tmp";
            watch.Filter = "file.txt";
            watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite; //more options
            watch.Changed += new FileSystemEventHandler(OnChanged);
            watch.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e) {
            if (e.FullPath == @"D:\tmp\file.txt") {
                // do stuff
            }
        }

        private void BtnWalletSetup_OnClick(object sender, RoutedEventArgs e) {
            var strLocation = "btnWalletSetup_Click";
            try {
                if (!File.Exists(MiningOperation.ElectrumPath)) {
                    MessageBox.Show("Electrum was not found.", ProgramTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else {
                    Process.Start(MiningOperation.ElectrumPath);
                }
            }
            catch {
                //ToDo: Stuff
            }
        }
    }
}
