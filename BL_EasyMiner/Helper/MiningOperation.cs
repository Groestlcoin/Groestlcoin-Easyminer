using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BL_EasyMiner.Helper {
    public class MiningOperation {
        public enum MiningMethods {
            GroestlCPU32,
            GroestlCPU64,
            GroestlGPU,
            GroestlNVGPU
        }

        public static string MinerExePath { get; set; }
        public static string MiningExe {
            get {
                string path = MinerExePath;
                for (int i = path.Length - 1; i >= 0; i += -1) {
                    if (path.ElementAt(i) != '\\') continue;
                    path = path.Substring(i, path.Length - i).Replace("\\", "");
                    break;
                }
                return path;
            }
        }

        public static string MiningBatch { get; set; }
        public static string ElectrumPath { get; set; }

        public static MiningMethods MiningMethod { get; set; }

        public static string MiningPool { get; set; } = "moria.dwarfpool.com:3345";

        public static string WalletAddress { get; set; }
        public static string LogFileLocation { get; set; }

        public static string WalletFolder
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet";

        public static bool IsWalletSetup => Directory.Exists(WalletFolder) && WalletAddress.Length > 0;

        public static bool NVidiaCheck = Directory.Exists(Environment.SystemDirectory + @"\NVCuda.DLL");

        private static bool HasOpenCl => File.Exists(Environment.SystemDirectory + @"\OpenCL.DLL");


        public static string StartMiningProcess() {
            string location = "StartMiningProcess";
            try {
                KillMiner();
                using (var newProcess = new Process()) {
                    using (newProcess) {
                        if (File.Exists(MiningBatch)) {
                            newProcess.StartInfo.FileName = MiningBatch;
                            newProcess.StartInfo.UseShellExecute = false;
                            newProcess.StartInfo.CreateNoWindow = true;
                            newProcess.StartInfo.RedirectStandardOutput = !HasOpenCl;
                            newProcess.StartInfo.RedirectStandardError = true;
                            newProcess.Start();
                        }
                        else {
                            throw new Exception(" StartMining.bat not found!");
                        }
                    }
                }
            }
            catch {
                //ToDo: Fill
            }
            return "";
        }

        public static void WriteBatchFile(bool? useDwarfPool, string poolAddress, string un, string pwd, string walletAddress) {
            var strLocation = "StartMiningProcess";
            try {
                var fileLine = string.Empty;
                var algo = string.Empty;
                fileLine += "\"" + MinerExePath + "\"";
                switch (MiningMethod) {
                    case MiningMethods.GroestlCPU32:
                    case MiningMethods.GroestlCPU64:
                    case MiningMethods.GroestlNVGPU:
                        algo = "-a groestl";
                        break;
                    case MiningMethods.GroestlGPU:
                        algo = "-k groestlcoin -I d";
                        break;
                }
                if (useDwarfPool == true) {
                    fileLine += $"{algo} -o stratum+tcp://moria.dwarfpool.com:3345 -u {walletAddress} -p x";
                }
                else {
                    fileLine += $"{algo} -o stratum_tcp://{poolAddress} -u {un} -p {pwd}";
                }
                if (MiningMethod == MiningMethods.GroestlGPU || MiningMethod == MiningMethods.GroestlNVGPU) {
                    fileLine = fileLine.Replace(" --threads 1", "");
                }
                fileLine += $"2>\"{LogFileLocation}\"";
                using (var writer = new StreamWriter(MiningBatch)) {
                    writer.Write(fileLine);
                    writer.Close();
                }
            }
            catch {
                //ToDo: Error Handling
            }
        }

        public static string GetAddress() {
            //Get the pubkey
            var pubkey = string.Empty;
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
            if (string.IsNullOrEmpty(pubkey)) return string.Empty;

            //Get coin util directory
            var coinUtilLocation = $@"{Directory.GetCurrentDirectory()}\Resources\coin-util.exe";

            //if CoinUtil file doesn't exist (AV?) then give up..
            if (!File.Exists(coinUtilLocation)) return string.Empty;

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
                //MessageBox.Show(info.Arguments); //Debug
                return address;
            }
        }

        public static void KillMiner() {
            foreach (Process p in Process.GetProcessesByName(MiningExe.Replace(".exe", ""))) {
                try {
                    p.Kill();
                }
                catch (Exception e) {

                }
            }
        }


    }
}
