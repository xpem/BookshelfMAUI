using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels;
using Bookshelf.Views;
using BookshelfServices.Books.Api;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;
using BookshelfServices.User.AuthServices;

namespace Bookshelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        //fonts: https://fonts.google.com/specimen/Playfair+Display
        //icons: https://fontawesome.com/icons/right-to-bracket?s=solid
        //resize icons: https://www.iloveimg.com/pt/redimensionar-imagem#resize-options,pixels
        var builder = MauiApp.CreateBuilder();
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
            });

        #region Dependency injections

        builder.Services.AddTransient<Main>();
        builder.Services.AddTransient<MainVM>();

        builder.Services.AddTransient<Access>();
        builder.Services.AddTransient<AccessVM>();

        //builder.Services.AddTransient<InsertUser>();
        //builder.Services.AddTransient<InsertUserVM>();

        //Services
        builder.Services.AddSingleton<INavigationServices, NavigationServices>();

        builder.Services.AddScoped<IUserAuthServices, UserAuthServices>();
        builder.Services.AddScoped<IUserServices, UserServices>();

        builder.Services.AddScoped<IBooksApiServices, BooksApiServices>();
        builder.Services.AddScoped<IBooksSyncServices, BooksSyncServices>();
       // builder.Services.AddScoped<IBooksLocalServices, BooksLocalServices>();


        #endregion

        return builder.Build();
    }
}
