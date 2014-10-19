using NDde.Client;
using Newtonsoft.Json;
//using ProcessUsage.Models;
using ProcessUsage.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TimeTracker.Backend.Models;
using TimeTracker.Models;
using TimeTracker.RestDataClient.TimeTracker.ClientRest;
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
                SettingsFileManager.WriteSettings(new Settings());

            TymestTray = new NotifyIcon();
            TymestTray.Text = "Tymest Client";
            TymestTray.Icon = Resources.time_8_multi_size;

            TymestTray.ContextMenu = new ContextMenu();

            traySignInOut = new MenuItem();
            traySignInOut.Click += traySignInOut_Click;
            traySignInOut.Text = "Sign in";

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
                StoreUncommitedActivity(true);
                Settings sett = SettingsFileManager.ReadSettings();
                sett.usertoken = String.Empty;
                SettingsFileManager.WriteSettings(sett);
            }
            else
            {
                TymestTray.ContextMenu.MenuItems.Remove(traySignInOut);
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
                    StoreUncommitedActivity(true);

                    trackedProcesses = new List<ProcessActivity>();
                    processWatcher = new ProcessWatcher(250, OnUserWorkingProcessChanged);
                    processWatcher.Start();

                    timer = new System.Timers.Timer(SettingsFileManager.LatestSettings.storeperiod);
                    timer.Elapsed += timer_Elapsed;
                    timer.Enabled = true;

                    Settings sett = SettingsFileManager.ReadSettings();

                    traySignInOut.Text = "Sign out (" + sett.username + ")";
                }
                else
                {
                    bool oldRunning = Running;

                    if (timer != null)
                        timer.Enabled = false;

                    if (processWatcher != null)
                        processWatcher.Stop();

                    if (oldRunning)
                        StoreUncommitedActivity(true);

                    traySignInOut.Text = "Sign in";
                }

                if (!TymestTray.ContextMenu.MenuItems.Contains(traySignInOut))
                    TymestTray.ContextMenu.MenuItems.Add(0, traySignInOut);
            }
        }

        void StoreUncommitedActivity(bool StoreActivitiesFromRam)
        {
            TimeTrackerEmbeddedDataService dService = new TimeTrackerEmbeddedDataService();

            var activities = dService.GetProcessActivities(Int32.MaxValue);

            foreach (var activity in activities)
            {
                if (activity.TimeTo == DateTime.MaxValue)
                    activity.TimeTo = DateTime.Now;

                //store to main DB
                var activityToRegister = new ActivityUpdateDto()
                {
                    //device info
                    DeviceId = activity.DeviceId,
                    DeviceName = Environment.MachineName,
                    DeviceTypeId = (int)DeviceType.Desktop,
                    OSTypeId = (int)OSType.Windows,

                    //process info
                    ProcessName = activity.ProcessName,
                    Resource = activity.Resource,
                    ResourceDescription = activity.ResourceDescription,
                    TimeFrom = activity.TimeFrom,
                    TimeTo = activity.TimeTo,
                    DurationInSec = (activity.TimeTo - activity.TimeFrom).TotalSeconds,
                };

                TimeTrackerDataService dataService = new TimeTrackerDataService(LoginForm.apiUrl, SettingsFileManager.LatestSettings.usertoken);
                var res = dataService.RegisterActivity(activityToRegister);

                if (res.ResponseStatus == ResponseStatus.Completed
                    && res.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    dService.DeleteProcessActivity(activity.Id);
                }
            }

            if (trackedProcesses != null && StoreActivitiesFromRam)
            {
                var trackedProcessesToRemove = trackedProcesses.ToArray();

                foreach (var proc in trackedProcessesToRemove)
                {
                    TimeTrackerEmbeddedDataService service = new TimeTrackerEmbeddedDataService();
                    if (proc.TimeTo == DateTime.MaxValue)
                        proc.TimeTo = DateTime.Now;

                    //store to main DB
                    var activityToRegister = new ActivityUpdateDto()
                    {
                        //device info
                        //DeviceId = proc.DeviceId,
                        DeviceName = Environment.MachineName,
                        DeviceTypeId = (int)DeviceType.Desktop,
                        OSTypeId = (int)OSType.Windows,

                        //process info
                        ProcessName = proc.ProcessName,
                        Resource = proc.Resource,
                        ResourceDescription = proc.ResourceDescription,
                        TimeFrom = proc.TimeFrom,
                        TimeTo = proc.TimeTo,
                        DurationInSec = (proc.TimeTo - proc.TimeFrom).TotalSeconds,
                    };

                    TimeTrackerDataService dataService = new TimeTrackerDataService(LoginForm.apiUrl, SettingsFileManager.LatestSettings.usertoken);
                    var res = dataService.RegisterActivity(activityToRegister);

                    if (res.ResponseStatus == ResponseStatus.Completed
                        && res.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        trackedProcesses.Remove(proc);
                    }
                }
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeTrackerEmbeddedDataService dService = new TimeTrackerEmbeddedDataService();

            lock (locker)
            {
                StoreUncommitedActivity(false);
                
                ////TO DO: zapazvane na dannite v db
                //foreach (var activity in dService.GetProcessActivities(Int32.MaxValue).OrderBy(p => p.TimeFrom))
                //{
                //    dService.DeleteProcessActivity(activity.Id);
                //}

                ////trackedProcesses.RemoveAll(p => p.UsagePeriods.Last().To.Value != DateTime.MaxValue);
            }
        }

        private void OnUserWorkingProcessChanged(System.Diagnostics.Process p_old, System.Diagnostics.Process p_new)
        {
            if (p_old != null)
            {
                //ProcessUsageInfo old_info = trackedProcesses
                //    .Where(p => p.WindowHandle == p_old.MainWindowHandle && p.Name == p_old.ProcessName)
                //    .SingleOrDefault();
                var old_infos = trackedProcesses
                    .Where(p =>
                        p.WindowHandle == p_old.MainWindowHandle
                        && p.ProcessName == p_old.ProcessName
                        && p.TimeTo == DateTime.MaxValue)
                    .OrderBy(p => p.TimeFrom)
                    .ToList();
                
                ProcessActivity old_info = old_infos
                    .FirstOrDefault();

                if (old_info != null)
                {
                    old_info.TimeTo = DateTime.Now;
                    old_info.DurationInSec = (old_info.TimeTo - old_info.TimeFrom).TotalSeconds;
                    SaveProcInfo(old_info);
                    trackedProcesses.Remove(old_info);

                    for (int k = 1; k < old_infos.Count; k++)
                    {
                        old_infos[k].TimeTo = old_info.TimeTo;
                        old_infos[k].DurationInSec = (old_info.TimeTo - old_infos[k].TimeFrom).TotalSeconds;
                        SaveProcInfo(old_infos[k]);
                        trackedProcesses.Remove(old_infos[k]);
                    }
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
                    try
                    {
                        info = (new Uri(info)).Host;
                    }
                    catch
                    {
                    }
                    break;
                case "firefox":
                    DdeClient dde = new DdeClient("Firefox", "WWW_GetWindowInfo");
                    dde.Connect();
                    string url1 = String.Empty;
                    try
                    {
                        url1 = dde.Request("URL", int.MaxValue);
                    }
                    catch
                    {
                        break;
                    }
                    dde.Disconnect();

                    if (url1.IndexOf('/') < 0)
                        break;

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
            TymestTray.Visible = false;
            //LoginForm.CloseActiveForm();
            Application.ExitThread();
            //ExitThread();
        }

        //protected override void ExitThreadCore()
        //{
        //    TymestTray.Visible = false;
        //    base.ExitThreadCore();
        //}
    }
}