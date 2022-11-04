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
        private ObservableCollection<int>? headlines = new ObservableCollection<int>();
        private string searchText = "";
        private int? currentArticleNumber = null;
        private string? article = null;
        private SessionSingleton session = SessionSingleton.GetInstance();
        private IFavoriteAdapter favAdapter = null;
        private string post = "";
        private string from = "";
        private string subject = "";
        private string currentGroup = "";

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
                favorites = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string>? FilteredGroups
        {
            get
            {
                if (SearchText == null || SearchText == "") return AllGroups;

                return new ObservableCollection<string>(AllGroups.Where(x => x.ToUpper().Contains(SearchText.ToUpper()) ||
                x.ToUpper().StartsWith(SearchText.ToUpper())));
            }
        }

        public ObservableCollection<int>? Headlines
        {
            get => headlines;
            set
            {
                headlines = value;
                OnPropertyChanged();
            }
        }

        public string Article
        {
            get => article;
            set
            {
                article = value;
                OnPropertyChanged();
            }
        }

        public string Post
        {
            get => post;
            set
            {
                post = value;
                OnPropertyChanged();
            }
        }

        public string From
        {
            get => from;
            set
            {
                from = value;
                OnPropertyChanged();
            }
        }

        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                OnPropertyChanged();
            }
        }

        public string CurrentGroup
        {
            get => currentGroup;
            set
            {
                currentGroup = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public MainMenuViewModel()
        {
            client = NntpClientSingleton.GetInstance();
            AddToFavorites = new AddCommand(AddSelectedToFavorites);
            RemoveFromFavorites = new AddCommand(RemoveSelectedFromFavorites);
            SelectGroup = new AddCommand(SelectGroupForPost);
            DownloadGroups = new AddCommand(DownloadAllGroups);
            GetHeadlines = new AddCommand(SelectGroupAndGetHeadLines);
            PostArticle = new AddCommand(PostArticleToGroup);
            favAdapter = new FavoriteAdapter(session.Username);
            favorites = new ObservableCollection<string>(favAdapter.LoadFavorites());
            Initialize();
        }

        public int? CurrentArticleNumber
        {
            get => currentArticleNumber;
            set
            {
                currentArticleNumber = value;
                OnPropertyChanged();
                GetArticleFromNumberAsync();
            }
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
        public AddCommand GetHeadlines { get; set; }
        public AddCommand PostArticle { get; set; } 

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

            favAdapter.SaveFavorites(favorites);
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

            favAdapter.SaveFavorites(favorites);
        }

        private void SelectGroupAndGetHeadLines(object parameter)
        {
            SelGroAndGetHeaAsync(parameter);
        }

        private async void SelectGroupForPost(object parameter)
        {
            currentGroup = (string)parameter;
            InternalResponse ir = await client.SelectGroup(currentGroup);
            if (ir.Response.ContainsKey(false)) { return; }
        }

        private async void SelGroAndGetHeaAsync(object parameter)
        {
            currentGroup = (string)parameter;
            InternalResponse ir = await client.GetHeadlinesAsync(currentGroup);
            if (ir.Response.ContainsKey(false)) { return; }

            Headlines = new ObservableCollection<int>((List<int>)ir.Payload);
        }

        private void DownloadAllGroups()
        {
            FileAdapter fileAdapter = new TxtFile();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\NewsReader\\Groups\\AllGroups.txt";

            string text = AllGroupsToString();
            fileAdapter.WriteTextToFile(path, text);
        }

        private string AllGroupsToString()
        {
            string text = "";
            foreach (string s in AllGroups)
            {
                text += s + "\n";
            }

            return text;
        }

        private async void GetArticleFromNumberAsync()
        {
            InternalResponse ir = await client.GetArticleAsync(CurrentArticleNumber);

            string toBePosted = "";
            if (ir.Payload == null) { Article = ir.Response[false]; return; }
            foreach (string s in ((ReadOnlyCollection<string>)ir.Payload))
            {
                toBePosted += s + "\n";
            }
            Article = toBePosted;
        }

        private async void PostArticleToGroup()
        {
            string toBePosted = StringOperations.ConvertTextToArticle(From, currentGroup, Subject, Post);
            InternalResponse ir = await client.PostAsync(toBePosted);

            if (ir.Response.ContainsKey(false)) { MessageBox.Show(ir.Response[false]); return; }
            MessageBox.Show($"Article posted in {currentGroup}");
        }
    }
}
