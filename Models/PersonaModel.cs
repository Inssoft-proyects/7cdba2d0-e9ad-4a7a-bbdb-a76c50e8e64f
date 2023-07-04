using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaModel : EntityModel
    {
        public int? idPersona { get; set; }
        public string numeroLicencia { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombreCompleto { get { return nombre + " " + apellidoPaterno + " " + apellidoMaterno; } }
        public int idCatTipoPersona { get; set; }
        public string tipoPersona { get; set; }
        public int idGenero { get; set; }
        public string genero { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public int idTipoLicencia { get; set; }
        public string tipoLicencia { get; set; }
        public DateTime vigenciaLicencia { get; set; }
        public virtual PersonaDireccionModel PersonaDireccion { get; set; }

    }
}
