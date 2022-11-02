using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            RemoveFromFavorites = new AddCommand(RemoveSelectedFromFavorites);
            SelectGroup = new AddCommand(SelectGroupAndGetHeadLines);
            DownloadGroups = new AddCommand(DownloadAllGroupsAsync);
            Initialize();
            Thread.Sleep(5000);
        }

        public async Task Initialize()
        {
            InternalResponse ir = await client.GetGroupListAsync();
            if (ir.Response.ContainsKey(false)) { MessageBox.Show("Error getting list of groups. Please restart the program."); }
            AllGroups = new ObservableCollection<string>((ReadOnlyCollection<string>)ir.Payload);
        }
        #endregion

        public AddCommand AddToFavorites { get; set; }
        public AddCommand RemoveFromFavorites { get; set; }
        public AddCommand SelectGroup { get; set; }
        public AddCommand DownloadGroups { get; set; }

        private void AddSelectedToFavorites(object parameter)
        {
            ListBox filteredBox = (ListBox)parameter;
            foreach (object o in filteredBox.SelectedItems)
            {
                Favorites.Add((string)o);
            }

            for (int i = 0; i < Favorites.Count; i++)
            {
                for (int j = 0; j < Favorites.Count; j++)
                {
                    if (i == j) continue;
                    if (Favorites[i] == Favorites[j])
                    {
                        Favorites.Remove(Favorites[j]);
                        j--;
                    }
                }
            }

            filteredBox.UnselectAll();
        }

        private void RemoveSelectedFromFavorites(object parameter)
        {
            ListBox favBox = (ListBox)parameter;
            for (int i = 0; i < Favorites.Count; i++)
            {
                if (favBox.SelectedItems.Contains(Favorites[i]))
                {
                    Favorites.Remove(Favorites[i]);
                    i--;
                }
            }

            favBox.UnselectAll();
        }

        private void SelectGroupAndGetHeadLines()
        {

        }

        private async void DownloadAllGroupsAsync()
        {
            FileAdapter fileAdapter = new TxtFile();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\NewsReader\\Groups\\AllGroups.txt";

            string text = await AllGroupsToStringAsync();
            fileAdapter.WriteTextToFile(path, text);
        }

        private async Task<string> AllGroupsToStringAsync()
        {
            string text = "";
            foreach (string s in AllGroups)
            {
                text += s + "\n";
            }

            return text;
        }
    }
}
