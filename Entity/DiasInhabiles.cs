using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class DiasInhabiles
    {
        public int idDiaInhabil { get; set; }

        public DateTime fecha { get; set; }

        public int idMunicipio { get; set; }

        public int todosMunicipiosBool { get; set; }

        public string todosMunicipiosDesc { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<CatMunicipios> Municipio { get; } = new List<CatMunicipios>();

    }
}

