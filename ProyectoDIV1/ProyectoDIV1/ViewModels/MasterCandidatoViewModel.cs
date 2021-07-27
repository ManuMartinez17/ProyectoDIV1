﻿using Newtonsoft.Json;
using ProyectoDIV1.DTOs;
using ProyectoDIV1.Entidades.Models;
using ProyectoDIV1.Helpers;
using ProyectoDIV1.Interfaces;
using ProyectoDIV1.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProyectoDIV1.ViewModels
{
    public class MasterCandidatoViewModel : BaseViewModel
    {
        private CandidatoDTO _candidato;
        private CandidatoService _candidatoService;
        public MasterCandidatoViewModel()
        {
            _candidatoService = new CandidatoService();
            OnSignOut = new Command(OnSignOutClicked);
            CheckWhetherTheUserIsSignIn();
        }

        private async void CheckWhetherTheUserIsSignIn()
        {
            try
            {
                var authenticationService = DependencyService.Resolve<IAuthenticationService>();
                if (authenticationService.IsSignIn())
                {
                    string email = authenticationService.BuscarEmail();
                    var candidato = await BuscarIdCandidato(email);
                    if (candidato != null)
                    {
                        Settings.Usuario = JsonConvert.SerializeObject(candidato);
                        LoadCandidato();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        private async Task<ECandidato> BuscarIdCandidato(string email)
        {

            try
            {
                var candidato = await _candidatoService.GetIdXEmail(email);
                return candidato;
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
            return default;   
        }

        private void OnSignOutClicked()
        {
            var authService = DependencyService.Resolve<IAuthenticationService>();
            authService.SignOut();
            Shell.Current.GoToAsync("//LoginPage");
        }

        public Command OnSignOut { get; set; }

        private void LoadCandidato()
        {
            ECandidato candidato = JsonConvert.DeserializeObject<ECandidato>(Settings.Usuario);
            CandidatoDTO candidatoDTO = new CandidatoDTO()
            {
                Candidato = candidato

            };
            Candidato = candidatoDTO;
        }

        public CandidatoDTO Candidato
        {
            get => _candidato;
            set => SetProperty(ref _candidato, value);
        }
    }
}