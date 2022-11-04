using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfProjectNewsReader.Model;
using wpfProjectNewsReader.Tools;

namespace wpfProjectNewsReader.ViewModel
{
    internal class OpenSavedSessionViewModel : Bindable
    {
        private SessionSingleton ss = SessionSingleton.GetInstance();

        public void OK(object parameter)
        {
            ss.Pin = parameter as string;
        }
    }
}
