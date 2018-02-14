using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using Newtonsoft.Json;
using GroestlCoin_EasyMiner_2018.Properties;

namespace GroestlCoin_EasyMiner_2018.Business_Logic {
    class MiningOperations {
        public enum GpuMiningSettings {
            None = 0,
            NVidia = 1,
            Amd = 2
        }

        public enum MiningPools {
            Dwarfpool = 0,
            Suprnova = 1,
            MiningPoolHub = 2,
            P2Pool = 3,
            Custom = 4
        }

        public static string WalletFolder
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet";

        public static bool WalletFileExists => File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Electrum-grs\wallets\default_wallet");

        public static string ExecutingDirectory => Directory.GetCurrentDirectory();

        public static string CpuDirectory => Path.Combine(ExecutingDirectory, @"Resources\Miners\CPU Miner\minerd.exe");
        public static string AMDDirectory => Path.Combine(ExecutingDirectory, @"Resources\Miners\AMD Miner\sgminer.exe");
        public static string NVididiaDirectory => Path.Combine(ExecutingDirectory, @"Resources\Miners\nVidia Miner\ccminer.exe");

        public static List<string> GpuModels {
            get {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                return (from ManagementBaseObject mo in searcher.Get() select mo.Properties["Description"].ToString()).ToList();
            }
        }

        public static bool HasNVidia {
            get {
                bool hasGpu;
                try {
                    hasGpu = GpuModels.Any(d => d.ToLower().Contains("nvidia") || d.ToLower().Contains("quadro"));
                }
                catch {
                    hasGpu = false;
                }
                return hasGpu;
            }
        }

        public static bool HasAmd {
            get {
                bool hasGpu;
                try {
                    hasGpu = GpuModels.Any(d => d.ToLower().Contains("amd") || d.ToLower().Contains("firepro"));
                }
                catch {
                    hasGpu = false;
                }
                return hasGpu;
            }
        }


        public static bool CpuStarted { get; set; } = false;
        public static bool GpuStarted { get; set; } = false;


        public static bool UseDwarfPool => Settings.Default.UseDwarfPool;
        public static string WalletAddress => Settings.Default.GrsWalletAddress;
        public static bool UseAutoIntensity => Settings.Default.UseAutoIntensity;
        public static int MiningIntensity => Settings.Default.MineIntensity;

        public static string MiningPoolAddress {
            get {
                switch (SelectedMiningPool) {
                    case MiningPools.Dwarfpool:
                        return GetAddressForPool(MiningPools.Dwarfpool);
                    case MiningPools.MiningPoolHub:
                        return GetAddressForPool(MiningPools.MiningPoolHub);
                    case MiningPools.Suprnova:
                        return GetAddressForPool(MiningPools.Suprnova);
                    case MiningPools.P2Pool:
                        return GetAddressForPool(MiningPools.P2Pool);
                    case MiningPools.Custom:
                        return GetAddressForPool(MiningPools.Custom);
                }
                return "";
            }
        }

        public static string MiningPoolUsername {
            get {
                switch (SelectedMiningPool) {
                    case MiningPools.Dwarfpool:
                        return GetUsernameForPool(MiningPools.Dwarfpool);
                    case MiningPools.MiningPoolHub:
                        return GetUsernameForPool(MiningPools.MiningPoolHub);
                    case MiningPools.Suprnova:
                        return GetUsernameForPool(MiningPools.Suprnova);
                    case MiningPools.P2Pool:
                        return GetUsernameForPool(MiningPools.P2Pool);
                    case MiningPools.Custom:
                        return GetUsernameForPool(MiningPools.Custom);
                }
                return "";
            }
        }

        public static string MiningPoolPassword {
            get {
                switch (SelectedMiningPool) {
                    case MiningPools.Dwarfpool:
                        return GetPasswordForPool(MiningPools.Dwarfpool);
                    case MiningPools.MiningPoolHub:
                        return GetPasswordForPool(MiningPools.MiningPoolHub);
                    case MiningPools.Suprnova:
                        return GetPasswordForPool(MiningPools.Suprnova);
                    case MiningPools.P2Pool:
                        return GetPasswordForPool(MiningPools.P2Pool);
                    case MiningPools.Custom:
                        return GetPasswordForPool(MiningPools.Custom);
                }
                return "";
            }
        }

        public static MiningPools SelectedMiningPool => (MiningPools)Settings.Default.SelectedMiningPool;

        /// <summary>
        /// Common mining poo variables used in all miners
        /// </summary>
        public static PublicMiningArgs CommonMiningPoolVariables => new PublicMiningArgs(MiningPoolAddress, MiningPoolUsername, MiningPoolPassword);


