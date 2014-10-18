using NDde.Client;
using Newtonsoft.Json;
//using ProcessUsage.Models;
using ProcessUsage.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TimeTracker.Windows.DataStore;
using TimeTracker.Windows.Properties;

namespace TimeTracker.Windows
{
    class TymestTrayContext : ApplicationContext
    {
        NotifyIcon TymestTray = null;
        MenuItem traySignInOut = null;
        MenuItem trayExit = null;

        ProcessWatcher processWatcher = null;
        //List<ProcessUsageInfo> trackedProcesses = null;
        List<ProcessActivity> trackedProcesses = null;

        System.Timers.Timer timer = null;
        readonly object locker = new object();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr Parent, int ChildAfter, string Class, string Window);

        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;

        public TymestTrayContext()
        {
            if (!File.Exists(SettingsFileManager.settingsPath))
                SettingsFileManager.WriteSettings(String.Empty, String.Empty, String.Empty);

            TymestTray = new NotifyIcon();
            TymestTray.Text = "Tymest Client";
            TymestTray.Icon = Resources.time_8_multi_size;

            TymestTray.ContextMenu = new ContextMenu();

            traySignInOut = new MenuItem();
            traySignInOut.Click += traySignInOut_Click;
            traySignInOut.Text = "Sign in";
            TymestTray.ContextMenu.MenuItems.Add(traySignInOut);

            trayExit = new MenuItem();
            trayExit.Text = "Exit";
            trayExit.Click += trayExit_Click;
            TymestTray.ContextMenu.MenuItems.Add(trayExit);
            TymestTray.Visible = true;

            Running = LoginForm.AttemptLogIn();
        }

        void traySignInOut_Click(object sender, EventArgs e)
        {
            if (Running)
            {
                Running = false;
                StoreUncommitedActivity();
                Settings sett = SettingsFileManager.ReadSettings();
                sett.usertoken = String.Empty;
                SettingsFileManager.WriteSettings(sett.username, String.Empty, sett.machine);
            }
            else
            {
                Running = LoginForm.AttemptLogIn();
            }
        }

        public bool Running
        {
            get { return timer != null ? timer.Enabled : false; }
            set
            {
                if (value)
                {
                    StoreUncommitedActivity();

                    //trackedProcesses = new List<ProcessUsageInfo>();
                    trackedProcesses = new List<ProcessActivity>();
                    processWatcher = new ProcessWatcher(250, OnUserWorkingProcessChanged);
                    processWatcher.Start();

                    timer = new System.Timers.Timer(15000); //600000);//10mins
                    timer.Elapsed += timer_Elapsed;
                    timer.Enabled = true;

                    Settings sett = SettingsFileManager.ReadSettings();

                    traySignInOut.Text = "Sign out (" + sett.username + ")";
                }
                else
                {
                    if (timer != null)
                        timer.Enabled = false;

                    if (processWatcher != null)
                        processWatcher.Stop();

                    traySignInOut.Text = "Sign in";
                }
            }
        }

