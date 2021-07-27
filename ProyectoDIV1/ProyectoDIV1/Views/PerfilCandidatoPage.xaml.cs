﻿using ProyectoDIV1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoDIV1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PerfilCandidatoPage : ContentPage
    {
        public PerfilCandidatoPage()
        {
            InitializeComponent();
            BindingContext = new PerfilCandidatoViewModel();
        }
    }
}