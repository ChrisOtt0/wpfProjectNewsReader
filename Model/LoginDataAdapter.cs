using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    internal class LoginDataAdapter : ILoginDataAdapter
    {
        string conf = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Newsreader\\Session\\login.conf";

        public LoginData RetrieveData(string pin)
        {
            if (!File.Exists(conf))
                return new LoginData("", "", "", "");

            string encryptedData = File.ReadAllText(conf);
            string decryptedData = SecurityTools.DecryptString(encryptedData, pin);
            string[] content = decryptedData.Split("\n");

            return new LoginData(content[0].Trim(), content[1].Trim(), content[2].Trim(), content[3].Trim());
        }

        public void SaveData(LoginData ld, string pin)
        {
            string data = $"{ld.UserName}\n{ld.Password}\n{ld.ServerName}\n{ld.ServerPort}";
            string encryptedData = SecurityTools.EncryptString(data, pin);
            File.WriteAllTextAsync(conf, encryptedData);
        }

        public void DeleteData()
        {
            if (!File.Exists(conf))
                return;
            File.Delete(conf);
        }

        public bool ConfExists()
        {
            return File.Exists(conf);
        }
    }
}
