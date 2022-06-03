using System.Windows.Input;

namespace Bookshelf.ViewModels.Components
{
    public class RatingBar : ViewModelBase
    {
        private int? rate;

        public int? Rate { get => rate; set { rate = value; OnPropertyChanged(); } }

        public string imgStar1 = "empty_star_64.png", imgStar2 = "empty_star_64.png", imgStar3 = "empty_star_64.png", imgStar4 = "empty_star_64.png", imgStar5 = "empty_star_64.png";

        public string ImgStar1 { get => imgStar1; set { imgStar1 = value; OnPropertyChanged(); } }

        public string ImgStar2 { get => imgStar2; set { imgStar2 = value; OnPropertyChanged(); } }

        public string ImgStar3 { get => imgStar3; set { imgStar3 = value; OnPropertyChanged(); } }

        public string ImgStar4 { get => imgStar4; set { imgStar4 = value; OnPropertyChanged(); } }

        public string ImgStar5 { get => imgStar5; set { imgStar5 = value; OnPropertyChanged(); } }

        public ICommand ImgStar1ClickCommand => new Command((e) => { BuildRatingBar(1); });

        public ICommand ImgStar2ClickCommand => new Command((e) => { BuildRatingBar(2); });

        public ICommand ImgStar3ClickCommand => new Command((e) => { BuildRatingBar(3); });

        public ICommand ImgStar4ClickCommand => new Command((e) => { BuildRatingBar(4); });

        public ICommand ImgStar5ClickCommand => new Command((e) => { BuildRatingBar(5); });

        protected void BuildRatingBar(int rating)
        {
            switch (rating)
            {
                case 1:
                    ImgStar2 = ImgStar3 = ImgStar4 = ImgStar5 = "empty_star_64.png";
                    ImgStar1 = "star_64.png";
                    break;
                case 2:
                    ImgStar3 = ImgStar4 = ImgStar5 = "empty_star_64.png";
                    ImgStar1 = ImgStar2 = "star_64.png";
                    break;
                case 3:
                    ImgStar4 = ImgStar5 = "empty_star_64.png";
                    ImgStar1 = ImgStar2 = ImgStar3 = "star_64.png";
                    break;
                case 4:
                    ImgStar5 = "empty_star_64.png";
                    ImgStar1 = ImgStar2 = ImgStar3 = ImgStar4 = "star_64.png";
                    break;
                case 5:
                    ImgStar1 = ImgStar2 = ImgStar3 = ImgStar4 = ImgStar5 = "star_64.png";
                    break;
            }

            Rate = rating;
        }
    }
}
