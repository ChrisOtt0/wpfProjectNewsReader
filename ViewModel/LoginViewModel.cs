using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
        private NntpClientSingleton client;

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
        private SessionSingleton session = SessionSingleton.GetInstance();
        private ILoginDataAdapter lda;

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
                session.Username = username;
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
            lda = new LoginDataAdapter();
            LoginCommand = new AddCommand(LoginAttempt);
            client = NntpClientSingleton.GetInstance();

            if (lda.ConfExists())
                AutoLogin();
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

            var pinDialog = new SaveSessionPrompt();
            if (pinDialog.ShowDialog() == true)
            {
                lda.SaveData(new LoginData(username, (parameter as PasswordBox).Password, ServerName, ServerPort.ToString()), session.Pin);
                session.Pin = SecurityTools.HashString((parameter as PasswordBox).Password);
            }
            else
            {
                session.Pin = SecurityTools.HashString((parameter as PasswordBox).Password);
            }

            ((App)App.Current).ChangeUserControl(App.container.Resolve<MainMenuView>());
        }
        #endregion

        private async void AutoLogin()
        {
            LoginData ld = null;
            var pinDialog = new OpenSavedSessionPrompt();
            if (pinDialog.ShowDialog() == false)
                return;

            ld = lda.RetrieveData(session.Pin);
            session.Pin = SecurityTools.HashString(ld.Password);

            if (ld.IsEmpty())
            {
                MessageBox.Show("Saved login data corrupted.");
                lda.DeleteData();
                return;
            }

            Username = ld.UserName;
            ServerName = ld.ServerName;
            ServerPort = int.Parse(ld.ServerPort);

            ConnectionLabel = "Connecting...";
            InternalResponse ir = await client.OpenConnectionAsync();
            bool res = ir.Response.ContainsKey(true);

            if (!res)
            {
                ConnectionLabel = ir.Response[false];
                return;
            }

            ir = await client.LoginAsync(Username, ld.Password);
            res = ir.Response.ContainsKey(true);

            if (!res)
            {
                ConnectionLabel = ir.Response[false];
                return;
            }

            ((App)App.Current).ChangeUserControl(App.container.Resolve<MainMenuView>());
        }
    }
}
