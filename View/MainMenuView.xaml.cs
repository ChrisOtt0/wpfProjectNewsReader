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
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : UserControl
    {
        IMainMenuViewModel viewModel;

        public MainMenuView(IMainMenuViewModel iMainMenuViewModel)
        {
            InitializeComponent();

            // Set viewmodel based on DI
            viewModel = iMainMenuViewModel;
            this.DataContext = viewModel;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((MainMenuViewModel)DataContext).GetHeadlines.Execute(((ListBoxItem)sender).Content);
        }

        private void ListBoxItem_MouseDoubleClickPost(object sender, MouseButtonEventArgs e)
        {
            ((MainMenuViewModel)DataContext).SelectGroup.Execute(((ListBoxItem)sender).Content);
        }
    }
}
