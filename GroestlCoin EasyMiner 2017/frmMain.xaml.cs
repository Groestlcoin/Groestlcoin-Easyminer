using GroestlCoin_EasyMiner_2018.Business_Logic;
using GroestlCoin_EasyMiner_2018.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace GroestlCoin_EasyMiner_2018 {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        #region Public Fields

        public DialogResult Result;

        #endregion Public Fields

        #region Private Fields

        private readonly BackgroundWorker _amdBg = new BackgroundWorker();
        private readonly BackgroundWorker _cpuBg = new BackgroundWorker();
        private readonly BackgroundWorker _nVidiaBg = new BackgroundWorker();
        private bool _minerStarted;

        #endregion Private Fields

        #region Public Constructors

        public MainWindow() {
            InitializeComponent();
            UxIntensityPopupText.Text = "Select lower values if you still want to use your PC.\nRaise the intensity if you are idle. (GPU Only). Values above 20 may be unstable.\nSelect 'Auto' for the miner to auto-select the best intensity.";

            _cpuBg.WorkerSupportsCancellation = true;
            _nVidiaBg.WorkerSupportsCancellation = true;
            _amdBg.WorkerSupportsCancellation = true;
            _cpuBg.DoWork += OnCpuBgOnDoWork;
            _amdBg.DoWork += OnAmdBgOnDoWork;
            _nVidiaBg.DoWork += OnNVidiaBgOnDoWork;

            Height = 450;
            Width = 580;
            WindowState = WindowState.Normal;

            PopulatePage();
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CpuMinerClosed;

        public event EventHandler GpuMinerClosed;

        #endregion Public Events

        #region Protected Methods

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
            MiningOperations.GpuStarted = false;
            if (MiningOperations.CpuStarted || MiningOperations.GpuStarted) return;
            if (_minerStarted) {
                BtnStart_OnClick(null, null);
            }
        }

        #endregion Protected Methods

        #region Private Methods

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
                uxIntervalSlider.IsEnabled = false;
                uxnVidiaRb.IsEnabled = false;
                uxnAMDRb.IsEnabled = false;
                TxtAddress.IsEnabled = false;
                TxtPool.IsEnabled = false;
                TxtUsername.IsEnabled = false;
                TxtPassword.IsEnabled = false;
                UxIntensityTxt.IsEnabled = false;
                uxPoolSelectorDdl.IsEnabled = false;
                UxAddressLabel.IsEnabled = false;
                GpuIntensityLbl.IsEnabled = false;
                UxIntensityHelp.Opacity = 0.56;
                ProgressBar.Visibility = Visibility.Visible;

                UxStandardSettings.IsExpanded = true;
                UxAdvancedSettings.IsExpanded = false;
                UxLogsExpander.IsExpanded = true;
                UxGetWalletAddressTxt.IsEnabled = false;

                if (UxCpuTgl.IsChecked == true) {
                    _cpuBg.RunWorkerAsync();
                    uxCpuMiningLogGroup.Visibility = Visibility.Visible;
                }
                if (uxnAMDRb.IsChecked == true) {
                    _amdBg.RunWorkerAsync();
                    uxGpuMiningLog.Visibility = Visibility.Collapsed;
                }
                if (uxnVidiaRb.IsChecked == true) {
                    _nVidiaBg.RunWorkerAsync();
                    uxGpuMiningLog.Visibility = Visibility.Visible;
                }
                ProgressBar.IsIndeterminate = true;
            }
            else {
                BtnStart.Content = "Start Mining";

                KillProcesses();

                uxIntervalSlider.IsEnabled = true;
                UxCpuTgl.IsEnabled = true;
                uxnVidiaRb.IsEnabled = true;
                uxnAMDRb.IsEnabled = true;
                TxtAddress.IsEnabled = true;
                TxtPool.IsEnabled = true;
                TxtUsername.IsEnabled = true;
                TxtPassword.IsEnabled = true;
                UxIntensityTxt.IsEnabled = true;
                uxPoolSelectorDdl.IsEnabled = true;
                UxAddressLabel.IsEnabled = true;
                GpuIntensityLbl.IsEnabled = true;
                UxGetWalletAddressTxt.IsEnabled = true;
                UxIntensityHelp.Opacity = 1;

                UxLogsExpander.IsExpanded = false;
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void KillProcesses() {
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
        }

        private void OnAmdBgOnDoWork(object sender, DoWorkEventArgs args) {
            if (_amdBg.CancellationPending) {
                args.Cancel = true;
                return;
            }
            #region Surrounding the Directory Path in Quotes
            var path = MiningOperations.AMDDirectory;
            var folderNames = path.Split('\\');

            folderNames = folderNames.Select(fn => (fn.Contains(' ')) ? $"\"{fn}\"" : fn)
                .ToArray();
            #endregion

            var fullPathWithQuotes = string.Join("\\", folderNames);

            using (var process = new Process()) {
                var commands = MiningOperations.GetAMDCommandLine(MiningOperations.CommonMiningPoolVariables, MiningOperations.UseAutoIntensity, MiningOperations.MiningIntensity.ToString(), "groestlcoin");

                ProcessStartInfo info = new ProcessStartInfo {
                    FileName = "cmd.exe",
                    Arguments = $"/C {fullPathWithQuotes} -g 4 -w 64 -k groestlcoin {commands}",
                    //  RedirectStandardOutput = true,
                    //  RedirectStandardError = true,
                    CreateNoWindow = false,
                    UseShellExecute = false
                };
                process.StartInfo = info;
                process.EnableRaisingEvents = true;

                #region Writing to local window

                //process.OutputDataReceived += (o, eventArgs) => {
                //    try {
                //        Dispatcher.Invoke(() => {
                //            var lines =
                //                uxGpuLog.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                //                    .ToList();

                //            if (lines.Count() == 30) {
                //                lines.RemoveAt(0);
                //            }
                //            lines.Add(eventArgs.Data);
                //            uxGpuLog.Text = string.Join(Environment.NewLine, lines);

                //            uxGpuScroller.ScrollToVerticalOffset(uxGpuScroller.ExtentHeight);
                //        });
                //    }
                //                        catch (Exception e) {
                //#if DEBUG
                //                            MessageBox.Show("Errr: " + e.Message);
                //                            //Do Nothing
                //#endif
                //                        }
                //                    };
                //                    process.ErrorDataReceived += (o, eventArgs) => {
                //                        try {
                //                            Dispatcher.Invoke(() => {
                //                                var lines = uxGpuLog.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                //                                if (lines.Count() == 30) {
                //                                    lines.RemoveAt(0);
                //                                }
                //                                lines.Add(eventArgs.Data);
                //                                uxGpuLog.Text = string.Join(Environment.NewLine, lines);

                //                                uxGpuScroller.ScrollToVerticalOffset(uxGpuScroller.ExtentHeight);
                //                            });
                //                        }
                //                        catch (Exception e) {
                //#if DEBUG
                //                            MessageBox.Show("Errr: " + e.Message);
                //                            //Do Nothing
                //#endif
                //                        }
                //                    };

                #endregion Writing to local window

                process.Start();
                MiningOperations.GpuStarted = true;
                //    process.BeginErrorReadLine();
                process.WaitForExit();
                Dispatcher.Invoke(() => OnGpuMinerClosed(new EventArgs()));
            }
        }

        private void OnCpuBgOnDoWork(object sender, DoWorkEventArgs args) {
            if (_cpuBg.CancellationPending) {
                args.Cancel = true;
                return;
            }
            if (File.Exists(MiningOperations.CpuDirectory)) {
                var commands = MiningOperations.GetCPUCommandLine(MiningOperations.CommonMiningPoolVariables);

                using (var process = new Process()) {
                    ProcessStartInfo info = new ProcessStartInfo {
                        FileName = "cmd.exe",
                        Arguments = "/C " + "\"" + MiningOperations.CpuDirectory + "\"" + $" -a groestl {commands}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    process.StartInfo = info;
                    process.EnableRaisingEvents = true;
                    process.ErrorDataReceived += (o, eventArgs) => {
                        try {
                            Dispatcher.Invoke(() => {
                                var lines = uxCpuLog.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                    .ToList();

                                if (lines.Count() == 30) {
                                    lines.RemoveAt(0);
                                }
                                lines.Add(eventArgs.Data);
                                uxCpuLog.Text = string.Join(Environment.NewLine, lines);

                                uxCpuScroller.ScrollToVerticalOffset(uxGpuScroller.ExtentHeight);
                            });
                        }
                        catch (Exception ex) {
                            MessageBox.Show("Error " + ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                        }
                    };
                    process.Start();
                    process.BeginErrorReadLine();
                    MiningOperations.CpuStarted = true;
                    process.WaitForExit();
                    Dispatcher.Invoke(() => OnCpuMinerClosed(new EventArgs()));
                }
            }
            else {
                Dispatcher.Invoke(() => {
                    MessageBox.Show("minerd.exe file not found. Please check your antivirus settings, re-run the installer and select repair");
                    OnCpuMinerClosed(new EventArgs());
                });
            }
        }

        private void OnNVidiaBgOnDoWork(object sender, DoWorkEventArgs args) {
            if (_nVidiaBg.CancellationPending) {
                args.Cancel = true;
                return;
            }

            var commands = MiningOperations.GetNVidiaCommandLine(MiningOperations.CommonMiningPoolVariables, MiningOperations.UseAutoIntensity, MiningOperations.MiningIntensity.ToString());

            using (var process = new Process()) {
                ProcessStartInfo info = new ProcessStartInfo {
                    FileName = "cmd.exe",
                    Arguments = "/C " + "\"" + MiningOperations.NVididiaDirectory + "\"" + $" -a groestl {commands}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                process.StartInfo = info;
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += (o, eventArgs) => {
                    try {
                        Dispatcher.Invoke(() => {
                            var lines = uxGpuLog.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                            if (lines.Count() == 30) {
                                lines.RemoveAt(0);
                            }
                            lines.Add(eventArgs.Data);
                            uxGpuLog.Text = string.Join(Environment.NewLine, lines);

                            uxGpuScroller.ScrollToVerticalOffset(uxGpuScroller.ExtentHeight);
                        });
                    }
                    catch (Exception ex) {
                        MessageBox.Show("Error " + ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                    }
                };
                process.Start();
                MiningOperations.GpuStarted = true;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                try {
                    Dispatcher.Invoke(() => OnGpuMinerClosed(new EventArgs()));
                }
                catch (Exception ex) {
                    MessageBox.Show("Error " + ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private void PoolRadioOptions(object sender, RoutedEventArgs e) {
            switch ((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex) {
                case MiningOperations.MiningPools.Dwarfpool:

                    break;
                case MiningOperations.MiningPools.Suprnova:
                    break;
                case MiningOperations.MiningPools.MiningPoolHub:
                    break;
                case MiningOperations.MiningPools.P2Pool:
                    break;
                case MiningOperations.MiningPools.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //if (RbUsedwarfPool == null || RbCustomPool == null) return;
            //if (RbUsedwarfPool.IsChecked == true) {
            //    uxViewdwarfPool.Visibility = Visibility.Visible;
            //    WpCustom1.Visibility = Visibility.Collapsed;
            //    WpCustom2.Visibility = Visibility.Collapsed;
            //    WpCustom3.Visibility = Visibility.Collapsed;

            //}
            //if (RbCustomPool.IsChecked != true) return;
            //WpCustom1.Visibility = Visibility.Visible;
            //WpCustom2.Visibility = Visibility.Visible;
            //if (uxnAMDRb.IsChecked == true) {
            //    WpCustom3.Visibility = Visibility.Visible;
            //}
            //uxViewdwarfPool.Visibility = Visibility.Collapsed;
        }

        private void PopulatePage() {
            if (Debugger.IsAttached) {
                Settings.Default.P2PoolSettings = null;
                Settings.Default.CustomSettings = null;
                Settings.Default.MiningPoolHubSettings = null;
                Settings.Default.SuprNovaSettings = null;
            }


            uxIntervalSlider.Value = Settings.Default.MineIntensity;
            TxtAddress.Text = string.IsNullOrEmpty(Settings.Default.GrsWalletAddress) ? MiningOperations.GetAddress() : Settings.Default.GrsWalletAddress;
            uxPoolSelectorDdl.SelectedIndex = Settings.Default.SelectedMiningPool;
            TxtPool.Text = MiningOperations.GetAddressForPool((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex);
            TxtUsername.Text = Settings.Default.MiningPoolUsername;
            TxtPassword.Text = Settings.Default.MiningPoolPassword;
            uxAutoIntensityChk.IsChecked = Settings.Default.UseAutoIntensity;
            UxIntensityTxt.Text = Settings.Default.MineIntensity.ToString();
            UxCpuTgl.IsChecked = Settings.Default.CPUMining;
            uxnVidiaRb.IsChecked = (MiningOperations.GpuMiningSettings)Settings.Default.GPUMining == MiningOperations.GpuMiningSettings.NVidia;
            uxnAMDRb.IsChecked = (MiningOperations.GpuMiningSettings)Settings.Default.GPUMining == MiningOperations.GpuMiningSettings.Amd;


            WpCustom3.Visibility = uxnAMDRb.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

            if (uxAutoIntensityChk.IsChecked == true) {
                UxIntensityTxt.Visibility = Visibility.Collapsed;
                uxIntervalSlider.Visibility = Visibility.Collapsed;
            }
            else {
                UxIntensityTxt.Visibility = Visibility.Visible;
                uxIntervalSlider.Visibility = Visibility.Visible;
            }

            if (Settings.Default.P2PoolSettings == null) {
                Settings.Default.P2PoolSettings = new StringCollection {
                    MiningOperations.GetAddressForPool(MiningOperations.MiningPools.P2Pool),
                    MiningOperations.GetUsernameForPool(MiningOperations.MiningPools.P2Pool),
                    MiningOperations.GetPasswordForPool(MiningOperations.MiningPools.P2Pool)
                    };
            }
            if (Settings.Default.CustomSettings == null) {
                Settings.Default.CustomSettings = new StringCollection {
                    MiningOperations.GetAddressForPool(MiningOperations.MiningPools.Custom),
                    MiningOperations.GetUsernameForPool(MiningOperations.MiningPools.Custom),
                    MiningOperations.GetPasswordForPool(MiningOperations.MiningPools.Custom)
                 };
            }
            if (Settings.Default.MiningPoolHubSettings == null) {
                Settings.Default.MiningPoolHubSettings = new StringCollection {
                    MiningOperations.GetAddressForPool(MiningOperations.MiningPools.MiningPoolHub),
                    MiningOperations.GetUsernameForPool(MiningOperations.MiningPools.MiningPoolHub),
                    MiningOperations.GetPasswordForPool(MiningOperations.MiningPools.MiningPoolHub)
                 };
            }
            if (Settings.Default.SuprNovaSettings == null) {
                Settings.Default.SuprNovaSettings = new StringCollection {
                    MiningOperations.GetAddressForPool(MiningOperations.MiningPools.Suprnova),
                    MiningOperations.GetUsernameForPool(MiningOperations.MiningPools.Suprnova),
                    MiningOperations.GetPasswordForPool(MiningOperations.MiningPools.Suprnova)
                };
            }
            Settings.Default.Save();
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (UxIntensityTxt != null) {
                UxIntensityTxt.Text = e.NewValue.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void SaveSettings() {
            Settings.Default.GrsWalletAddress = TxtAddress.Text;
            Settings.Default.SelectedMiningPool = (byte)uxPoolSelectorDdl.SelectedIndex;

            switch ((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex) {
                case MiningOperations.MiningPools.Dwarfpool:
                    Settings.Default.MiningPoolUsername = TxtUsername.Text;
                    Settings.Default.MiningPoolPassword = TxtPassword.Text;
                    break;
                case MiningOperations.MiningPools.MiningPoolHub:
                    Settings.Default.MiningPoolHubSettings[0] = TxtUsername.Text;
                    Settings.Default.MiningPoolHubSettings[1] = TxtPassword.Text;
                    break;
                case MiningOperations.MiningPools.Suprnova:
                    Settings.Default.SuprNovaSettings[0] = TxtUsername.Text;
                    Settings.Default.SuprNovaSettings[1] = TxtPassword.Text;
                    break;
                case MiningOperations.MiningPools.P2Pool:
                    Settings.Default.P2PoolSettings[0] = TxtPool.Text;
                    Settings.Default.P2PoolSettings[1] = TxtUsername.Text;
                    Settings.Default.P2PoolSettings[2] = TxtPassword.Text;
                    break;
                case MiningOperations.MiningPools.Custom:
                    Settings.Default.CustomSettings[0] = TxtPool.Text;
                    Settings.Default.CustomSettings[1] = TxtUsername.Text;
                    Settings.Default.CustomSettings[2] = TxtPassword.Text;
                    break;
            }
            Settings.Default.MineIntensity = int.Parse(UxIntensityTxt.Text);
            Settings.Default.CPUMining = UxCpuTgl.IsChecked == true;
            Settings.Default.UseAutoIntensity = uxAutoIntensityChk.IsChecked == true;

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

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this?.DragMove();
        }

        private void UxAdvancedSettings_OnExpanded(object sender, RoutedEventArgs e) {
            UxLogsExpander.IsExpanded = false;
            UxStandardSettings.IsExpanded = false;
        }

        private void UxAmdRb_OnChecked(object sender, RoutedEventArgs e) {
            uxnVidiaRb.Checked -= UxnVidiaRb_OnChecked;
            uxnVidiaRb.IsChecked = false;
            uxnVidiaRb.Checked += UxnVidiaRb_OnChecked;
            WpCustom3.Visibility = Visibility.Visible;
        }

        private void UxnAMDRb_OnUnchecked(object sender, RoutedEventArgs e) {
            WpCustom3.Visibility = Visibility.Collapsed;
        }

        private void UxnVidiaRb_OnChecked(object sender, RoutedEventArgs e) {
            uxnAMDRb.Checked -= UxAmdRb_OnChecked;
            uxnAMDRb.IsChecked = false;
            uxnAMDRb.Checked += UxAmdRb_OnChecked;
            WpCustom3.Visibility = Visibility.Collapsed;
        }

        private void UxAutoIntensityChk_OnChecked(object sender, RoutedEventArgs e) {
            uxIntervalSlider.Visibility = Visibility.Collapsed;
            UxIntensityTxt.Visibility = Visibility.Collapsed;
        }

        private void UxAutoIntensityChk_OnUnchecked(object sender, RoutedEventArgs e) {
            UxIntensityTxt.Visibility = Visibility.Visible;
            uxIntervalSlider.Visibility = Visibility.Visible;
        }

        private void UxCpuLog_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            try {
                Clipboard.SetText(uxCpuLog.Text);
                MessageBox.Show(this, "Copied to Clipboard");
            }
            catch {
            }
        }

        private void UxGetWalletAddressTxt_Click(object sender, RoutedEventArgs e) {
            if (!MiningOperations.WalletFileExists) {
                StartingGuide guide = new StartingGuide {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                };
                guide.FromMainWindow = true;
                guide.ShowDialog();
            }
            else {
                if (TxtAddress.Text == MiningOperations.GetAddress()) return;
                var messageBoxResult = MessageBox.Show(this, "Warning: Resetting your mining address will reset your rewards. Are you sure?", "Address Warning", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes) {
                    TxtAddress.Text = MiningOperations.GetAddress();
                }
            }
        }

        private void UxGpuLog_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            try {
                Clipboard.SetText(uxGpuLog.Text);
                MessageBox.Show(this, "Copied to Clipboard");
            }
            catch {
            }
        }

        private void UxIntensityHelp_OnMouseEnter(object sender, MouseEventArgs e) {
            UxIntensityPopup.IsOpen = true;
        }

        private void UxIntensityHelp_OnMouseLeave(object sender, MouseEventArgs e) {
            UxIntensityPopup.IsOpen = false;
        }

        private void UxIntensityTxt_OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UxLogsExpander_OnExpanded(object sender, RoutedEventArgs e) {
            UxAdvancedSettings.IsExpanded = false;
        }

        private void UxStandardSettings_OnExpanded(object sender, RoutedEventArgs e) {
            if (UxAdvancedSettings != null) {
                UxAdvancedSettings.IsExpanded = false;
            }
        }

        private void UxViewDwarfPoolHl_OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private bool ValidateSettings(out List<string> errors) {
            errors = new List<string>();

            if (string.IsNullOrEmpty(TxtAddress.Text)) {
                errors.Add("Please specify an address before starting to mine.");
            }
            if (string.IsNullOrEmpty(TxtPool.Text)) {
                errors.Add("Please specify a mining pool address.");
            }
            if (string.IsNullOrEmpty(TxtUsername.Text)) {
                errors.Add("Please specify a mining pool username.");
            }
            if (string.IsNullOrEmpty(TxtPassword.Text)) {
                errors.Add("Please specify a mining pool password.");
            }
            if (uxnAMDRb.IsChecked == false && uxnVidiaRb.IsChecked == false && UxCpuTgl.IsChecked == false) {
                errors.Add("Please select what to mine with (CPU, AMD / nVidia)");
            }
            return !errors.Any();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            KillProcesses();
        }

        private void UxPoolSelectorDdl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (TxtAddress != null && TxtUsername != null && TxtPassword != null) {
                TxtPool.Text = MiningOperations.GetAddressForPool((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex);
                TxtUsername.Text = MiningOperations.GetUsernameForPool((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex);
                TxtPassword.Text = MiningOperations.GetPasswordForPool((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex);
            }
            SetStatsURL();
        }

        private void SetStatsURL() {
            if (uxViewdwarfPool != null) uxViewdwarfPool.Visibility = Visibility.Visible;

            if (uxViewStatsHl != null) {
                switch ((MiningOperations.MiningPools)uxPoolSelectorDdl.SelectedIndex) {
                    case MiningOperations.MiningPools.Dwarfpool:
                        uxViewStatsHl.NavigateUri = new Uri($"https://dwarfpool.com/grs/address?wallet={TxtUsername?.Text}");
                        TxtPool.IsEnabled = false;
                        break;
                    case MiningOperations.MiningPools.Suprnova:
                        uxViewStatsHl.NavigateUri = new Uri("https://grs.suprnova.cc/index.php?page=dashboard");
                        TxtPool.IsEnabled = false;
                        break;
                    case MiningOperations.MiningPools.MiningPoolHub:
                        uxViewStatsHl.NavigateUri = new Uri("https://groestlcoin.miningpoolhub.com/index.php?page=account&action=pooledit");
                        TxtPool.IsEnabled = false;
                        break;
                    case MiningOperations.MiningPools.P2Pool:
                        TxtPool.IsEnabled = true;
                        var address = TxtPool?.Text.ToLower().Replace("stratum.tcp://", "");
                        if (!string.IsNullOrEmpty(address)) {
                            uxViewStatsHl.NavigateUri = new Uri($"http://{address}/static");
                        }
                        else {
                            uxViewStatsHl.NavigateUri = null;
                        }
                        break;
                    case MiningOperations.MiningPools.Custom:
                        uxViewdwarfPool.Visibility = Visibility.Collapsed;
                        TxtPool.IsEnabled = true;
                        break;
                }
            }
        }

        private void TxtPool_OnTextChanged(object sender, TextChangedEventArgs e) {
            SetStatsURL();
        }

        #endregion Private Methods



    }
}