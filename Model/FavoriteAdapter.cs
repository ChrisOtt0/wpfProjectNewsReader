using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfProjectNewsReader.Tools;

namespace wpfProjectNewsReader.Model
{
    public class FavoriteAdapter : IFavoriteAdapter
    {
        SessionSingleton session = SessionSingleton.GetInstance();
        FileAdapter fileAdapter = new TxtFile();
        string? currentUser = null;

        public FavoriteAdapter(string user)
        {
            currentUser = user;
        }

        public void SaveFavorites(IEnumerable<string> favorites)
        {
            if (favorites == null) return;
            string text = "";
            foreach (string s in favorites)
            {
                text += s + "\n";
            }

            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + 
                "\\Documents\\NewsReader\\Groups\\" + 
                SecurityTools.HashString(currentUser) + ".txt";

            string encryptedText = SecurityTools.EncryptString(text, session.Pin);

            fileAdapter.WriteTextToFile(path, encryptedText);
        }

        public IEnumerable<string> LoadFavorites()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                "\\Documents\\NewsReader\\Groups\\" +
                SecurityTools.HashString(currentUser) + ".txt";

            if (!File.Exists(path)) yield break;

            string text = fileAdapter.GetAllTextFromFile(path);

            string decryptedText = SecurityTools.DecryptString(text, session.Pin);

            string[] lines = decryptedText.Split('\n');

            foreach (string line in lines)
            {
                if (line == "" || line == " ") continue;
                if (line != null) yield return line;
            }
        }
    }
}
