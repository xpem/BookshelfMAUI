using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books.GoogleApi;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels.GoogleSearch
{
    public class GoogleBooksResultsVM : ViewModelBase//, IQueryAttributable
    {

        string pageTitle = "Busca";

        string urlteste = "http://books.google.com/books/content?id=6RCcBAAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api";

        public string Urlteste
        {
            get => urlteste; set { if (urlteste != value) { urlteste = value; OnPropertyChanged(); } }
        }

        public string PageTitle
        {
            get => pageTitle; set { if (pageTitle != value) { pageTitle = value; OnPropertyChanged(); } }
        }

        private int CurrentPage;
        private int TotalItems;

        public ObservableCollection<UIGoogleBook> GoogleBooksList { get; } = new();

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
                    SearchBookList();
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadMoreCommand => new Command(() =>
        {
            CurrentPage++;
            LoadGoogleBooks(CurrentPage);
        });

        /// <summary>
        /// is necessary the config: android:usesCleartextTraffic="true"
        /// </summary>
        /// <param name="pageNumber"></param>
        private async void LoadGoogleBooks(int pageNumber)
        {

            IsBusy = true;

            string _searchText = "";
            if (!string.IsNullOrEmpty(SearchText))
                _searchText = SearchText.ToUpper();

            int startIndex = 0;

            if (pageNumber > 0)
                startIndex = pageNumber * 10;


            if (startIndex == 0 || startIndex < TotalItems)
            {
                (List<UIGoogleBook> googleBooksListResult, TotalItems) = await BookshelfServices.Books.GoogleBooksApi.GoogleBooksApiService.GetBooks(_searchText, startIndex);

                foreach (var googleBookItem in googleBooksListResult)
                {
                    GoogleBooksList.Add(googleBookItem);
                }
            }

            IsBusy = false;
        }

        public ICommand CreateBookCommand => new Command(async (e) => { await Shell.Current.GoToAsync($"{nameof(AddBook)}"); });

        private async void SearchBookList()
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
                        LoadGoogleBooks(CurrentPage);

                        SearchingBookList = false;
                    }
                    catch (Exception ex)
                    { throw ex; }
                }
            }
        }
    }
}
