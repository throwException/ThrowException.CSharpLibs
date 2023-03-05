using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThrowException.CSharpLibs.ArgumentsParserLib;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserTest
{
    public class BaseOptions
    {
        [Option('d', "debug", Type = OptionType.Flag, DefaultValue = true, LongDescription = "Enables debug output")]
        public bool Debug { get; private set; }

        [Option('v', "verbose", Type = OptionType.CountedFlag, DefaultValue = 0, LongDescription = "Enables more verbose output (add multiple times for further effect)")]
        public uint Verbose { get; private set; }

        [Option('c', "config", Type = OptionType.IniConfigFile, Required = false, LongDescription = "Ini style config file with further options")]
        public string IniConfigFile { get; private set; }

        [Option("xml", Type = OptionType.XmlConfigFile, Required = false, LongDescription = "XML config file with further options.")]
        public string XmlConfigFile { get; private set; }
    }

    [Verb("test", true)]
    public class TestOptions : BaseOptions
    { 
        [Option('n', "number", LongDescription = "The number")]
        public int Number { get; private set; }

        [Option("name", LongDescription = "Name of the subject")]
        public string Name { get; private set; }

        [Option("file", Parser = typeof(TypeParserFilename), LongDescription = "Input file")]
        public string File { get; private set; }

        [Option("data", LongDescription = "Additional data")]
        public byte[] Data { get; private set; }
    }

    public enum Machine
    { 
        Server,
        Laptop,
        Desktop,
        Tablet,
    }

    [Verb("pong")]
    public class PongOptions : BaseOptions
    {
        [Option('x', "xray", DefaultValue = 27, LongDescription = "X-Ray strength; default value: 27")]
        public int Xray { get; private set; }

        [Option("command", LongDescription = "Issued command")]
        public string Command { get; private set; }

        [Option("machine")]
        public Machine Machine { get; private set; }

        [Option("height")]
        public double Height { get; private set; }

        [Option("width", Positional = 1, ShortValueDescription = "width of stuff", LongDescription = "Width of important stuff")]
        public double Width { get; private set; }
    }

    [Verb("clonk")]
    public class ClonkOptions : BaseOptions
    {
        [Option("things")]
        public IEnumerable<string> Things { get; private set; }

        [Option("factor", Positional = 1, ShortValueDescription = "factor to apply")]
        public decimal Factor { get; private set; }

        [Option("coefficients", Positional = 2, ShortValueDescription = "list of coefficents")]
        public IEnumerable<decimal> Coefficients { get; private set; }
    }

    [TestFixture()]
    public class ArgumentTest
    {
        [Test()]
        public static void TestConfigFiles()
        {
            const string iniConfigFile = "/tmp/config.ini";
            const string iniConfigText = "\ncommand = ola\n\n";
            File.WriteAllText(iniConfigFile, iniConfigText);

            const string xmlConfigFile = "/tmp/config.xml";
            const string xmlConfigText = "\n<config>\n<height>3.75</height>\n</config>\n\n";
            File.WriteAllText(xmlConfigFile, xmlConfigText);

            const string commandLine = "pong --machine Laptop -d --verbose -vvv -c /tmp/config.ini --xml /tmp/config.xml -- 1.75";

            Parser.Create<TestOptions, PongOptions, ClonkOptions>()
                .Parse(commandLine)
                .With<TestOptions>(t =>
                {
                    Assert.Fail("Wront test options parsed.");
                })
                .With<PongOptions>(t =>
                {
                    Assert.AreEqual(t.Machine, Machine.Laptop);
                    Assert.True(t.Debug);
                    Assert.AreEqual(t.Verbose, 4);
                    Assert.AreEqual(t.Command, "ola");
                    Assert.AreEqual(t.Height, 3.75d);
                })
                .With<ClonkOptions>(t =>
                {
                    Assert.Fail("Wront test options parsed.");

                });

            File.Delete(iniConfigFile);
            File.Delete(xmlConfigFile);
        }

        [Test]
        public static void SingleQuotedAndBytesTest()
        {
            const string filename = "/bin/bash";
            const string commandLine = "-d --verbose -vvv -n 3 --data cafebabe1337 --name 'great stuff' --file " + filename;

            Parser.Create<TestOptions, PongOptions, ClonkOptions>()
                .Parse(commandLine)
                .With<TestOptions>(t =>
                {
                    Assert.True(t.Debug);
                    Assert.AreEqual(t.Verbose, 4);
                    Assert.AreEqual(t.Data, new byte[] { 0xca, 0xfe, 0xba, 0xbe, 0x13, 0x37 });
                    Assert.AreEqual(t.Name, "great stuff");
                    Assert.AreEqual(t.File, filename);
                })
                .With<PongOptions>(t =>
                {
                    Assert.Fail("Wront test options parsed.");
                })
                .With<ClonkOptions>(t =>
                {
                    Assert.Fail("Wront test options parsed.");

                });
        }

        [Test]
        public static void MultipleValuesTest()
        {
            const string commandLine = "-d -vvv --things t1 t2 --things t3 --things t4 t5 t6 -- clonk 1.3 2.5 2.6 2.75";

            Parser.Create<TestOptions, PongOptions, ClonkOptions>()
                .LongUsage()
                .Parse(commandLine)
                .With<TestOptions>(t =>
                {
                    Assert.Fail("Wront test options parsed.");
                })
                .With<PongOptions>(t =>
                {
                    Assert.Fail("Wront test options parsed.");
                })
                .With<ClonkOptions>(t =>
                {
                    Assert.True(t.Debug);
                    Assert.AreEqual(t.Verbose, 3);
                    Assert.AreEqual(t.Things.Count(), 6);
                    Assert.AreEqual(t.Things.ElementAt(0), "t1");
                    Assert.AreEqual(t.Things.ElementAt(1), "t2");
                    Assert.AreEqual(t.Things.ElementAt(2), "t3");
                    Assert.AreEqual(t.Things.ElementAt(3), "t4");
                    Assert.AreEqual(t.Things.ElementAt(4), "t5");
                    Assert.AreEqual(t.Things.ElementAt(5), "t6");
                    Assert.AreEqual(t.Factor, 1.3d);
                    Assert.AreEqual(t.Coefficients.Count(), 3);
                    Assert.AreEqual(t.Coefficients.ElementAt(0), 2.5d);
                    Assert.AreEqual(t.Coefficients.ElementAt(1), 2.6d);
                    Assert.AreEqual(t.Coefficients.ElementAt(2), 2.75d);
                });
        }

        [Test]
        public static void BadCommandLineTest()
        {
            const string commandLine = "-d -vvv --things t1 t2 --things t3 --things t4 t5 t6 clonk 1.3 2.5 2.6 2.75";

            Assert.Throws<ArgumentsParseException>(() => {
                Parser.Create<TestOptions, PongOptions, ClonkOptions>()
                    .Parse(commandLine);
            });
        }

        [Test]
        public static void MissingArgumentTest()
        {
            const string commandLine = "pong";

            Assert.Throws<ArgumentsParseException>(() => {
                Parser.Create<TestOptions, PongOptions, ClonkOptions>()
                    .Parse(commandLine);
            });
        }
    }
}
