using GroestlCoin_EasyMiner_2017.Properties;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows;
using System;
using GroestlCoin_EasyMiner_2017.Business_Logic;

namespace GroestlCoin_EasyMiner_2017 {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public DialogResult Result;
        private readonly BackgroundWorker _amdBg = new BackgroundWorker();
        private readonly BackgroundWorker _cpuBg = new BackgroundWorker();
        private readonly BackgroundWorker _nVidiaBg = new BackgroundWorker();
        private bool _minerStarted;

        public MainWindow() {
            InitializeComponent();
            UxIntensityPopupText.Text = "Maximum Intensity Should be around 20.\nLower this if you still want to use your PC.";

            ConfigureBackgroundWorkers();
            PopulatePage();
        }

        public event EventHandler CpuMinerClosed;

        public event EventHandler GpuMinerClosed;

        protected virtual void OnCpuMinerClosed(EventArgs e) {
            if (CpuMinerClosed != null) {
                MiningOperations.CpuStarted = false;
            }
            if (MiningOperations.CpuStarted || MiningOperations.GpuStarted) return;
            if (_minerStarted) {
                BtnStart_OnClick(null, null);
            }
        }

        protected virtual void OnGpuMinerClosed(EventArgs e) {
            if (GpuMinerClosed != null) {
                MiningOperations.GpuStarted = false;
            }
            if (MiningOperations.CpuStarted || MiningOperations.GpuStarted) return;
            if (_minerStarted)
            {
                BtnStart_OnClick(null, null);
            }
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e) {
            _minerStarted = !_minerStarted;
            if (_minerStarted) {
                List<string> errors;
                if (!ValidateSettings(out errors)) {
                    MessageBox.Show(this,
                        $"Unable to start miner, please rectify the following issues and try again:{Environment.NewLine + string.Join(Environment.NewLine, errors)} ");
                    return;
                }
                SaveSettings();

                BtnStart.Content = "Stop Mining";

                UxLogsExpander.Visibility = Visibility.Visible;

                UxCpuTgl.IsEnabled = false;
                uxnVidiaRb.IsEnabled = false;
                uxnAMDRb.IsEnabled = false;
                TxtAddress.IsEnabled = false;
                TxtPool.IsEnabled = false;
                TxtUsername.IsEnabled = false;
                TxtPassword.IsEnabled = false;
                UxDonationMinutesTxt.IsEnabled = false;
                UxFundSwitchRb.IsEnabled = false;
                UxIntensityTxt.IsEnabled = false;

                UxAdvancedSettings.IsExpanded = false;
                UxLogsExpander.IsExpanded = true;

                if (UxCpuTgl.IsChecked == true) {
                    _cpuBg.RunWorkerAsync();
                    uxCpuMiningLogGroup.Visibility = Visibility.Visible;
                }
                if (uxnAMDRb.IsChecked == true) {
                    _amdBg.RunWorkerAsync();
                    uxGpuMiningLog.Visibility = Visibility.Visible;
                }
                if (uxnVidiaRb.IsChecked == true) {
                    _nVidiaBg.RunWorkerAsync();
                    uxGpuMiningLog.Visibility = Visibility.Visible;
                }
                ProgressBar.IsIndeterminate = true;
            }
            else {
                BtnStart.Content = "Start Mining";

                var processes = Process.GetProcessesByName("minerd");
                foreach (var process in processes) {
                    process.Kill();
                }
                processes = Process.GetProcessesByName("ccminer");
                foreach (var process in processes) {
                    process.Kill();
                }
                processes = Process.GetProcessesByName("sgminer");
                foreach (var process in processes) {
                    process.Kill();
                }

                UxCpuTgl.IsEnabled = true;
                uxnVidiaRb.IsEnabled = true;
                uxnAMDRb.IsEnabled = true;
                TxtAddress.IsEnabled = true;
                TxtPool.IsEnabled = true;
                TxtUsername.IsEnabled = true;
                TxtPassword.IsEnabled = true;
                UxDonationMinutesTxt.IsEnabled = true;
                UxFundSwitchRb.IsEnabled = true;
                UxIntensityTxt.IsEnabled = true;

                UxLogsExpander.IsExpanded = false;
                ProgressBar.IsIndeterminate = false;
            }
        }

