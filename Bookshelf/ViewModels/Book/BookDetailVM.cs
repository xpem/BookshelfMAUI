using BLL.Books;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using Bookshelf.Views.Book;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class BookDetailVM(IBooksBLL _booksServices) : ViewModelBase, IQueryAttributable
    {

        #region bind variables.

        public string title, authors, pages, genre, situation, bookKey, comment, subtitleAndVol, volume, cover;
        private int? rate;

        public int? Rate { get => rate; set { rate = value; OnPropertyChanged(nameof(Rate)); } }

        public string Cover { get => cover; set { if (value != cover) { cover = value; OnPropertyChanged(); } } }

        public string Title { get => title; set { if (title != value) { title = value; OnPropertyChanged(); } } }

        public string Authors { get => authors; set { if (authors != value) { authors = value; OnPropertyChanged(); } } }

        public string Pages { get => pages; set { if (pages != value) { pages = value; OnPropertyChanged(); } } }

        public string Genre { get => genre; set { if (genre != value) { genre = value; OnPropertyChanged(); } } }

        public string Situation { get => situation; set { if (situation != value) { situation = value; OnPropertyChanged(); } } }

        public string SubtitleAndVol { get => subtitleAndVol; set { if (subtitleAndVol != value) { subtitleAndVol = value; OnPropertyChanged(); } } }

        //public int Rate { get => rate; set { rate = value; OnPropertyChanged(); } }

        public string Comment { get => comment; set { if (comment != value) { comment = value; OnPropertyChanged(); } } }

        #endregion

        private int LocalId { get; set; }

        private int ExternalId { get; set; }

        private string SituationOri { get; set; }

        private int RateOri { get; set; }

        private string CommentOri { get; set; }

        private bool UpdatesEnableds { get; set; }

        #region Ui properties

        private ObservableCollection<string> statusList = new() { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };

        public ObservableCollection<string> StatusList { get => statusList; set { if (statusList != value) { statusList = value; OnPropertyChanged(); } } }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible, lblHSituationIsVisible, btnConfIsEnabled, imgCoverIsVisible = false;

        public bool RatingBarIsVisible { get => ratingBarIsVisible; set { if (ratingBarIsVisible != value) { ratingBarIsVisible = value; OnPropertyChanged(); } } }

        public bool LblRatingBarIsVisible { get => lblRatingBarIsVisible; set { if (lblRatingBarIsVisible != value) { lblRatingBarIsVisible = value; OnPropertyChanged(); } } }

        public bool EdtCommentIsVisible { get => edtCommentIsVisible; set { if (edtCommentIsVisible != value) { edtCommentIsVisible = value; OnPropertyChanged(); } } }

        public bool LblHSituationIsVisible { get => lblHSituationIsVisible; set { if (lblHSituationIsVisible != value) { lblHSituationIsVisible = value; OnPropertyChanged(); } } }

        public bool BtnConfIsEnabled { get => btnConfIsEnabled; set { if (btnConfIsEnabled != value) { btnConfIsEnabled = value; OnPropertyChanged(); } } }

        private string btnConfText = "Confirmar";

        public string BtnConfText { get => btnConfText; set { if (btnConfText != value) { btnConfText = value; OnPropertyChanged(); } } }

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

                switch ((Models.Books.Status)pkrStatusSelectedIndex)
                {
                    case Models.Books.Status.None:
                        RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                        break;
                    case Models.Books.Status.Reading:
                    case Models.Books.Status.Interrupted:
                    case Models.Books.Status.IllRead:
                        RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                        break;
                    case Models.Books.Status.Read:
                        EdtCommentIsVisible = RatingBarIsVisible = LblRatingBarIsVisible = true;
                        break;
                    default:
                        break;
                }

                OnPropertyChanged(nameof(PkrStatusSelectedIndex));
            }
        }

        public bool ImgCoverIsVisible { get => imgCoverIsVisible; set { if (value != imgCoverIsVisible) { imgCoverIsVisible = value; OnPropertyChanged(); } } }

        #endregion

        public ICommand ConfirmCommand => new Command(async (e) => { await UpdateBookSituation(); });

        public ICommand CallHistoricCommand => new Command(async (e) => await Shell.Current.GoToAsync($"{nameof(BookHistoric)}?BookId={(ExternalId)}"));

        /// <summary>
        /// navigate to update book
        /// </summary>
        public ICommand NavToUpdateBookCommand => new Command(async (e) => { await Shell.Current.GoToAsync($"{nameof(AddBook)}?Id={LocalId}", true); });

        /// <summary>
        /// inactivate book
        /// </summary>
        public ICommand InactivateBookCommand =>
            new Command(async (e) =>
            {
                if (await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja excluir este livro?", "Sim", "Cancelar"))
                {
                    _ = _booksServices.InactivateBookAsync(((App)App.Current).Uid, LocalId);

                    if (!await Application.Current.MainPage.DisplayAlert("Aviso", "Livro excluído!", null, "Ok"))
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
            });

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            LocalId = Convert.ToInt32(query["Id"]);
            situation = "0";
            rate = 0;

            _ = GetBook(Convert.ToInt32(LocalId));
        }

        private async Task GetBook(int bookId)
        {
            Models.Books.Book book = await _booksServices.GetBookAsync(((App)App.Current).Uid, bookId);

            if (book.Id > 0)
                ExternalId = book.Id.Value;
            else ExternalId = 0;

            string subtitleAndVol = "";

            if (!string.IsNullOrEmpty(book.SubTitle))
                subtitleAndVol = book.SubTitle;

            if (!string.IsNullOrEmpty(book.SubTitle) && book.Volume != null)
                subtitleAndVol += "; ";

            if (book.Volume != null)
                subtitleAndVol += "Vol.: " + book.Volume;

            if (!string.IsNullOrEmpty(book.Cover))
            {
                ImgCoverIsVisible = true;
                Cover = book.Cover;
            }

            Title = book.Title;
            Authors = book.Authors;
            Genre = book.Genre;
            Pages = book.Pages.ToString();
            Comment = book.Comment;
            SubtitleAndVol = subtitleAndVol;

            //LblHCommentIsVisible = false;
            LblHSituationIsVisible = true;

            pkrStatusSelectedIndexOri = (int)book.Status;

            if (book.Status != Models.Books.Status.None)
            {
                Situation = SituationOri = book.Status.ToString();
                Rate = RateOri = (int)book.Score;
                Comment = CommentOri = book.Comment;

                PkrStatusSelectedIndex = (int)book.Status;

                UpdatesEnableds = true;
                EdtCommentIsVisible = RatingBarIsVisible = LblRatingBarIsVisible = false;

                if (book.Status == Models.Books.Status.Read)
                {
                    LblRatingBarIsVisible = RatingBarIsVisible = true;

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
                UpdatesEnableds = true;
                //UpdatesEnableds = false;
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
                _ = _booksServices.UpdateBookSituationAsync(((App)App.Current).Uid, LocalId, (Models.Books.Status)PkrStatusSelectedIndex, rate, Comment);

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
