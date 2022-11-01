using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using wpfProjectNewsReader.Tools;
using wpfProjectNewsReader.View;

namespace wpfProjectNewsReader.ViewModel
{
    public class LoginViewModel : Bindable, ILoginViewModel
    {
        #region Commands
        public AddCommand LoginCommand { get; set; } = new AddCommand(() =>
        {
            // If statements to very login first
            if (false)
            {

            }

            // If success
            ((App)App.Current).ChangeUserControl(App.container.Resolve<MainMenuView>());
        });
        #endregion
    }
}
