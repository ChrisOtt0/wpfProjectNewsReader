using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpfProjectNewsReader.ViewModel;

namespace wpfProjectNewsReader.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        ILoginViewModel viewModel;
        public LoginView(ILoginViewModel iLoginViewModel)
        {
            InitializeComponent();

            // Set viewmodel received from DI as binding context
            viewModel = iLoginViewModel;
            this.DataContext = viewModel;
        }
    }
}