        private void ConfigureBackgroundWorkers() {
            var executingAssembly = Directory.GetCurrentDirectory();
            _cpuBg.WorkerSupportsCancellation = true;
            _nVidiaBg.WorkerSupportsCancellation = true;
            _amdBg.WorkerSupportsCancellation = true;



            #region Complete Tasks

            _cpuBg.DoWork += (sender, args) => {
                if (_cpuBg.CancellationPending) {
                    args.Cancel = true;
                    return;
                }
                if (File.Exists(executingAssembly + @"\Resources\Miners\CPU Miner\minerd.exe"))
                {
                    using (var process = new Process()) {
                        ProcessStartInfo info = new ProcessStartInfo {
                            FileName = "cmd.exe",
                            Arguments = "/C " + "\"" + executingAssembly + @"\Resources\Miners\CPU Miner\minerd.exe" + "\"" + $@" -a groestl -o stratum+tcp://{MiningOperations.MiningPoolAddress} -u {MiningOperations.MiningPoolUsername} -p {MiningOperations.MiningPoolPassword} -i {MiningOperations.MiningIntensity}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        process.StartInfo = info;
                        process.EnableRaisingEvents = true;
                        process.ErrorDataReceived += (o, eventArgs) => Dispatcher.Invoke(() => {
                            uxCpuLog.Text += eventArgs.Data + Environment.NewLine;
                            uxCpuScroller.ScrollToVerticalOffset(uxCpuScroller.ExtentHeight);
                        }
                        );
                        process.Start();
                        process.BeginErrorReadLine();
                        MiningOperations.CpuStarted = true;
                        process.WaitForExit();
                        Dispatcher.Invoke(() => OnCpuMinerClosed(new EventArgs()));
                    }
                }
            };

            _amdBg.DoWork += (sender, args) => {
                if (_amdBg.CancellationPending) {
                    args.Cancel = true;
                    return;
                }

                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = executingAssembly + @"\Resources\Miners\AMD Miner\sgminer.exe",
                        Arguments = $"-I {MiningOperations.MiningIntensity} -g 4 -w 64 -k groestlcoin --no-submit-stale -o stratum+tcp://{MiningOperations.MiningPoolAddress} -u {MiningOperations.MiningPoolUsername} -p {MiningOperations.MiningPoolPassword} ",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    process.StartInfo = info;
                    process.EnableRaisingEvents = true;
                    process.ErrorDataReceived += (o, eventArgs) => Dispatcher.Invoke(() => {
                        uxGpuLog.Text += eventArgs.Data + Environment.NewLine;
                        uxGpuScroller.ScrollToVerticalOffset(uxGpuScroller.ExtentHeight);
                    }
                    );
                    process.Start();
                    MiningOperations.GpuStarted = true;
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    Dispatcher.Invoke(() => OnGpuMinerClosed(new EventArgs()));
                }
            };

            _nVidiaBg.DoWork += (sender, args) => {
                if (_nVidiaBg.CancellationPending) {
                    args.Cancel = true;
                    return;
                }
                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = "cmd.exe",
                        Arguments = "/C " + "\""+ executingAssembly + @"\Resources\Miners\nVidia Miner\ccminer.exe"  + "\"" + $@" -a groestl -o stratum+tcp://{MiningOperations.MiningPoolAddress} -u {MiningOperations.MiningPoolUsername} -p {MiningOperations.MiningPoolPassword} -i {MiningOperations.MiningIntensity}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = false,
                        UseShellExecute = true
                    };
                    process.StartInfo = info;
                    process.EnableRaisingEvents = true;
                    process.OutputDataReceived += (o, eventArgs) => Dispatcher.Invoke(() => {
                        uxGpuLog.Text += eventArgs.Data + Environment.NewLine;
                        uxGpuScroller.ScrollToVerticalOffset(uxGpuScroller.ExtentHeight);
                    }
                    );
                    process.Start();
                    MiningOperations.GpuStarted = true;
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    Dispatcher.Invoke(() => OnGpuMinerClosed(new EventArgs()));
                }
            };
        }

        #endregion Complete Tasks

        private void PoolRadioOptions(object sender, RoutedEventArgs e) {
            if (RbUsedwarfPool == null || RbCustomPool == null) return;
            if (RbUsedwarfPool.IsChecked == true) {
                WpCustom1.Visibility = Visibility.Collapsed;
                WpCustom2.Visibility = Visibility.Collapsed;
            }
            if (RbCustomPool.IsChecked != true) return;
            WpCustom1.Visibility = Visibility.Visible;
            WpCustom2.Visibility = Visibility.Visible;
        }

        private void PopulatePage() {
            TxtAddress.Text = string.IsNullOrEmpty(Settings.Default.GrsWalletAddress) ? MiningOperations.GetAddress() : Settings.Default.GrsWalletAddress;
            RbUsedwarfPool.IsChecked = Settings.Default.UseDwarfPool;
            TxtPool.Text = Settings.Default.MiningPoolAddress;
            TxtUsername.Text = Settings.Default.MiningPoolUsername;
            TxtPassword.Text = Settings.Default.MiningPoolPassword;
            UxIntensityTxt.Text = Settings.Default.MineIntensity.ToString();
            UxDonationMinutesTxt.Text = Settings.Default.DonateDuration.ToString();
            UxFundSwitchRb.IsChecked = Settings.Default.DonationFund;
            UxCpuTgl.IsChecked = Settings.Default.CPUMining;
            uxnVidiaRb.IsChecked = (MiningOperations.GpuMiningSettings)Settings.Default.GPUMining == MiningOperations.GpuMiningSettings.NVidia;
            uxnAMDRb.IsChecked = (MiningOperations.GpuMiningSettings)Settings.Default.GPUMining == MiningOperations.GpuMiningSettings.Amd;
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
                Settings.Default.GPUMining = (byte)MiningOperations.GpuMiningSettings.NVidia;
            }
            else if (uxnAMDRb.IsChecked == true) {
                Settings.Default.GPUMining = (byte)MiningOperations.GpuMiningSettings.Amd;
            }
            else {
                Settings.Default.GPUMining = (byte)MiningOperations.GpuMiningSettings.None;
            }
            Settings.Default.Save();
        }

        private void UxAdvancedSettings_OnExpanded(object sender, RoutedEventArgs e) {
            UxLogsExpander.IsExpanded = false;
        }

        private void UxAmdRb_OnChecked(object sender, RoutedEventArgs e) {
            if (uxnVidiaRb.IsChecked != true) return;
            uxnVidiaRb.Checked -= UxnVidiaRb_OnChecked;
            uxnVidiaRb.IsChecked = false;
            uxnVidiaRb.Checked += UxnVidiaRb_OnChecked;
        }

        private void UxGetWalletAddressTxt_Click(object sender, RoutedEventArgs e) {
            if (!MiningOperations.WalletFileExists) {
                StartingGuide guide = new StartingGuide {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                };
                guide.ShowDialog();
            }
            else {
                TxtAddress.Text = MiningOperations.GetAddress();
                MessageBox.Show("Address Updated");
            }
        }

        private void UxIntensityHelp_OnMouseEnter(object sender, MouseEventArgs e) {
            UxIntensityPopup.IsOpen = true;
        }

        private void UxIntensityHelp_OnMouseLeave(object sender, MouseEventArgs e) {
            UxIntensityPopup.IsOpen = false;
        }

        private void UxLogsExpander_OnExpanded(object sender, RoutedEventArgs e) {
            UxAdvancedSettings.IsExpanded = false;
        }

        private void UxnVidiaRb_OnChecked(object sender, RoutedEventArgs e) {
            if (uxnAMDRb.IsChecked != true) return;
            uxnAMDRb.Checked -= UxAmdRb_OnChecked;
            uxnAMDRb.IsChecked = false;
            uxnAMDRb.Checked += UxAmdRb_OnChecked;
        }

        private bool ValidateSettings(out List<string> errors) {
            errors = new List<string>();

            if (string.IsNullOrEmpty(TxtAddress.Text)) {
                errors.Add("Please specify an address before starting to mine.");
            }
            if (RbUsedwarfPool.IsChecked != false) return !errors.Any();
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
            return !errors.Any();
        }
    }
}