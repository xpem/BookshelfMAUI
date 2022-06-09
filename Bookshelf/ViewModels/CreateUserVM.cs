﻿using Bookshelf.Utils;
using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfModels.User;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Connectivity;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class CreateUserVM : ViewModelBase
    {
        readonly IUserServices userService;
        readonly IBooksSyncServices booksSyncServices;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty] 
        string confirmPassword;

        [ObservableProperty]
        bool btnCreateUserIsEnabled = true;

        public CreateUserVM(INavigationServices _navigation, IUserServices _userService, IBooksSyncServices _booksSyncServices)
        {
            navigation = _navigation;
            userService = _userService;
            booksSyncServices = _booksSyncServices;
        }

        private bool VerifyFileds()
        {
            bool validInformation = true;

            if (string.IsNullOrEmpty(Email))
            {
                validInformation = false;
            }
            else if (!Validations.ValidateEmail(Email))
            {
                _ = Application.Current.MainPage.DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(Password))
            {
                validInformation = false;
            }
            else if (Password.Length < 4)
            {
                validInformation = false;
            }
            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                validInformation = false;
            }
            if (ConfirmPassword.ToUpper() != Password.ToUpper())
            {
                validInformation = false;
            }

            if (!validInformation)
            {
                _ = Application.Current.MainPage.DisplayAlert("Aviso", "Preencha os campos e confirme a senha corretamente", null, "Ok");
            }

            return validInformation;
        }

        public ICommand CreateUserCommand => new Command(async () =>
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                _ = await Application.Current.MainPage.DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            if (VerifyFileds())
            {
                BtnCreateUserIsEnabled = false;

                //
                User user = await userService.InsertUser(email, password);

                if (user != null)
                {
                    if (user.Error != null)
                    {
                        if (user.Error == ErrorType.EMAIL_EXISTS)
                            await Application.Current.MainPage.DisplayAlert("Aviso", "Email já cadastrado!", null, "Ok");
                        else
                            await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível cadastrar o usuário!", null, "Ok");
                    }
                    else
                    {
                        bool res = await Application.Current.MainPage.DisplayAlert("Aviso", "Usuário cadastrado!", null, "Ok");

                        if (!res)
                        {
                            Thread thread = new(booksSyncServices.SyncLocalDb) { IsBackground = true };
                            thread.Start();

                            Application.Current.MainPage = new NavigationPage();
                            _ = (Application.Current.MainPage.Navigation).PushAsync(navigation.ResolvePage<Main>(), true);
                        }
                    }
                }
            }
        });
    }
}
