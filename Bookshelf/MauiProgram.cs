using ApiDAL;
using ApiDAL.Interfaces;
using BLL;
using BLL.Books;
using BLL.Books.Historic;
using BLL.Books.Historic.Interfaces;
using BLL.Books.Historic.Sync;
using BLL.Books.Sync;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.ViewModels;
using Bookshelf.ViewModels.Book;
using Bookshelf.ViewModels.GoogleSearch;
using Bookshelf.Views;
using Bookshelf.Views.Book;
using Bookshelf.Views.GoogleSearch;
using CommunityToolkit.Maui;
using DbContextDAL;
using DBContextDAL;

namespace Bookshelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        //todo
        //criar uma branch para o desenvolvimento da timeline
        //criar branch para o desenvolvimento da abstração da camada dal.

        MauiAppBuilder builder = MauiApp.CreateBuilder();

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

        builder.Services.AddUIServices();

        #region UIServices

        builder.Services.AddSingleton<ISyncServices, SyncServices>();

        #endregion;

        builder.Services.AddBLLServices();

        builder.Services.AddDbContext<BookshelfDbContext>();

        builder.Services.ApiDALServices();

        builder.Services.DALServices();

        #endregion

        return builder.Build();
    }

    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        services.AddTransient<Main>();
        services.AddTransient<MainVM>();

        services.AddTransient<SignIn>();
        services.AddTransient<SignInVM>();

        services.AddTransient<SignUp>();
        services.AddTransient<SignUpVM>();

        services.AddTransient<UpdatePassword>();
        services.AddTransient<UpdatePasswordVM>();

        services.AddTransient<AddBook>();
        services.AddTransient<AddBookVM>();

        services.AddTransient<BookList>();
        services.AddTransient<BookListVM>();

        services.AddTransient<BookDetail>();
        services.AddTransient<BookDetailVM>();

        services.AddTransient<GoogleBooksResults>();
        services.AddTransient<GoogleBooksResultsVM>();

        services.AddTransient<BookHistoric>();
        services.AddTransient<BookHistoricVM>();

        services.AddTransient<FirstSyncProcess>();
        services.AddTransient<FirstSyncProcessVM>();
        return services;
    }

    public static IServiceCollection AddBLLServices(this IServiceCollection services)
    {
        services.AddScoped<IBookHistoricApiBLL, BookHistoricApiBLL>();
        services.AddScoped<IBookHistoricBLL, BookHistoricBLL>();
        services.AddScoped<IBookBLL, BookBLL>();
        services.AddScoped<IBookApiBLL, BooksApiBLL>();
        services.AddScoped<IUserBLL, UserBLL>();

        services.AddScoped<IBookHistoricSyncBLL, BookHistoricSyncBLL>();

        services.AddScoped<IBuildDbBLL, BuildDbBLL>();
        services.AddScoped<IBookSyncBLL, BookSyncBLL>();
        services.AddScoped<IBooksOperationBLL, BooksOperationBLL>();

        return services;
    }

    public static IServiceCollection ApiDALServices(this IServiceCollection services)
    {
        services.AddScoped<IHttpClientFunctions, HttpClientFunctions>();
        services.AddScoped<IUserApiDAL, UserApiDAL>();
        services.AddScoped<IBookHistoricApiDAL, BookHistoricApiDAL>();
        services.AddScoped<IBookApiDAL, BookApiDAL>();
        services.AddScoped<IOperationQueueDAL, OperationQueueDAL>();

        return services;
    }

    public static IServiceCollection DALServices(this IServiceCollection services)
    {
        services.AddScoped<IBookDAL, BookDAL>();
        services.AddScoped<IBookHistoricDAL, BookHistoricDAL>();
        services.AddScoped<IUserDAL, UserDAL>();

        return services;
    }

}
