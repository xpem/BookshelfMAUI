using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using Models.Books.GoogleApi;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels.GoogleSearch
{
    public class GoogleBooksResultsVM : ViewModelBase, IQueryAttributable
    {

        string pageTitle = "Busca";

        public string PageTitle
        {
            get => pageTitle; set { if (pageTitle != value) { pageTitle = value; OnPropertyChanged(); } }
        }

        private int CurrentPage;
        private int TotalItems;

        public ObservableCollection<UIGoogleBook> GoogleBooksList { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (GoogleBooksList.Count > 0)
                GoogleBooksList.Clear();

            _ = LoadGoogleBooksAsync(0);
        }

        public bool SearchingBookList { get; set; }

        //private ObservableCollection<string> parametersList = new() { "Título", "Autor" };
        //public ObservableCollection<string> ParametersList { get => parametersList; set { if (parametersList != value) { parametersList = value; OnPropertyChanged(); } } }

        string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    _ = SearchBookList();
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadMoreCommand => new Command(() => { CurrentPage++; _ = LoadGoogleBooksAsync(CurrentPage); });

        /// <summary>
        /// is necessary the config: android:usesCleartextTraffic="true"
        /// </summary>
        /// <param name="pageNumber"></param>
        private async Task LoadGoogleBooksAsync(int pageNumber)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                IsBusy = true;
                string _searchText = SearchText.ToUpper();

                int startIndex = 0;

                if (pageNumber > 0)
                    startIndex = pageNumber * 10;

                if (startIndex == 0 || startIndex < TotalItems)
                {
                    (List<UIGoogleBook> googleBooksListResult, TotalItems) = await BLL.Books.GoogleBooksApi.GoogleBooksApiBLL.GetBooks(_searchText, startIndex);

                    foreach (UIGoogleBook googleBookItem in googleBooksListResult)
                    {
                        GoogleBooksList.Add(googleBookItem);
                    }
                }
                IsBusy = false;
            }
        }

        public ICommand CreateBookCommand => new Command(async (e) => { await Shell.Current.GoToAsync($"{nameof(AddBook)}"); });

        private async Task SearchBookList()
        {
            //
            if (!SearchingBookList)
            {
                SearchingBookList = true;

                while (SearchingBookList)
                {
                    try
                    {
                        //
                        await Task.Delay(2000);

                        if (GoogleBooksList.Count > 0)
                            GoogleBooksList.Clear();

                        CurrentPage = 0;
                        await LoadGoogleBooksAsync(CurrentPage);

                        SearchingBookList = false;
                    }
                    catch { throw; }
                }
            }
            OnPropertyChanged();
        }
    }
}
