using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class FavoriteManager
    {
        FileAdapter fileAdapter = new TxtFile();
        string? currentUser = null;

        public FavoriteManager(string user)
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

            fileAdapter.WriteTextToFile(path, text);
        }

        public IEnumerable<string> LoadFavorites()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                "\\Documents\\NewsReader\\Groups\\" +
                SecurityTools.HashString(currentUser) + ".txt";

            if (!File.Exists(path)) yield break;

            string text = fileAdapter.GetAllTextFromFile(path);
            string[] lines = text.Split('\n');

            foreach (string line in lines)
            {
                if (line != null) yield return line;
            }
        }
    }
}
