using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books;
using BookshelfServices.Books;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class BookListVM : ViewModelBase
    {

        #region Vars

        readonly IBooksServices booksServices;

        private ObservableCollection<UIBookItem> booksList;

        public ObservableCollection<UIBookItem> BooksList { get => booksList; set { booksList = value; OnPropertyChanged(); } }

        private UIBookItem bookItem;

        public UIBookItem BookItem
        {
            get => bookItem;
            set
            {
                bookItem = value;
                if (bookItem != null)
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
                OnPropertyChanged();
            }
        }

        private string pageTitle, searchTitle;

        public string PageTitle { get => pageTitle; set { pageTitle = value; OnPropertyChanged(); } }

        public string SearchTitle
        {
            get
            {
                return searchTitle;
            }
            set
            {
                searchTitle = value;
                SearchBookList();
                OnPropertyChanged();
            }
        }

        private bool isLoading;
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(); } }

        public int SituationIndex { get; set; }


        private int currentPage;

        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }

        #endregion

        public BookListVM(INavigationServices _navigation, IBooksServices _booksServices)
        {
            navigation = _navigation;
            booksServices = _booksServices;
        }

        public void OnNavigatingTo(int _situationIndex)
        {
            SituationIndex = _situationIndex;

            BooksList = new ObservableCollection<BookshelfModels.Books.UIBookItem>();
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
            BooksList = new ObservableCollection<UIBookItem>();
            CurrentPage = 1;
            LoadBooks(CurrentPage);
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

            foreach (UIBookItem bookItem in await booksServices.GetBookSituationByStatus(pageNumber, SituationIndex, textoBusca))
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

                        BooksList = new ObservableCollection<UIBookItem>();
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
