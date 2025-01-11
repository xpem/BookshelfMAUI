using Bookshelf.Views;
using CommunityToolkit.Mvvm.Input;
using Models.Books;
using Services.Books.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class BookListVM(IBookService _booksServices) : ViewModelBase, IQueryAttributable
    {

        #region Vars

        private ObservableCollection<UIBookItem> booksList = [];

        public ObservableCollection<UIBookItem> BooksList
        {
            get => booksList;
            set
            {
                if (booksList != value)
                {
                    SetProperty(ref (booksList), value);
                }
            }
        }


        string pageTitle;

        public string PageTitle
        {
            get => pageTitle; set { if (pageTitle != value) { SetProperty(ref (pageTitle), value); } }
        }

        int totalBooksItens;

        public int TotalBooksItens { get => totalBooksItens; set { if (totalBooksItens != value) { SetProperty(ref (totalBooksItens), value); } } }

        string searchTitle;

        public string SearchTitle
        {
            get => searchTitle;
            set
            {
                if (searchTitle != value)
                {
                    SetProperty(ref (searchTitle), value);
                    _ = SearchBookList();
                }
            }
        }

        public int? SituationIndex { get; set; }

        public int CurrentPage { get; set; }

        #endregion

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.TryGetValue("Situation", out object outValue))
                SituationIndex = Convert.ToInt16(outValue);
            else { SituationIndex = 0; }

            if (BooksList.Count > 0)
            {
                BooksList.Clear();
            }

            CurrentPage = 1;

            LoadBooks(CurrentPage).ConfigureAwait(false);
        }

        [RelayCommand]
        public Task LoadMore()
        {
            CurrentPage++;
            return LoadBooks(CurrentPage);
        }

        [RelayCommand]
        public Task Appearing()
        {
            SituationIndex ??= 0;

            if (BooksList.Count > 0)
                BooksList.Clear();

            CurrentPage = 1;
            return LoadBooks(CurrentPage);

        }


        /// <summary>
        /// true se o processo de busca estiver em funcionamento
        /// </summary>
        public bool SearchingBookList { get; set; }

        /// <summary>
        /// Get books by status
        /// </summary>
        private async Task LoadBooks(int pageNumber)
        {
            PageTitle = "Carregando lista...";
            IsBusy = true;

            string _searchText = "";
            if (!string.IsNullOrEmpty(SearchTitle))
                _searchText = SearchTitle.ToLower();

            List<UIBookItem> _booksList = await _booksServices.GetByStatusAsync(((App)App.Current).Uid.Value, pageNumber, SituationIndex.Value, _searchText);

            foreach (UIBookItem bookItem in _booksList)
            {
                bookItem.Cover = !string.IsNullOrEmpty(bookItem.Cover) ? bookItem.Cover : "cover.jpg";
                BooksList.Add(bookItem);
            }

            //Definição do título da interface
            PageTitle = "Livros ";
            switch (SituationIndex)
            {
                case 0: break;
                case 1: PageTitle += " que vou ler"; break;
                case 2: PageTitle += " que estou lendo"; break;
                case 3: PageTitle += " lidos"; break;
                case 4: PageTitle += " interrompidos"; break;
            }

            IsBusy = false;
        }

        /// <summary>
        /// Filtra a lista pela busca por texto
        /// </summary>
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

                        if (BooksList.Count > 0)
                            BooksList.Clear();

                        CurrentPage = 1;
                        _ = LoadBooks(CurrentPage);

                        SearchingBookList = false;
                    }
                    catch
                    { throw; }
                }
            }
        }
    }
}
