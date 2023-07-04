using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatDelegacionesOficinasTransporte
    {
        public int IdOficinaTransporte { get; set; }

        public string NombreOficina { get; set; }

        public string JefeOficina { get; set; }

        public int IdMunicipio { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<CatMunicipios> CatMunicipios { get; } = new List<CatMunicipios>();


    }
}
