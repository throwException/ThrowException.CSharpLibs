using System;
using System.Threading;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class StageConnection
    {
        private readonly Stage _source;
        private readonly Stage _destination;
        private Thread _processor;
        private long _totalBytes = 0;

        public bool Failed { get; private set; }
        public Action<long> UpdateTotalBytes { get; set; }

        public StageConnection(Stage source, Stage destination)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            Failed = false;
            _source = source;
            _destination = destination;
        }

        public void Start()
        {
            _processor = new Thread(Process);
            _processor.Start();
        }

        public bool Done
        {
            get { return !_processor.IsAlive; }
        }

        public void WaitForDone()
        {
            _processor.Join();
        }

        private void Process()
        {
            var buffer = new byte[65536];
            int bytes = 1;

            while ((bytes > 0) || (!_source.EndOfOutput))
            {
                try
                {
                    bytes = _source.Output.Read(buffer, 0, buffer.Length);
                    _destination.Input.Write(buffer, 0, bytes);
                    _totalBytes += bytes;
                    UpdateTotalBytes?.Invoke(_totalBytes);
                }
                catch
                {
                    Failed = true;
                    break;
                }
            }

            _destination.Input.Close();
        }
    }
}
