using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public enum ProductivityTypeEnum
    {
        VeryProductive = 2,
        Productive = 1,
        Neutral = 0,
        Distractive = -1,
        VeryDistractive = -2,
    }
}