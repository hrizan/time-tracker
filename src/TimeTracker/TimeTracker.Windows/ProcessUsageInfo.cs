using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Windows;

namespace ProcessUsage.Models
{
    public class ProcessUsageInfo : INotifyPropertyChanged
    {
        private IntPtr _windowHandle;
        public IntPtr WindowHandle
        {
            get
            {
                return _windowHandle;
            }

            set
            {
                if (_windowHandle == value)
                {
                    return;
                }
                _windowHandle = value;
                RaisePropertyChanged("WindowHandle");
            }
        }

        private string _machineName;
        public string MachineName
        {
            get
            {
                return _machineName;
            }

            set
            {
                if (_machineName == value)
                {
                    return;
                }
                _machineName = value;
                RaisePropertyChanged("MachineName");
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private List<UsagePeriod> _usagePeriods = new List<UsagePeriod>();
        public List<UsagePeriod> UsagePeriods
        {
            get
            {
                return _usagePeriods;
            }

            set
            {
                if (_usagePeriods == value)
                {
                    return;
                }
                _usagePeriods = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
