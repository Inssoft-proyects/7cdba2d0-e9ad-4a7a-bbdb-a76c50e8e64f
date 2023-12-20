using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
	public class PDFAccidenteDetalladoModel
	{
        public CapturaAccidentesModel ParteAccidente { get; set; }
        public DatosAccidenteModel ParteAccidenteComplemento { get; set; }
        public List<CapturaAccidentesModel> VehiculosInvolucrados { get; set; }
        public List<CapturaAccidentesModel> Clasificaciones { get; set; }
        public List<CapturaAccidentesModel> Factores { get; set; }
        public List<CapturaAccidentesModel> CausasDeterminantes { get; set; }
        public List<CapturaAccidentesModel> Infracciones { get; set; }
        public string ADisposicion { get; set; }
        public string entregadoA { get; set; }
        public string sede { get; set; }
        public string ParteNombre { get; set; }
		public string PartePuesto { get; set; }
        public List<PDFMotivosInfracciones> MotivosInfraccion { get; set; }
        public List<CapturaAccidentesModel> Involucrados { get; set; }
        public string ElaboraConsignacion { get; set; }
        public string NoOficio { get; set; }
        public string AgenciaRecibe { get; set; }
        public string recibe { get; set; }
        public string Elabora { get; set; }
        public string Supervisor { get; set; }
        public string Autoriza { get; set; }
    }

    public class PDFMotivosInfracciones
    {
        public int idInfraccion { get; set; }
        public List<string> Motivos { get; set; }
    }
    public class PDFInvolucradosVehiculos
    {
        
    }
}
