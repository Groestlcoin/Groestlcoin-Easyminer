using System;
using System.Collections.Generic;
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

        public MainWindow() {
            InitializeComponent();
            var executingAssembly = System.IO.Directory.GetCurrentDirectory();
            MiningOperation.MinerExePath = executingAssembly + @"\Scripts\";
            MiningOperation.MiningBatch = executingAssembly + @"\Scripts\StartMining.bat";
            MiningOperation.ElectrumPath = executingAssembly + @"\Prerequisites\Electrum-GRS\electrum-grs.exe";
            MiningOperation.LogFileLocation = executingAssembly + @"\scripts\minelog.txt";
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

        private void UpdateOutputWindow() {
            TxtOutput.Document.Blocks.Clear();
            var location = "UpdateOutputWindow";
            //Read in the last 10 lines
            List<string> text = File.ReadLines(MiningOperation.LogFileLocation).Reverse().Take(10).ToList();
            TextRange range = new TextRange(TxtOutput.Document.ContentStart, TxtOutput.Document.ContentEnd);
            range.Text = text.ToString();
        }


        private void BtnStart_OnClick(object sender, RoutedEventArgs e) {
            _minerStarted = !_minerStarted;
            var location = "startMiningBtn_Click " + (_minerStarted ? "started" : "ending");
            try {
                if (_minerStarted) {
                    if (RbCustomPool.IsChecked == true && TxtPool.Text.Length == 0) {
                        MessageBox.Show("You must enter a custom pool address to continue.", ProgramTitle,
                            MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    BtnStart.Content = "Stop Mining";
                    //DisableEnableControls(false);
                    if (RbCustomAddress.IsChecked == true) {
                        HelperLogic.WriteToRegistry(HelperLogic.RegistryValue.CustomAddress, TxtAddress.Text);
                    }
                    else {
                        if (HelperLogic.GetSetting("Easyminer", "Settings", "CustomAddress", "").Length > 0) {
                            Interaction.DeleteSetting("Easyminer", "Settings", "CustomAddress");
                        }
                    }
                    if (RbCustomPool.IsChecked == true) {
                        HelperLogic.WriteToRegistry(HelperLogic.RegistryValue.CustomMiningPool, TxtPool.Text);
                        HelperLogic.WriteToRegistry(HelperLogic.RegistryValue.CustomUsername, TxtUsername.Text);
                        HelperLogic.WriteToRegistry(HelperLogic.RegistryValue.CustomPassword, TxtPassword.Text);
                    }
                    else {
                        Interaction.DeleteSetting("easyminer", "Settings", "CustomMiningPool");
                        Interaction.DeleteSetting("easyminer", "Settings", "CustomUsername");
                        Interaction.DeleteSetting("easyminer", "Settings", "CustomPassword");
                    }
                    TxtOutput.AppendText(Environment.NewLine + "Mining Started at " + DateTime.Now);

                    switch (Result) {
                        case System.Windows.Forms.DialogResult.Yes:
                            if (SystemTypeCheck.InternalCheckIsWow64()) {
                                MiningOperation.MiningMethod = MiningOperation.MiningMethods.GroestlCPU64;
                            }
                            else {
                                MiningOperation.MiningMethod = MiningOperation.MiningMethods.GroestlCPU32;
                            }
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            MiningOperation.MiningMethod = MiningOperation.MiningMethods.GroestlGPU;
                            break;
                    }
                    MiningOperation.WriteBatchFile((bool)RbUsedwarfPool.IsChecked, TxtPool.Text, TxtUsername.Text,
                        TxtPassword.Text, TxtAddress.Text);
                    MiningOperation.StartMiningProcess();
                    timer.Start();
                    ProgressBar.Visibility = Visibility.Visible;
                }
                else {
                    BtnStart.Content = "Start Miner";
                    MiningOperation.KillMiner();
                    ProgressBar.Visibility = Visibility.Hidden;
                    //DisableEnableControls(true);
                }
            }
            catch {
                //ToDo: Build error message
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

        private void BtnWalletSetup_OnClick(object sender, RoutedEventArgs e) {
            var strLocation = "btnWalletSetup_Click";
            try
            {
                if (!File.Exists(MiningOperation.ElectrumPath))
                {
                    MessageBox.Show("Electrum was not found.", ProgramTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else
                {
                    Process.Start(MiningOperation.ElectrumPath);
                }
            }
            catch
            {
                //ToDo: Stuff
            }
        }
    }
}
