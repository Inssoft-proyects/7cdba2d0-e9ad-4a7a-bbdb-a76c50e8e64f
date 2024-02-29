using GuanajuatoAdminUsuarios.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesModel : EntityModel
    {



        public int? transito { get; set; }
        public string ObsevacionesApl { get; set; }
        public int idInfraccion { get; set; }
        public int? idOficial { get; set; }
        public int? idDependencia { get; set; }
        public int? idDelegacion { get; set; }
        public int? idVehiculo { get; set; }
        public int? idAplicacion { get; set; }
        public int? idGarantia { get; set; }
        public int? idEstatusInfraccion { get; set; }
        public int? idMunicipio { get; set; }
        public string municipio { get; set; }
        public int? idTramo { get; set; }
        public int? idCarretera { get; set; }
        public int? idPropitario { get; set; }
        
        public int? idPersona { get; set; }
        public int? idPersonaInfraccion { get; set; }
        public int? idCortesia { get; set; }
        
        public string placasVehiculo { get; set; }
        public string folioInfraccion { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ?fechaNacimiento { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime fechaInfraccion { get; set; } 
		public DateTime horaInfraccion { get; set; } 

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime fechaVencimiento { get; set; } = DateTime.Now;

		public string aplicacion { get; set; }

		public void CalcularFechas()
        {
            // Agregar 10 d�as a la fecha de imposici�n para obtener la fecha de vencimiento
            fechaVencimiento = fechaInfraccion.AddDays(10);
        }
        public string kmCarretera { get; set; }
        public string observaciones { get; set; }
        public string? lugarCalle { get; set; }
        public string? lugarNumero { get; set; }
        public string? lugarColonia { get; set; }
        public string? lugarEntreCalle { get; set; }
        public bool? infraccionCortesia { get; set; }
        public int? infraccionCortesiaValue { get; set; }

        public string infraccionCortesiaString { get; set; }

        public int cortesiaInt { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public bool isPropietarioConductor { get; set; }
        public string strIsPropietarioConductor { get; set; }
        public string estatusInfraccion { get; set; }
        public string observacionesCortesia { get; set; }
        public string nombreOficial { get; set; }
        public string apellidoPaternoOficial { get; set; }
        public string apellidoMaternoOficial { get; set; }
        public string nombreCompletoOficial
        {
            get
            {
                return $"{nombreOficial} {apellidoPaternoOficial} {apellidoMaternoOficial}";
            }
        }
        public string tramo { get; set; }
        public string telefono { get; set; }      
        public string carretera { get; set; }
        public virtual VehiculoModel Vehiculo { get; set; }
        public PersonaModel Persona { get; set; }
        
        public virtual PersonaInfraccionModel PersonaInfraccion { get; set; }
         public virtual PersonaModel PersonaInfraccion2 { get; set; }

        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }

        #region Columnas Adicionales Reportes
        public string delegacion { get; set; }
        public string NombreConductor { get; set; }
        public string NombrePropietario { get; set; }
        public string NombreGarantia { get; set; }

        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }
        public int Total { get; set; }
        #endregion
    }
    public class InfraccionesReportModel : EntityModel
    {

        public string horaInfraccion { get; set; }
        public string AplicadaA { get; set; }
        public string observaciones { get; set; }
        public decimal Uma { get; set; }
        public int idInfraccion { get; set; }
        public string folioInfraccion { get; set; }
        public DateTime fechaInfraccion { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public string estatusInfraccion { get; set; }
        public string nombreOficial { get; set; }  
        public string municipio { get; set; }
        public string carretera { get; set; }
        public string tramo { get; set; }
        public string kmCarretera { get; set; }
        public string nombreConductor { get; set; }
        public string domicilioConductor { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? fechaNacimientoConductor { get; set; }
        public int? edadConductor { get; set; }
        public string generoConductor { get; set; }
        public string telefonoConductor { get; set; }
        public string numLicenciaConductor { get; set; }
        public string tipoLicenciaConductor { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? vencimientoLicConductor { get; set; }
        public string placas {  get; set; }
        public string tipoVehiculo { get; set; }
        public string marcaVehiculo { get; set; }
        public string nombreSubmarca { get; set; }
        public string modelo { get; set; }
        public string color { get; set; }
        public string nombrePropietario { get; set; }
        public string domicilioPropietario { get; set; }
        public string serie { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public string nombreEntidad { get; set; }
        public string tipoServicio { get; set; }
        public string numeroEconomico {  get; set; }
        public string cortesia { get; set; }

        public bool tieneCortesia { get; set; }
        public decimal montoCalificacion { get; set; }
        public decimal montoPagado { get; set; }
        public string reciboPago { get; set; }
        public string oficioCondonacion { get;set; }
        public DateTime? fechaPago { get; set; }
        public string lugarPago { get; set; }
        public string concepto { get; set; }
        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }
        public int idGarantia { get; set; }
        public virtual List<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }
    }
}
