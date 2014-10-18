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
        static readonly string settingsPath = Application.StartupPath + "\\settings.xml";

        public static void WriteSettings()
        {
            using (TextWriter writer = File.CreateText(settingsPath))
            {
                Settings sett = new Settings
                {
                    //testovi danni
                    username = "1",
                    usertoken = "2",
                    machinetoken = "3"
                };

                new XmlSerializer(typeof(Settings)).Serialize(writer, sett);
                writer.Flush();
            }
        }

        public static void ReadSettings()
        {
            Settings sett;

            using (StreamReader reader = new StreamReader(settingsPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                sett = (Settings)serializer.Deserialize(reader);
                reader.Close();
            }
        }
    }

    [Serializable]
    public class Settings
    {
        public string username { get; set; }
        public string usertoken { get; set; }
        public string machinetoken { get; set; }
    }
}
