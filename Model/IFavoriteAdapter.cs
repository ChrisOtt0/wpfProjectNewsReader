using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public interface IFavoriteAdapter
    {
        public void SaveFavorites(IEnumerable<string> favorites);

        public IEnumerable<string> LoadFavorites();
    }
}
