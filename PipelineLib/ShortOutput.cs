using System;
using System.IO;
using System.Text;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class ShortOutput : Output
    {
        private readonly MemoryStream _stream;

        public ShortOutput(Stage source) : base(source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            _stream = new MemoryStream();
        }

        public override string Text
        {
            get { return Encoding.UTF8.GetString(Data); }
        }

        public override byte[] Data
        {
            get { return _stream.ToArray(); }
        }

        protected override void ProcessBytes(byte[] buffer, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            _stream.Write(buffer, 0, count);
        }
    }
}
