using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books;
using BookshelfModels.Books.GoogleApi;
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

        //UIGoogleBook listItem;

        //public UIGoogleBook ListItem
        //{
        //    get => listItem;
        //    set
        //    {
        //        if (listItem != value)
        //        {
        //            listItem = value;

        //            if (listItem is not null)
        //            {
        //                Shell.Current.GoToAsync($"{nameof(AddBook)}?GoogleKey={listItem.Id}", true);
        //                //if (SituationIndex == -1)
        //                //    Shell.Current.GoToAsync($"{nameof(AddBook)}?Key={bookItem.Key}", true);
        //                //else
        //                //    Shell.Current.GoToAsync($"{nameof(BookDetail)}?Key={bookItem.Key}", true);

        //                //bookItem = null;
        //            }
        //            OnPropertyChanged();
        //        }
        //    }
        //}


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (GoogleBooksList.Count > 0)
                GoogleBooksList.Clear();

            LoadGoogleBooks(0);
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
                    SearchBookList();
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadMoreCommand => new Command(() => { CurrentPage++; LoadGoogleBooks(CurrentPage); });

        public GoogleBooksResultsVM()
        {
        }

        /// <summary>
        /// is necessary the config: android:usesCleartextTraffic="true"
        /// </summary>
        /// <param name="pageNumber"></param>
        private async void LoadGoogleBooks(int pageNumber)
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
                    (List<UIGoogleBook> googleBooksListResult, TotalItems) = await BookshelfServices.Books.GoogleBooksApi.GoogleBooksApiBLL.GetBooks(_searchText, startIndex);

                    foreach (var googleBookItem in googleBooksListResult)
                    {
                        GoogleBooksList.Add(googleBookItem);
                    }
                }
                IsBusy = false;
            }
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
                    { throw; }
                }
            }
        }
    }
}
