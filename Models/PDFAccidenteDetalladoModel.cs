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
        public string ParteNombre { get; set; }
		public string PartePuesto { get; set; }
	}
}