        public static string GetAddressForPool(MiningPools pool) {
            switch (pool) {
                case MiningPools.Dwarfpool:
                    return GetBestDwarfServer();
                case MiningPools.MiningPoolHub:
                    return Settings.Default.MiningPoolHubSettings == null ? "hub.miningpoolhub.com:12004" : Settings.Default.MiningPoolHubSettings[0];
                case MiningPools.Suprnova:
                    return Settings.Default.SuprNovaSettings == null ? "grs.suprnova.cc:5544" : Settings.Default.SuprNovaSettings[0];
                case MiningPools.P2Pool:
                    return Settings.Default.P2PoolSettings == null ? "" : Settings.Default.P2PoolSettings[0];
                case MiningPools.Custom:
                    return Settings.Default.CustomSettings == null ? "" : Settings.Default.CustomSettings[0];
                default:
                    return "";
            }
        }

        public static string GetUsernameForPool(MiningPools pool) {
            switch (pool) {
                case MiningPools.Dwarfpool:
                    return WalletAddress;
                case MiningPools.MiningPoolHub:
                    return Settings.Default.MiningPoolHubSettings == null ? "" : Settings.Default.MiningPoolHubSettings[1];
                case MiningPools.Suprnova:
                    return Settings.Default.SuprNovaSettings == null ? "" : Settings.Default.SuprNovaSettings[1];
                case MiningPools.P2Pool:
                    return Settings.Default.P2PoolSettings == null ? WalletAddress : Settings.Default.P2PoolSettings[1];
                case MiningPools.Custom:
                    return Settings.Default.CustomSettings == null ? WalletAddress : Settings.Default.CustomSettings[1];
                default:
                    return "";
            }
        }

        public static string GetPasswordForPool(MiningPools pool) {
            switch (pool) {
                case MiningPools.Dwarfpool:
                    return "x";
                case MiningPools.MiningPoolHub:
                    return Settings.Default.MiningPoolHubSettings == null ? "x" : Settings.Default.MiningPoolHubSettings[2];
                case MiningPools.Suprnova:
                    return Settings.Default.SuprNovaSettings == null ? "x" : Settings.Default.SuprNovaSettings[2];
                case MiningPools.P2Pool:
                    return Settings.Default.P2PoolSettings == null ? "x" : Settings.Default.P2PoolSettings[2];
                case MiningPools.Custom:
                    return Settings.Default.CustomSettings == null ? "x" : Settings.Default.CustomSettings[2];
                default:
                    return "";
            }
        }



        public static string GetBestDwarfServer() {
            var usAddress = "moria.dwarfpool.com";
            var euAddress = "erebor.dwarfpool.com";

            var ping = new Ping();

            long americanServerTime = 9999;
            long europeanServerTime = 9999;

            for (int i = 0; i < 4; i++) {
                var americanServer = ping.Send(usAddress, 4);
                var europeanServer = ping.Send(euAddress, 4);
                if (americanServer?.Status != IPStatus.Success) continue;
                if (americanServer?.RoundtripTime < americanServerTime) {
                    americanServerTime = americanServer.RoundtripTime;
                }
                if (europeanServer?.Status != IPStatus.Success) continue;
                if (europeanServer?.RoundtripTime < americanServerTime) {
                    europeanServerTime = europeanServer.RoundtripTime;
                }
            }
            return (americanServerTime <= europeanServerTime ? usAddress : euAddress) + ":3345";
        }

        public static string GetCPUCommandLine(PublicMiningArgs arguments) {
            return $"{arguments}";
        }

        public static string GetNVidiaCommandLine(PublicMiningArgs arguments, bool useAutoIntensity, string intensity) {
            var sb = new StringBuilder();
            if (SelectedMiningPool == MiningPools.P2Pool) {
                sb.Append("--submit-stale");
            }
            var intensityArgs = useAutoIntensity ? string.Empty : $" -i {intensity}";
            sb.Append(intensityArgs);
            sb.Append(arguments);

            return sb.ToString();
        }

        public static string GetAMDCommandLine(PublicMiningArgs arguments, bool useAutoIntensity, string intensity, string kernal = "") {
            var sb = new StringBuilder();
            sb.Append(SelectedMiningPool != MiningPools.P2Pool ? " --no-submit-stale" : "");

            var intensityArgs = useAutoIntensity ? string.Empty : $" -i {intensity} ";

            sb.Append(arguments);
            sb.Append(intensityArgs);
            sb.Append("--text-only ");

            if (!string.IsNullOrEmpty(kernal)) {
                sb.Append($" --kernelfile {kernal}");
            }

            return sb.ToString();
        }


        public static string GetAddress() {
            try {
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
            catch {
                return string.Empty;
            }
        }


        public class PublicMiningArgs {
            public string PoolAddress;
            public string PoolUsername;
            public string PoolPassword;

            public PublicMiningArgs(string PoolAddress, string PoolUsername, string PoolPassword) {
                this.PoolAddress = PoolAddress;
                this.PoolUsername = PoolUsername;
                this.PoolPassword = PoolPassword;
            }

            public override string ToString() {
                return $" -o stratum+tcp://{PoolAddress.ToLower().Replace("stratum+tcp://", "").Trim()} -u {PoolUsername.Trim()} -p {PoolPassword.Trim()} ";
            }
        }

    }
}
