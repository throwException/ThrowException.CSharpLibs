using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class SpeedCounterTest
    {
        [Test()]
        public void ShortTest()
        {
            var speed = new SpeedCounter();
            speed.Add(0);
            speed.Add(1);
            speed.Add(2);
            Thread.Sleep(1000);
            speed.Add(5L * 1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);
            Assert.True(Regex.IsMatch(speed.DataPerSecond, @"^(4\.9[0-9])|(5.0[0-9]) MiB/s$"), "Unexpected speed {0}", speed.DataPerSecond);
            Assert.Greater(speed.EstimatedTimespanRemaining(10L * 1024L * 1024L), TimeSpan.FromSeconds(0.9d));
            Assert.Less(speed.EstimatedTimespanRemaining(10L * 1024L * 1024L), TimeSpan.FromSeconds(1.1d));
            Assert.AreEqual("less than a minute", speed.EstimatedTimeRemaining(10L * 1024L * 1024L));
            Assert.Greater(speed.EstimatedTimespanRemaining(500L * 1024L * 1024L), TimeSpan.FromSeconds(98d));
            Assert.Less(speed.EstimatedTimespanRemaining(500L * 1024L * 1024L), TimeSpan.FromSeconds(100d));
            Assert.AreEqual("2 minutes", speed.EstimatedTimeRemaining(500L * 1024L * 1024L));
            Assert.Greater(speed.EstimatedTimespanRemaining(5000L * 1024L * 1024L), TimeSpan.FromSeconds(995d));
            Assert.Less(speed.EstimatedTimespanRemaining(5000L * 1024L * 1024L), TimeSpan.FromSeconds(1005d));
            Assert.AreEqual("17 minutes", speed.EstimatedTimeRemaining(5000L * 1024L * 1024L));
            Assert.Greater(speed.EstimatedTimespanRemaining(50000L * 1024L * 1024L), TimeSpan.FromSeconds(9950d));
            Assert.Less(speed.EstimatedTimespanRemaining(50000L * 1024L * 1024L), TimeSpan.FromSeconds(10050d));
            Assert.AreEqual("2 hours 47 minutes", speed.EstimatedTimeRemaining(50000L * 1024L * 1024L));
            Assert.Greater(speed.EstimatedTimespanRemaining(500000L * 1024L * 1024L), TimeSpan.FromSeconds(99500d));
            Assert.Less(speed.EstimatedTimespanRemaining(500000L * 1024L * 1024L), TimeSpan.FromSeconds(100500d));
            Assert.AreEqual("1 days 4 hours", speed.EstimatedTimeRemaining(500000L * 1024L * 1024L));
        }

        [Test()]
        public void LongTest()
        {
            var speed = new SpeedCounter();
            speed.Window = TimeSpan.FromSeconds(0.5d);
            speed.Add(0);

            Thread.Sleep(200);
            speed.Add(1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);

            Thread.Sleep(200);
            speed.Add(2L * 1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);

            Thread.Sleep(200);
            speed.Add(3L * 1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);

            Thread.Sleep(200);
            speed.Add(4L * 1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);
        }

        [Test()]
        public void ChangingTest()
        {
            var speed = new SpeedCounter();
            speed.Window = TimeSpan.FromSeconds(0.3d);
            speed.Add(0);

            Thread.Sleep(200);
            speed.Add(1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);

            Thread.Sleep(200);
            speed.Add(2L * 1024L * 1024L);
            Thread.Sleep(200);
            speed.Add(2L * 1024L * 1024L);
            Thread.Sleep(200);
            speed.Add(2L * 1024L * 1024L);
            Assert.AreEqual(0.0d, speed.BytesPerSecond);

            Thread.Sleep(200);
            speed.Add(3L * 1024L * 1024L);
            Thread.Sleep(200);
            speed.Add(4L * 1024L * 1024L);
            Thread.Sleep(200);
            speed.Add(5L * 1024L * 1024L);
            Assert.Greater(speed.BytesPerSecond, 4.9d * 1024d * 1024d);
            Assert.Less(speed.BytesPerSecond, 5.1d * 1024d * 1024d);
        }
    }
}
