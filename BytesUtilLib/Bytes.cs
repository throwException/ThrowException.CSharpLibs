﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ThrowException.CSharpLibs.BytesUtilLib
{
    public static class Bytes
    {
        public static string PadInt(this int value, int length)
        {
            var stringValue = value.ToString();

            while (stringValue.Length < length)
            {
                stringValue = "0" + stringValue;
            }

            return stringValue;
        }

        public static byte[] HashSha256(this byte[] bytes)
        {
            using (var sha = new SHA256Managed())
            {
                return sha.ComputeHash(bytes);
            }
        }

        public static byte[] Pad(this byte[] bytes, int length)
        {
            bytes.ArgumentNotNull();
            var result = new byte[Math.Max(bytes.Length, length)];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }

        public static sbyte[] ToSbytes(this byte[] bytes)
        {
            bytes.ArgumentNotNull();
            var sbytes = new sbyte[bytes.Length];
            Buffer.BlockCopy(bytes, 0, sbytes, 0, bytes.Length);
            return sbytes;
        }

        public static byte[] ToBytes(this sbyte[] sbytes)
        {
            sbytes.ArgumentNotNull();
            var bytes = new byte[sbytes.Length];
            Buffer.BlockCopy(sbytes, 0, bytes, 0, sbytes.Length);
            return bytes;
        }

        public static void ArgumentHasBytes(this byte[] argument, int count)
        {
            if (argument == null)
            {
                throw new ArgumentNullException();
            }

            if (argument.Length != count)
            {
                throw new ArgumentException("Wrong byte count.");
            }
        }

        public static byte[] TryParseHexBytes(this string text)
        {
            text.ArgumentNotNull();

            if (text.Length % 2 != 0)
            {
                return null;
            }

            var buffer = new byte[text.Length / 2];

            for (int index = 0; index < text.Length / 2; index++)
            {
                if (byte.TryParse(text.Substring(index * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte value))
                {
                    buffer[index] = value;
                }
                else
                {
                    return null;
                }
            }

            return buffer;
        }

        public static byte[] ParseHexBytes(this string text)
        {
            text.ArgumentNotNull();

            if (text.Length % 2 != 0)
            {
                throw new FormatException("Invalid text.");
            }

            var buffer = new byte[text.Length / 2];

            for (int index = 0; index < text.Length / 2; index++)
            {
                buffer[index] = byte.Parse(text.Substring(index * 2, 2), NumberStyles.HexNumber);
            }

            return buffer;
        }

        public static string ToHexString(this byte[] data)
        {
            return string.Join(string.Empty, data.Select(b => string.Format("{0:x2}", b)).ToArray());
        }

        public static string ToHexStringGroupFour(this byte[] data)
        {
            data.ArgumentNotNull();

            var parts = new List<string>();

            for (int i = 0; i < data.Length; i += 4)
            {
                var length = Math.Min(4, data.Length - i);
                var part = data.Part(i, length).ToHexString();
                parts.Add(part);
            }

            return string.Join(" ", parts);
        }

        public static byte[] Concat(this byte[] part0, params byte[][] parts)
        {
            if (part0 == null)
                throw new ArgumentNullException("part0");
            if (parts == null)
                throw new ArgumentNullException("parts");

            byte[] data = part0;

            foreach (byte[] part in parts)
            {
                data = data.Concat(part);
            }

            return data;
        }

        public static byte[] Concat(this byte[] data1, byte[] data2)
        {
            if (data1 == null)
                throw new ArgumentNullException("data1");
            if (data2 == null)
                throw new ArgumentNullException("data2");

            byte[] concat = new byte[data1.Length + data2.Length];
            System.Buffer.BlockCopy(data1, 0, concat, 0, data1.Length);
            System.Buffer.BlockCopy(data2, 0, concat, data1.Length, data2.Length);
            return concat;
        }

        public static byte[] Part(this byte[] data, int index, int length)
        {
            data.ArgumentNotNull();

            if (index < 0 || index + length > data.Length)
                throw new FormatException("Index or length out of range.");

            byte[] part = new byte[length];
            System.Buffer.BlockCopy(data, index, part, 0, length);
            return part;
        }

        public static byte[] Part(this byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (index < 0 || index > data.Length)
                throw new ArgumentException("Index or length out of range.");

            return data.Part(index, data.Length - index);
        }

        public static bool Equal(this byte[] data1, byte[] data2)
        {
            if (data1 == null)
                throw new ArgumentNullException("data1");
            if (data2 == null)
                throw new ArgumentNullException("data2");

            if (data1.Length == data2.Length)
            {
                bool equal = true;

                for (int index = 0; index < data1.Length; index++)
                {
                    equal &= data1[index] == data2[index];
                }

                return equal;
            }
            else
            {
                return false;
            }
        }

        public static bool StartWith(this byte[] data1, byte[] data2)
        {
            if (data1 == null)
                throw new ArgumentNullException("data1");
            if (data2 == null)
                throw new ArgumentNullException("data2");

            return Equal(Part(data1, 0, data2.Length), data2);
        }

        public static byte[] Xor(this byte[] data1, byte[] data2)
        {
            if (data1 == null)
                throw new ArgumentNullException("data1");
            if (data2 == null)
                throw new ArgumentNullException("data2");

            byte[] output = new byte[data1.Length];

            for (int i = 0; i < data1.Length; i++)
            {
                output[i] = (byte)((int)data1[i] ^ (int)data2[i]);
            }

            return output;
        }

        public static void XorAdd(this byte[] data1, byte[] data2)
        {
            if (data1 == null)
                throw new ArgumentNullException("data1");
            if (data2 == null)
                throw new ArgumentNullException("data2");
            if (data1.Length != data2.Length)
                throw new ArgumentException("Data length mismatch");

            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = (byte)((int)data1[i] ^ (int)data2[i]);
            }
        }

        public static void XorAdd(this int[] data1, int[] data2)
        {
            if (data1 == null)
                throw new ArgumentNullException("data1");
            if (data2 == null)
                throw new ArgumentNullException("data2");
            if (data1.Length != data2.Length)
                throw new ArgumentException("Data length mismatch");

            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = data1[i] ^ data2[i];
            }
        }

    public static void XorAdd(this uint[] data1, uint[] data2)
    {
        if (data1 == null)
            throw new ArgumentNullException("data1");
        if (data2 == null)
            throw new ArgumentNullException("data2");
        if (data1.Length != data2.Length)
            throw new ArgumentException("Data length mismatch");

        for (int i = 0; i < data1.Length; i++)
        {
            data1[i] = data1[i] ^ data2[i];
        }
    }

        public static byte[] Copy(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            byte[] newData = new byte[data.Length];
            Buffer.BlockCopy(data, 0, newData, 0, data.Length);
            return newData;
        }

        public static byte[] Expand(this byte[] data, int length)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (length < data.Length)
                throw new ArgumentException("Length must be greater or equal to the length of data.");

            byte[] newData = new byte[length];
            Buffer.BlockCopy(data, 0, newData, 0, data.Length);
            return newData;
        }

        public static void Display(this byte[] data, string name)
        {
            data.ArgumentNotNull();
            name.ArgumentNotNull();

            string firstBytes = string.Empty;

            for (int index = 0; index < Math.Min(data.Length, 8); index++)
            {
                firstBytes += string.Format("{0:x2} ", data[index]);
            }

            string lastBytes = string.Empty;

            for (int index = Math.Max(8, data.Length - 8); index < data.Length; index++)
            {
                lastBytes += string.Format("{0:x2} ", data[index]);
            }

            string text = string.Format("{0}: {1}... {2}length {3}", name, firstBytes, lastBytes, data.Length);

            Console.WriteLine(text);
        }

        public static bool AreEqual(this byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if (b == null || a == null)
            {
                return false;
            }
            else if (a.Length == b.Length)
            {
                for (int index = 0; index < a.Length; index++)
                {
                    if (a[index] != b[index])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AreEqual(this sbyte[] a, sbyte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if (b == null || a == null)
            {
                return false;
            }
            else if (a.Length == b.Length)
            {
                for (int index = 0; index < a.Length; index++)
                {
                    if (a[index] != b[index])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Zeroize(this uint[] data)
        {
            data.ArgumentNotNull();

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }

        public static void Zeroize(this int[] data)
        {
            data.ArgumentNotNull();

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }

        public static void Zeroize(this byte[] data)
        {
            data.ArgumentNotNull();

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }

        public static byte[] ToBytesOrNull(this Stream stream)
        {
            if (stream == null)
            {
                return null; 
            }
            else
            {
                return stream.ToBytes(); 
            }
        }


        public static byte[] ToBytes(this Stream stream)
        {
            stream.ArgumentNotNull();

            using (var memory = new MemoryStream())
            {
                var count = 1;
                var buffer = new byte[1024];

                while (count > 0)
                {
                    count = stream.Read(buffer, 0, buffer.Length);

                    if (count > 0)
                    {
                        memory.Write(buffer, 0, count);
                    }
                }

                return memory.ToArray();
            }
        }

        public static byte[] Random(int count)
        {
            using (var random = RandomNumberGenerator.Create())
            {
                var buffer = new byte[count];
                random.GetBytes(buffer);
                return buffer;
            }
        }

        public static string FormatBytes(this long bytes)
        { 
            if (bytes >= 1024L * 1024L * 1024L * 1024L)
            {
                return string.Format("{0:0.00} TiB", bytes / (1024d * 1024d * 1024d * 1024d));
            }
            else if (bytes >= 1024L * 1024L * 1024L)
            {
                return string.Format("{0:0.00} GiB", bytes / (1024d * 1024d * 1024d));
            }
            else if (bytes >= 1024L * 1024L)
            {
                return string.Format("{0:0.00} MiB", bytes / (1024d * 1024d));
            }
            else if (bytes >= 1024L)
            {
                return string.Format("{0:0.0} KiB", bytes / 1024d);
            }
            else
            {
                return bytes + " Bytes";
            }
        }

        public static string FormatBytesOf(this long bytes, long total)
        {
            long max = Math.Max(bytes, total);

            if (max >= 1024L * 1024L * 1024L * 1024L)
            {
                return string.Format("{0:0.00} / {1:0.00} TiB", 
                    bytes / (1024d * 1024d * 1024d * 1024d),
                    total / (1024d * 1024d * 1024d * 1024d));
            }
            else if (max >= 1024L * 1024L * 1024L)
            {
                return string.Format("{0:0.00} / {1:0.00} GiB",
                    bytes / (1024d * 1024d * 1024d),
                    total / (1024d * 1024d * 1024d));
            }
            else if (max >= 1024L * 1024L)
            {
                return string.Format("{0:0.00} / {1:0.00} MiB",
                    bytes / (1024d * 1024d),
                    total / (1024d * 1024d));
            }
            else if (max >= 1024L)
            {
                return string.Format("{0:0.0} / {1:0.0} KiB",
                    bytes / 1024d,
                    total / 1024d);
            }
            else
            {
                return bytes + " / " + total + " Bytes";
            }
        }
    }
}
