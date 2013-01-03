using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iBoox.Helpers
{
    public static class StringHelpers
    {
        public static string ToLongString(this TimeSpan time)
        {
            string output = String.Empty;

            if (time.Days > 0)
                output += time.Days + " дней ";

            if ((time.Days == 0 || time.Days == 1) && time.Hours > 0)
                output += time.Hours + " часов ";

            if (time.Days == 0 && time.Minutes > 0)
                output += time.Minutes + " мин ";

            if (output.Length == 0)
                output += time.Seconds + " сек";

            return output.Trim();
        }
    }
}