using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books;
using BookshelfModels.Books.GoogleApi;
using BookshelfServices.Books;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookshelf.ViewModels.GoogleSearch
{
    public class GoogleBooksResultsVM : ViewModelBase//, IQueryAttributable
    {

        string pageTitle;

        public string PageTitle
        {
            get => pageTitle; set { if (pageTitle != value) { pageTitle = value; OnPropertyChanged(); } }
        }

        private int CurrentPage;

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

        private async void LoadGoogleBooks(int? pageNumber)
        {
            PageTitle = "Carregando lista...";
            IsBusy = true;

            string textoBusca = "";
            if (!string.IsNullOrEmpty(SearchText))
textoBusca = SearchText.ToUpper();

        var googleBooksListResult = await BookshelfServices.Books.GoogleBooksApi.GoogleBooksApiService.GetBooks(textoBusca, 0);

            //(var booksList, TotalBooksItens) = await booksServices.GetBookSituationByStatus(pageNumber, SituationIndex.Value, textoBusca);

            foreach (var googleBookItem in googleBooksListResult)
            {
                GoogleBooksList.Add(googleBookItem);
            }

            ////Definição do título da interface
            //PageTitle = "Livros ";
            //switch (SituationIndex)
            //{
            //    case 0: PageTitle += " do arquivo"; break;
            //    case 1: PageTitle += " que vou ler"; break;
            //    case 2: PageTitle += " que estou lendo"; break;
            //    case 3: PageTitle += " lidos"; break;
            //    case 4: PageTitle += " interrompidos"; break;
            //}

            IsBusy = false;
        }

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

                        CurrentPage = 1;
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
