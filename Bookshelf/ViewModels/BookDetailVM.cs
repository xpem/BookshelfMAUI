using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfServices.Books;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class BookDetailVM : RatingBar, IQueryAttributable
    {
        readonly IBooksServices booksServices;

        #region bind variables.

        public string title, authors, pages, genre, situation, bookKey, comment, subtitleAndVol, volume;
        public int rate;

        public string Title { get => title; set { title = value; OnPropertyChanged(); } }

        public string Authors { get => authors; set { authors = value; OnPropertyChanged(); } }

        public string Pages { get => pages; set { pages = value; OnPropertyChanged(); } }

        public string Genre { get => genre; set { genre = value; OnPropertyChanged(); } }

        public string Situation { get => situation; set { situation = value; OnPropertyChanged(); } }

        public string SubtitleAndVol { get => subtitleAndVol; set { subtitleAndVol = value; OnPropertyChanged(); } }

        //public int Rate { get => rate; set { rate = value; OnPropertyChanged(); } }

        public string Comment { get => comment; set { comment = value; OnPropertyChanged(); } }

        #endregion

        #region

        private string BookKey { get; set; }

        private string SituationOri { get; set; }

        private int RateOri { get; set; }

        private string CommentOri { get; set; }

        private bool UpdatesEnableds { get; set; }

        #endregion

        #region Ui properties

        private ObservableCollection<string> statusList = new() { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };

        public ObservableCollection<string> StatusList { get => statusList; set { statusList = value; OnPropertyChanged(); } }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible, //lblHCommentIsVisible,
            lblHSituationIsVisible,
            btnConfIsEnabled;

        public bool RatingBarIsVisible { get => ratingBarIsVisible; set { ratingBarIsVisible = value; OnPropertyChanged(); } }

        public bool LblRatingBarIsVisible { get => lblRatingBarIsVisible; set { lblRatingBarIsVisible = value; OnPropertyChanged(); } }

        public bool EdtCommentIsVisible { get => edtCommentIsVisible; set { edtCommentIsVisible = value; OnPropertyChanged(); } }

        public bool LblHSituationIsVisible { get => lblHSituationIsVisible; set { lblHSituationIsVisible = value; OnPropertyChanged(); } } 

        public bool BtnConfIsEnabled { get => btnConfIsEnabled; set { btnConfIsEnabled = value; OnPropertyChanged(); } }

        private string btnConfText = "Confirmar";

        public string BtnConfText { get => btnConfText; set { btnConfText = value; OnPropertyChanged(); } }

        private int pkrStatusSelectedIndex = 0, pkrStatusSelectedIndexOri;

        public int PkrStatusSelectedIndex
        {
            get => pkrStatusSelectedIndex;
            set
            {

                pkrStatusSelectedIndex = value;

                //if (pkrStatusSelectedIndexOri != pkrStatusSelectedIndex)
                //{
                //    BtnConfIsVisible = true;
                //}

                switch ((BookshelfModels.Books.Situation)pkrStatusSelectedIndex)
                {
                    case BookshelfModels.Books.Situation.None:
                        RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                        break;
                    case BookshelfModels.Books.Situation.Reading:
                    case BookshelfModels.Books.Situation.Interrupted:
                    case BookshelfModels.Books.Situation.IllRead:
                        RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                        break;
                    case BookshelfModels.Books.Situation.Read:
                        EdtCommentIsVisible = RatingBarIsVisible = LblRatingBarIsVisible = true;
                        break;
                    default:
                        break;
                }

                OnPropertyChanged();
            }
        }

        #endregion

        public BookDetailVM(IBooksServices _booksServices)
        {
            booksServices = _booksServices;
        }



        public ICommand ConfirmCommand => new Command(async (e) => { await UpdateBookSituation(); });

        /// <summary>
        /// navigate to update book
        /// </summary>
        public ICommand NavToUpdateBookCommand => new Command(async (e) => { await Shell.Current.GoToAsync($"{nameof(AddBook)}?Key={BookKey}", true); });

        /// <summary>
        /// inactivate book
        /// </summary>
        public ICommand InactivateBookCommand =>
            new Command(async (e) =>
            {
                if (await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja excluir esse livro?", "Sim", "Cancelar"))
                {
                    booksServices.InactivateBook(BookKey);

                    if (!await Application.Current.MainPage.DisplayAlert("Aviso", "Livro excluído!", null, "Ok"))
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
            });

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookKey = query["Key"].ToString();
            situation = "0";
            rate = 0;

            GetBook(BookKey);
        }

        private async void GetBook(string bookKey)
        {
            BookshelfModels.Books.Book book = await booksServices.GetBook(bookKey);

            string subtitleAndVol = "";

            if (!string.IsNullOrEmpty(book.SubTitle))
            {
                subtitleAndVol = book.SubTitle;
            }
            if (!string.IsNullOrEmpty(book.SubTitle) && !string.IsNullOrEmpty(book.Volume))
            {
                subtitleAndVol += "; ";
            }
            if (!string.IsNullOrEmpty(book.Volume))
            {
                subtitleAndVol += "Vol.: " + book.Volume;
            }

            Title = book.Title;
            Authors = book.Authors;
            Genre = book.Genre;
            Pages = book.Pages.ToString();
            Comment = book.Rating.Comment;
            SubtitleAndVol = subtitleAndVol;

            //LblHCommentIsVisible = false;
            LblHSituationIsVisible = true;

            pkrStatusSelectedIndexOri = (int)book.Situation;

            if (book.Situation != BookshelfModels.Books.Situation.None)
            {
                Situation = SituationOri = book.Situation.ToString();
                Rate = RateOri = (int)book.Rating.Rate;
                Comment = CommentOri = book.Rating.Comment;

                PkrStatusSelectedIndex = (int)book.Situation;

                UpdatesEnableds = true;
                EdtCommentIsVisible = RatingBarIsVisible = LblRatingBarIsVisible = false;

                if (book.Situation == BookshelfModels.Books.Situation.Read)
                {
                    LblRatingBarIsVisible = RatingBarIsVisible = true;
                    BuildRatingBar(Rate.Value);

                    if (!string.IsNullOrEmpty(Comment))
                        edtCommentIsVisible = true;
                }

                BtnConfText = "Alterar";
            }
            else
            {
                RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                Situation = "0";
                Rate = 0;
                Comment = "";
                UpdatesEnableds = false;
            }
        }

        /// <summary>
        /// update book situation
        /// </summary>
        private async Task UpdateBookSituation()
        {
            if (!UpdatesEnableds)
            {
                UpdatesEnableds = false;
                BtnConfText = "Confirmar";
                return;
            }

            BtnConfIsEnabled = false;
            bool alterou = false;

            int rate = Convert.ToInt32(Rate);

            if (RateOri != rate)
            {
                alterou = true;
            }
            else if (SituationOri != PkrStatusSelectedIndex.ToString())
            {
                alterou = true;

                if (PkrStatusSelectedIndex == 3)
                {
                    if (Comment != CommentOri)
                    {
                        alterou = true;
                    }
                }
            }

            if (alterou)
            {
                booksServices.UpdateBookSituation(BookKey, (BookshelfModels.Books.Situation)PkrStatusSelectedIndex, rate, Comment);

                if (!await Application.Current.MainPage.DisplayAlert("Aviso", "Situação alterada", null, "Ok"))
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                _ = Application.Current.MainPage.DisplayAlert("Aviso", "Sem alterações", null, "Ok");
                BtnConfIsEnabled = true;
            }

        }
    }
}
