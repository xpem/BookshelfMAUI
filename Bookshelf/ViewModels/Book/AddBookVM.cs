using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Books.GoogleApi;
using Models.DTOs;
using Services.Books.Interfaces;
using Services.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels.Book
{
    public partial class AddBookVM(IBookService bookBLL) : ViewModelBase, IQueryAttributable
    {
        private int? rate;

        public int? Rate
        {
            get => rate; set
            {
                SetProperty(ref (rate), value);
            }
        }

        private UIGoogleBook UIGoogleBook { get; set; }

        #region Properties


        private string LocalId, BookId, GoogleKey;

        private string title, subTitle, volume, authors, year, isbn, pages, genre, comment, situation, cover;

        public string Cover
        {
            get => cover; set
            {
                if (value != cover)
                {
                    SetProperty(ref (cover), value);
                }
            }
        }
        public string Title
        {
            get => title; set
            {
                if (value != title)
                {
                    SetProperty(ref (title), value);
                }
            }
        }

        public string SubTitle
        {
            get => subTitle; set
            {
                if (value != subTitle)
                {
                    SetProperty(ref (subTitle), value);
                }
            }
        }

        public string Volume
        {
            get => volume; set
            {
                if (value != volume)
                {
                    SetProperty(ref (volume), value);
                }
            }
        }

        public string Authors
        {
            get => authors; set
            {
                if (value != authors)
                {
                    SetProperty(ref (authors), value);
                }
            }
        }

        public string Year
        {
            get => year; set
            {
                if (value != year)
                {
                    SetProperty(ref (year), value);
                }
            }
        }

        public string Isbn
        {
            get => isbn; set
            {
                if (value != isbn)
                {
                    SetProperty(ref (isbn), value);
                }
            }
        }

        public string Pages
        {
            get => pages; set
            {
                if (value != pages)
                {
                    SetProperty(ref (pages), value);
                }
            }
        }

        public string Genre
        {
            get => genre; set
            {
                if (value != genre)
                {
                    SetProperty(ref (genre), value);
                }
            }
        }

        public string Comment
        {
            get => comment; set
            {
                if (value != comment)
                {
                    SetProperty(ref (comment), value);
                }
            }
        }

        public string Situation
        {
            get => situation; set
            {
                if (value != situation)
                {
                    SetProperty(ref (situation), value);
                }
            }
        }

        #endregion

        #region Ui properties

        private ObservableCollection<string> statusList = ["Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido"];

        public ObservableCollection<string> StatusList
        {
            get => statusList; set
            {
                if (value != statusList)
                {
                    SetProperty(ref (statusList), value);
                }
            }
        }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible, imgCoverIsVisible = false;

        public bool RatingBarIsVisible
        {
            get => ratingBarIsVisible; set
            {
                if (value != ratingBarIsVisible)
                {
                    SetProperty(ref (ratingBarIsVisible), value);
                }
            }
        }

        public bool LblRatingBarIsVisible
        {
            get => lblRatingBarIsVisible; set
            {
                if (value != lblRatingBarIsVisible)
                {
                    SetProperty(ref (lblRatingBarIsVisible), value);
                }
            }
        }

        public bool EdtCommentIsVisible
        {
            get => edtCommentIsVisible; set
            {
                if (value != edtCommentIsVisible)
                {
                    SetProperty(ref (edtCommentIsVisible), value);
                }
            }
        }

        public bool ImgCoverIsVisible
        {
            get => imgCoverIsVisible; set
            {
                if (value != imgCoverIsVisible)
                {
                    SetProperty(ref (imgCoverIsVisible), value);
                }
            }
        }

        private int pkrStatusSelectedIndex = 1;

        public int PkrStatusSelectedIndex
        {
            get => pkrStatusSelectedIndex;
            set
            {
                if (value != pkrStatusSelectedIndex)
                {
                    SetProperty(ref (pkrStatusSelectedIndex), value);

                    //fields visibilities conditions acoording selected situation reading
                    RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = pkrStatusSelectedIndex == 3;
                }
            }
        }

        #region btnInsert propeties

        private bool btnInsertIsVisible = true, btnInsertIsEnabled = true, lblTitleIsEnabled = true;
        private string btnInsertText, btnAddBookImageSourceGlyph;

        public bool BtnInsertIsVisible
        {
            get => btnInsertIsVisible; set
            {
                if (value != btnInsertIsVisible)
                {
                    SetProperty(ref (btnInsertIsVisible), value);
                }
            }
        }

        public bool BtnInsertIsEnabled
        {
            get => btnInsertIsEnabled; set
            {
                if (value != btnInsertIsEnabled)
                {
                    SetProperty(ref (btnInsertIsEnabled), value);
                }
            }
        }

        public bool LblTitleIsEnabled
        {
            get => lblTitleIsEnabled; set
            {
                if (value != lblTitleIsEnabled)
                {
                    SetProperty(ref (lblTitleIsEnabled), value);
                }
            }
        }

        public string BtnInsertText
        {
            get => btnInsertText; set
            {
                if (value != btnInsertText)
                {
                    SetProperty(ref (btnInsertText), value);
                }
            }
        }

        public string BtnAddBookImageSourceGlyph
        {
            get => btnAddBookImageSourceGlyph; set
            {
                if (value != btnAddBookImageSourceGlyph)
                {
                    SetProperty(ref (btnAddBookImageSourceGlyph), value);
                }
            }
        }

        [RelayCommand]
        public async Task InsertBook()
        {
            try
            {
                if (await VerrifyFields())
                {
                    BtnInsertIsEnabled = false;

                    int? _year = !string.IsNullOrEmpty(Year) ? Convert.ToInt32(Year) : null;
                    int? _volume = !string.IsNullOrEmpty(Volume) ? Convert.ToInt32(Volume) : null;

                    if (Cover?.Length > 2000) Cover = null;

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

                        Models.Responses.BLLResponse uptRes = await bookBLL.UpdateAsync(((App)Application.Current).Uid, IsOn, book);

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
                        Models.Responses.BLLResponse addRes = await bookBLL.AddAsync(((App)Application.Current).Uid, IsOn, book);

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

        #endregion
        #endregion

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null)
            {
                if (query.TryGetValue("Id", out var idValue))
                    LocalId = idValue.ToString();

                if (query.TryGetValue("GoogleBook", out var googleBookValue))
                    UIGoogleBook = googleBookValue as UIGoogleBook;
            }

            Rate = 0;
            Situation = "0";
            BtnInsertText = "Cadastrar";
            BtnAddBookImageSourceGlyph = IconFont.Plus;
            pkrStatusSelectedIndex = 1;

            if (string.IsNullOrEmpty(LocalId))
            {
                if (UIGoogleBook is not null)
                    GetGoogleBookAsync(UIGoogleBook);

                if (!string.IsNullOrEmpty(Title) || UIGoogleBook is not null)
                {
                    Models.DTOs.Book _book = await bookBLL.GetbyTitleOrGoogleIdAsync(((App)Application.Current).Uid, Title, UIGoogleBook.Id);

                    if (_book is not null)
                    {
                        BuildBook(_book);

                        LocalId = _book.LocalId.ToString();
                    }
                }

                if (string.IsNullOrEmpty(LocalId))
                {
                    RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;

                    if (UIGoogleBook is null)
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
        protected async Task GetBookAsync(int localId) => BuildBook(await bookBLL.GetAsync(((App)Application.Current).Uid, localId));

        private async Task<bool> VerrifyFields()
        {
            bool ValidInfo = true;

            if (string.IsNullOrEmpty(Title))
                ValidInfo = false;

            if (string.IsNullOrEmpty(Authors))
                ValidInfo = false;

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

            if (!ValidInfo)
                await Application.Current.MainPage.DisplayAlert("Aviso", "Preencha os campos obrigatórios", null, "Ok");

            return ValidInfo;
        }
    }
}