        void StoreUncommitedActivity()
        {
            TimeTrackerEmbeddedDataService dService = new TimeTrackerEmbeddedDataService();

            foreach (var activity in dService.GetProcessActivities(Int32.MaxValue))
            {
                if (activity.TimeTo == DateTime.MaxValue)
                    activity.TimeTo = DateTime.Now;

                //TO DO - store to main DB

                dService.DeleteProcessActivity(activity.Id);
            }

            if (trackedProcesses != null)
            {
                foreach (var proc in trackedProcesses)
                {
                    TimeTrackerEmbeddedDataService service = new TimeTrackerEmbeddedDataService();
                    if (proc.TimeTo == DateTime.MaxValue)
                        proc.TimeTo = DateTime.Now;

                    //TO DO - store to main DB
                }

                trackedProcesses.Clear();
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeTrackerEmbeddedDataService dService = new TimeTrackerEmbeddedDataService();

            lock (locker)
            {
                //TO DO: zapazvane na dannite v db
                foreach (var activity in dService.GetProcessActivities(Int32.MaxValue).OrderBy(p => p.TimeFrom))
                {
                    dService.DeleteProcessActivity(activity.Id);
                }

                //trackedProcesses.RemoveAll(p => p.UsagePeriods.Last().To.Value != DateTime.MaxValue);
            }
        }

        private void OnUserWorkingProcessChanged(System.Diagnostics.Process p_old, System.Diagnostics.Process p_new)
        {
            if (p_old != null)
            {
                //ProcessUsageInfo old_info = trackedProcesses
                //    .Where(p => p.WindowHandle == p_old.MainWindowHandle && p.Name == p_old.ProcessName)
                //    .SingleOrDefault();
                ProcessActivity old_info = trackedProcesses
                    .Where(p => 
                        p.WindowHandle == p_old.MainWindowHandle 
                        && p.ProcessName == p_old.ProcessName
                        && p.TimeTo == DateTime.MaxValue)
                    .SingleOrDefault();

                if (old_info != null)
                {
                    old_info.TimeTo = DateTime.Now;
                    old_info.DurationInSec = (old_info.TimeTo - old_info.TimeFrom).TotalSeconds;
                    SaveProcInfo(old_info);
                    trackedProcesses.Remove(old_info);
                }
            }

            if (p_new != null && p_new.ProcessName != "Idle")
            {
                //ProcessUsageInfo new_info = trackedProcesses
                //    .Where(p => p.WindowHandle == p_new.MainWindowHandle && p.Name == p_new.ProcessName)
                //    .SingleOrDefault();

                ProcessActivity new_info = new ProcessActivity();
                new_info.ProcessName = p_new.ProcessName;
                new_info.WindowHandle = p_new.MainWindowHandle;
                new_info.TimeFrom = DateTime.Now;
                new_info.Resource = GetAdditionalInfo(p_new);
                new_info.ResourceDescription = p_new.MainWindowTitle;
                new_info.DeviceName = Environment.MachineName;
                trackedProcesses.Add(new_info);
            }
        }

        private string GetAdditionalInfo(Process proc)
        {
            string info = String.Empty;
            IntPtr id;
            IntPtr length;
            StringBuilder text;

            switch (proc.ProcessName)
            {
                case "iexplore":
                    id = proc.MainWindowHandle;
                    id = FindWindowEx(id, 0, "WorkerW", null);
                    id = FindWindowEx(id, 0, "ReBarWindow32", null);
                    id = FindWindowEx(id, 0, "Address Band Root", null);
                    id = FindWindowEx(id, 0, "Edit", null);
                    length = SendMessage(id, WM_GETTEXTLENGTH, 0, null) + 1;
                    text = new StringBuilder(length.ToInt32());
                    SendMessage(id, WM_GETTEXT, length.ToInt32(), text);
                    info = text.ToString();
                    info = (new Uri(info)).Host;
                    break;
                case "firefox":
                    DdeClient dde = new DdeClient("Firefox", "WWW_GetWindowInfo");
                    dde.Connect();
                    string url1 = dde.Request("URL", int.MaxValue);
                    dde.Disconnect();
                    info = url1
                        .Replace("\"",String.Empty)
                        .Replace("\0", String.Empty);

                    string temp = info.Substring(0, info.IndexOf("://") + 3);
                    info = info.Substring(info.IndexOf("://") + 3);
                    info = info.Substring(0, info.IndexOf('/'));
                    info = temp + info;
                    info = (new Uri(info)).Host;
                    break;
                case "chrome":
                    //id = FindWindowEx(proc.MainWindowHandle, 0, "Chrome_AutocompleteEditView", null); //Chrome_OmniboxView
                    //length = SendMessage(id, WM_GETTEXTLENGTH, 0, null) + 1;
                    //text = new StringBuilder(length.ToInt32());
                    //SendMessage(id, WM_GETTEXT, length.ToInt32(), text);
                    //info = text.ToString();
                    break;
            }

            return info;
        }

        void SaveProcInfo(ProcessActivity activity)
        {
            TimeTrackerEmbeddedDataService dService = new TimeTrackerEmbeddedDataService();

            lock (locker)
                dService.AddProcActivity(activity);
        }

        void trayExit_Click(object sender, EventArgs e)
        {
            Running = false;
            StoreUncommitedActivity();
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            TymestTray.Visible = false;
            base.ExitThreadCore();
        }
    }
}