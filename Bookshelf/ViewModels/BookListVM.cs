using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books;
using BookshelfServices.Books;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class BookListVM : ViewModelBase
    {

        #region Vars

        readonly IBooksServices booksServices;

        public ObservableCollection<UIBookItem> BooksList { get; } = new();

        private UIBookItem bookItem;

        public UIBookItem BookItem
        {
            get => bookItem;
            set
            {
                if (bookItem != value)
                {
                    bookItem = value;
                    if (bookItem is not null)
                    {
                        if (SituationIndex == -1)
                        {
                            //define the page
                            Page page = navigation.ResolvePage<CreateBook>();

                            //pass parameter
                            (page?.BindingContext as CreateBookVM).OnNavigatingTo(bookItem.Key);

                            //push ui
                            (Application.Current?.MainPage?.Navigation).PushAsync(page, true);
                        }
                        else
                        {
                            //define the page
                            Page page = navigation.ResolvePage<BookDetail>();

                            //pass parameter
                            (page?.BindingContext as BookDetailVM).OnNavigatingTo(bookItem.Key);

                            //push ui
                            (Application.Current?.MainPage?.Navigation).PushAsync(page, true);
                        }
                        bookItem = null;
                    }
                    SetProperty(ref bookItem, value);
                }
            }
        }

        [ObservableProperty]
        string pageTitle;

        [ObservableProperty]
        int totalBooksItens;

        private string searchTitle;

        public string SearchTitle
        {
            get => searchTitle;
            set
            {
                if (searchTitle != value)
                {
                    SearchBookList();                 
                    SetProperty(ref searchTitle, value);
                }
            }
        }

        [ObservableProperty]
        private bool isLoading;

        public int SituationIndex { get; set; }

        public int CurrentPage { get; set; }


        #endregion

        public BookListVM(INavigationServices _navigation, IBooksServices _booksServices)
        {
            navigation = _navigation;
            booksServices = _booksServices;
        }

        public void OnNavigatingTo(int _situationIndex)
        {
            SituationIndex = _situationIndex;

            if (BooksList.Count > 0)
                BooksList.Clear();

            LoadBooks(1);
        }

        public ICommand LoadMoreCommand => new Command(() =>
        {
            CurrentPage++;
            LoadBooks(CurrentPage);
        });


        /// <summary>
        /// true se o processo de busca estiver em funcionamento
        /// </summary>
        public bool SearchingBookList { get; set; }

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            if (BooksList.Count > 0)
                BooksList.Clear();            
            LoadBooks(1);
        });

        /// <summary>
        /// Get books by status
        /// </summary>
        private async void LoadBooks(int pageNumber)
        {
            PageTitle = "Carregando lista...";
            IsLoading = true;

            string textoBusca = "";
            if (!string.IsNullOrEmpty(SearchTitle))
                textoBusca = SearchTitle.ToUpper();

            string teste = "0";
#if WINDOWS
teste = "2";
#endif
            Console.WriteLine("teste: "+teste);

            (var booksList, TotalBooksItens) = await booksServices.GetBookSituationByStatus(pageNumber, SituationIndex, textoBusca);

            foreach (UIBookItem bookItem in booksList)
            {
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

            IsLoading = false;
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

                        LoadBooks(1);

                        SearchingBookList = false;
                    }
                    catch (Exception ex)
                    { throw ex; }
                }
            }
        }
    }
}
