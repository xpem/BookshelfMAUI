using ApiDAL;
using ApiDAL.Interfaces;
using BLL.Books;
using BLL.Books.Historic;
using BLL.Books.Historic.Interfaces;
using BLL.Books.Historic.Sync;
using BLL.Books.Sync;
using BLL.Sync;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.ViewModels;
using Bookshelf.ViewModels.Book;
using Bookshelf.ViewModels.GoogleSearch;
using Bookshelf.Views;
using Bookshelf.Views.Book;
using Bookshelf.Views.GoogleSearch;
using CommunityToolkit.Maui;
using LocalDbDAL.Books;
using LocalDbDAL.Books.BookHistoric;
using LocalDbDAL.User;

namespace Bookshelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        //todo
        //criar uma branch para o desenvolvimento da timeline
        //criar branch para o desenvolvimento da abstração da camada dal.

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

        #region UIs

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

        builder.Services.AddTransient<BookHistoric>();
        builder.Services.AddTransient<BookHistoricVM>();

        #endregion

        #region UIServices

        builder.Services.AddSingleton<ISyncServices, SyncServices>();

        #endregion;

        #region BLL

        builder.Services.AddScoped<IBookHistoricApiBLL, BookHistoricApiBLL>();
        builder.Services.AddScoped<IBookHistoricBLL, BookHistoricBLL>();
        builder.Services.AddScoped<IBooksBLL, BooksBLL>();
        builder.Services.AddScoped<IBooksApiBLL, BooksApiBLL>();
        builder.Services.AddScoped<IUserBLL, UserBLL>();

        builder.Services.AddScoped<IBookHistoricSyncBLL, BookHistoricSyncBLL>();
        builder.Services.AddScoped<IBookSyncBLL, BookSyncBLL>();       

        #endregion

        #region LocalDAL

        builder.Services.AddScoped<IBookHistoricLocalDAL, BookHistoricLocalDAL>();
        builder.Services.AddScoped<IUserLocalDAL, UserLocalDAL>();
        builder.Services.AddScoped<IBookLocalDAL, BookLocalDAL>();

        #endregion;

        #region ApiDAL

        builder.Services.AddScoped<IHttpClientFunctions, HttpClientFunctions>();
        builder.Services.AddScoped<IUserApiDAL, UserApiDAL>();
        builder.Services.AddScoped<IBookHistoricApiDAL, BookHistoricApiDAL>();
        builder.Services.AddScoped<IBookApiDAL, BookApiDAL>();
        #endregion

        #endregion

        return builder.Build();
    }
}
