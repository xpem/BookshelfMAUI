using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.Books;
using BookshelfModels.Books.GoogleApi;
using BookshelfServices.Books;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class AddBookVM : RatingBar, IQueryAttributable
    {

        #region Properties

        //
        private bool IsUpdate = false;

        private string BookKey, GoogleKey;

        private string title, subTitle, volume, authors, year, isbn, pages, genre, comment, situation, cover;

        public string Cover { get => cover; set { if (value != cover) { cover = value; OnPropertyChanged(); } } }
        public string Title { get => title; set { if (value != title) { title = value; OnPropertyChanged(); } } }

        public string SubTitle { get => subTitle; set { if (value != subTitle) { subTitle = value; OnPropertyChanged(); } } }

        public string Volume { get => volume; set { if (value != volume) { volume = value; OnPropertyChanged(); } } }

        public string Authors { get => authors; set { if (value != authors) { authors = value; OnPropertyChanged(); } } }

        public string Year { get => year; set { if (value != year) { year = value; OnPropertyChanged(); } } }

        public string Isbn { get => isbn; set { if (value != isbn) { isbn = value; OnPropertyChanged(); } } }

        public string Pages { get => pages; set { if (value != pages) { pages = value; OnPropertyChanged(); } } }

        public string Genre { get => genre; set { if (value != genre) { genre = value; OnPropertyChanged(); } } }

        public string Comment { get => comment; set { if (value != comment) { comment = value; OnPropertyChanged(); } } }

        public string Situation { get => situation; set { if (value != situation) { situation = value; OnPropertyChanged(); } } }

        #endregion

        #region Ui properties

        private ObservableCollection<string> statusList = new() { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };
        public ObservableCollection<string> StatusList { get => statusList; set { if (value != statusList) { statusList = value; OnPropertyChanged(); } } }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible, imgCoverIsVisible = false;

        public bool RatingBarIsVisible { get => ratingBarIsVisible; set { if (value != ratingBarIsVisible) { ratingBarIsVisible = value; OnPropertyChanged(); } } }

        public bool LblRatingBarIsVisible { get => lblRatingBarIsVisible; set { if (value != lblRatingBarIsVisible) { lblRatingBarIsVisible = value; OnPropertyChanged(); } } }

        public bool EdtCommentIsVisible { get => edtCommentIsVisible; set { if (value != edtCommentIsVisible) { edtCommentIsVisible = value; OnPropertyChanged(); } } }

        public bool ImgCoverIsVisible { get => imgCoverIsVisible; set { if (value != imgCoverIsVisible) { imgCoverIsVisible = value; OnPropertyChanged(); } } }

        private int pkrStatusSelectedIndex = 0;

        public int PkrStatusSelectedIndex
        {
            get => pkrStatusSelectedIndex;
            set
            {
                if (value != pkrStatusSelectedIndex)
                {
                    pkrStatusSelectedIndex = value;

                    //fields visibilities conditions acoording selected situation reading
                    RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = pkrStatusSelectedIndex == 3;

                    OnPropertyChanged();
                }
            }
        }

        #region btnInsert propeties

        private bool btnInsertIsVisible = true, btnInsertIsEnabled = true, lblTitleIsEnabled = true;
        private string btnInsertText, btnAddBookImageSourceGlyph;

        public bool BtnInsertIsVisible { get => btnInsertIsVisible; set { if (value != btnInsertIsVisible) { btnInsertIsVisible = value; OnPropertyChanged(); } } }

        public bool BtnInsertIsEnabled { get => btnInsertIsEnabled; set { if (value != btnInsertIsEnabled) { btnInsertIsEnabled = value; OnPropertyChanged(); } } }

        public bool LblTitleIsEnabled { get => lblTitleIsEnabled; set { if (value != lblTitleIsEnabled) { lblTitleIsEnabled = value; OnPropertyChanged(); } } }

        public string BtnInsertText { get => btnInsertText; set { if (value != btnInsertText) { btnInsertText = value; OnPropertyChanged(); } } }

        public string BtnAddBookImageSourceGlyph { get => btnAddBookImageSourceGlyph; set { if (value != btnAddBookImageSourceGlyph) { btnAddBookImageSourceGlyph = value; OnPropertyChanged(); } } }


        /// <summary>
        /// btn insert book command
        /// </summary>
        public ICommand InsertBookCommand => new Command(async (e) => { await InsertBook(); });

        readonly IBooksServices booksServices;

        #endregion

        #endregion

        public AddBookVM(IBooksServices _booksServices)
        {
            booksServices = _booksServices;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.ContainsKey("Key"))
                BookKey = query["Key"].ToString();

            if (query != null && query.ContainsKey("GoogleKey"))
                GoogleKey = query["GoogleKey"].ToString();

            Rate = 0;
            Situation = "0";
            BtnInsertText = "Cadastrar";
            BtnAddBookImageSourceGlyph = IconFont.Plus;

            if (string.IsNullOrEmpty(BookKey))
            {
                if (!string.IsNullOrEmpty(GoogleKey))
                    await GetGoogleBook();

                if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(GoogleKey))
                {
                    Book _book = await booksServices.GetBookbyTitleAndGoogleId(Title.ToLower(), GoogleKey);

                    if (_book is not null)
                    {
                        BuildBook(_book);

                        BookKey = _book.BookKey;
                    }
                }

                if (string.IsNullOrEmpty(BookKey))
                {
                    pkrStatusSelectedIndex = 0;

                    RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;

                    if (string.IsNullOrEmpty(GoogleKey))
                        Cover = Title = SubTitle = Authors = Year = Pages = "";

                    Isbn = Genre = Volume = "";
                }
            }
            else
                _ = Task.Run(() => GetBook(BookKey));


            
        }

        protected async Task GetGoogleBook()
        {
            UIGoogleBook _googleBook = await BookshelfServices.Books.GoogleBooksApi.GoogleBooksApiService.GetBook(GoogleKey);

            if (_googleBook != null)
            {
                Cover = _googleBook.Thumbnail;
                Title = _googleBook.Title;
                SubTitle = _googleBook.Subtitle;
                Authors = _googleBook.Authors;
                Year = _googleBook.PublishedDate;
                Pages = _googleBook.PageCount.ToString();
            }
            else
                GoogleKey = null;

        }

        protected void BuildBook(Book book)
        {

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

            GoogleKey ??= book.GoogleId;
            Cover ??= book.Cover;
            
            if (!string.IsNullOrEmpty(Cover))
            {
                ImgCoverIsVisible = true;
                LblTitleIsEnabled = false;
            }

            if (book.Situation > 0)
            {
                Situation = book.Situation.ToString();
                Rate = book.Rating.Rate.Value;
                if (book.Rating.Rate.HasValue)
                    BuildRatingBar(book.Rating.Rate.Value);
                Comment = book.Rating.Comment;
            }
            else
            {
                RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = BtnInsertIsVisible = false;
                Situation = "0";
                Rate = null;
                Comment = "";
            }

            BtnAddBookImageSourceGlyph = IconFont.Edit;
            BtnInsertText = "Alterar";
            IsUpdate = true;
        }

        /// <summary>
        /// get book by book key
        /// </summary>
        /// <param name="BookKey"></param>
        protected async void GetBook(string BookKey) => BuildBook(await booksServices.GetBook(BookKey));

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
                    Cover = Cover,
                    GoogleId = GoogleKey
                };

                //cadastra o livro 
                string mensagem;

                //caso tenha avaliação
                if (pkrStatusSelectedIndex > 0)
                {
                    int rate = 0;
                    if (pkrStatusSelectedIndex == 3)
                        rate = Convert.ToInt32(Math.Round(Convert.ToDecimal(Rate), MidpointRounding.AwayFromZero));


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
                        mensagem = res;
                    else
                        mensagem += " atualizados";
                }
                else
                {
                    string res = await booksServices.AddBook(book);

                    if (res != null)
                        mensagem = res;
                    else
                        mensagem += " cadastrados";
                }

                bool resposta = await Application.Current.MainPage.DisplayAlert("Aviso", mensagem, null, "Ok");

                if (!resposta)
                {
                    await Shell.Current.GoToAsync($"//{nameof(Main)}");
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
            //if (string.IsNullOrEmpty(Year))
            //{
            //    ValidInfo = false;
            //}
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
            //if (string.IsNullOrEmpty(Genre))
            //{
            //    ValidInfo = false;
            //}

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
