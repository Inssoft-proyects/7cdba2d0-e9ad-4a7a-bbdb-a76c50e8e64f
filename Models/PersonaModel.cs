using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaModel : EntityModel
    {
        public int? idPersona { get; set; }
        public string numeroLicencia { get; set; }
        public string numeroLicenciaBusqueda { get; set; }
        public string numeroLicenciaFisico { get; set; }

        public string CURP { get; set; }
       
        
        [MinLength(10, ErrorMessage = "El CURO debe tener al menos 10 caracteres.")]
        public string CURPBusqueda { get; set; }
        public string CURPFisico { get; set; }

        public string RFC { get; set; }
        [MinLength(10, ErrorMessage = "El RFC debe tener al menos 10 caracteres.")]
        public string RFCBusqueda { get; set; }
        public string RFCFisico { get; set; }

        public string nombre { get; set; }
        public string nombreBusqueda { get; set; }
        public string nombreFisico { get; set; }
        
        public string apellidoPaterno { get; set; }
        public string apellidoPaternoBusqueda { get; set; }
        public string apellidoPaternoFisico { get; set; }

        public string apellidoMaterno { get; set; }
        public string apellidoMaternoBusqueda { get; set; }
        public string apellidoMaternoFisico { get; set; }

        public string nombreCompleto { get {
                
                return (nombre??"-") + " " + (apellidoPaterno??"-") + " " + (apellidoMaterno ?? "-"); } }
        public int? idCatTipoPersona { get; set; }
        public string? tipoPersona { get; set; }
        public int? idGenero { get; set; }
        public string genero { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public int? idTipoLicencia { get; set; }
        public int? idTipoLicenciaInfraccion { get; set; }
        public string? telefonoInfraccion { get; set; }
        public string? tipoLicencia { get; set; }
        public string? telefono { get; set; }

        public string? codigoPostal { get; set; }

        public string? colonia { get; set; }

        public string? calle { get; set; }

        public string? numero { get; set; }
        public string correo { get; set; }
        public string correoInfraccion { get; set; }
        public bool generoBool { get; set; }
      
        public DateTime? vigenciaLicencia { get; set; }
        public DateTime? vigenciaLicenciaFisico { get; set; }

        public virtual PersonaDireccionModel PersonaDireccion { get; set; }

        public int total { get; set; }
    }
}
