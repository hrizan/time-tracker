using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessUsage.Helpers
{
    public class ProcessHelper
    {
        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working). 
        /// The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633505(v=vs.85).aspx
        /// </summary>
        /// <returns>Handle of the forregroung window</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633522(v=vs.85).aspx
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="lpdwProcessId">Identifier of the process that created the window</param>
        /// <returns>Identifier of the thread that created the specified window</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        /// <summary>
        /// Get the process that user is currently working with.
        /// </summary>
        /// <returns>Process info of the processs that user is currently working with</returns>
        public static Process GetCurrentUserWorkingProcess()
        {
            IntPtr foregroundWindowHandler = GetForegroundWindow();
            
            //check if foregroundWindowHandler is null for some reason (when currently used process window is losing focus)
            if (foregroundWindowHandler == null)
            {
                return null;
            }

            //get process id by handle
            uint userWorkingProcessId = 0;
            GetWindowThreadProcessId(foregroundWindowHandler, out userWorkingProcessId);

            //current windows processes list
            var processes = Process.GetProcesses();
            //get process info by process id
            var currentUserWorkingProcess = processes.Where(p => p.Id == userWorkingProcessId).FirstOrDefault();
            
            return currentUserWorkingProcess;
        }

        /// <summary>
        ///  Get the process name that user is currently working with
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserWorkingProcessName()
        {
            var currentProcess = GetCurrentUserWorkingProcess();
            return currentProcess.ProcessName;
        }
    }
}
