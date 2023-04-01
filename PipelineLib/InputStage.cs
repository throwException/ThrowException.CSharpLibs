using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class InputStage : Stage
    {
        private readonly MemoryStream _stream;

        public InputStage(byte[] data)
        {
            Label = "input";
            _stream = new MemoryStream(data);
        }

        public override void Close()
        {
            _stream.Close();
        }

        public InputStage(string value)
            : this(Encoding.UTF8.GetBytes(value))
        { 
        }

        public override Stream Input => throw new NotImplementedException();

        public override Stream Output => _stream;

        public override bool EndOfOutput { get { return _stream.Position >= _stream.Length; } }

        public override bool Done { get { return EndOfOutput; } }

        public override bool Failed { get { return false; } }

        public override IEnumerable<string> Messages { get { return new string[0]; } }

        public override void Start()
        {
        }

        public override void Terminate()
        {
        }

        public override void WaitForDone()
        {
        }
    }
}
