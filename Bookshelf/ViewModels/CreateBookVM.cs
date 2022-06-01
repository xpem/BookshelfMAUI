using Bookshelf.Utils.Navigation;
using BookshelfModels.Books;
using BookshelfServices.Books;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class CreateBookVM : ViewModelBase
    {

        #region Properties

        //
        private bool IsUpdate = false;

        private string BookKey;

        private string title, subTitle, volume, authors, year, isbn, pages, genre, comment, situation, rate;

        public string Title { get => title; set { title = value; OnPropertyChanged(); } }

        public string SubTitle { get => subTitle; set { subTitle = value; OnPropertyChanged(); } }

        public string Volume { get => volume; set { volume = value; OnPropertyChanged(); } }

        public string Authors { get => authors; set { authors = value; OnPropertyChanged(); } }

        public string Year { get => year; set { year = value; OnPropertyChanged(); } }

        public string Isbn { get => isbn; set { isbn = value; OnPropertyChanged(); } }

        public string Pages { get => pages; set { pages = value; OnPropertyChanged(); } }

        public string Genre { get => genre; set { genre = value; OnPropertyChanged(); } }

        public string Comment { get => comment; set { comment = value; OnPropertyChanged(); } }

        public string Situation { get => situation; set { situation = value; OnPropertyChanged(); } }

        public string Rate { get => rate; set { rate = value; OnPropertyChanged(); } }

        #endregion


        #region Ui properties

        private ObservableCollection<string> statusList = new() { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };
        public ObservableCollection<string> StatusList { get => statusList; set { statusList = value; OnPropertyChanged(); } }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible;

        public bool RatingBarIsVisible { get => ratingBarIsVisible; set { ratingBarIsVisible = value; OnPropertyChanged(); } }

        public bool LblRatingBarIsVisible { get => lblRatingBarIsVisible; set { lblRatingBarIsVisible = value; OnPropertyChanged(); } }

        public bool EdtCommentIsVisible { get => edtCommentIsVisible; set { edtCommentIsVisible = value; OnPropertyChanged(); } }

        private int pkrStatusSelectedIndex = 0;

        public int PkrStatusSelectedIndex
        {
            get => pkrStatusSelectedIndex;
            set
            {
                pkrStatusSelectedIndex = value;

                //fields visibilities conditions acoording selected situation reading
                RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = pkrStatusSelectedIndex == 3;

                OnPropertyChanged();
            }
        }


        #region btnInsert propeties

        private bool btnInsertIsVisible = true, btnInsertIsEnabled = true;
        private string btnInsertText;

        public bool BtnInsertIsVisible { get => btnInsertIsVisible; set { btnInsertIsVisible = value; OnPropertyChanged(); } }

        public bool BtnInsertIsEnabled { get => btnInsertIsEnabled; set { btnInsertIsEnabled = value; OnPropertyChanged(); } }


        public string BtnInsertText { get => btnInsertText; set { btnInsertText = value; OnPropertyChanged(); } }

        /// <summary>
        /// btn insert book command
        /// </summary>
        public ICommand InsertBookCommand => new Command(async (e) => { await InsertBook(); });

        readonly IBooksServices booksServices;
        #endregion

        #endregion

        public CreateBookVM(INavigationServices _navigation, IBooksServices _booksServices)
        {
            navigation = _navigation;
            booksServices = _booksServices;

            Rate = Situation = "0";
            BtnInsertText = "Cadastrar";
            if (string.IsNullOrEmpty(BookKey))
            {
                pkrStatusSelectedIndex = 0;

                RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                Title = SubTitle = Authors = Year = Isbn = Pages = Genre = Volume = "";
            }
            else
            {
                _ = Task.Run(() => GetBook(BookKey));
            }
        }

        public override Task OnNavigatingTo(object parameter)
        {
            if (parameter is not null)
                BookKey = parameter.ToString();

            return base.OnNavigatingTo(parameter);
        }

        /// <summary>
        /// get book by book key
        /// </summary>
        /// <param name="BookKey"></param>
        protected async void GetBook(string BookKey)
        {
            Book book = await booksServices.GetBook(BookKey);

            Title = book.Title;
            SubTitle = book.SubTitle;
            Authors = book.Authors;
            Year = book.Year.ToString();
            Isbn = book.Isbn;
            Pages = book.Pages.ToString();
            Genre = book.Genre;
            Volume = book.Volume;

            Comment = book.Rating.Comment;
            PkrStatusSelectedIndex = Convert.ToInt32(book.Situation);

            if (book.Situation > 0)
            {
                Situation = book.Situation.ToString();
                Rate = book.Rating.Rate.ToString();
                Comment = book.Rating.Comment;
            }
            else
            {
                RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = BtnInsertIsVisible = false;
                Situation = "0";
                Rate = "";
                Comment = "";
            }

            BtnInsertText = "Alterar";
            IsUpdate = true;
        }

        private async Task InsertBook()
        {
            if (await VerrifyFields())
            {
                BtnInsertIsEnabled = false;

                Book book = new()
                {
                    Title = Title,
                    SubTitle = SubTitle,
                    Authors = Authors,
                    Year = Convert.ToInt32(Year),
                    Isbn = Isbn,
                    Pages = Convert.ToInt32(Pages),
                    Genre = Genre,
                    Volume = Volume,
                };

                //cadastra o livro 
                string mensagem;

                //caso tenha avaliação
                if (pkrStatusSelectedIndex > 0)
                {
                    int rate = 0;
                    if (pkrStatusSelectedIndex == 3)
                    {
                        rate = Convert.ToInt32(Math.Round(Convert.ToDecimal(Rate), MidpointRounding.AwayFromZero));
                    }

                    book.Situation = (Situation)pkrStatusSelectedIndex;

                    book.Rating = new Rating
                    {
                        Rate = rate,
                        Comment = Comment,
                    };

                    mensagem = "Livro e avaliação";
                }
                else
                {
                    book.Situation = 0;

                    book.Rating = new Rating
                    {
                        Rate = 0,
                        Comment = "",
                    };
                    mensagem = "Livro";
                }

                if (!string.IsNullOrEmpty(BookKey))
                {
                    book.BookKey = BookKey;

                    string res = await booksServices.UpdateBook(book);

                    if (res != null)
                    {
                        mensagem = res;
                    }
                    else
                    {
                        mensagem += " atualizados";
                    }
                }
                else
                {
                    string res = await booksServices.AddBook(book);

                    if (res != null)
                    {
                        mensagem = res;
                    }
                    else
                    {
                        mensagem += " cadastrados";
                    }
                }

                bool resposta = await Application.Current.MainPage.DisplayAlert("Aviso", mensagem, null, "Ok");

                if (!resposta)
                {
                    await navigation.NavigateBack();
                }
            }
        }

        private async Task<bool> VerrifyFields()
        {
            bool ValidInfo = true;
            if (string.IsNullOrEmpty(Title))
            {
                ValidInfo = false;
            }
            if (string.IsNullOrEmpty(Authors))
            {
                ValidInfo = false;
            }
            if (string.IsNullOrEmpty(Year))
            {
                ValidInfo = false;
            }
            if (string.IsNullOrEmpty(Pages))
            {
                if (Int32.TryParse(Pages, out int pages))
                {
                    if (pages <= 0)
                    {
                        ValidInfo = false;
                    }
                }
                else
                {
                    ValidInfo = false;
                }

            }
            if (string.IsNullOrEmpty(Genre))
            {
                ValidInfo = false;
            }

            if (!ValidInfo)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Preencha os campos obrigatórios", null, "Ok");
            }
            else
            {
                if (!IsUpdate)
                {
                    if (await booksServices.VerifyBookbyTitle(Title))
                        ValidInfo = false;
                }

                if (!ValidInfo)
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "Livro já cadastrados", null, "Ok");
                }
            }
            return ValidInfo;

        }

    }
}
