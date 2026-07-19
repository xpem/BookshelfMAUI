using ApiRepo;
using ApiRepo.Interfaces;
using Bookshelf.Services.Sync;
using Bookshelf.ViewModels;
using Bookshelf.ViewModels.Book;
using Bookshelf.ViewModels.GoogleSearch;
using Bookshelf.Views;
using Bookshelf.Views.Book;
using Bookshelf.Views.GoogleSearch;
using CommunityToolkit.Maui;
using System.Net;
using Services.Books.Historic.Sync;
using Services.Books.Historic;
using Services.Books.Interfaces;
using Services.Books.Sync;
using Services.Books;
using Services;
using Services.User;
using Services.Books.Historic.Interfaces;
using Repos;
using Repos.Interfaces;

namespace Bookshelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

        //fonts: https://fonts.google.com/specimen/Playfair+Display
        //icons: https://fontawesome.com/icons/right-to-bracket?s=solid
        builder.UseMauiApp<App>().ConfigureMauiHandlers(handlers => { })
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

        builder.Services.AddSingleton<ISyncService, SyncService>();

        #endregion;

        builder.Services.AddService();

        builder.Services.AddDbContextFactory<BookshelfDbContext>();

        builder.Services.AddApiRepo();

        builder.Services.AddRepo();

        #endregion

        return builder.Build();
    }

    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {

        services.AddTransient<AppShell, AppShellVM>();
        services.AddTransientWithShellRoute<Main, MainVM>(nameof(Main));
        services.AddTransientWithShellRoute<SignIn, SignInVM>(nameof(SignIn));
        services.AddTransientWithShellRoute<SignUp, SignUpVM>(nameof(SignUp));
        services.AddTransientWithShellRoute<UpdatePassword, UpdatePasswordVM>(nameof(UpdatePassword));
        services.AddTransientWithShellRoute<AddBook, AddBookVM>(nameof(AddBook));
        services.AddTransientWithShellRoute<BookList, BookListVM>(nameof(BookList));
        services.AddTransientWithShellRoute<BookDetail, BookDetailVM>(nameof(BookDetail));
        services.AddTransientWithShellRoute<GoogleBooksResults, GoogleBooksResultsVM>(nameof(GoogleBooksResults));
        services.AddTransientWithShellRoute<BookHistoric, BookHistoricVM>(nameof(BookHistoric));
        services.AddTransientWithShellRoute<Historic, HistoricVM>(nameof(Historic));
        services.AddTransientWithShellRoute<FirstSyncProcess, FirstSyncProcessVM>(nameof(FirstSyncProcess));

        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<IBookHistoricApiServices, BookHistoricApiServices>();
        services.AddScoped<IBookHistoricService, BookHistoricService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IBookApiService, BooksApiService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserSessionService, UserSessionService>();

        services.AddScoped<IBookHistoricSyncService, BookHistoricSyncService>();

        services.AddScoped<IBuildDbService, BuildDbService>();
        services.AddScoped<IBookSyncService, BookSyncService>();
        services.AddScoped<IBooksOperationService, BooksOperationService>();


        return services;
    }

    public static IServiceCollection AddApiRepo(this IServiceCollection services)
    {
        services.AddScoped<IHttpClientFunctions, HttpClientFunctions>();
        services.AddScoped<IUserApiRepo, UserApiRepo>();
        services.AddScoped<IBookHistoricApiRepo, BookHistoricApiRepo>();
        services.AddScoped<IBookApiRepo, BookApiRepo>();
        services.AddScoped<IOperationQueueRepo, OperationQueueRepo>();

        return services;
    }

    public static IServiceCollection AddRepo(this IServiceCollection services)
    {
        services.AddScoped<IBookRepo, BookRepo>();
        services.AddScoped<IBookHistoricRepo, BookHistoricRepo>();
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<ISyncCursorRepo, SyncCursorRepo>();

        return services;
    }

}
