using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EstadisticaInfraccionMotivosModel
    {
        public string Motivo { get; set; }
        public int Contador { get; set; }
        public int NumeroMotivos { get; set; }
        public int ContadorMotivos { get; set; }
        public int ResultadoMultiplicacion { get; set; }
        public int TotalReal { get; set; }
        public int Total { get; set; }
        public int totalp { get; set; }
        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }




        public int Con1Motivo { get; set; }
        public int Con2Motivos { get; set; }
        public int Con3Motivos { get; set; }
     

            public string TodosMotivos
            {
                get
                {
                    return $"{Con1Motivo}<br />{Con2Motivos}<br />Co{Con3Motivos}";
                }
            }

    }
}
