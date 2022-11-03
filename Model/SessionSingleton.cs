using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    internal class SessionSingleton
    {
        private static SessionSingleton? instance;
        private string? username = "";

        public string? Username { get => username; set => username = value; }

        private SessionSingleton() { }

        public static SessionSingleton? GetInstance()
        {
            if (instance == null) instance = new SessionSingleton();
            return instance;
        }
    }
}
