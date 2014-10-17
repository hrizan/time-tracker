using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Helpers
{
    public class ConfigHelper
    {
        public static string GetAppSettingsValue(string key)
        {
            var setting = System.Web.Configuration.WebConfigurationManager.AppSettings[key];
            if ((!string.IsNullOrEmpty(setting)) && (!string.IsNullOrWhiteSpace(setting)))
            {
                return setting;
            }

            return string.Empty;
        }
    }
}