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
using System.Windows.Shapes;
using wpfProjectNewsReader.ViewModel;

namespace wpfProjectNewsReader.View
{
    /// <summary>
    /// Interaction logic for OpenSavedSessionPrompt.xaml
    /// </summary>
    public partial class OpenSavedSessionPrompt : Window
    {
        private OpenSavedSessionViewModel viewModel;
        public OpenSavedSessionPrompt()
        {
            viewModel = new OpenSavedSessionViewModel();
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OK(PinBox.Password);
            DialogResult = true;
            this.Close();
        }
        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
