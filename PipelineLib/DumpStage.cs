using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class DumpStage : Stage
    {
        private readonly Stream _file;

        public DumpStage(string filename)
        {
            _file = File.OpenRead(filename);
        }

        public override void Close()
        {
            _file.Close();
        }

        public override Stream Input => throw new NotImplementedException();

        public override Stream Output => _file;

        public override bool EndOfOutput { get { return _file.Position >= _file.Length; } }

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
