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
        static LoginForm CurrentForm = null;

        public static string apiUrl = "http://77.70.26.137/timetracker-v1/api";
        
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

            //LogInModel login = new LogInModel();
            //login.DeviceName = Environment.MachineName;
            //login.DeviceOSType = oSTypeId;
            //login.UserName = textBoxUser.Text;
            //login.Password = textBoxPass.Text;

            TimeTrackerDataService dataService = new TimeTrackerDataService(apiUrl);

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

        public static void CloseActiveForm()
        {
            if (CurrentForm != null)
            {
                CurrentForm.Close();
                CurrentForm.Dispose();
                CurrentForm = null;
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
                CurrentForm = new LoginForm();
                CurrentForm.textBoxUser.Text = sett.username;
                DialogResult result = CurrentForm.ShowDialog();
                success = result == DialogResult.OK;
                CurrentForm = null;
            }
            else
            {
                TimeTrackerDataService dataService = new TimeTrackerDataService(apiUrl, sett.usertoken);
                success = true;
            }

            LoggingInProgress = false;
            return success;
            //return true;
        }
    }
}