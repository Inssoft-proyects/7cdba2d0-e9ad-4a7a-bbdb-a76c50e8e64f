using System.ComponentModel.DataAnnotations;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class TransitoTransporteModel
    {
        #region Depositos
        public int IdDeposito { get; set; }

        public int IdSolicitud { get; set; }

        public int IdDelegacion { get; set; }

        public int IdMarca { get; set; }

        public int IdSubmarca { get; set; }

        public int IdPension { get; set; }

        public int IdTramo { get; set; }

        public int IdColor { get; set; }

        public string Serie { get; set; }

        public string Placa { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaIngreso { get; set; }

        public DateTime FechaLiberacion { get; set; }

        public string Folio { get; set; }

        public string Km { get; set; }

        public int Liberado { get; set; }

        public int DepositoEstatus { get; set; }

        public string Autoriza { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public int ActualizadoPor { get; set; }

        public int IdDependenciaGenera { get; set; }

        public int IdDependenciaTransito { get; set; }

        public int IdDependenciaNoTransito { get; set; }

        //public int Estatus { get; set; }
        #endregion

        #region Solicitudes

        public string FolioSolicitud { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaSolicitud { get; set; }

        public string solicitanteNombre { get; set; }

        public string solicitanteAp { get; set; }

        public string solicitanteAm { get; set; }

        public int IdVehiculo { get; set; }

        public int IdInfraccion { get; set; }
        public int SolicitudEstatus { get; set; }

        #endregion

        #region Grua

        public int IdGrua { get; set; }

        //public string Grua { get; set; }

        #endregion

        #region Infracciones

        public string FolioInfraccion { get; set; }

        #endregion


        #region Vehiculo

        public string serie { get; set; }

        public string tarjeta { get; set; }

        public string vigenciaTarjeta { get; set; }

        public string marca { get; set; }

        public string submarca { get; set; }

        public string tipoVehiculo { get; set; }

        public string modelo { get; set; }

        public string color { get; set; }

        public string entidadRegistro { get; set; }

        public string tipoServicio { get; set; }

        public string propietario { get; set; }

        public string numeroEconomico { get; set; }

        public int idPersonaFisica { get; set; }

        public int idPersonaMoral { get; set; }

        public int EstatusVehiculo { get; set; }
        #endregion

        #region Pension
        public string pension { get; set; }
        #endregion

        #region Dependencia
        public int IdDependencia { get; set; }

        public string NombreDependencia { get; set; }

        #endregion


        public string marcaVehiculo { get; set; }

        public string nombreSubmarca { get; set; }

        public string delegacion { get; set; }

        public string Color { get; set; }



        public string tramo { get; set; }

        #region Concesionario
        public int IdConcesionario { get; set; }
        public string Concesionario { get; set; }

        #endregion
        //p.FolioSolicitud + " " + p.FechaSolicitud + " " + p.FolioInfraccion
        public string fullSolicitudfolioInfraccion
        {
            get
            {
                return @"Fecha: " + FechaSolicitud.ToString("dd/MM/yyyy") + "\r\n\n " +
                "Solicitud: " + FolioSolicitud + "\r\n\n " +
                "Infracción: " + FolioInfraccion;
            }
        }
        //p => p.Placa + " " + p.submarca + " " + p.modelo
        public string fullVehiculo
        {
            get
            {
                return @"Placas: " + Placa + "\r\n\n " +
                "Prop: " + propietario + "\r\n\n " +
                "Descr: " + marcaVehiculo + " " + nombreSubmarca + " " + modelo;
            }
        }

        public string fullDependencia
        {
            get
            {
                return @"Envia: " + NombreDependencia;
            }
        }
    }
}
