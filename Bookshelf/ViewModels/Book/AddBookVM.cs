using Models;
using Models.Books.GoogleApi;
using Models.DTOs;
using Services.Books;
using Services.Books.Interfaces;
using Services.Handlers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels.Book
{
    public class AddBookVM(IBookBLL bookBLL) : ViewModelBase, IQueryAttributable
    {
        private int? rate;

        public int? Rate { get => rate; set { rate = value; OnPropertyChanged(nameof(Rate)); } }

        private UIGoogleBook uIGoogleBook { get; set; }

        #region Properties


        private string LocalId, BookId, GoogleKey;

        private string title, subTitle, volume, authors, year, isbn, pages, genre, comment, situation, cover;

        public string Cover { get => cover; set { if (value != cover) { cover = value; OnPropertyChanged(nameof(Cover)); } } }
        public string Title { get => title; set { if (value != title) { title = value; OnPropertyChanged(nameof(Title)); } } }

        public string SubTitle { get => subTitle; set { if (value != subTitle) { subTitle = value; OnPropertyChanged(nameof(SubTitle)); } } }

        public string Volume { get => volume; set { if (value != volume) { volume = value; OnPropertyChanged(nameof(Volume)); } } }

        public string Authors { get => authors; set { if (value != authors) { authors = value; OnPropertyChanged(nameof(Authors)); } } }

        public string Year { get => year; set { if (value != year) { year = value; OnPropertyChanged(nameof(Year)); } } }

        public string Isbn { get => isbn; set { if (value != isbn) { isbn = value; OnPropertyChanged(nameof(Isbn)); } } }

        public string Pages { get => pages; set { if (value != pages) { pages = value; OnPropertyChanged(nameof(Pages)); } } }

        public string Genre { get => genre; set { if (value != genre) { genre = value; OnPropertyChanged(nameof(Genre)); } } }

        public string Comment { get => comment; set { if (value != comment) { comment = value; OnPropertyChanged(nameof(Comment)); } } }

        public string Situation { get => situation; set { if (value != situation) { situation = value; OnPropertyChanged(nameof(Situation)); } } }

        #endregion

        #region Ui properties

        private ObservableCollection<string> statusList = new() { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };
        public ObservableCollection<string> StatusList { get => statusList; set { if (value != statusList) { statusList = value; OnPropertyChanged(nameof(StatusList)); } } }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible, imgCoverIsVisible = false;

        public bool RatingBarIsVisible { get => ratingBarIsVisible; set { if (value != ratingBarIsVisible) { ratingBarIsVisible = value; OnPropertyChanged(nameof(RatingBarIsVisible)); } } }

        public bool LblRatingBarIsVisible { get => lblRatingBarIsVisible; set { if (value != lblRatingBarIsVisible) { lblRatingBarIsVisible = value; OnPropertyChanged(nameof(LblRatingBarIsVisible)); } } }

        public bool EdtCommentIsVisible { get => edtCommentIsVisible; set { if (value != edtCommentIsVisible) { edtCommentIsVisible = value; OnPropertyChanged(nameof(EdtCommentIsVisible)); } } }

        public bool ImgCoverIsVisible { get => imgCoverIsVisible; set { if (value != imgCoverIsVisible) { imgCoverIsVisible = value; OnPropertyChanged(nameof(ImgCoverIsVisible)); } } }

        private int pkrStatusSelectedIndex = 1;

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

                    OnPropertyChanged(nameof(PkrStatusSelectedIndex));
                }
            }
        }

        #region btnInsert propeties

        private bool btnInsertIsVisible = true, btnInsertIsEnabled = true, lblTitleIsEnabled = true;
        private string btnInsertText, btnAddBookImageSourceGlyph;

        public bool BtnInsertIsVisible { get => btnInsertIsVisible; set { if (value != btnInsertIsVisible) { btnInsertIsVisible = value; OnPropertyChanged(nameof(BtnInsertIsVisible)); } } }

        public bool BtnInsertIsEnabled { get => btnInsertIsEnabled; set { if (value != btnInsertIsEnabled) { btnInsertIsEnabled = value; OnPropertyChanged(nameof(BtnInsertIsEnabled)); } } }

        public bool LblTitleIsEnabled { get => lblTitleIsEnabled; set { if (value != lblTitleIsEnabled) { lblTitleIsEnabled = value; OnPropertyChanged(nameof(LblTitleIsEnabled)); } } }

        public string BtnInsertText { get => btnInsertText; set { if (value != btnInsertText) { btnInsertText = value; OnPropertyChanged(nameof(BtnInsertText)); } } }

        public string BtnAddBookImageSourceGlyph { get => btnAddBookImageSourceGlyph; set { if (value != btnAddBookImageSourceGlyph) { btnAddBookImageSourceGlyph = value; OnPropertyChanged(nameof(BtnAddBookImageSourceGlyph)); } } }


        /// <summary>
        /// btn insert book command
        /// </summary>
        public ICommand InsertBookCommand => new Command(async (e) => { await InsertBook(); });

        #endregion
        #endregion

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null)
            {
                if (query.ContainsKey("Id"))
                    LocalId = query["Id"].ToString();

                if (query.ContainsKey("GoogleBook"))
                    uIGoogleBook = query["GoogleBook"] as UIGoogleBook;

            }
            Rate = 0;
            Situation = "0";
            BtnInsertText = "Cadastrar";
            BtnAddBookImageSourceGlyph = IconFont.Plus;
            pkrStatusSelectedIndex = 1;

            if (string.IsNullOrEmpty(LocalId))
            {
                if (uIGoogleBook is not null)
                    GetGoogleBookAsync(uIGoogleBook);

                if (!string.IsNullOrEmpty(Title) || uIGoogleBook is not null)
                {
                    Models.DTOs.Book _book = await bookBLL.GetBookbyTitleOrGoogleIdAsync(((App)Application.Current).Uid, Title, uIGoogleBook.Id);

                    if (_book is not null)
                    {
                        BuildBook(_book);

                        LocalId = _book.LocalId.ToString();
                    }
                }

                if (string.IsNullOrEmpty(LocalId))
                {
                    RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;

                    if (uIGoogleBook is null)
                        Cover = Title = SubTitle = Authors = Year = Pages = "";

                    Isbn = Genre = Volume = "";
                }
            }
            else _ = GetBookAsync(Convert.ToInt32(LocalId));
        }

        protected void GetGoogleBookAsync(UIGoogleBook uIGoogleBook)
        {
            Cover = uIGoogleBook.Thumbnail;
            Title = uIGoogleBook.Title;
            SubTitle = uIGoogleBook.Subtitle;
            Authors = uIGoogleBook.Authors;
            Year = uIGoogleBook.PublishedDate;
            Pages = uIGoogleBook.PageCount.ToString();

            if (!string.IsNullOrEmpty(Cover))
            {
                ImgCoverIsVisible = true;
                LblTitleIsEnabled = false;
            }
        }

        protected void BuildBook(Models.DTOs.Book book)
        {
            BookId = book.Id.ToString();
            Title = book.Title;
            SubTitle = book.SubTitle;
            Authors = book.Authors;
            Year = book.Year.ToString();
            Isbn = book.Isbn;
            Pages = book.Pages.ToString();
            Genre = book.Genre;
            Volume = book.Volume.ToString();
            Comment = book.Comment;
            PkrStatusSelectedIndex = Convert.ToInt32(book.Status);

            GoogleKey ??= book.GoogleId;
            Cover ??= book.Cover;

            if (!string.IsNullOrEmpty(Cover))
            {
                ImgCoverIsVisible = true;
                LblTitleIsEnabled = false;
            }

            if (book.Status > 0)
            {
                Situation = book.Status.ToString();
                Rate = book.Score.Value;

                if (book.Score.HasValue)
                    Rate = book.Score.Value;

                Comment = book.Comment;
            }
            else
            {
                RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                Situation = "0";
                Rate = null;
                Comment = "";
            }

            BtnAddBookImageSourceGlyph = IconFont.Edit;
            BtnInsertText = "Alterar";
            BtnInsertIsVisible = true;
        }

        /// <summary>
        /// get book by book key
        /// </summary>
        protected async Task GetBookAsync(int localId) => BuildBook(await bookBLL.GetBookAsync(((App)Application.Current).Uid, localId));

        private async Task InsertBook()
        {
            try
            {
                if (await VerrifyFields())
                {
                    BtnInsertIsEnabled = false;

                    int? _year = !string.IsNullOrEmpty(Year) ? Convert.ToInt32(Year) : null;
                    int? _volume = !string.IsNullOrEmpty(Volume) ? Convert.ToInt32(Volume) : null;

                    if (Cover.Length > 2000) Cover = null;

                    Models.DTOs.Book book = new()
                    {
                        Title = Title.Truncate(100),
                        SubTitle = SubTitle.Truncate(100),
                        Authors = Authors.Truncate(150),
                        Year = _year,
                        Isbn = Isbn.Truncate(100),
                        Pages = Convert.ToInt32(Pages),
                        Genre = Genre.Truncate(50),
                        Volume = _volume,
                        Cover = Cover,
                        GoogleId = GoogleKey,
                    };

                    //cadastra o livro 
                    string mensagem;

                    //caso tenha avaliação
                    if (pkrStatusSelectedIndex > 0)
                    {
                        int rate = 0;
                        if (pkrStatusSelectedIndex == 3)
                            rate = Convert.ToInt32(Math.Round(Convert.ToDecimal(Rate), MidpointRounding.AwayFromZero));


                        book.Status = (Status)pkrStatusSelectedIndex;

                        book.Score = rate;
                        book.Comment = comment.Truncate(350);

                        mensagem = "Livro e avaliação";
                    }
                    else
                    {
                        book.Status = 0;
                        book.Score = 0;
                        book.Comment = "";
                        mensagem = "Livro";
                    }

                    if (!string.IsNullOrEmpty(LocalId))
                    {
                        book.LocalId = Convert.ToInt32(LocalId);

                        if (!string.IsNullOrEmpty(BookId))
                            book.Id = Convert.ToInt32(BookId);

                        Models.Responses.BLLResponse uptRes = await bookBLL.UpdateBookAsync(((App)Application.Current).Uid, IsOn, book);

                        if (!uptRes.Success)
                        {
                            if (uptRes.Content is not null)
                                mensagem = uptRes.Content as string;
                            else mensagem = "Ocorreu um erro ao atualizar o livro";
                        }
                        else
                            mensagem += " atualizados";
                    }
                    else
                    {
                        Models.Responses.BLLResponse addRes = await bookBLL.AddBookAsync(((App)Application.Current).Uid, IsOn, book);

                        if (!addRes.Success)
                        {
                            if (addRes.Content is not null)
                                mensagem = addRes.Content as string;
                            else mensagem = "Ocorreu um erro ao cadastrar o livro";
                        }
                        else
                            mensagem += " cadastrados";
                    }

                    bool resposta = await Application.Current.MainPage.DisplayAlert("Aviso", mensagem, null, "Ok");

                    if (!resposta)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }
            catch (Exception) { throw; }
        }

        private async Task<bool> VerrifyFields()
        {
            bool ValidInfo = true;
            if (string.IsNullOrEmpty(Title))
                ValidInfo = false;
            else if (await bookBLL.CheckIfExistsBookWithSameTitleAsync(((App)Application.Current).Uid, Title, !string.IsNullOrEmpty(BookId) ? Convert.ToInt32(BookId) : null))
                await Application.Current.MainPage.DisplayAlert("Aviso", "Livro já cadastrado!", null, "Ok");

            if (string.IsNullOrEmpty(Authors))
                ValidInfo = false;

            //if (string.IsNullOrEmpty(Year))
            //{
            //    ValidInfo = false;
            //}
            if (string.IsNullOrEmpty(Pages))
            {
                if (int.TryParse(Pages, out int pages))
                {
                    if (pages <= 0)
                        ValidInfo = false;
                }
                else
                    ValidInfo = false;
            }
            //if (string.IsNullOrEmpty(Genre))
            //{
            //    ValidInfo = false;
            //}

            if (!ValidInfo)
                await Application.Current.MainPage.DisplayAlert("Aviso", "Preencha os campos obrigatórios", null, "Ok");

            return ValidInfo;

        }

    }
}
