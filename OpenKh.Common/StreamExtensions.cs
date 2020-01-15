using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xe.BinaryMapper;

namespace OpenKh.Common
{
    public static class StreamExtensions
    {
        public static T FromBegin<T>(this T stream) where T : Stream => stream.SetPosition(0);

        public static T SetPosition<T>(this T stream, int position) where T : Stream
        {
            stream.Position = position;
            return stream;
        }

        public static T AlignPosition<T>(this T stream, int alignValue) where T : Stream =>
            stream.SetPosition(Helpers.Align((int)stream.Position, alignValue));

        public static List<T> ReadList<T>(this Stream stream, int offset, int count)
            where T : class
        {
            stream.Position = offset;
            return stream.ReadList<T>(count);
        }

        public static List<T> ReadList<T>(this Stream stream, int count)
            where T : class
        {
            return Enumerable.Range(0, count)
                .Select(x => BinaryMapping.ReadObject<T>(stream, (int)stream.Position))
                .ToList();
        }

        public static int ReadInt32(this Stream stream) =>
            new BinaryReader(stream).ReadInt32();

        public static uint ReadUInt32(this Stream stream) =>
            new BinaryReader(stream).ReadUInt32();

        public static List<int> ReadInt32List(this Stream stream, int offset, int count)
        {
            stream.Position = offset;
            return stream.ReadInt32List(count);
        }

        public static List<int> ReadInt32List(this Stream stream, int count)
        {
            var reader = new BinaryReader(stream);
            return Enumerable.Range(0, count)
                .Select(x => reader.ReadInt32())
                .ToList();
        }

        public static byte[] ReadBytes(this Stream stream) =>
            stream.ReadBytes((int)(stream.Length - stream.Position));

        public static byte[] ReadBytes(this Stream stream, int length)
        {
            var data = new byte[length];
            stream.Read(data, 0, length);
            return data;
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            var data = stream.SetPosition(0).ReadBytes();
            stream.Position = 0;
            return data;
        }

        public static string ReadString(this Stream stream, int maxLength) =>
            stream.ReadString(maxLength, Encoding.UTF8);

        public static string ReadString(this Stream stream, int maxLength, Encoding encoding)
        {
            var data = stream.ReadBytes(maxLength);
            var terminatorIndex = Array.FindIndex(data, x => x == 0);
            return encoding.GetString(data, 0, terminatorIndex < 0 ? maxLength : terminatorIndex);
        }

        public static int WriteList<T>(this Stream stream, IEnumerable<T> items)
            where T : class
        {
            var oldPosition = (int)stream.Position;
            foreach (var item in items)
                BinaryMapping.WriteObject<T>(stream, item, oldPosition);

            return (int)stream.Position - oldPosition;
        }

        public static void Write(this Stream stream, byte[] data) =>
            stream.Write(data, 0, data.Length);

        public static int Write(this Stream stream, IEnumerable<int> items)
        {
            var oldPosition = (int)stream.Position;
            var writer = new BinaryWriter(stream);
            foreach (var item in items)
                writer.Write(item);

            return (int)stream.Position - oldPosition;
        }

        public static int Write(this Stream stream, int value) =>
            stream.Write((uint)value);

        public static int Write(this Stream stream, uint value)
        {
            var oldPosition = stream.Position;

            stream.WriteByte((byte)((value >> 0) & 0xFF));
            stream.WriteByte((byte)((value >> 8) & 0xFF));
            stream.WriteByte((byte)((value >> 16) & 0xFF));
            stream.WriteByte((byte)((value >> 24) & 0xFF));

            return (int)(stream.Position - oldPosition);
        }

        public static void Copy(this Stream source, Stream destination, int length, int bufferSize = 65536)
        {
            int read;
            byte[] buffer = new byte[Math.Min(length, bufferSize)];

            while ((read = source.Read(buffer, 0, Math.Min(length, bufferSize))) != 0)
            {
                destination.Write(buffer, 0, read);
                length -= read;
            }
        }
    }
}
