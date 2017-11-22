using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BL_EasyMiner.Helper;
using GroestlCoin_EasyMiner_2017.Properties;

namespace GroestlCoin_EasyMiner_2017 {
    /// <summary>
    /// Interaction logic for Starting_Guide.xaml
    /// </summary>
    public partial class StartingGuide : Window {
        public StartingGuide() {
            InitializeComponent();
            if (!Settings.Default.FirstLaunch) {
                ShowMainWindow();
                Close();
            }
            Populate();
        }

        private void Populate() {
            if (File.Exists(MiningOperation.WalletFolder)) {
                uxStepContent.Text =
                    "GroeslMiner has detected your receiving address from your default Electrum-GRS wallet.";
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
        }

        private void UxContinueBtn_OnClick(object sender, RoutedEventArgs e) {
            if (Settings.Default.FirstLaunch) {
                ShowMainWindow();
            }

            Close();
        }

        private void ShowMainWindow() {
            Settings.Default.FirstLaunch = true;
            Settings.Default.Save();
            MainWindow main = new MainWindow();
            main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            main.Show();
        }

        private void UxCheckInstallBtn_OnClick(object sender, RoutedEventArgs e) {
            Populate();
        }
    }
}
