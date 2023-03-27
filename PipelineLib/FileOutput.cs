using System;
using System.IO;
using System.Text;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class FileOutput : Output
    {
        private readonly Stream _stream;

        public FileOutput(Stage source, string filename) : base(source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            _stream = File.OpenWrite(filename);
        }

        public override string Text
        {
            get { return string.Empty; }
        }

        public override byte[] Data
        {
            get { return new byte[0]; }
        }

        protected override void Close()
        {
            _stream.Close();
        }

        protected override void ProcessBytes(byte[] buffer, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            _stream.Write(buffer, 0, count);
        }
    }
}
