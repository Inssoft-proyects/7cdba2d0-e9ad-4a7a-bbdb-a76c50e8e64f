namespace GuanajuatoAdminUsuarios.Models
{
    public class GruasSalidaVehiculosModel
    {
        public int idGrua { get; set; }
        public int idDeposito { get; set; }

        public int abanderamiento { get; set; }
        public int arrastre { get; set; }
        public int salvamento { get; set; }

        public string grua { get; set; }
        public string tipoGrua { get; set; }

        public float costoTotalPorGrua { get; set; }

    }
}
