using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TimeTracker.Windows
{
    public class UsagePeriod
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }
                _title = value;
            }
        }



        private string _additional;
        public string AdditionalInfo
        {
            get
            {
                return _additional;
            }

            set
            {
                if (_additional == value)
                {
                    return;
                }
                _additional = value;
            }
        }

        private DateTime _from;
        public DateTime From
        {
            get
            {
                return _from;
            }

            set
            {
                if (_from == value)
                {
                    return;
                }
                _from = value;
            }
        }

        private DateTime? _to;
        public DateTime? To
        {
            get
            {
                return _to;
            }

            set
            {
                if (_to == value)
                {
                    return;
                }
                _to = value;
            }
        }

        public TimeSpan? Interval
        {
            get
            {
                if (To != null)
                {
                    return To.Value - From;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
