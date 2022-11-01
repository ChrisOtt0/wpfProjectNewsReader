using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;
using wpfProjectNewsReader.Model;
using wpfProjectNewsReader.Tools;
using wpfProjectNewsReader.View;

namespace wpfProjectNewsReader.ViewModel
{
    public class LoginViewModel : Bindable, ILoginViewModel
    {
        #region Nntp Client related stuff
        private NntpClientSingleton client = NntpClientSingleton.GetInstance();

        public string ServerName
        {
            get => client.ServerName;
            set
            {
                client.ServerName = value;
                OnPropertyChanged();
            }
        }

        public int ServerPort
        {
            get => client.ServerPort;
            set
            {
                client.ServerPort = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Fields
        private string username = "";
        private string connectionLabel = "";

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionLabel 
        { 
            get => connectionLabel; 
            set
            {
                connectionLabel = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public LoginViewModel()
        {
            LoginCommand = new AddCommand(LoginAttempt);
        }

        #region Commands
        public AddCommand LoginCommand { get; set; }

        public async void LoginAttempt(object parameter)
        {
            ConnectionLabel = "Connecting...";
            // Open the connection
            InternalResponse ir = await client.OpenConnectionAsync();
            bool res = ir.Response.ContainsKey(true);

            if (!res)
            {
                ConnectionLabel = ir.Response[false];
                return;
            }

            // Login with credentials
            ir = await client.LoginAsync(Username, (parameter as PasswordBox).Password);
            res = ir.Response.ContainsKey(true);

            if (!res)
            {
                ConnectionLabel = ir.Response[false];
                return;
            }

            // ASK TO SAVE WITH PIN? //
            ((App)App.Current).ChangeUserControl(App.container.Resolve<MainMenuView>());
        }
        #endregion


    }
}
