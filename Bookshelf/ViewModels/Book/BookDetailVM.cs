using Bookshelf.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Models.DTOs;
using Services.Books.Interfaces;
using System.Collections.ObjectModel;

namespace Bookshelf.ViewModels.Book
{
    public partial class BookDetailVM(IBookService _booksServices) : ViewModelBase, IQueryAttributable
    {

        #region bind variables.

        public string title, authors, pages, genre, situation, bookKey, comment, subtitleAndVol, volume, cover;
        private int? rate;

        public int? Rate
        {
            get => rate; set
            {
                SetProperty(ref (rate), value);
            }
        }

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
                if (title != value)
                {
                    SetProperty(ref (title), value);
                }
            }
        }

        public string Authors
        {
            get => authors; set
            {
                if (authors != value)
                {
                    SetProperty(ref (authors), value);
                }
            }
        }

        public string Pages
        {
            get => pages; set
            {
                if (pages != value)
                {
                    SetProperty(ref (pages), value);
                }
            }
        }

        public string Genre
        {
            get => genre; set
            {
                if (genre != value)
                {
                    SetProperty(ref (genre), value);
                }
            }
        }

        public string Situation
        {
            get => situation; set
            {
                if (situation != value)
                {
                    SetProperty(ref (situation), value);
                }
            }
        }

        public string SubtitleAndVol
        {
            get => subtitleAndVol; set
            {
                if (subtitleAndVol != value)
                {
                    SetProperty(ref (subtitleAndVol), value);
                }
            }
        }
        public string Comment
        {
            get => comment; set
            {
                if (comment != value)
                {
                    SetProperty(ref (comment), value);
                }
            }
        }

        #endregion

        private int LocalId { get; set; }

        private int ExternalId { get; set; }

        private string SituationOri { get; set; }

        private int RateOri { get; set; }

        private string CommentOri { get; set; }

        private bool UpdatesEnableds { get; set; }

        #region Ui properties

        private ObservableCollection<string> statusList = ["Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido"];

        public ObservableCollection<string> StatusList
        {
            get => statusList; set
            {
                if (statusList != value)
                {
                    SetProperty(ref (statusList), value);
                }
            }
        }

        private bool ratingBarIsVisible, lblRatingBarIsVisible, edtCommentIsVisible, lblHSituationIsVisible, btnConfIsEnabled, imgCoverIsVisible = false;

        public bool RatingBarIsVisible
        {
            get => ratingBarIsVisible; set
            {
                if (ratingBarIsVisible != value)
                {
                    SetProperty(ref (ratingBarIsVisible), value);
                }
            }
        }

        public bool LblRatingBarIsVisible
        {
            get => lblRatingBarIsVisible; set
            {
                if (lblRatingBarIsVisible != value)
                {
                    SetProperty(ref (lblRatingBarIsVisible), value);
                }
            }
        }

        public bool EdtCommentIsVisible
        {
            get => edtCommentIsVisible; set
            {
                if (edtCommentIsVisible != value)
                {
                    SetProperty(ref (edtCommentIsVisible), value);
                }
            }
        }

        public bool LblHSituationIsVisible
        {
            get => lblHSituationIsVisible; set
            {
                if (lblHSituationIsVisible != value)
                {
                    SetProperty(ref (lblHSituationIsVisible), value);
                }
            }
        }

        public bool BtnConfIsEnabled
        {
            get => btnConfIsEnabled; set
            {
                if (btnConfIsEnabled != value)
                {
                    SetProperty(ref (btnConfIsEnabled), value);
                }
            }
        }

        private string btnConfText = "Confirmar";

        public string BtnConfText
        {
            get => btnConfText; set
            {
                if (btnConfText != value)
                {
                    SetProperty(ref (btnConfText), value);
                }
            }
        }

        private int pkrStatusSelectedIndex = 0, pkrStatusSelectedIndexOri;

        public int PkrStatusSelectedIndex
        {
            get => pkrStatusSelectedIndex;
            set
            {
                if (pkrStatusSelectedIndex != value)
                {
                    SetProperty(ref (pkrStatusSelectedIndex), value);

                    //if (pkrStatusSelectedIndexOri != pkrStatusSelectedIndex)
                    //{
                    //    BtnConfIsVisible = true;
                    //}

                    switch ((Status)pkrStatusSelectedIndex)
                    {
                        case Status.None:
                            RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                            break;
                        case Status.Reading:
                        case Status.Interrupted:
                        case Status.IllRead:
                            RatingBarIsVisible = LblRatingBarIsVisible = EdtCommentIsVisible = false;
                            break;
                        case Status.Read:
                            EdtCommentIsVisible = RatingBarIsVisible = LblRatingBarIsVisible = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public bool ImgCoverIsVisible
        {
            get => imgCoverIsVisible; set
            {
                if (value != imgCoverIsVisible)
                {
                    SetProperty(ref imgCoverIsVisible, value);
                }
            }
        }

        #endregion

        [RelayCommand]
        public async Task Confirm() => await UpdateBookSituation();

        [RelayCommand]
        public async Task CallHistoric() => await Shell.Current.GoToAsync($"{nameof(Models.DTOs.BookHistoric)}?BookId={ExternalId}");

        [RelayCommand]
        public async Task NavToUpdateBook() => await Shell.Current.GoToAsync($"{nameof(AddBook)}?Id={LocalId}", true);

        [RelayCommand]
        public async Task InactivateBook()
        {
            if (await Application.Current.Windows[0].Page.DisplayAlert("Confirmação", "Deseja excluir este livro?", "Sim", "Cancelar"))
            {
                _ = _booksServices.InactivateBookAsync(((App)Application.Current).Uid.Value, Connectivity.NetworkAccess == NetworkAccess.Internet, LocalId);

                if (!await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Livro excluído!", null, "Ok"))
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            LocalId = Convert.ToInt32(query["Id"]);
            situation = "0";
            rate = 0;

            GetBook(Convert.ToInt32(LocalId)).ConfigureAwait(false);
        }

        private async Task GetBook(int bookId)
        {
            Models.DTOs.Book book = await _booksServices.GetAsync(((App)Application.Current).Uid.Value, bookId);

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

            if (book.Status != Status.None)
            {
                Situation = SituationOri = book.Status.ToString();
                Rate = RateOri = (int)book.Score;
                Comment = CommentOri = book.Comment;

                PkrStatusSelectedIndex = (int)book.Status;

                UpdatesEnableds = true;
                EdtCommentIsVisible = RatingBarIsVisible = LblRatingBarIsVisible = false;

                if (book.Status == Status.Read)
                {
                    LblRatingBarIsVisible = RatingBarIsVisible = true;

                    if (!string.IsNullOrEmpty(Comment))
                        edtCommentIsVisible = true;
                }

                BtnConfText = "Salvar";
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
                _ = _booksServices.UpdateBookSituationAsync(((App)Application.Current).Uid.Value, Connectivity.NetworkAccess == NetworkAccess.Internet, LocalId, (Status)PkrStatusSelectedIndex, rate, Comment);

                if (!await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Situação alterada", null, "Ok"))
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                _ = Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Sem alterações", null, "Ok");
                BtnConfIsEnabled = true;
            }
        }
    }
}
