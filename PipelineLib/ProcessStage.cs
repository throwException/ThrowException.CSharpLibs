using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class ProcessStage : Stage
    {
        private readonly List<string> _messages;
        private readonly string _binaryPath;
        private readonly List<KeyValuePair<string, string>> _environment;
        private readonly string _arguments;
        private Process _process;
        private Thread _stderrReader;

        public ProcessStage(string label, string binaryPath, IEnumerable<KeyValuePair<string, string>> environment, string argumentsFormat, params object[] argumentsArguments)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            if (binaryPath == null) throw new ArgumentNullException(nameof(binaryPath));
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (argumentsFormat == null) throw new ArgumentNullException(nameof(argumentsFormat));
            if (argumentsArguments == null) throw new ArgumentNullException(nameof(argumentsArguments));

            Label = label;
            _messages = new List<string>();
            _binaryPath = binaryPath;
            _environment = new List<KeyValuePair<string, string>>(environment);
            _arguments = string.Format(argumentsFormat, argumentsArguments);
        }

        public ProcessStage(string label, string binaryPath, string argumentsFormat, params object[] argumentsArguments)
            : this(label, binaryPath, new KeyValuePair<string, string>[0], argumentsFormat, argumentsArguments)
        {
        }

        public ProcessStage WithEnvironment(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            _environment.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        private void StderrReader()
        {
            while (!_process.StandardError.EndOfStream)
            {
                _messages.Add(_process.StandardError.ReadLine());
            }
        }

        public override Stream Input 
        {
            get { return _process.StandardInput.BaseStream; }
        }

        public override Stream Output
        {
            get { return _process.StandardOutput.BaseStream; }
        }

        public override bool Done
        {
            get { return _process.HasExited; }
        }

        public override bool Failed
        {
            get { return _process.HasExited && _process.ExitCode != 0; }
        }

        public override IEnumerable<string> Messages
        {
            get { return _messages; }
        }

        public override bool EndOfOutput
        {
            get { return _process.StandardOutput.EndOfStream; }
        }

        public override void Terminate()
        {
            if (!_process.HasExited)
            {
                try
                {
                    _process.Kill();
                }
                catch
                {
                    /* ignore any errors */
                }
            }

            _process.WaitForExit();
            _stderrReader.Join();
        }

        public override void Start()
        {
            try
            {
                var info = new ProcessStartInfo(_binaryPath, _arguments)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };
                foreach (var variable in _environment)
                {
                    info.Environment.Add(variable.Key, variable.Value);
                }
                _process = Process.Start(info);
                _stderrReader = new Thread(StderrReader);
                _stderrReader.Start();
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                throw;
            }
        }

        public override void WaitForDone()
        {
            _process.WaitForExit();
            _stderrReader.Join();
        }

        public override void Close()
        {
        }
    }
}
