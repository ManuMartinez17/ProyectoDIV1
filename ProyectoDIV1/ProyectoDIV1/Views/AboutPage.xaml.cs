﻿using ProyectoDIV1.Interfaces;
using ProyectoDIV1.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoDIV1.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            CheckWhetherTheUserIsSignIn();
            BindingContext = new AboutViewModel();
        }
        private async void CheckWhetherTheUserIsSignIn()
        {
            try
            {
                var authenticationService = DependencyService.Resolve<IAuthenticationService>();
                if (!authenticationService.IsSignIn())
                    await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}