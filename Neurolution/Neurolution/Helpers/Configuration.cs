using System;
using System.Globalization;
using IniFileLibrary;

namespace Neurolution.Helpers
{
    //Provides access to configuration stored in corresponding file.
    public class Configuration
    {
        private readonly IniFile _iniFile;

        public Configuration(string fileName)
        {
            _iniFile = new IniFile(fileName);
        }


        public String GetString(string section, string name, string defaultValue = "")
        {
            return _iniFile.IniReadValue(section, name, defaultValue);
        }

        public float GetFloat(string section, string name, float defaultValue = 0f)
        {
            return
                float.Parse(
                    _iniFile.IniReadValue(section, name,
                        defaultValue.ToString(CultureInfo.InvariantCulture).Replace(',', '.')),
                    CultureInfo.InvariantCulture);
        }

        public bool GetBool(string section, string name, bool defaultValue = false)
        {
            return bool.Parse(_iniFile.IniReadValue(section, name, defaultValue.ToString()));
        }

        public int GetInt(string section, string name, int defaultValue = 0)
        {
            return int.Parse(_iniFile.IniReadValue(section, name, defaultValue.ToString(CultureInfo.InvariantCulture)),
                CultureInfo.InvariantCulture);
        }
    }
}
