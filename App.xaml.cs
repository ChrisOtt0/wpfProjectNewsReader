using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;
using wpfProjectNewsReader.View;
using wpfProjectNewsReader.ViewModel;

namespace wpfProjectNewsReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // UnityContainer holding viewmodels for DI
        public static IUnityContainer container = new UnityContainer();

        // Reference to ContentControl
        public ContentControl CCRef { get; set; } = null;

        public App()
        {
            // Create instances of needed services and viewmodels, some should be kept alive for the duration of the program runtime
            container.RegisterType<ILoginViewModel, LoginViewModel>();
            container.RegisterSingleton<IMainMenuViewModel, MainMenuViewModel>();
        }

        // Method for changing the view content
        public void ChangeUserControl(UserControl view)
        {
            this.CCRef.Content = view;
        }
    }
}
