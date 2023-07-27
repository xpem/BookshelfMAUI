﻿using Bookshelf.ViewModels;
using Bookshelf.ViewModels.GoogleSearch;
using Bookshelf.Views;
using Bookshelf.Views.GoogleSearch;
using BLL.Books;
using BLL.Books.Api;
using BLL.Books.Sync;
using BLL.User;
using CommunityToolkit.Maui;

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

        builder.Services.AddScoped<IBooksSyncBLL, BooksSyncBLL>();
        builder.Services.AddScoped<IBooksBLL, BooksBLL>();

        #endregion

        return builder.Build();
    }
}
