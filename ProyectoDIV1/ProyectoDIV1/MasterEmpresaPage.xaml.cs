﻿using ProyectoDIV1.ViewModels;
using ProyectoDIV1.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoDIV1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterEmpresaPage : Shell
    {
        Dictionary<string, Type> routes = new Dictionary<string, Type>();

        public Dictionary<string, Type> Routes { get { return routes; } }

        public MasterEmpresaPage()
        {

            InitializeComponent();
            RegisterRoutes();
            BindingContext = new MasterEmpresaViewModel();
        }

        private void RegisterRoutes()
        {
            routes.Add(nameof(BusquedaJobPage), typeof(BusquedaJobPage));
            routes.Add(nameof(BusquedaSkillsPage), typeof(BusquedaSkillsPage));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }
    }
}