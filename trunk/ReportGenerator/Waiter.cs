using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ReportGenerator
{
    public class Waiter
    {
        int Pause;
        int RandomRange;
        Regex regex;
        Match matchPause;
        Match matchTime;
        Match matchTimeRange;
        const string pausePattern = @"\[Wait:((\d)*)\]";
        const string timeRangePattern = @"\[SendAt:(((\d\d):(\d\d))-((\d\d):(\d\d)))\]";
        string clearedBody;
 
        public Waiter(string inputtext)
        {
            matchPause = Regex.Match(inputtext, pausePattern);
            matchTimeRange = Regex.Match(inputtext, timeRangePattern);
            clearedBody = Regex.Replace(Regex.Replace(inputtext, timeRangePattern, string.Empty, RegexOptions.IgnoreCase), timeRangePattern, string.Empty, RegexOptions.IgnoreCase);

        }
        public void Wait()
        {
            if (matchPause.Success)
            {
                int.TryParse(matchPause.Groups[1].Value, out Pause);
                Pause *= 60000;
            }
            else if (matchTimeRange.Success && matchTimeRange.Groups.Count > 6)
            {

                DateTime now = DateTime.Now;
                DateTime start = new DateTime(now.Year, now.Month, now.Day, Convert.ToInt32(matchTimeRange.Groups[3].Value), Convert.ToInt32(matchTimeRange.Groups[4].Value), 0);
                DateTime end = new DateTime(now.Year, now.Month, now.Day, Convert.ToInt32(matchTimeRange.Groups[6].Value), Convert.ToInt32(matchTimeRange.Groups[7].Value), 0);
                Pause = Convert.ToInt32(start.Subtract(now).TotalSeconds);
                RandomRange = Convert.ToInt32(end.Subtract(start).TotalSeconds);
                Random random = new Random(Convert.ToInt32(now.Ticks % int.MaxValue));
                Pause += random.Next(0, RandomRange);
                Pause *= 1000;
            }
            Thread.Sleep(Pause);


        }
        public string GetClearedBody()
        {
            return clearedBody;
        }

    }
}
