using Bookshelf.ViewModels;
using Bookshelf.ViewModels.GoogleSearch;
using Bookshelf.Views;
using Bookshelf.Views.GoogleSearch;
using BookshelfServices.Books;
using BookshelfServices.Books.Api;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;
using CommunityToolkit.Maui;

namespace Bookshelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        var builder = MauiApp.CreateBuilder();


        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

        //fonts: https://fonts.google.com/specimen/Playfair+Display
        //icons: https://fontawesome.com/icons/right-to-bracket?s=solid
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("PlayfairDisplay-Medium.ttf", "PlayfairDisplayMedium");
                fonts.AddFont("PlayfairDisplay-Regular.ttf", "PlayfairDisplayRegular");
                fonts.AddFont("PlayfairDisplay-Italic.ttf", "PlayfairDisplayItalic");
                fonts.AddFont("PlayfairDisplay-Bold.ttf", "PlayfairDisplayBold");
                fonts.AddFont("EBGaramond-Italic.ttf", "EBGaramondItalic");
                fonts.AddFont("EBGaramond-Bold.ttf", "EBGaramondBold");
                fonts.AddFont("EBGaramond-SemiBold.ttf", "EBGaramondSemiBold");
                fonts.AddFont("EBGaramond-Regular.ttf", "EBGaramondRegular");
                fonts.AddFont("Free-Solid-900.otf", "FontAwesome");
            });

        #region Dependency injections


        builder.Services.AddTransient<Main>();
        builder.Services.AddTransient<MainVM>();

        builder.Services.AddTransient<SignIn>();
        builder.Services.AddTransient<SignInVM>();

        builder.Services.AddTransient<SignUp>();
        builder.Services.AddTransient<SignUpVM>();

        builder.Services.AddTransient<UpdatePassword>();
        builder.Services.AddTransient<UpdatePasswordVM>();

        builder.Services.AddTransient<AddBook>();
        builder.Services.AddTransient<AddBookVM>();

        builder.Services.AddTransient<BookList>();
        builder.Services.AddTransient<BookListVM>();

        builder.Services.AddTransient<BookDetail>();
        builder.Services.AddTransient<BookDetailVM>();

        builder.Services.AddTransient<GoogleBooksResults>();
        builder.Services.AddTransient<GoogleBooksResultsVM>();

        builder.Services.AddScoped<IUserServices, UserServices>();

        builder.Services.AddScoped<IBooksApiServices, BooksApiServices>();
        builder.Services.AddScoped<IBooksSyncServices, BooksSyncServices>();
        builder.Services.AddScoped<IBooksServices, BooksServices>();

        #endregion

        return builder.Build();
    }
}
