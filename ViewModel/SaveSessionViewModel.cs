using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using wpfProjectNewsReader.Model;
using wpfProjectNewsReader.Tools;
using wpfProjectNewsReader.View;

namespace wpfProjectNewsReader.ViewModel
{
    public class SaveSessionViewModel : Bindable
    {
        private SessionSingleton ss = SessionSingleton.GetInstance();

        public void OK(object parameter)
        {
            ss.Pin = parameter as string;
        }
    }
}
