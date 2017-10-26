using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL_EasyMiner.Helper
{
    public class MiningOperation
    {

        private static string MinerExePath { get; set; }
        private string MiningExe
        {
            get
            {
                string path = MinerExePath;
                for (int i = path.Length - 1; i >= 0; i += -1)
                {
                    if (path.ElementAt(i) != '\\') continue;
                    path = path.Substring(i, path.Length - i).Replace("\\", "");
                    break;
                }
                return path;
            }
        }

        private static string MiningBatch { get; set; }

        public static void SetMinerMath(string path)
        {
            MinerExePath = path;
        }

        public string StartMiningProcess()
        {
            string location = "StartMiningProcess";
            try
            {
                KillMiner();
                var _newProcess = new Process();
                using (_newProcess)
                {
                    if (File.Exists(MiningBatch))
                    {
                        
                    }
                }

            }
            catch
            {
                
            }
            return "";
        }

        private void KillMiner()
        {
            foreach (Process p in Process.GetProcessesByName(MiningExe.Replace(".exe", "")))
            {
                try
                {
                    p.Kill();
                }
                catch (Exception e)
                {

                }
            }
        }


    }
}
