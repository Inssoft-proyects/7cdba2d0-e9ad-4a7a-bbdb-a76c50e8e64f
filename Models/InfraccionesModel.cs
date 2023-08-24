using GuanajuatoAdminUsuarios.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesModel : EntityModel
    {
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
        public int? idPersona { get; set; }
        public int? idPersonaInfraccion { get; set; }
        public string placasVehiculo { get; set; }
        public string folioInfraccion { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime fechaNacimiento { get; set; } = DateTime.Now;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime fechaInfraccion { get; set; } = DateTime.Now;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime fechaVencimiento { get; set; } = DateTime.Now;

        public void CalcularFechas()
        {
            // Agregar 10 días a la fecha de imposición para obtener la fecha de vencimiento
            fechaVencimiento = fechaInfraccion.AddDays(10);
        }
        public string kmCarretera { get; set; }
        public string observaciones { get; set; }
        public string lugarCalle { get; set; }
        public string lugarNumero { get; set; }
        public string lugarColonia { get; set; }
        public string lugarEntreCalle { get; set; }
        public bool? infraccionCortesia { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public bool isPropietarioConductor { get; set; }
        public string strIsPropietarioConductor { get; set; }
        public string estatusInfraccion { get; set; }
        public string observacionesCortesia { get; set; }
        
        public virtual VehiculoModel Vehiculo { get; set; }
        public PersonaModel Persona { get; set; }
        public virtual PersonaInfraccionModel PersonaInfraccion { get; set; }
        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }

        #region Columnas Adicionales Reportes
        public string delegacion { get; set; }
        public string NombreConductor { get; set; }
        public string NombrePropietario { get; set; }
        public string NombreGarantia { get; set; }

        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }

        #endregion
    }
}
