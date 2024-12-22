using System.Windows.Input;

namespace Bookshelf.Components;

public partial class RatingBar : VerticalStackLayout
{
    public RatingBar()
    {
        InitializeComponent();
    }


    public static readonly BindableProperty HeightIconsProperty = BindableProperty.Create(
        propertyName: nameof(HeightIcons), returnType: typeof(int), declaringType: typeof(BorderedEntry), defaultValue: 30, defaultBindingMode: BindingMode.TwoWay);

    public int HeightIcons { get => (int)GetValue(HeightIconsProperty); set { SetValue(HeightIconsProperty, value); } }


    public static readonly BindableProperty RatingBarIsVisibleProperty = BindableProperty.Create(
        propertyName: nameof(RatingBarIsVisible),
        returnType: typeof(bool),
        declaringType: typeof(RatingBar),
        defaultValue: false
        );

    public bool RatingBarIsVisible
    {
        get
        {
            return (bool)GetValue(RatingBarIsVisibleProperty);
        }
        set { SetValue(RatingBarIsVisibleProperty, value); }
    }

    public static readonly BindableProperty RateProperty = BindableProperty.Create(
        propertyName: nameof(Rate),
        returnType: typeof(int),
        declaringType: typeof(RatingBar),
        defaultValue: 0,
        defaultBindingMode: BindingMode.TwoWay
        );

    public int Rate
    {
        get
        {
            int _rate = (int)GetValue(RateProperty);
            BuildRatingBar(_rate);
            return (int)GetValue(RateProperty);
        }
        set { SetValue(RateProperty, value); }
    }

    readonly static string emptyStar = "star_empty.png";
    readonly static string solidStar = "star_solid.png";

    public string imgStar1 = emptyStar, imgStar2 = emptyStar, imgStar3 = emptyStar, imgStar4 = emptyStar, imgStar5 = emptyStar;

    public string ImgStar1 { get => imgStar1; set { imgStar1 = value; OnPropertyChanged(nameof(ImgStar1)); } }

    public string ImgStar2 { get => imgStar2; set { imgStar2 = value; OnPropertyChanged(nameof(ImgStar2)); } }

    public string ImgStar3 { get => imgStar3; set { imgStar3 = value; OnPropertyChanged(nameof(ImgStar3)); } }

    public string ImgStar4 { get => imgStar4; set { imgStar4 = value; OnPropertyChanged(nameof(ImgStar4)); } }

    public string ImgStar5 { get => imgStar5; set { imgStar5 = value; OnPropertyChanged(nameof(ImgStar5)); } }

    public ICommand ImgStar1ClickCommand => new Command((e) => { Rate = 1; });

    public ICommand ImgStar2ClickCommand => new Command((e) => { Rate = 2; });

    public ICommand ImgStar3ClickCommand => new Command((e) => { Rate = 3; });

    public ICommand ImgStar4ClickCommand => new Command((e) => { Rate = 4; });

    public ICommand ImgStar5ClickCommand => new Command((e) => { Rate = 5; });

    protected void BuildRatingBar(int _rate)
    {
        switch (_rate)
        {
            case 0:
                ImgStar1 = ImgStar2 = ImgStar3 = ImgStar4 = ImgStar5 = emptyStar;
                break;
            case 1:
                ImgStar2 = ImgStar3 = ImgStar4 = ImgStar5 = emptyStar;
                ImgStar1 = solidStar;
                break;
            case 2:
                ImgStar3 = ImgStar4 = ImgStar5 = emptyStar;
                ImgStar1 = ImgStar2 = solidStar;
                break;
            case 3:
                ImgStar4 = ImgStar5 = emptyStar;
                ImgStar1 = ImgStar2 = ImgStar3 = solidStar;
                break;
            case 4:
                ImgStar5 = emptyStar;
                ImgStar1 = ImgStar2 = ImgStar3 = ImgStar4 = solidStar;
                break;
            case 5:
                ImgStar1 = ImgStar2 = ImgStar3 = ImgStar4 = ImgStar5 = solidStar;
                break;

        }
    }
}