using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using SlimDX.Direct3D9;
using GroestlCoin_EasyMiner_2017.Business_Logic;
using GroestlCoin_EasyMiner_2017.Properties;

namespace GroestlCoin_EasyMiner_2017.Business_Logic {
    class MiningOperations {
        public enum GpuMiningSettings {
            None = 0,
            NVidia = 1,
            Amd = 2
        }

        public static string WalletFolder
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet";

        public static bool WalletFileExists => File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet");

        public static AdapterCollection GpuModels => new Direct3D().Adapters;

        public static List<string> GpuModels2 {
            get {
                ManagementObjectSearcher searcher =
    new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                return (from ManagementBaseObject mo in searcher.Get() select mo.Properties["Description"].ToString()).ToList();
            }
        }

        public static bool HasNVidia {
            get {
                var hasGpu = false;
                try {
                    hasGpu = GpuModels.Any(d => d.Details.Description.ToLower().Contains("nvidia"));
                }
                catch {
                    hasGpu = false;
                }
                finally {
                    if (hasGpu == false) {
                        hasGpu = GpuModels2.Any(d => d.ToLower().Contains("nvidia"));
                    }
                }
                return hasGpu;
            }
        }

        public static bool HasAmd {
            get {
                var hasGpu = false;
                try {
                    hasGpu = GpuModels.Any(d => d.Details.Description.ToLower().Contains("amd"));
                }
                catch {
                    hasGpu = false;
                }
                finally {
                    if (hasGpu == false) {
                        hasGpu = GpuModels2.Any(d => d.ToLower().Contains("amd"));
                    }
                }
                return hasGpu;
            }
        }


        public static bool CpuStarted { get; set; } = false;
        public static bool GpuStarted { get; set; } = false;


        public static bool UseDwarfPool => Settings.Default.UseDwarfPool;
        public static string WalletAddress => Settings.Default.GrsWalletAddress;
        public static int MiningIntensity => Settings.Default.MineIntensity;
        public static string MiningPoolAddress => UseDwarfPool ? GetBestDwarfServer() : Settings.Default.MiningPoolAddress;
        public static string MiningPoolUsername => UseDwarfPool ? WalletAddress : Settings.Default.MiningPoolUsername;
        public static string MiningPoolPassword => UseDwarfPool ? "x" : Settings.Default.MiningPoolPassword;


        public static string GetBestDwarfServer() {
            var usAddress = "moria.dwarfpool.com";
            var euAddress = "erebor.dwarfpool.com";

            var ping = new Ping();
            var americanServer = ping.Send(usAddress);
            var europeanServer = ping.Send(euAddress);

            long usTime = 9999;
            long euTime = 9999;

            if (americanServer?.Status == IPStatus.Success) {
                usTime = americanServer.RoundtripTime;
            }
            if (europeanServer?.Status == IPStatus.Success) {
                euTime = europeanServer.RoundtripTime;
            }
            if (usTime <= euTime) {
                return usAddress + ":3345";
            }
            else {
                return euAddress + ":3345";
            }
        }

        public static string GetAddress() {
            //Get the pubkey
            var pubkey = String.Empty;
            //If electrum default wallet exists, read the file. 
            if (File.Exists(WalletFolder)) {
                using (StreamReader r = new StreamReader(WalletFolder)) {
                    string json = r.ReadToEnd();
                    //Deserialize the json string to a dynamic array.
                    dynamic array = JsonConvert.DeserializeObject(json);
                    foreach (var item in array) {
                        //Deserialise the inner json string to get the receiving addresses
                        dynamic line = JsonConvert.DeserializeObject(item.Value.ToString());
                        foreach (var item2 in line) {
                            //Get the first address and break from loop
                            pubkey = item2.Value.receiving.First;
                            break;
                        }
                        break;
                    }
                }
            }
            //If it didn't manage to get any public key, give up..
            if (String.IsNullOrEmpty(pubkey)) return String.Empty;

            //Get coin util directory
            var coinUtilLocation = $@"{Directory.GetCurrentDirectory()}\Resources\coin-util.exe";

            //if CoinUtil file doesn't exist (AV?) then give up..
            if (!File.Exists(coinUtilLocation)) return String.Empty;

            //Fire up coin util to get the public key. Return the output.
            using (var process = new Process()) {
                ProcessStartInfo info = new ProcessStartInfo {
                    FileName = @"cmd.exe",
                    Arguments = $@"/C " + "\"" + coinUtilLocation + "\"" + $" -a GRS pubkey-to-addr {pubkey}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                process.StartInfo = info;
                process.EnableRaisingEvents = true;
                process.Start();
                var address = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return address;
            }
        }
    }
}
