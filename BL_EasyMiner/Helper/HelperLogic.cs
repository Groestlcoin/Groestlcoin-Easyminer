using System;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace BL_EasyMiner.Helper
{
    public class HelperLogic
    {
        private enum RegistryValue
        {
            CustomAddress,
            CustomUsername,
            CustomPassword,
            CustomMiningPool,
            CustomSelectedAlgorithm
        }



        public static string RegAddress => GetSetting("Easyminer", "Settings", "CustomAddress", null);
        public static string RegUserName => GetSetting("Easyminer", "Settings", "CustomUsername", null);
        public static string RegPassword => GetSetting("Easyminer", "Settings", "CustomUsername", null);
        public static string RegPool => GetSetting("Easyminer", "Settings", "CustomMiningPool", null);


        public static string GetSetting(string applicationName, string sectionName, string key, string DefaultValue)
        {
            StringBuilder path;
            RegistryKey registryKey;

            path = new StringBuilder(@"Software\VB and VBA Program Settings");

            // Generate the path of the key
            if (string.IsNullOrEmpty(applicationName) == false)
            {
                path.Append('\\');
                path.Append(applicationName);

                if (string.IsNullOrEmpty(sectionName) == false)
                {
                    path.Append('\\');
                    path.Append(sectionName);
                }
            }
            // Open key
            registryKey = Registry.CurrentUser.OpenSubKey(path.ToString());


            // If the key does not exist, return the default Open key
            if (registryKey == null)
            {
                return DefaultValue;
            }
            try
            {
                //Read the value contained in the key
                return (string)registryKey.GetValue(key, DefaultValue);
            }
            finally
            {
                //registryKey.Dispose();
            }
        }

        private void WriteToRegistry(RegistryValue enumRegValue, string value)
        {
            string strLocation = "WriteToRegistry";
            try
            {
                switch (enumRegValue)
                {
                    case RegistryValue.CustomAddress:
                        Interaction.SaveSetting("Easyminer", "Settings", "CustomAddress", value);
                        break;
                    case RegistryValue.CustomMiningPool:
                        Interaction.SaveSetting("Easyminer", "Settings", "CustomMiningPool", value);
                        Interaction.SaveSetting("Easyminer", "Settings", "numberofthreads", value);
                        break;
                    case RegistryValue.CustomUsername:
                        Interaction.SaveSetting("Easyminer", "Settings", "CustomUsername", value);
                        break;
                    case RegistryValue.CustomPassword:
                        Interaction.SaveSetting("Easyminer", "Settings", "CustomPassword", value);
                        break;
                    case RegistryValue.CustomSelectedAlgorithm:
                        Interaction.SaveSetting("Easyminer", "Settings", "CustomSelectedAlgorithm", "2");
                        break;
                }
            }
            catch (Exception ex)
            {
                // BuildErrorMessage(strModule, strLocation, ex.Message);
            }
        }
    }
}
