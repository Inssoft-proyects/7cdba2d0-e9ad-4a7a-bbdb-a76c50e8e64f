using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaDireccionModel : EntityModel
    {
        public int idPersonasDirecciones { get; set; }
        public int? idEntidad { get; set; }
        public string entidad { get; set; }
        public int? idMunicipio { get; set; }
        public string municipio { get; set; }
        public string codigoPostal { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        public string numero { get; set; }
        public long telefono { get; set; }
        public string correo { get; set; }
        public int? idPersona { get; set; }

    }
}
