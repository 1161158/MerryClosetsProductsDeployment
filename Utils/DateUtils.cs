using System;

namespace MerryClosets.Utils
{
    public class DateUtils {
        public static string GetCurrentDateTimestamp() {
            DateTime dt = DateTime.Now;
            return String.Format("{0:F}", dt);
        }
    }
}