using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Linq;
using System.IO;
using ThrowException.CSharpLibs.LogLib;
using LogLibLogger = ThrowException.CSharpLibs.LogLib.Logger;

namespace LogTest
{
    [TestFixture()]
    public class LogTests
    {
        [Test()]
        public void FireAndForgetTest()
        {
            using (var log = new LogLibLogger())
            {
                log.Verbose("test {0}", 1);
                log.Debug("test {0}", 1);
                log.Info("test {0}", 1);
                log.Notice("test {0}", 1);
                log.Warning("test {0}", 1);
                log.Error("test {0}", 1);
                log.Error(new Exception("test"));
                log.Critical("test {0}", 1);
                log.Critical(new Exception("test"));
            }
        }

        [Test()]
        public void LogFileTest()
        {
            foreach (var filename in Directory.GetFiles("/tmp", "testlogfile*"))
            {
                File.Delete(filename);
            }
            using (var log = new LogLibLogger())
            {
                log.EnableLogFile(LogSeverity.Verbose, "/tmp/testlogfile");
                log.Verbose("test {0}", 1);
                log.Debug("test {0}", 1);
                log.Info("test {0}", 1);
                log.Notice("test {0}", 1);
                log.Warning("test {0}", 1);
                log.Error("test {0}", 1);
                log.Error(new Exception("test"));
                log.Critical("test {0}", 1);
                log.Critical(new Exception("test"));
            }
            var logFile = Directory.GetFiles("/tmp", "testlogfile*").Single();
            var lines = File.ReadAllLines(logFile);
            Assert.AreEqual(lines.Count(), 9);
        }
    }
}
