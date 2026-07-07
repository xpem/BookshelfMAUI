using Bookshelf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Books.GoogleApi;
using Services.Books;
using Services.Utils;
using System.Collections.ObjectModel;


namespace Bookshelf.ViewModels.GoogleSearch
{
    public partial class GoogleBooksResultsVM : ViewModelBase
    {

        string pageTitle = "Busca";

        public string PageTitle
        {
            get => pageTitle; set { if (pageTitle != value) { SetProperty(ref (pageTitle), value); } }
        }

        bool isNotConnected, isConnected;

        public bool IsNotConnected
        {
            get => isNotConnected; set { if (IsNotConnected != value) { SetProperty(ref (isNotConnected), value); } }
        }

        public bool IsConnected
        {
            get => isConnected; set { if (IsConnected != value) { SetProperty(ref (isConnected), value); } }
        }

        private int CurrentPage;
        private int TotalItems;
        private CancellationTokenSource _searchCts;

        private ObservableCollection<UIGoogleBook> googleBooksList = [];

        public ObservableCollection<UIGoogleBook> GoogleBooksList
        {
            get => googleBooksList;
            set
            {
                if (googleBooksList != value)
                {
                    SetProperty(ref (googleBooksList), value);
                }
            }
        }

        [RelayCommand]
        public Task Appearing()
        {
            IsConnected = IsOn;
            IsNotConnected = !IsOn;
            return Task.CompletedTask;
        }

        string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    SetProperty(ref (searchText), value);
                    DebounceSearch();
                }
            }
        }

        [RelayCommand]
        public async Task LoadMore()
        {
            CurrentPage++;
            await LoadGoogleBooksAsync(CurrentPage);
        }

        /// <summary>
        /// is necessary the config: android:usesCleartextTraffic="true"
        /// </summary>
        /// <param name="pageNumber"></param>
        private async Task LoadGoogleBooksAsync(int pageNumber)
        {
            IsConnected = IsOn;
            IsNotConnected = !IsOn;

            if (IsNotConnected) { return; }

            if (!string.IsNullOrEmpty(SearchText))
            {
                IsBusy = true;

                string _searchText = SearchText.RemoveDiacritics().ToUpper();

                // Open Library uses 1-based pages
                int page = pageNumber + 1;

                int loadedCount = (pageNumber) * 10;

                if (loadedCount == 0 || loadedCount < TotalItems)
                {
                   (List<UIGoogleBook> googleBooksListResult, TotalItems) = await OpenLibraryApiService.GetBooks(_searchText, page);

                    foreach (UIGoogleBook googleBookItem in googleBooksListResult)
                    {
                        GoogleBooksList.Add(googleBookItem);
                    }
                }

                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task CreateBook() => await Shell.Current.GoToAsync($"{nameof(AddBook)}");

        private async void DebounceSearch()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                await Task.Delay(800, token);

                if (token.IsCancellationRequested) return;

                GoogleBooksList.Clear();
                CurrentPage = 0;
                await LoadGoogleBooksAsync(CurrentPage);
            }
            catch (TaskCanceledException)
            {
                // esperado quando o usuário continua digitando
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
            }
        }
    }
}
