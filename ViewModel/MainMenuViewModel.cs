using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using wpfProjectNewsReader.Model;
using wpfProjectNewsReader.Tools;

namespace wpfProjectNewsReader.ViewModel
{
    public class MainMenuViewModel : Bindable, IMainMenuViewModel
    {
        #region Fields
        private NntpClientSingleton client;
        private ObservableCollection<string>? allGroups;
        private ObservableCollection<string>? favorites = new ObservableCollection<string>();
        private string searchText = "";

        public ObservableCollection<string>? AllGroups
        {
            get => allGroups;
            set
            {
                allGroups = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                OnPropertyChanged("FilteredGroups");
            }
        }

        public ObservableCollection<string>? Favorites
        {
            get => favorites;
            set
            {
                Favorites = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string>? FilteredGroups
        {
            get
            {
                if (SearchText == null) return AllGroups;

                return new ObservableCollection<string>(AllGroups.Where(x => x.ToUpper().StartsWith(SearchText.ToUpper())));
            }
        }
        #endregion

        #region Constructor
        public MainMenuViewModel()
        {
            client = NntpClientSingleton.GetInstance();
            AddToFavorites = new AddCommand(AddSelectedToFavorites);
            Initialize();
        }

        public async Task Initialize()
        {
            InternalResponse ir = await client.GetGroupListAsync();
            if (ir.Response.ContainsKey(false)) { MessageBox.Show("Error getting list of groups. Please restart the program."); }
            AllGroups = new ObservableCollection<string>((ReadOnlyCollection<string>)ir.Payload);
        }
        #endregion

        public AddCommand AddToFavorites { get; set; }

        private void AddSelectedToFavorites(object parameter)
        {
            ListBox filteredBox = (ListBox)parameter;
            foreach (object o in filteredBox.SelectedItems)
            {
                Favorites.Add((string)o);
            }
        }
    }
}
