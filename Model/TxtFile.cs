using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    internal class TxtFile : FileAdapter
    {
        public override void AppendTextToFile(string path, string text)
        {
            File.AppendAllText(path, text);
        }

        public override string GetAllTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        public override void WriteTextToFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}
