using UnityEngine;
using System.Collections;
using System;

namespace MFrameWork
{
    public static class MTimeHelper
    {
        static public readonly DateTime Time1970 = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);

        static private DateTime serverTime = new DateTime();
        static private DateTime serverTimeUpdateTime = DateTime.Now;
        static public DateTime ServerTime
        {
            get
            {
                var time = new DateTime(serverTime.Ticks + (DateTime.Now.Ticks - serverTimeUpdateTime.Ticks));
                return time;
            }
            set
            {
                serverTime = value;
                serverTimeUpdateTime = DateTime.Now;
            }
        }

        public static string SecondConvertTime(long second, string format = "d:hh:mm:ss")
        {
            long hour;
            if (format.Contains("d"))
            {
                hour = second % 86400 / 3600;
                format = format.Replace("d", (second / 86400).ToString());
            }
            else
            {
                hour = second / 3600;
            }

            format = format.Replace("hh", hour.ToString("D2"));
            format = format.Replace("h", hour.ToString());
            format = format.Replace("mm", (second % 3600 / 60).ToString("D2"));
            format = format.Replace("m", (second % 3600 / 60).ToString());
            format = format.Replace("ss", (second % 60).ToString("D2"));
            format = format.Replace("s", (second % 60).ToString());
            return format;
        }

        public static string GetTimeStringByDataTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            
            if (dateTime != null)
            {
                return dateTime.ToLocalTime().ToString(format);
            }
            return "";
        }

        //得到当天开始时间
        public static DateTime GetTodayStart(DateTime data)
        {
            int year = data.Year;
            int month = data.Month;
            int day = data.Day;
            DateTime start = new DateTime(year, month, day, 0, 0, 0);
            return start;
        }

        //得到当天所有的秒数
        public static int GetTodayTotalSecond(DateTime data)
        {
            return data.Hour * 3600 + data.Minute * 60 + data.Second;
        }

        #region 时间增量

        
        //计算给定的时间和当天的时间相差的秒数
        public static int GetDeltaSecondsByDayTime(DateTime todayTime, int lastSecond)
        {
            return lastSecond - GetTodayTotalSecond(todayTime);
        }

        //计算两个时间相差的秒数，返回为正数
        public static int GetDeltaSeconds(long beforeSecond, DateTime lastTime)
        {
            DateTime beforeTime = TimestampToDateTime(beforeSecond);
            int deltaSeconds = (int)TicksToSeconds(lastTime.Ticks - beforeTime.Ticks);
            deltaSeconds = Mathf.Abs(deltaSeconds);
            return deltaSeconds;
        }
        //服务器当前时间与指定时间相差的秒数,返回为正数
        static public int DeltaSecondsWithServerTime(long beforeSecond)
        {
            return GetDeltaSeconds(beforeSecond, ServerTime);
        }

        #endregion

        #region DateTime和时间戳互转

        //将DateTime转换为时间戳
        static public long DateTimeToTimestamp(DateTime data)
        {
            TimeSpan time = data - Time1970;
            return (long)time.TotalSeconds;
        }

        //将时间戳转换为DateTime
        static public DateTime TimestampToDateTime(long timestamp)
        {
            return Time1970.AddSeconds(timestamp);
        }

        static public DateTime TimestampToDateTime(double timestamp)
        {
            return Time1970.AddSeconds(timestamp);
        }
        static public DateTime TimestampToDateTime(string timestamp)
        {
            return TimestampToDateTime(Convert.ToInt64(timestamp));
        }
        #endregion

        #region Ticks和秒数互转

        //将Ticks转换为时间戳
        static public long TicksToSeconds(string ticks)
        {
            return TicksToSeconds(Convert.ToInt64(ticks));
        }

        static public long TicksToSeconds(long ticks)
        {
            return ticks / 10000000;
        }

        static public long SecondsToTicks(long seconds)
        {
            return seconds * 10000000;
        }
        #endregion

    }
}

