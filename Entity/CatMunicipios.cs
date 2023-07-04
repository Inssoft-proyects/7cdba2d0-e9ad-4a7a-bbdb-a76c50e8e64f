using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatMunicipios
    {
        public int IdMunicipio { get; set; }

        public string Municipio { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<DiasInhabiles> DiasInhabiles { get; } = new List<DiasInhabiles>();

        public virtual ICollection<CatDelegacionesOficinasTransporte> CatDelegacionesOficinasTransportes { get; } = new List<CatDelegacionesOficinasTransporte>();


    }
}

