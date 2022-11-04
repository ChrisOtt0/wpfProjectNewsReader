using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class LoginData
    {
        private readonly string userName;
        private readonly string password;
        private readonly string serverName;
        private readonly string serverPort;

        public string UserName { get => userName; }
        public string Password { get => password; }
        public string ServerName { get => serverName; }
        public string ServerPort { get => serverPort; }

        public LoginData(string userName, string password, string serverName, string serverPort)
        {
            this.userName = userName;
            this.password = password;
            this.serverName = serverName;
            this.serverPort = serverPort;
        }

        public bool IsEmpty()
        {
            return
                (userName == null || userName == "") ||
                (password == null || password == "") ||
                (serverName == null || serverName == "") ||
                (serverPort == null || serverName == "");
        }
    }
}
