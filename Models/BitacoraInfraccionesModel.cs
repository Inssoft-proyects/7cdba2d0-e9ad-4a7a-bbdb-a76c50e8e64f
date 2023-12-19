namespace GuanajuatoAdminUsuarios.Models
{
    public class BitacoraInfraccionesModel
    {

        public string folio { get; set; }

         public string operacion { get; set; }
         public string fecha { get; set; }
        public string hora { get; set; }
        public string ip { get; set; }
        public string nombre { get; set; }
        public string desc { get { return @"se registro infraccion con folio " + folio; } }

    }
}
