namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaInfraccionModel : EntityModel
    {
        public int idPersonaInfraccion { get; set; }
        public string numeroLicencia { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public int idCatTipoPersona { get; set; }
        public string tipoPersona { get; set; }
        public string nombreCompleto { get { return nombre + " " + apellidoPaterno + " " + apellidoMaterno; } }
        public virtual PersonaDireccionModel PersonaDireccion { get; set; }

    }
}
