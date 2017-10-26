using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BL_EasyMiner;
using BL_EasyMiner.Helper;
using GroestlCoin_EasyMiner_2017.Properties;
using Microsoft.Win32;

namespace GroestlCoin_EasyMiner_2017
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ProgramTitle = "GroestlCoin Easyminer 2017";


        public MainWindow()
        {
            InitializeComponent();
            MiningOperation.SetMinerMath(System.Reflection.Assembly.GetExecutingAssembly().CodeBase + @"\Scripts\");
            try
            {
                if (HelperLogic.RegAddress.Length > 0)
                {
                    RbCustomPool.IsChecked = true;
                    TxtAddress.Text = HelperLogic.RegAddress;
                }
                else
                {
                    RbUseElectrum.IsChecked = true;
                }
                if (HelperLogic.RegPool.Length > 0)
                {
                    RbCustomPool.IsChecked = true;
                    TxtPool.Text = HelperLogic.RegPool;
                    TxtUsername.Text = HelperLogic.RegUserName;
                    TxtPassword.Text = HelperLogic.RegPassword;
                }
                else
                {
                    RbUsedwarfPool.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                //Todo: Do something here
            }
        }



    }
}
