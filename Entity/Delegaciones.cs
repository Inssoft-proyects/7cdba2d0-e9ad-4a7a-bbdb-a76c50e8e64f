using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class Delegaciones
    {
        public int IdDelegacion { get; set; }

        public string Delegacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<CatOficinasRenta> OficinasRenta { get; } = new List<CatOficinasRenta>();

        public virtual ICollection<CatAgenciasMinisterio> AgenciasMinisterios { get; } = new List<CatAgenciasMinisterio>();


    }
}
