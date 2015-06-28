﻿using System;

namespace Orchard.Glimpse.Extensions {
    public static class TimespanExtensions
    {
        public static string ToTimingString(this TimeSpan timespan) {
            return timespan.TotalMilliseconds.ToTimingString();
        }
        public static string ToTimingString(this double milliseconds)
        {
            return string.Format("{0:0,0.00} ms", milliseconds);
        }
    }
}