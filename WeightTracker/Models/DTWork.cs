using Java.Time.Chrono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightTracker.Models
{
    public static class DTWork
    {
        public static string DateId(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
        public static string DateId(DayResult dayResult)
        {
            return dayResult.Date.ToString("dd.MM.yyyy");
        }
        public static DateTime FromId(string dateId)
        {
            string[] date = dateId.Split('.');
            return new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
        }
        public static string MonthYearId(DateTime date)
        {
            return date.ToString("MMM, yyyy");
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1.Date > date2.Date ? date1 : date2;
        }
        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1.Date < date2.Date ? date1 : date2;
        }
    }
}
