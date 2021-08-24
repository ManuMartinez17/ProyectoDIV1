﻿using Acr.UserDialogs;
using Newtonsoft.Json;
using ProyectoDIV1.Entidades.Models;
using ProyectoDIV1.Helpers;
using ProyectoDIV1.Interfaces;
using ProyectoDIV1.Models;
using ProyectoDIV1.Services;
using ProyectoDIV1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProyectoDIV1.ViewModels
{
    [QueryProperty(nameof(Texto), nameof(Texto))]
    public class BusquedaSkillsViewModel : BaseViewModel
    {
        private string _texto;
        private ECandidato _candidato = new ECandidato();
        private JobAndSkillService serviceJobsandSkills = new JobAndSkillService();
        private ObservableCollection<Skill> _tiposDeKills;

        public BusquedaSkillsViewModel()
        {

            _candidato = JsonConvert.DeserializeObject<ECandidato>(Settings.Candidato);
            InsertarCommand = new Command((param) => ExecuteInsertarSkills(param));
            BackCommand = new Command(BackClicked);
            BorrarCommand = new Command((param) => ExecuteBorrarSkills(param));
            GuardarCommand = new Command(OnGuardarClicked);
        }

        private async void BackClicked()
        {
            var authenticationService = DependencyService.Resolve<IAuthenticationService>();
            if (authenticationService.IsSignIn())
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                Application.Current.MainPage = new NavigationPage();
                await Application.Current.MainPage.Navigation.PushAsync(new PerfilTrabajoPage());
            }
        }

        private void ExecuteBorrarSkills(object param)
        {
            string habilidad = param as string;
            if (_candidato != null)
            {
                var query = _candidato.Habilidades.Where(x => x.Nombre.Equals(habilidad)).FirstOrDefault();
                if (query != null)
                {
                    _candidato.Habilidades.Remove(query);
                }
                Settings.Candidato = JsonConvert.SerializeObject(_candidato);
            }
        }
        private void ExecuteInsertarSkills(object param)
        {
            string habilidad = param as string;
            Lista item = new Lista()
            {
                Nombre = habilidad
            };
            if (_candidato == null)
            {
                _candidato = new ECandidato();
            }
            _candidato.Habilidades.Add(item);
            Settings.Candidato = JsonConvert.SerializeObject(_candidato);
        }


        public Command InsertarCommand { get; set; }
        public Command BackCommand { get; set; }
        public Command BorrarCommand { get; set; }
        public Command GuardarCommand { get; set; }
        public ObservableCollection<Skill> TiposDeKills
        {
            get { return _tiposDeKills; }
            set { SetProperty(ref _tiposDeKills, value); }
        }
        public string Texto
        {
            get
            {
                return _texto;
            }
            set
            {
                _texto = value;
                LoadSkills(value);
            }
        }

        private async void LoadSkills(string value)
        {
            var token = JsonConvert.DeserializeObject<Token>(Settings.Token);
            if (token != null && !string.IsNullOrWhiteSpace(value))
            {
                var lista = await serviceJobsandSkills.GetListJobsRelatedSkills($"skills/versions/latest/skills?q=.{value}", token.access_token);
                TiposDeKills = new ObservableCollection<Skill>(lista.data);
            }
        }

        private async void OnGuardarClicked()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("guardando...");
                if (_candidato != null)
                {
                    if (_candidato.Habilidades.Count == 0)
                    {
                        UserDialogs.Instance.Alert("Guarde por lo menos una habilidad.");
                        return;
                    }   
                    var authenticationService = DependencyService.Resolve<IAuthenticationService>();
                    if (authenticationService.IsSignIn())
                    {
                        var query = await new CandidatoService().GetCandidatoFirebaseObjectAsync(_candidato.UsuarioId);
                        await new FirebaseHelper().UpdateAsync(_candidato, Constantes.COLLECTION_CANDIDATO, query);
                        Settings.Usuario = JsonConvert.SerializeObject(_candidato);
                        await Shell.Current.GoToAsync($"../../{nameof(EditarHojaDeVidaPage)}");
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage();
                        Settings.Candidato = JsonConvert.SerializeObject(_candidato);
                        await Application.Current.MainPage.Navigation.PushAsync(new PerfilTrabajoPage());
                    }
                   
                }
                else
                {
                    UserDialogs.Instance.Alert("Guarde por lo menos una habilidad.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }


    }
}
