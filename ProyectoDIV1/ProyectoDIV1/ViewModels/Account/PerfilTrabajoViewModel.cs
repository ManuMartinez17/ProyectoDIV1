﻿using Acr.UserDialogs;
using Newtonsoft.Json;
using ProyectoDIV1.DTOs;
using ProyectoDIV1.Entidades.Models;
using ProyectoDIV1.Helpers;
using ProyectoDIV1.Services;
using ProyectoDIV1.Services.FirebaseServices;
using ProyectoDIV1.Services.Helpers;
using ProyectoDIV1.Services.Interfaces;
using ProyectoDIV1.ViewModels.Buscadores;
using ProyectoDIV1.Views;
using ProyectoDIV1.Views.Account;
using ProyectoDIV1.Views.Buscadores;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProyectoDIV1.ViewModels.Account
{

    public class PerfilTrabajoViewModel : BaseViewModel
    {
        #region attributes
        private readonly FirebaseHelper _firebase;
        private Token _token;
        private ECandidato _candidatoReceived = new ECandidato();
        private ArchivosDTO _archivos = new ArchivosDTO();
        private ObservableCollection<Lista> _habilidades;
        JobAndSkillService serviceJobsandSkills = new JobAndSkillService();
        private FirebaseStorageHelper _FirebaseStorageHelper;
        #endregion

        #region constructor
        public PerfilTrabajoViewModel()
        {
            _firebase = new FirebaseHelper();
            _FirebaseStorageHelper = new FirebaseStorageHelper();
            BuscarToken();
            GenerarToken();
            _archivos = JsonConvert.DeserializeObject<ArchivosDTO>(Settings.Archivos);
            CandidatoReceived = JsonConvert.DeserializeObject<ECandidato>(Settings.Candidato);
            RegisterCommand = new Command(RegistrarClicked);
            SignInCommand = new Command(OnSignInClicked);
            SearchJobCommand = new Command(async (text) => await ExecuteBusquedaJob(text));
            SearchSkillsCommand = new Command(async (text) => await ExecuteBusquedaSkills(text));
        }

        private async Task ExecuteBusquedaJob(object text)
        {
            await PopupNavigation.Instance.PushAsync(new PopupLoadingPage());
            var texto = text as string;
            try
            {
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    Settings.Candidato = JsonConvert.SerializeObject(CandidatoReceived);
                    Application.Current.MainPage = new MasterCandidatoPage();
                    await Shell.Current.GoToAsync($"{nameof(BusquedaJobPage)}?{nameof(BusquedaJobViewModel.Texto)}={texto}");
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task ExecuteBusquedaSkills(object text)
        {
            await PopupNavigation.Instance.PushAsync(new PopupLoadingPage());
            var texto = text as string;
            try
            {
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    Settings.Candidato = JsonConvert.SerializeObject(CandidatoReceived);
                    Application.Current.MainPage = new MasterCandidatoPage();
                    await Shell.Current.GoToAsync($"{nameof(BusquedaSkillsPage)}?{nameof(BusquedaSkillsViewModel.Texto)}={texto}");
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }

        private void BuscarToken()
        {
            _token = JsonConvert.DeserializeObject<Token>(Settings.Token);
        }


        #endregion

        #region commands
        public Command RegisterCommand { get; set; }
        public Command SignInCommand { get; set; }
        public Command BorrarHabilidadCommand
        {
            get
            {
                return new Command<Lista>((skill) =>
               {

                   if (skill != null)
                   {
                       Habilidades.Remove(skill);
                       _candidatoReceived.Habilidades.Remove(skill);
                       Settings.Candidato = JsonConvert.SerializeObject(_candidatoReceived);
                   }
               });
            }
        }
        public Command SearchJobCommand { get; set; }
        public Command SearchSkillsCommand { get; set; }

        #endregion

        #region Properties
        public ECandidato CandidatoReceived
        {
            get { return _candidatoReceived; }
            set
            {
                Habilidades = value != null ? new ObservableCollection<Lista>(value.Habilidades) : null;
                SetProperty(ref _candidatoReceived, value);

            }
        }

        public ObservableCollection<Lista> Habilidades
        {
            get { return _habilidades; }
            set { SetProperty(ref _habilidades, value); }
        }
        #endregion

        #region methods
        private void GenerarToken()
        {
            if (_token == null)
            {
                var token = serviceJobsandSkills.GenerarToken();
                Settings.Token = JsonConvert.SerializeObject(token);
            }
            else
            {
                if (_token.Expiration < DateTime.Now)
                {
                    var token = serviceJobsandSkills.GenerarToken();
                    Settings.Token = JsonConvert.SerializeObject(token);
                }
            }
        }

        private async void OnSignInClicked()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
        }

        private async void RegistrarClicked()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CandidatoReceived.Expectativa))
                {
                    UserDialogs.Instance.Alert("Ingrese una expectativa.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(CandidatoReceived.Profesion) || CandidatoReceived.Habilidades.Count == 0)
                {
                    UserDialogs.Instance.Alert("Tiene que tener una profesión, y una habilidad.");
                    return;
                }
                UserDialogs.Instance.ShowLoading("cargando...");
                CandidatoReceived.Habilidades = new List<Lista>();
                foreach (var item in Habilidades)
                {
                    CandidatoReceived.Habilidades.Add(item);
                }
                var authService = DependencyService.Resolve<IAuthenticationService>();
                if (await authService.Register($"{CandidatoReceived.Nombre} {CandidatoReceived.Apellido}",
                    CandidatoReceived.Email, _archivos.Password))
                {
                    if (_archivos.ImagenPerfil != null)
                    {
                        Stream stream = new MemoryStream(_archivos.ImagenPerfil);
                        CandidatoReceived.Rutas.RutaImagenRegistro = await _FirebaseStorageHelper.UploadFile(stream,
                            CandidatoReceived.Rutas.NombreImagenRegistro, Constantes.CARPETA_IMAGENES_PERFIL);
                    }
                    if (_archivos.Archivo != null)
                    {
                        Stream stream = new MemoryStream(_archivos.Archivo);
                        CandidatoReceived.Rutas.RutaArchivoRegistro = await _FirebaseStorageHelper.UploadFile(stream,
                            CandidatoReceived.Rutas.NombreArchivoRegistro, Constantes.CARPETA_HOJASDEVIDA);
                    }
                    await _firebase.CrearAsync(CandidatoReceived, Constantes.COLLECTION_CANDIDATO);
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Toast("se ha registrado satisfactoriamente", TimeSpan.FromSeconds(2));
                    await Task.Delay(2000);
                    Settings.Candidato = null;
                    Application.Current.MainPage = new MasterCandidatoPage();
                    await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                Debug.WriteLine(ex.Message);
            }

        }
        #endregion
    }
}