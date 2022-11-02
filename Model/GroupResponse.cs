using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class GroupResponse
    {
        private readonly int first;
        private readonly int last;
        private readonly string? name;

        public int First { get => this.first; }
        public int Last { get => this.last; }
        public string? Name { get => this.name; }

        public GroupResponse(string input)
        {
            var values = input.Split(' ');
            this.first = int.Parse(values[1]);
            this.last = int.Parse(values[2]);
            this.name = values[3];
        }
    }
}
