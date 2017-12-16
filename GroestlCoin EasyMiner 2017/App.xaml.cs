using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GroestlCoin_EasyMiner_2018 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() : base() {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += OnDomainUnhandledException;


        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.ExceptionObject;
            string errorMessage = $"An unhandled exception occurred: {e.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OnDispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {

        }
    }
}
