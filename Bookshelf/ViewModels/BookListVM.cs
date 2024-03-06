using BLL.Books;
using Bookshelf.ViewModels.Components;
using Models.Books;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class BookListVM(IBookBLL _booksServices) : ViewModelBase, IQueryAttributable
    {

        #region Vars

        public ObservableCollection<UIBookItem> BooksList { get; } = [];

        //UIBookItem bookItem;

        //public UIBookItem BookItem
        //{
        //    get => bookItem;
        //    set
        //    {
        //        if (bookItem != value)
        //        {
        //            bookItem = value;

        //            if (bookItem is not null)
        //            {
        //                if (SituationIndex == -1)
        //                    Shell.Current.GoToAsync($"{nameof(AddBook)}?Key={bookItem.Key}", true);
        //                else
        //                    Shell.Current.GoToAsync($"{nameof(BookDetail)}?Key={bookItem.Key}", true);

        //                bookItem = null;
        //            }
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        string pageTitle;

        public string PageTitle
        {
            get => pageTitle; set { if (pageTitle != value) { pageTitle = value; OnPropertyChanged(nameof(PageTitle)); } }
        }

        int totalBooksItens;

        public int TotalBooksItens { get => totalBooksItens; set { if (totalBooksItens != value) { totalBooksItens = value; OnPropertyChanged(nameof(TotalBooksItens)); } } }

        string searchTitle;

        public string SearchTitle
        {
            get => searchTitle;
            set
            {
                if (searchTitle != value)
                {
                    searchTitle = value;
                    SearchBookList();
                    OnPropertyChanged(nameof(SearchTitle));
                }
            }
        }

        public int? SituationIndex { get; set; }

        public int CurrentPage { get; set; }

        #endregion

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            SituationIndex = Convert.ToInt16(query["Situation"].ToString());

            if (BooksList.Count > 0)
                BooksList.Clear();

            CurrentPage++;
            _ = LoadBooks(CurrentPage);
        }

        public ICommand LoadMoreCommand => new Command(() =>
        {
            CurrentPage++;
            _ = LoadBooks(CurrentPage);
        });

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            if (BooksList.Count > 0)
                BooksList.Clear();

            CurrentPage = 1;
            _ = LoadBooks(CurrentPage);
        });


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

            List<UIBookItem> _booksList = await _booksServices.GetBooksByStatusAsync(((App)App.Current).Uid, pageNumber, SituationIndex.Value, _searchText);

            foreach (UIBookItem bookItem in _booksList)
            {
                bookItem.Cover = !string.IsNullOrEmpty(bookItem.Cover) ? bookItem.Cover : "cover.jpg";
                BooksList.Add(bookItem);
            }

            //Definição do título da interface
            PageTitle = "Livros ";
            switch (SituationIndex)
            {
                case 0: PageTitle += " do arquivo"; break;
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
