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
    /// Interaction logic for SaveSessionPrompt.xaml
    /// </summary>
    public partial class SaveSessionPrompt : Window
    {
        private SaveSessionViewModel viewModel;

        public SaveSessionPrompt()
        {
            viewModel = new SaveSessionViewModel();
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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
