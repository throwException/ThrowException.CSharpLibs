using System;
using System.Collections.Generic;
using System.Linq;

namespace ThrowException.CSharpLibs.BytesUtilLib
{
    public class SpeedCounter
    {
        private class Entry
        { 
            public DateTime Time { get; private set; }
            public long Bytes { get; private set; }

            public Entry(long bytes)
            {
                Time = DateTime.UtcNow;
                Bytes = bytes;
            }
        }

        private readonly Queue<Entry> _entries;
        private Entry _latest = null;

        public TimeSpan Window { get; set; } = TimeSpan.FromSeconds(10d);
        public string TextDays { get; set; } = "days";
        public string TextHours { get; set; } = "hours";
        public string TextMinutes { get; set; } = "minutes";
        public string TextLessThanAMinute { get; set; } = "less than a minute";

        public SpeedCounter()
        {
            _entries = new Queue<Entry>();
        }

        public void Add(long bytes)
        {
            _latest = new Entry(bytes);
            _entries.Enqueue(_latest);

            while (_entries.Any() && DateTime.UtcNow.Subtract(_entries.Peek().Time) > Window)
            {
                _entries.Dequeue();
            }
        }

        public TimeSpan EstimatedTimespanRemaining(long totalBytes)
        {
            var bytesPerSecond = BytesPerSecond;
            var remainingBytes = totalBytes - _latest.Bytes;
            if (bytesPerSecond < 0.001d)
            {
                return TimeSpan.FromDays(1000);
            }
            else
            {
                return TimeSpan.FromSeconds(remainingBytes / bytesPerSecond);
            }
        }

        public string EstimatedTimeRemaining(long totalBytes)
        {
            var eta = EstimatedTimespanRemaining(totalBytes);

            if (eta.TotalDays >= 1d)
            {
                var days = Math.Floor(eta.TotalDays);
                var hours = Math.Round(eta.TotalHours - (days * 24d));
                return string.Format("{0} {1} {2} {3}", days, TextDays, hours, TextHours);
            }
            else if (eta.TotalHours >= 1d)
            {
                var hours = Math.Floor(eta.TotalHours);
                var minutes = Math.Round(eta.TotalMinutes - (hours * 60d));
                return string.Format("{0} {1} {2} {3}", hours, TextHours, minutes, TextMinutes);
            }
            else if (eta.TotalMinutes >= 1d)
            {
                var minutes = Math.Round(eta.TotalMinutes);
                return string.Format("{0} {1}", minutes, TextMinutes);
            }
            else
            {
                return TextLessThanAMinute;
            }
        }

        public string DataPerSecond
        { 
            get
            {
                var value = BytesPerSecond;

                if (value >= 1024d * 1024d * 1024d)
                {
                    return string.Format("{0:0.00} GiB/s", value / (1024d * 1024d * 1024d));
                }
                if (value >= 1024d * 1024d)
                {
                    return string.Format("{0:0.00} MiB/s", value / (1024d * 1024d));
                }
                if (value >= 1024d)
                {
                    return string.Format("{0:0.00} KiB/s", value / 1024d);
                }
                else
                {
                    return string.Format("{0:0.00} Bytes/s", value);
                }
            }
        }

        public double BytesPerSecond
        { 
            get
            {
                if (_entries.Count >= 2)
                {
                    var earliest = _entries.Peek();
                    var deltaSeconds = (_latest.Time - earliest.Time).TotalSeconds;
                    var deltaBytes = _latest.Bytes - earliest.Bytes;
                    return deltaBytes / deltaSeconds;
                }
                else
                {
                    return 0d;
                }
            }
        }
    }
}
