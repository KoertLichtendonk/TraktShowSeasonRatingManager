using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TraktShowSeasonRatingManager.Configuration
{
    public class ConfigManager
    {
        private static readonly ConfigManager _Instance = new ConfigManager();

        static ConfigManager()
        {
        }

        public static ConfigManager Instance
        {
            get
            {
                return _Instance;
            }
        }

        public Config Data;

        private ConfigManager()
        {
            string filePath = GetFileOrDirectory(null, null);
            string fileName = GetFileOrDirectory(null, "Config.json");

            if (!(Directory.Exists(filePath)))
            {
                Directory.CreateDirectory(filePath);
            }

            if (!(File.Exists(fileName)))
            {
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    Data = new Config();

                    string json = JsonConvert.SerializeObject(Data, Formatting.Indented);

                    sw.Write(json);
                    sw.Close();
                }
            }
            string config = File.ReadAllText(fileName);

            Data = JsonConvert.DeserializeObject<Config>(config);
        }

        public static string GetFileOrDirectory(string? path = null, string? file = null)
        {
            string projectName = Assembly.GetCallingAssembly().GetName().Name.Replace(" ", "");

            string rootPath = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), projectName);

            if (!String.IsNullOrEmpty(path))
            {
                string tmpPath = string.Format("{0}\\{1}", rootPath, path);

                if (!(Directory.Exists(tmpPath)))
                {
                    Directory.CreateDirectory(tmpPath);
                }

                if (String.IsNullOrEmpty(file))
                {
                    return tmpPath;
                }
            }

            if (!String.IsNullOrEmpty(file))
            {
                return string.Format("{0}\\{1}\\{2}", rootPath, path, file);
            }

            return rootPath;
        }

        public void Save()
        {
            string fileName = GetFileOrDirectory(null, @"Config.json");
            string json = JsonConvert.SerializeObject(Data, Formatting.Indented);

            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                sw.Write(json);
                sw.Close();
            }
        }
    }
}
