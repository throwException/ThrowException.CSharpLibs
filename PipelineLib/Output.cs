using System;
using System.Threading;
using ThrowException.CSharpLibs.BytesUtilLib;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public abstract class Output : IDisposable
    {
        private readonly Stage _source;
        private Thread _processor;

        public bool Failed { get; private set; }

        protected Output(Stage source)
        {
            Failed = false;
            _source = source;
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

        protected abstract void ProcessBytes(byte[] buffer, int count);

        private void Process()
        {
            var buffer = new byte[65536];
            int bytes = 1;

            while ((bytes > 0) || (!_source.EndOfOutput))
            {
                try
                {
                    bytes = _source.Output.Read(buffer, 0, buffer.Length);
                    ProcessBytes(buffer, bytes);
                }
                catch
                {
                    Failed = true;
                    bytes = 0;
                }
            }
        }

        public void Dispose()
        {
            Close();
        }

        protected abstract void Close();

        public abstract string Text { get; }

        public abstract byte[] Data { get; }

        public string Hex { get { return Data.ToHexString(); } }
    }
}
