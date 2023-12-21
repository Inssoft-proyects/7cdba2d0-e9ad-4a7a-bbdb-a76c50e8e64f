using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatMotivosInfraccionModel :EntityModel
    {
        public int IdCatMotivoInfraccion { get; set; }
        public string Nombre { get; set; }
        public string Fundamento { get; set; }
        public int CalificacionMinima { get; set; }
        public int CalificacionMaxima { get; set; }
        public int idConcepto { get; set; }
        public string concepto { get; set; }
        public int idSubConcepto { get; set; }
        public string subConcepto { get;set; }
        public bool ValorEstatusMotivosInfraccion { get; set; }
        public string estatusDesc { get; set; }


    }
}
