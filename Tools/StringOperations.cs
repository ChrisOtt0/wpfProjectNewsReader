using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Tools
{
    public static class StringOperations
    {
        public static string ConvertTextToArticle(string from, string group, string subject, string content)
        {
            return $"From: {from}\r\nNewsgroups: {group}\r\nSubject: {subject}\r\n\r\n{content}\r\n.\r\n";
        }
    }
}
