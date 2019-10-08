using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace RedHoney.Serialization
{
    using Debug = Log.ContextDebug<ConfigManager>;

    /////////////////////////////////////////////////////////////////////////
    public class ConfigManager
    {
        // Regex used to parse each line of the configuration file.
        // It considers any "key=value" and "key:value" lines as valid, ignoring any trailing space/tab
        private static string lineRegexPattern = "^(?:\\s*)(.+?)(?:\\s*)[=:](?:\\s*)(.+?)(?:\\s*)$";

        // Key-value map of the configuration
        private Dictionary<string, string> configMap = new Dictionary<string, string>();


        /////////////////////////////////////////////////////////////////////////
        /// Creates a ConfigManager using a path to a file
        public ConfigManager(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"[ERROR] File {path} is missing!");
                return;
            }

            // Geting the file contents
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                Match m = Regex.Match(line, lineRegexPattern);
                if (m.Success)
                {
                    string key = m.Groups[1].Captures[0].Value;
                    string val = m.Groups[2].Captures[0].Value;
                    configMap[key] = val;
                }
            }
            reader.Close();
        }

        /////////////////////////////////////////////////////////////////////////
        /// Is this configuration valid?
        public bool IsValid()
        {
            return configMap.Count > 0;
        }

        /////////////////////////////////////////////////////////////////////////
        /// Gets a string from the configuration
        public bool Get(string key, ref string value)
        {
            return Get(key, ref value);
        }

        /////////////////////////////////////////////////////////////////////////
        /// Gets a bool from the configuration
        public bool Get(string key, ref bool value)
        {
            return Get(key, ref value, bool.TryParse);
        }

        /////////////////////////////////////////////////////////////////////////
        /// Gets an integer from the configuration
        public bool Get(string key, ref int value)
        {
            return Get(key, ref value, int.TryParse);
        }

        /////////////////////////////////////////////////////////////////////////
        /// Gets a float from the configuration
        public bool Get(string key, ref float value)
        {
            return Get(key, ref value,
                (string strVal, out float floatVal) =>
                    float.TryParse(strVal,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out floatVal)
                );
        }

        /////////////////////////////////////////////////////////////////////////
        /// Gets a double from the configuration
        public bool Get(string key, ref double value)
        {
            return Get(key, ref value,
                (string strVal, out double floatVal) =>
                    double.TryParse(strVal,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out floatVal)
                );
        }

        /////////////////////////////////////////////////////////////////////////
        /// Gets a parameter from the configuration with convertFunction
        private delegate bool ConvertFunc<T>(string key, out T value);
        private bool Get<T>(string key, ref T value, ConvertFunc<T> convertFunc = null)
        {
            if (configMap.TryGetValue(key, out string strValue))
            {
                if ((convertFunc != null) && convertFunc(strValue, out T converted))
                {
                    value = converted;
                    return true;
                }
            }
            return false;
        }

    }
}