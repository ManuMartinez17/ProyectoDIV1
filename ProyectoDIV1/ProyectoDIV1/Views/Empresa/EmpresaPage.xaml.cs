﻿using ProyectoDIV1.ViewModels.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoDIV1.Views.Empresa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmpresaPage : ContentPage
    {
        public EmpresaPage()
        {
            InitializeComponent();
            BindingContext = new EmpresaViewModel();
        }
    }
}