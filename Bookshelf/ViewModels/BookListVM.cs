using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books;
using BookshelfServices.Books;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class BookListVM : ViewModelBase, IQueryAttributable
    {

        #region Vars

        readonly IBooksBLL booksServices;

        public ObservableCollection<UIBookItem> BooksList { get; } = new();

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
            get => pageTitle; set { if (pageTitle != value) { pageTitle = value; OnPropertyChanged(); } }
        }

        int totalBooksItens;

        public int TotalBooksItens { get => totalBooksItens; set { if (totalBooksItens != value) { totalBooksItens = value; OnPropertyChanged(); } } }

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
                    OnPropertyChanged();
                }
            }
        }

        public int? SituationIndex { get; set; }

        public int CurrentPage { get; set; }

        #endregion

        public BookListVM(IBooksBLL _booksServices)
        {
            booksServices = _booksServices;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            SituationIndex = Convert.ToInt16(query["Situation"].ToString());

            if (BooksList.Count > 0)
                BooksList.Clear();

            CurrentPage++;
            LoadBooks(CurrentPage);
        }

        public ICommand LoadMoreCommand => new Command(() =>
        {
            CurrentPage++;
            LoadBooks(CurrentPage);
        });

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            if (BooksList.Count > 0)
                BooksList.Clear();

            CurrentPage = 1;
            LoadBooks(CurrentPage);

        });


        /// <summary>
        /// true se o processo de busca estiver em funcionamento
        /// </summary>
        public bool SearchingBookList { get; set; }

        /// <summary>
        /// Get books by status
        /// </summary>
        private async void LoadBooks(int? pageNumber)
        {
            PageTitle = "Carregando lista...";
            IsBusy = true;

            string _searchText = "";
            if (!string.IsNullOrEmpty(SearchTitle))
                _searchText = SearchTitle.ToLower();

            (var booksList, TotalBooksItens) = await booksServices.GetBookSituationByStatus(pageNumber, SituationIndex.Value, _searchText);

            foreach (UIBookItem bookItem in booksList)
            {
                if(bookItem.Cover is null)
                {
                    bookItem.Cover = "cover.jpg";
                }
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
                        LoadBooks(CurrentPage);

                        SearchingBookList = false;
                    }
                    catch (Exception ex)
                    { throw ex; }
                }
            }
        }
    }
}
