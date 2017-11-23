using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SlimDX.Direct3D9;

namespace BL_EasyMiner.Helper {
    public class MiningOperation {
        public enum GpuMiningSettings {
            None = 0,
            NVidia = 1,
            Amd = 2
        }

        public static string WalletFolder
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet";

        public static bool WalletFileExists => File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet");

        public static AdapterCollection GpuModels => new Direct3D().Adapters;

        public static bool HasNVidia => GpuModels.Any(d => d.Details.Description.ToLower().Contains("nvidia"));

        public static bool HasAmd => GpuModels.Any(d => d.Details.Description.ToLower().Contains("amd"));

        public static bool CpuStarted { get; set; } = false;
        public static bool GpuStarted { get; set; } = false;



        public static string GetAddress() {
            //Get the pubkey
            var pubkey = String.Empty;
            //If electrum default wallet exists, read the file. 
            if (File.Exists(MiningOperation.WalletFolder)) {
                using (StreamReader r = new StreamReader(MiningOperation.WalletFolder)) {
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
