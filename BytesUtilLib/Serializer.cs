using System;
using System.IO;
using System.Text;

namespace ThrowException.CSharpLibs.BytesUtilLib
{
    public class Serializer : IDisposable
    {
        private System.IO.Stream _stream;

        public System.IO.Stream BaseStream
        {
            get { return _stream; }
        }

        public Serializer(System.IO.Stream stream)
        {
            _stream = stream;
        }

        public Serializer()
        {
            _stream = new MemoryStream();
        }

        public Serializer(byte[] data)
        {
            _stream = new MemoryStream(data);
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        public string Hex
        {
            get
            {
                return Data.ToHexString();
            }
        }

        public byte[] Data
        {
            get 
            {
                if (_stream is MemoryStream)
                {
                    return ((MemoryStream)_stream).ToArray();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void Write(Guid value)
        {
            Write(value.ToByteArray());
        }

        public void Write(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
        }

        public void Pad(int blockSize)
        {
            var length = _stream.Length;
            if (length % blockSize != 0)
            {
                var padding = blockSize - (_stream.Length % blockSize);
                Write(new byte[padding]);
            }
        }

        public void Write(int value)
        {
            Write(LittleConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            Write(LittleConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            Write(value ? (byte)0 : (byte)1);
        }

        public void Write(short value)
        {
            Write(LittleConverter.GetBytes(value));
        }

        public void Write(ushort value)
        {
            Write(LittleConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            Write(LittleConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            Write(LittleConverter.GetBytes(value));
        }

        public void Write(byte value)
        {
            Write(new byte[]{ value });
        }

        public void Write(sbyte value)
        {
            Write(new byte[]{ (byte)value });
        }

        public void WritePrefixed(byte[] value)
        {
            Write(value.Length);
            Write(value);
        }

        public void Write(string value, int length)
        {
            if (value.Length > length)
                throw new ArgumentException();
            
            Write(Encoding.UTF8.GetBytes(value).Pad(length));
        }

        public void WritePrefixed(string value)
        {
            WritePrefixed(Encoding.UTF8.GetBytes(value));
        }

        public void Write(DateTime value)
        {
            Write(value.Ticks);
        }

        public void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }
    }
}

