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
        public int IdTipoLicencia { get; set; }
        public int IdCatTipoPersona { get; set; }
        public DateTime fechaNacimiento { get; set; }

        public DateTime vigenciaLicencia { get; set; }
        public string TipoPersona { get; set; }
        public string TipoLicencia { get; set; }
        public string Genero { get; set; }

        public int IdTipoCarga { get; set; }
        public string Poliza { get; set; }

        public int IdDelegacion { get; set; }
        public int IdPension { get; set; }
        public int IdFormaTraslado { get; set; }

        //////////
        ///MODEL INVOLUCRADOS///////////
        ///

        public int IdVehiculoInvolucrado { get; set; }
        public int IdConductorInvolucrado { get; set; }
        public int IdPropietarioInvolucrado { get; set; }
        public int IdFormaTrasladoInvolucrado { get; set; }
        public string VehiculoInvolucrado { get; set; }
        public string PropietarioInvolucrado { get; set; }
        public string FormaTrasladoInvolucrado { get; set; }
        public string Pension { get; set; }
        public string Motor { get; set; }
        public string Capacidad { get; set; }
        public string ConductorInvolucrado { get; set; }





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
        public int IdCausaAccidenteEdit { get; set; }
        
        public string CausaAccidente { get; set; }

        public string DescripcionCausa { get; set; }

        public int idPersonaInvolucrado { get; set; }
        public string licencia { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }

        /////////////////
        ///CAPTURA PARTE 3//////////////
        ///

        public float montoCamino { get; set; }
        public float montoCarga { get; set; }
        public float montoPropietarios { get; set; }
        public float montoOtros { get; set; }
        public int IdInfraccionAccidente { get; set; }
        public int IdInfraccion { get; set; }//Folio de infraccion
        public float montoVehiculo { get; set; }











    }
}
