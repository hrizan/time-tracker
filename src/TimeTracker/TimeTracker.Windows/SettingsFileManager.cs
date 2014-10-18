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

        public static void WriteSettings(string user, string token, string machine)
        {
            using (TextWriter writer = File.CreateText(settingsPath))
            {
                Settings sett = new Settings
                {
                    username = user,
                    usertoken = token,
                    machine = machine
                };

                new XmlSerializer(typeof(Settings)).Serialize(writer, sett);
                writer.Flush();
            }
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

            return sett;
        }
    }

    [Serializable]
    public class Settings
    {
        public string username { get; set; }
        public string usertoken { get; set; }
        public string machine { get; set; }
    }
}
