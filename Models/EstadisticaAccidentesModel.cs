using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EstadisticaAccidentesModel
    {
        public string Motivo { get; set; }
        public int Contador { get; set; }
    }


    public class ListadoAccidentesPorAccidenteModel
    {
        public string Numreporteaccidente { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Delegacion { get; set; }
        public string municipio { get; set; }
		public string carretera { get; set; }	
		public string tramo { get; set; }
        public string kilometro { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string Vehiculo { get; set; }
        public string NombredelOficial { get; set; }
        public string Dañosalcamino { get; set; }
        public string Dañosacarga { get; set; }
        public string Dañosapropietario { get; set; }
        public string Otrosdaños { get; set; }
        public string Lesionados { get; set; }
        public string Muertos { get; set; }
        public string FactoresOpciones { get; set; }
        public string Causas { get; set; }
        public string CausasDescripcion { get; set; }
		public int NumeroSecuencial { get; set; }
		public int idMunicipio { get; set; }
		public int idDelegacion { get; set; }


		public int IdOficial { get; set; }
		public int idCarretera { get; set; }
		public int idTramo { get; set; }
		public int idClasificacionAccidente { get; set; }
		public int idTipoLicencia { get; set; }
		public int idCausaAccidente { get; set; }
		public int idFactorAccidente { get; set; }
		public int IdTipoVehiculo { get; set; }
		public int IdTipoServicio { get; set; }
		public int IdSubtipoServicio { get; set; }

		public int idFactorOpcionAccidente { get; set; }

	}

    public class ListadoAccidentesPorVehiculoModel
    {
        public string Numreporteaccidente { get; set; }
        public string NumVeh { get; set; }
        public string PlacasVeh { get; set; }
        public string SerieVeh { get; set; }
        public string PropietarioVeh { get; set; }
        public string TipoVeh { get; set; }
        public string ServicioVeh { get; set; }
        public string Marca { get; set; }
        public string Submarca { get; set; }
        public string Modelo { get; set; }
        public string ConductorVeh { get; set; }
        public string DepositoVehículo { get; set; }
        public string Delegacion { get; set; }
        public string Municipio { get; set; }
        public string Carretera { get; set; }
        public string Tramo { get; set; }
        public string Kilómetro { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string NombredelOficial { get; set; }
        public string Dañosalcamino { get; set; }
        public string DañosaCarga { get; set; }
        public string Dañosapropietario { get; set; }
        public string Otrosdaños { get; set; }
        public string Lesionados { get; set; }
        public string Muertos { get; set; }
        public string Causas { get; set; }
        public string CausasDescripcion { get; set; }
		public int NumeroContinuo { get; set; }	
		public DateTime fecha { get; set; }
		public TimeSpan hora { get; set; }
		public int idMunicipio { get; set; }
		public int idDelegacion { get; set; }


		public int IdOficial { get; set; }
		public int idCarretera { get; set; }
		public int idTramo { get; set; }
		public int idClasificacionAccidente { get; set; }
		public int idTipoLicencia { get; set; }
		public int idCausaAccidente { get; set; }
		public int idFactorAccidente { get; set; }
		public int IdTipoVehiculo { get; set; }
		public int IdTipoServicio { get; set; }
		public int IdSubtipoServicio { get; set; }

		public int idFactorOpcionAccidente { get; set; }





	}
}
