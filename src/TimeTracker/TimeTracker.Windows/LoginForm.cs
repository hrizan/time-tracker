using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTracker.Backend.Models;
using TimeTracker.Windows.Models;

namespace TimeTracker.Windows
{
    public partial class LoginForm : Form
    {
        static bool LoggingInProgress = false;
        
        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            LogInModel login = new LogInModel();
            login.UserName = textBoxUser.Text.Trim();
            login.Password = textBoxPass.Text;
            login.DeviceName = Environment.MachineName;
            login.DeviceOSType = (int)OSType.Windows;

            DataServiceHelper helper = new DataServiceHelper(String.Empty, Guid.NewGuid());
            string token = helper.LogIn(login);

            if (token != String.Empty) //nekva proverka za uspeshen login????
            {
                SettingsFileManager.WriteSettings(textBoxUser.Text, token, Environment.MachineName);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                //MessageBox.Show("Unsucce", "");
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
                //insert login logic here
            }

            LoggingInProgress = false;
            return success;
            //return true;
        }
    }
}