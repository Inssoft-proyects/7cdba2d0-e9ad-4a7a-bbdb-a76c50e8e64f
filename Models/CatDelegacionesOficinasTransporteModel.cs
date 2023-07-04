using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatDelegacionesOficinasTransporteModel
    {
        public int IdOficinaTransporte { get; set; }

        public string NombreOficina { get; set; }

        public string JefeOficina { get; set; }

        public int IdMunicipio { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string Municipio { get; set; }

        public string estatusDesc { get; set; }

        public bool ValorEstatusDelegacionOfTrasporte { get; set; }


    }
}
