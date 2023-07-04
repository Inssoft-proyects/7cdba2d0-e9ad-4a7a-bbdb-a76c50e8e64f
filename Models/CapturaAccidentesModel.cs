using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CapturaAccidentesModel
    {
        public int? IdAccidente { get; set; }
        public string NumeroReporte { get; set; }
        [Required(ErrorMessage = "-El campo Fecha es obligatorio")]
        public DateTime? Fecha { get; set; }
        [Required(ErrorMessage = "-El campo Hora  es obligatorio")]
        public TimeSpan? Hora { get; set; }

        [Required(ErrorMessage = "-Debe seleccionar una opción para Municipio")]
        public int? IdMunicipio { get; set; }

        [Required(ErrorMessage = "-Debe seleccionar una opción para Carretera")]
        public int? IdCarretera { get; set; }

        [Required(ErrorMessage = "-Debe seleccionar una opción para Tramo")]
        public int? IdTramo { get; set; }

        [Required(ErrorMessage = "-El campo Kilometro es obligatorio")]
        public string Kilometro { get; set; }
        public int EstatusReporte { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int ActualizadoPor { get; set; }
        public int estatus { get; set; }

        public string Municipio { get; set; }
        public string Tramo { get; set; }
        public string Carretera { get; set; }
        public List<CapturaAccidentesModel> ListaAccidentes { get; internal set; }

        /// <summary>
        /// MODEL VEHICULOS
        /// </summary>

        public int IdVehiculo { get; set; }
        public int IdMarcaVehiculo { get; set; }
        public int IdSubmarca { get; set; }
        public int IdEntidad { get; set; }
        public int IdColor { get; set; }
        public int IdTipoVehiculo { get; set; }
        public int IdCatTipoServicio { get; set; }
        public int IdPersona { get; set; }
        public string Serie { get; set; }
        public string Placa { get; set; }
        public string Tarjeta { get; set; }
        public DateTime VigenciaTarjeta { get; set; }
        public string Marca { get; set; }
        public string Submarca { get; set; }
        public string TipoVehiculo { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string EntidadRegistro { get; set; }
        public string TipoServicio { get; set; }
        public string Propietario { get; set; }

        public string NumeroEconomico { get; set; }
        public int IdPersonaFisica { get; set; }
        public int IdPersonaMoral { get; set; }
        
        public string DatoBusquedaVehiculo { get; set; }

        ///////////
        /////////////////MODEL CLASIFICACION//////////
        ///
        public int IdClasificacionAccidente { get; set; }

        public string NombreClasificacion { get; set; }


        //////////////////
        ///MODEL CAPTURA PARRTE 2
        ///

        public int IdFactorAccidente { get; set; }
        public int IdFactorOpcionAccidente { get; set; }
        public string FactorAccidente { get; set; }
        public string FactorOpcionAccidente { get; set; }
        public int IdCausaAccidente { get; set; }
        public string CausaAccidente { get; set; }

        public string DescripcionCausa { get; set; }













    }
}
