﻿using ProyectoDIV1.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoDIV1.DTOs
{
    public class NotificacionDTO
    {
        public ENotificacion Notificacion { get; set; } = new ENotificacion();
        public ECandidato CandidatoEmisor { get; set; } = new ECandidato();
        public ECandidato CandidatoReceptor { get; set; } = new ECandidato();
        public EEmpresa EmpresaEmisor { get; set; } = new EEmpresa();
        public EEmpresa EmpresaReceptor { get; set; } = new EEmpresa();
        public string FullNameCandidato => $"De: {CandidatoEmisor.Nombre} {CandidatoEmisor.Apellido}";
        public string IsVisto => Notificacion.EstadoVisto == false ? "icon_noView.png" : "icon_view.png";
        public string IsAccepted => Notificacion.EstadoAceptado == false ? "Pendiente" : "Aceptado";

        public string IconIsAccepted => Notificacion.EstadoAceptado == false ? "icon_pending.png" : "icon_checked.png";
    }
}
