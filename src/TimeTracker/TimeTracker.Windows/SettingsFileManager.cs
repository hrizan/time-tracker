using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TimeTracker.Windows
{
    public static class SettingsFileManager
    {
        public static readonly string settingsPath = Application.StartupPath + "\\settings.xml";
        public static Settings LatestSettings = null;

        public static void WriteSettings(Settings sett)
        {
            using (TextWriter writer = File.CreateText(settingsPath))
            {
                new XmlSerializer(typeof(Settings)).Serialize(writer, sett);
                writer.Flush();
            }

            LatestSettings = sett;
        }

        public static Settings ReadSettings()
        {
            Settings sett;

            using (StreamReader reader = new StreamReader(settingsPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                sett = (Settings)serializer.Deserialize(reader);
                reader.Close();
            }

            LatestSettings = sett;

            return sett;
        }
    }

    [Serializable]
    public class Settings
    {
        public Settings()
        {
            username = String.Empty;
            usertoken = String.Empty;
            machine = String.Empty;
            deviceid = null;
            storeperiod = 60000;
        }

        public string username { get; set; }
        public string usertoken { get; set; }
        public string machine { get; set; }
        public Guid? deviceid { get; set; }
        public int storeperiod { get; set; }
    }
}
