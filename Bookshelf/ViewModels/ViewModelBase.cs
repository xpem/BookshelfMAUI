﻿namespace Bookshelf.ViewModels
{
    public partial class ViewModelBase : BindableObject
    {

        bool isBusy;

        public bool IsBusy
        {
            get => isBusy; set { if (isBusy != value) { isBusy = value; OnPropertyChanged(nameof(IsBusy)); } }
        }

        public bool IsNotBusy => !isBusy;

        protected static bool IsOn => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
