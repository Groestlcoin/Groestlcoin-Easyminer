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
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Windows.Forms.Timer;

namespace GroestlCoin_EasyMiner_2017 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>d
    public partial class MainWindow : Window {
        public DialogResult Result;
        private const string ProgramTitle = "GroestlCoin Easyminer 2017";
        private readonly BackgroundWorker _amdBg = new BackgroundWorker();
        private readonly BackgroundWorker _cpuBg = new BackgroundWorker();
        private readonly BackgroundWorker _nVidiaBg = new BackgroundWorker();
        private bool _minerStarted = false;
        private Timer ProgressTimer = new Timer();
        private Timer timer;
        public MainWindow() {
            InitializeComponent();
            var executingAssembly = System.IO.Directory.GetCurrentDirectory();
            MiningOperation.MinerExePath = executingAssembly + @"\Scripts\";
            MiningOperation.MiningBatch = executingAssembly + @"\Scripts\StartMining.bat";
            MiningOperation.ElectrumPath = executingAssembly + @"\Prerequisites\Electrum-GRS\electrum-grs.exe";
            MiningOperation.LogFileLocation = executingAssembly + @"\scripts\minelog.txt";


            UxIntensityPopupText.Text =
                 "Maximum Intensity Should be around 20.\nLower this if you still want to use your PC.";

            ConfigureBackgroundWorkers();
            PopulatePage();


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

        private void PopulatePage() {
            TxtAddress.Text = string.IsNullOrEmpty(Settings.Default.GrsWalletAddress) ? MiningOperation.GetAddress() : Settings.Default.GrsWalletAddress;
            RbUsedwarfPool.IsChecked = Properties.Settings.Default.UseDwarfPool;
            TxtPool.Text = Settings.Default.MiningPoolAddress;
            TxtUsername.Text = Settings.Default.MiningPoolUsername;
            TxtPassword.Text = Settings.Default.MiningPoolPassword;
            UxIntensityTxt.Text = Settings.Default.MineIntensity.ToString();
            UxDonationMinutesTxt.Text = Settings.Default.DonateDuration.ToString();
            UxFundSwitchRb.IsChecked = Settings.Default.DonationFund;
            UxCpuTgl.IsChecked = Settings.Default.CPUMining;
            uxnVidiaRb.IsChecked = (HelperLogic.GPUMiningSettings)Settings.Default.GPUMining == HelperLogic.GPUMiningSettings.NVidia;
            uxnAMDRb.IsChecked = (HelperLogic.GPUMiningSettings)Settings.Default.GPUMining == HelperLogic.GPUMiningSettings.Amd;
        }

      
        private void SaveSettings() {
            Settings.Default.GrsWalletAddress = TxtAddress.Text;
            Settings.Default.UseDwarfPool = RbUsedwarfPool.IsChecked == true;
            Settings.Default.MiningPoolAddress = TxtAddress.Text;
            Settings.Default.MiningPoolUsername = TxtUsername.Text;
            Settings.Default.MiningPoolPassword = TxtPassword.Text;
            Settings.Default.MineIntensity = int.Parse(UxIntensityTxt.Text);
            Settings.Default.DonateDuration = int.Parse(UxDonationMinutesTxt.Text);
            Settings.Default.DonationFund = UxFundSwitchRb.IsChecked == true;
            Settings.Default.CPUMining = UxCpuTgl.IsChecked == true;

            if (uxnVidiaRb.IsChecked == true) {
                Settings.Default.GPUMining = (byte)HelperLogic.GPUMiningSettings.NVidia;
            }
            else if (uxnAMDRb.IsChecked == true) {
                Settings.Default.GPUMining = (byte)HelperLogic.GPUMiningSettings.Amd;
            }
            else {
                Settings.Default.GPUMining = (byte)HelperLogic.GPUMiningSettings.None;
            }
            Settings.Default.Save();
        }

        private static void OnChanged(object source, FileSystemEventArgs e) {
            if (e.FullPath == @"D:\tmp\file.txt") {
                // do stuff
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

        private void AddressRadioButtons_OnChecked(object sender, RoutedEventArgs e) {
            try {
                if (RbUseElectrum.IsChecked == true) {
                    ////Check if wallet is set up
                    //Process.Start(".\address.vbs");
                    //while (MiningOperation.WalletAddress == "") {
                    //    Thread.Sleep(500);
                    //}
                    //if (MiningOperation.IsWalletSetup) {
                    //    TxtAddress.Text = MiningOperation.WalletAddress;
                    //    TxtAddress.IsReadOnly = true;
                    //}
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

        private void BtnStart_OnClick(object sender, RoutedEventArgs e) {
            _minerStarted = !_minerStarted;
            if (_minerStarted) {
                List<string> errors = new List<string>();
                if (!ValidateSettings(out errors)) {
                    MessageBox.Show(this,
                        $"Unable to start miner, please rectify the following issues and try again:{Environment.NewLine + string.Join(Environment.NewLine, errors)} ");
                    return;
                }

                SaveSettings();

                BtnStart.Content = "Stop Mining";
                var addr = TxtAddress.Text;

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

        private void ConfigureBackgroundWorkers() {
            var executingAssembly = System.IO.Directory.GetCurrentDirectory();

            _cpuBg.DoWork += (sender, args) => {
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = executingAssembly + @"\Resources\Miners\CPU Miner\minerd.exe",
                        Arguments =
                            $@"-a groestl -o stratum+tcp://moria.dwarfpool.com:3345 -u {args.Argument.ToString().Trim().Replace(Environment.NewLine, " ")} -p x"
                    };
                    process.StartInfo = info;
                    process.Start();
                }
            };

            _amdBg.DoWork += (sender, args) => {
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = executingAssembly + @"\Resources\Miners\AMD Miner\sgminer.exe",
                        Arguments = $"-I 19 -g 4 -w 64 -k groestlcoin --no-submit-stale -o stratum+tcp://moria.dwarfpool.com:3345 -u {args.Argument.ToString().Trim().Replace(Environment.NewLine, " ")} -p x "
                    };
                    process.StartInfo = info;
                    process.Start();
                }
            };

            _nVidiaBg.DoWork += (sender, args) => {
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = executingAssembly + @"\Resources\Miners\nVidia Miner\ccminer.exe",
                        Arguments = $@"-a groestl -o stratum+tcp://moria.dwarfpool.com:3345 -u {args.Argument.ToString().Trim().Replace(Environment.NewLine, " ")} -p x",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    };
                    MessageBox.Show(info.FileName + " " + info.Arguments);
                    process.StartInfo = info;
                    process.Start();


                }
            };
        }

        private void UpdateOutputWindow() {
            var location = "UpdateOutputWindow";
            //Read in the last 10 lines
            List<string> text = File.ReadLines(MiningOperation.LogFileLocation).Reverse().Take(10).ToList();

        }
        private bool ValidateSettings(out List<string> errors) {
            errors = new List<string>();

            if (string.IsNullOrEmpty(TxtAddress.Text)) {
                errors.Add("Please specify an address before starting to mine.");
            }
            if (RbUsedwarfPool.IsChecked == false) {
                if (string.IsNullOrEmpty(TxtAddress.Text)) {
                    errors.Add("Please specify a mining pool address or use Dwarfpool");
                }
                if (string.IsNullOrEmpty(TxtUsername.Text)) {
                    errors.Add("Please specify a mining pool username or use Dwarfpool");
                }
                if (string.IsNullOrEmpty(TxtPassword.Text)) {
                    errors.Add("Please specify a mining pool password or use Dwarfpool");
                }
                if (uxnAMDRb.IsChecked == false && uxnVidiaRb.IsChecked == false && UxCpuTgl.IsChecked == false) {
                    errors.Add("Please select what to mine with (CPU, AMD / nVidia)");
                }
            }
            return !errors.Any();
        }

        private void UxFundSwitchRb_OnChecked(object sender, RoutedEventArgs e) {
            //       throw new NotImplementedException();
        }

        private void UxFundSwitchRb_OnUnchecked(object sender, RoutedEventArgs e) {
            //       throw new NotImplementedException();
        }

        private void UxIntensityHelp_OnMouseEnter(object sender, MouseEventArgs e) {
            UxIntensityPopup.IsOpen = true;
        }

        private void UxIntensityHelp_OnMouseLeave(object sender, MouseEventArgs e) {
            UxIntensityPopup.IsOpen = false;
        }

        private void PoolRadioOptions(object sender, RoutedEventArgs e) {
            if (RbUsedwarfPool != null && RbCustomPool != null) {
                if (RbUsedwarfPool.IsChecked == true) {
                    WpCustom1.Visibility = Visibility.Collapsed;
                    WpCustom2.Visibility = Visibility.Collapsed;
                }
                if (RbCustomPool.IsChecked == true) {
                    WpCustom1.Visibility = Visibility.Visible;
                    WpCustom2.Visibility = Visibility.Visible;
                }
            }

        }

        private void UxAmdRb_OnChecked(object sender, RoutedEventArgs e) {
            if (uxnVidiaRb.IsChecked == true) {
                uxnVidiaRb.Checked -= UxnVidiaRb_OnChecked;
                uxnVidiaRb.IsChecked = false;
                uxnVidiaRb.Checked += UxnVidiaRb_OnChecked;
            }

        }

        private void UxnVidiaRb_OnChecked(object sender, RoutedEventArgs e) {
            if (uxnAMDRb.IsChecked == true) {
                uxnAMDRb.Checked -= UxAmdRb_OnChecked;
                uxnAMDRb.IsChecked = false;
                uxnAMDRb.Checked += UxAmdRb_OnChecked;
            }
        }

        private void UxGetWalletAddressTxt_Click(object sender, RoutedEventArgs e) {
            StartingGuide guide = new StartingGuide
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
            };
            guide.ShowDialog();
        }
    }
}
