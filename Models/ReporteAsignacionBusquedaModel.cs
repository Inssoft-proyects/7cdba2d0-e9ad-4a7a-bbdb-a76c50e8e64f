using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ReporteAsignacionBusquedaModel
    {
        public int? IdGrua { get; set; }
        public int? IdPension { get; set; }
        public int? IdEvento { get; set; }
        public string Evento { get; set; } = null!;
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaFin { get; set; }
        public List<ReporteAsignacionModel> ListReporteAsignacion { get; set; }
    }
}
