using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTracker.Backend.Models;
using TimeTracker.RestDataClient.TimeTracker.ClientRest;
using TimeTracker.Windows.Models;

namespace TimeTracker.Windows
{
    public partial class LoginForm : Form
    {
        static bool LoggingInProgress = false;

        public static string apiUrl = "http://localhost:52359/Api";
        
        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string authKey = "";
            string computerName = Environment.MachineName;
            int deviceTypeId = (int)DeviceType.Desktop;
            int oSTypeId = (int)OSType.Windows;
            Guid? deviceId = null;
            string username = textBoxUser.Text;
            string password = textBoxPass.Text;
            TimeTrackerDataService dataService = new TimeTrackerDataService(apiUrl, authKey);

            var loginModelWithDevice = new LoginModelWithDevice()
            {
                UserName = username,
                Password = password,
                DeviceName = computerName,
                DeviceType = deviceTypeId,
                DeviceOSType = oSTypeId
            };

            var response = dataService.LoginWithDevice(loginModelWithDevice);
            if (response.ResponseStatus == ResponseStatus.Completed
                && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userProfile = response.Data;
                authKey = userProfile.AuthToken;
                deviceId = userProfile.DeviceId;

                Settings sett = new Settings();
                sett.username = userProfile.UserName;
                sett.usertoken = userProfile.AuthToken;
                sett.deviceid = userProfile.DeviceId;
                sett.machine = Environment.MachineName;

                SettingsFileManager.WriteSettings(sett);
                DialogResult = DialogResult.OK;
                Close();

                //Debug.WriteLine("DeviceId:" + userProfile.DeviceId.ToString());
            }
            else
            {
                MessageBox.Show("Wrong username or password, or no Internet connection!", "Login error!");
            }
        }

        public static bool AttemptLogIn()
        {
            if (LoggingInProgress) return false;
            LoggingInProgress = true;

            bool success = false;

            Settings sett = SettingsFileManager.ReadSettings();
            
            if (sett.usertoken == String.Empty)
            {
                LoginForm login = new LoginForm();
                login.textBoxUser.Text = sett.username;
                DialogResult result = login.ShowDialog();
                success = result == DialogResult.OK;
            }
            else
            {
                TimeTrackerDataService dataService = new TimeTrackerDataService(apiUrl, sett.usertoken);
            }

            LoggingInProgress = false;
            return success;
            //return true;
        }
    }
}