using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public interface ILoginDataAdapter
    {
        public LoginData RetrieveData(string pin);

        public void SaveData(LoginData ld, string pin);

        public void DeleteData();

        public bool ConfExists();
    }
}
