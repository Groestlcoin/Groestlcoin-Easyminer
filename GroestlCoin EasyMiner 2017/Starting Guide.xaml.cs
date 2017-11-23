using System;
using System.IO;
using System.Text;
using System.Windows;
using GroestlCoin_EasyMiner_2017.Business_Logic;
using GroestlCoin_EasyMiner_2017.Properties;

namespace GroestlCoin_EasyMiner_2017 {
    /// <summary>
    /// Interaction logic for Starting_Guide.xaml
    /// </summary>
    public partial class StartingGuide {
        public StartingGuide() {
            InitializeComponent();
            if (!Settings.Default.FirstLaunch) {
                ShowMainWindow();
                Close();
            }
            Populate();
        }

        private void Populate() {
            if (File.Exists(MiningOperations.WalletFolder)) {
                uxStepContent.Text =
                    "GroeslMiner has detected your receiving address from your default Electrum-GRS wallet. You can change this at any time if required." + Environment.NewLine + Environment.NewLine + "Receiving Address: " + MiningOperations.GetAddress();
                uxCheckInstallBtn.Visibility = Visibility.Collapsed;
            }
            else {
                StringBuilder sb = new StringBuilder();
                sb.Append("GroestlMiner will automatically detect a receiving address from your Electrum-GRS Wallet." + Environment.NewLine);
                sb.Append("Download the Electrum-GRS Wallet from here: https://www.groestlcoin.org/groestlcoin-electrum-wallet/" + Environment.NewLine);
                sb.Append(
                    "Install the Electrum-GRS Wallet. Once it is installed, click 'Check Electrum Install'. This should find your receiving address.");
                uxStepContent.Text = sb.ToString();
                uxCheckInstallBtn.Visibility = Visibility.Visible;
            }


            if (MiningOperations.HasNVidia) {
                uxHardwareTxt.Text = "Setup has detected that you are using an nVidia graphics card. This will be automatically set. If this is wrong, please change before starting to mine.";
            }
            else if (MiningOperations.HasAmd) {
                uxHardwareTxt.Text = "Setup has detected that you are using an AMD graphics card. This will be automatically set. If this is wrong, please change before starting to mine.";
            }
            else {
                uxHardwareTxt.Text = "Setup has not detected any graphics card. CPU mining will be automatically set. If this is wrong, please change before starting to mine.";
            }
        }

        private void UxContinueBtn_OnClick(object sender, RoutedEventArgs e) {
            if (Settings.Default.FirstLaunch) {
                ShowMainWindow();
            }
            Close();
        }

        private void ShowMainWindow() {
            //Set defaults for GPUs
            if (Settings.Default.FirstLaunch) {
                if (MiningOperations.HasNVidia) {
                    Settings.Default.GPUMining = (byte)MiningOperations.GpuMiningSettings.NVidia;
                }
                else if (MiningOperations.HasAmd) {
                    Settings.Default.GPUMining = (byte)MiningOperations.GpuMiningSettings.Amd;
                }
                else {
                    Settings.Default.CPUMining = true;
                    Settings.Default.GPUMining = (byte)MiningOperations.GpuMiningSettings.None;
                }
                Settings.Default.GrsWalletAddress = MiningOperations.GetAddress();
                Settings.Default.FirstLaunch = false;
                Settings.Default.Save();
            }
            MainWindow main = new MainWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            main.Show();
        }

        private void UxCheckInstallBtn_OnClick(object sender, RoutedEventArgs e) {
            Populate();
        }
    }
}
