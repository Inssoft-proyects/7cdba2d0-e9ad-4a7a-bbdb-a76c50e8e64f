using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class MotivosInfraccionModel
    {
        public int IdMotivoInfraccion { get; set; }

        public string Nombre { get; set; }

        public string Fundamento { get; set; }

        public int? CalificacionMinima { get; set; }

        public int? CalificacionMaxima { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string EstatusDesc { get; set; }

         public bool ValorEstatusMotivosInfraccion { get; set; }

    }
}
