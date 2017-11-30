using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GroestlCoin_EasyMiner_2017 {
    /// <summary>
    /// Interaction logic for Titlebar.xaml
    /// </summary>
    public partial class Titlebar : UserControl {
        private bool _isMainWindow = false;

        public bool IsMainWindow {
            get {
                return _isMainWindow;
            }
            set {
                if (value == false) {
                    uxMinimiseBtn.Visibility = Visibility.Hidden;
                    uxMinimiseBtn.IsEnabled = false;
                }
                _isMainWindow = value;
            }
        }
        private bool _isAbout = false;

        public bool IsAbout {
            get {
                return _isAbout;
            }
            set {
                if (value) {
                    uxMinimiseBtn.Visibility = Visibility.Hidden;
                    uxMinimiseBtn.IsEnabled = false;
                    uxAboutBtn.Visibility = Visibility.Hidden;
                    uxAboutBtn.IsEnabled = false;
                }
                _isAbout = value;
            }
        }


        public Titlebar() {
            InitializeComponent();
        }

        private void UxCloseBtn_OnClick(object sender, RoutedEventArgs e) {
            if (IsMainWindow) {
                Application.Current.Shutdown();
            }
            else {
                Window.GetWindow(this)?.Hide();
            }
        }

        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Window.GetWindow(this)?.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var win = Window.GetWindow(this);
            if (win != null) {
                win.WindowState = WindowState.Minimized;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (win != null)
            {
                var about = new About
                {
                    Owner = win,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                about.ShowDialog();

            }
        }
    }
}
