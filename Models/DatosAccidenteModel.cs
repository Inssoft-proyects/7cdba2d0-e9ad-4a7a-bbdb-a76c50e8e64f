namespace GuanajuatoAdminUsuarios.Models
{
    public class DatosAccidenteModel
    { 
        public float montoCamino { get; set; }
        public float montoCarga { get; set; }
        public float montoPropietarios { get; set; }
        public float montoOtros { get; set; }
        public float Latitud { get; set; }
        public float Longitud { get; set; }
        public int IdCiudad { get; set; }
        public int IdCertificado { get; set; }
        public string entregaObjetos { get; set; }
        public string entregaOtros { get; set; }
        public string consignacionHechos { get; set; }
        public string numeroOficio { get; set; }
        public int IdAgenciaMinisterio { get; set; }
        public string RecibeMinisterio { get; set; }
        public int IdElabora { get; set; }
        public int IdSupervisa { get; set; }
        public int IdAutoriza { get; set; }
        public int IdElaboraConsignacion { get; set; }
        public int IdAutoridadEntrega { get; set; }
        public int IdAutoridadDisposicion { get; set; }
        public int EstadoArmas { get; set; }
        public int EstadoDrogas { get; set; }
        public int EstadoValores { get; set; }
        public int EstadoPrendas { get; set; }
        public int EstadoOtros { get; set; }
        public bool ArmasBool =>EstadoArmas == 1;
        public bool DrogasBool => EstadoDrogas == 1;
        public bool ValoresBool => EstadoValores == 1;
        public bool PrendasBool => EstadoPrendas == 1;
        public bool OtrosBool => EstadoOtros == 1;
    }
}
