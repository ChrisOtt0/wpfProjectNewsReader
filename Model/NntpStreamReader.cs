using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class NntpStreamReader : StreamReader
    {
        public NntpStreamReader(Stream stream) 
            : base(stream, Encoding.UTF8, true) { }

        public NntpStreamReader(string path) 
            : base(path) { }

        public NntpStreamReader(Stream stream, bool detectEncodingFromByteOrderMarks)
            : base(stream, detectEncodingFromByteOrderMarks) { }

        public NntpStreamReader(Stream stream, Encoding encoding)
            : base(stream, encoding) { }

        public NntpStreamReader(string path, bool detectEncodingFromByteOrderMarks)
            : base(path, detectEncodingFromByteOrderMarks) { }

        public NntpStreamReader(string path, Encoding encoding)
            : base(path, encoding) { }

        public NntpStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
            : base(stream, encoding, detectEncodingFromByteOrderMarks) { }

        public NntpStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks) { }

        public NntpStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
            : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }

        public NntpStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
            : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }

        public NntpStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen)
            : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen) { }

        public IEnumerable<string> ReadAllLines()
        {
            string readLine;
            while ((readLine = this.ReadLine()) != null)
            {
                if (readLine == ".") break;

                if (readLine.StartsWith(".."))
                    readLine = readLine.Substring(1);

                yield return readLine;
            }
        }
    }
}
