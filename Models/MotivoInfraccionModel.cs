namespace GuanajuatoAdminUsuarios.Models
{
    public class MotivoInfraccionModel : EntityModel
    {
        public int idMotivoInfraccion { get; set; }
        public string nombre { get; set; }
        public string fundamento { get; set; }
        public int? calificacionMinima { get; set; }
        public int? calificacionMaxima { get; set; }
        public int idCatMotivosInfraccion { get; set; }
        public int idInfraccion { get; set; }
        public int? calificacion { get; set; }
        public string catMotivo { get; set; }
        public int IdSubConcepto { get; set; }
        public string subConcepto { get; set; }
        public int idConcepto { get; set; }
        public string concepto { get; set; }


    }
}
