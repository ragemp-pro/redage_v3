using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Redage.SDK
{
    public class Settings
    {
        public static T ReadAsync<T>(string filePath, T config)
        {
            var path = @$"settings/{filePath}.json";

            if (!File.Exists(path))
            {
                using var sw = File.CreateText(path);
                    
                sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
                sw.Flush();
                sw.Close();
            }
            else
            {
                using var r = new StreamReader(path);
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<T>(json);
                r.Close();
            }

            return config;
        }
        public static void Save<T>(string filePath, T config)
        {
            var path = @$"settings/{filePath}.json";

            File.WriteAllText(path, string.Empty);
            using var sw = new StreamWriter(path, true, Encoding.UTF8);
            sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
            sw.Close();
        }
    }
}