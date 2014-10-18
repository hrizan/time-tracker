using ProcessUsage.Models;
using ProcessUsage.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TimeTracker.Windows.Properties;

namespace TimeTracker.Windows
{
    class TymestTrayContext : ApplicationContext
    {
        NotifyIcon TymestTray = null;
        MenuItem trayExit = null;

        ProcessWatcher processWatcher = null;
        List<ProcessUsageInfo> trackedProcesses = null;

        StreamWriter currentWriter = null;
        System.Timers.Timer timer = null;

        readonly string temp_storage_path = Application.StartupPath + "\\temp_storage.tmst";

        public TymestTrayContext()
        {
            TymestTray = new NotifyIcon();
            TymestTray.Text = "Tymest Client";
            TymestTray.Icon = Resources.time_8_multi_size;

            TymestTray.ContextMenu = new ContextMenu();

            trayExit = new MenuItem();
            trayExit.Text = "Exit";
            trayExit.Click += trayExit_Click;
            TymestTray.ContextMenu.MenuItems.Add(trayExit);

            trackedProcesses = new List<ProcessUsageInfo>();
            processWatcher = new ProcessWatcher(500, OnUserWorkingProcessChanged);
            processWatcher.Start();

            timer = new System.Timers.Timer(15000); //600000);//10mins
            timer.Elapsed += timer_Elapsed;
            currentWriter = new StreamWriter(temp_storage_path);
            timer.Enabled = true;

            TymestTray.Visible = true;

            //just for test :)
            //SettingsFileManager.WriteSettings();
            //SettingsFileManager.ReadSettings();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            currentWriter.Flush();
            currentWriter.Dispose();

            ////////////////////////////////////////////////////
            //TODO - izpra6tane na zapisanite danni kum DB!!!!//
            ////////////////////////////////////////////////////

            File.Delete(temp_storage_path);
            currentWriter = new StreamWriter(temp_storage_path);
        }

        private void OnUserWorkingProcessChanged(System.Diagnostics.Process p_old, System.Diagnostics.Process p_new)
        {
            if (p_old != null)
            {
                ProcessUsageInfo old_info = trackedProcesses
                    .Where(p => p.WindowHandle == p_old.MainWindowHandle && p.Name == p_old.ProcessName)
                    .SingleOrDefault();

                if (old_info != null && old_info.UsagePeriods.Count > 0)
                {
                    old_info.UsagePeriods[old_info.UsagePeriods.Count - 1].To = DateTime.Now;
                    SaveProcInfo(old_info, old_info.UsagePeriods[old_info.UsagePeriods.Count - 1]);
                }
            }

            if (p_new != null && p_new.ProcessName != "Idle")
            {
                ProcessUsageInfo new_info = trackedProcesses
                    .Where(p => p.WindowHandle == p_new.MainWindowHandle && p.Name == p_new.ProcessName)
                    .SingleOrDefault();

                if (new_info == null)
                {
                    new_info = new ProcessUsageInfo();
                    new_info.MachineName = Environment.MachineName;
                    new_info.Name = p_new.ProcessName;
                    new_info.WindowHandle = p_new.MainWindowHandle;
                    trackedProcesses.Add(new_info);
                }

                UsagePeriod up = new UsagePeriod();
                up.From = DateTime.Now;
                up.To = DateTime.MaxValue;
                up.Title = p_new.MainWindowTitle;
                new_info.UsagePeriods.Add(up);
            }
        }

        void SaveProcInfo(ProcessUsageInfo info, UsagePeriod period)
        {
                //info.WindowHandle.ToInt64() + "\r\n" +
                //info.Name + "\r\n" +
                //info.MachineName + "\r\n" +
                //period.Title + "\r\n" +
                //period.AdditionalInfo + "\r\n" +
                //period.From.ToString("dd.MM.yyyy-hh.mm.ss") + "\r\n" +
                //period.To.Value.ToString("dd.MM.yyyy-hh.mm.ss");

            currentWriter.Write(
                info.WindowHandle.ToInt64() + "|" +
                info.Name + "|" +
                info.MachineName + "|" +
                period.Title + "|" +
                period.AdditionalInfo + "|" +
                period.From.ToString("dd.MM.yyyy-hh.mm.ss") + "|" +
                period.To.Value.ToString("dd.MM.yyyy-hh.mm.ss") + "\r\n");
        }

        void trayExit_Click(object sender, EventArgs e)
        {
            processWatcher.Stop();
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            TymestTray.Visible = false;
            base.ExitThreadCore();
        }
    }
}