using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class GruasConcesionariosModel
    {
        public int IdGrua { get; set; }
        public int IdConcesionario { get; set; }
        public string Concesionario { get; set; }
        public string noEconomico { get; set; }
        public string placas { get; set; }
        public int IdTipoGrua { get; set; }
        public string TipoGrua { get; set; }
        public string modelo { get; set; }
        public string capacidad { get; set; }
        //public string clasificacion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }

    }
}
