using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public abstract class FileAdapter
    {
        public abstract string GetAllTextFromFile(string path);

        public abstract void WriteTextToFile(string path, string text);

        public abstract void AppendTextToFile(string path, string text);
    }
}
