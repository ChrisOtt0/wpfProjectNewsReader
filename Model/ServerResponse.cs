using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class ServerResponse
    {
        private readonly int code;
        private readonly string message;

        public int Code { get => code; }
        public string Message { get => message; }

        public ServerResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
