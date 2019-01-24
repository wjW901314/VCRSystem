
using System;

namespace MergeWave
{
    public class CommonUtil
    {
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1971, 1, 1, 0, 0, 0,0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}