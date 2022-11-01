using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class ServerResponse
    {
        private readonly int code;
        private readonly string message;
        public ReadOnlyCollection<string>? Lines { get; }

        public int Code { get => code; }
        public string Message { get => message; }

        public ServerResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
            Lines = null;
        }

        public ServerResponse(int code, string message, ReadOnlyCollection<string> lines)
        {
            this.code = code;
            this.message = message;
            this.Lines = lines;
        }
    }
}
