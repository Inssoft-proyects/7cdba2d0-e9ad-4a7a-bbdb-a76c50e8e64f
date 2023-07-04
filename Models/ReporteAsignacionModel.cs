using GuanajuatoAdminUsuarios.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ReporteAsignacionModel
    {
        public int idSolicitud { get; set; }
        public string vehiculoCarretera { get; set; }
        public string vehiculoTramo { get; set; }
        public string vehiculoKm { get; set; }

        /// <summary>
        /// Filtro tbl solicitudes
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fechaSolicitud { get; set; }

        //ToDo:no  esta Fecha Salida de donde lo tomo se agrega a tabla depositos
        //Este campo es Join de Depositos
        /// <summary>
        /// tbl Depositos
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fechaLiberacion { get; set; }

        /// <summary>
        /// Filtro por textfield no por id sobre mismo solicitudes
        /// </summary>
        public string evento { get; set; }

        public string solicitanteNombre { get; set; }
        public string solicitanteAp { get; set; }
        public string solicitanteAm { get; set; }

        public string fullName
        {
            get
            {
                return solicitanteNombre.ToString() + " " +
                solicitanteAp + " " +
                solicitanteAm;
            }
        }

        //Direccion
        public string solicitanteColonia { get; set; }
        public string solicitanteCalle { get; set; }
        public string solicitanteNumero { get; set; }

        public string fullcolonia
        {
            get
            {
                return solicitanteColonia.ToString() + " " +
                solicitanteCalle + " " +
                solicitanteNumero;
            }
        }

        public string tipoVehiculo { get; set; }

        //ToDo:sobre que filtro grua? ya que el drop esta sobre id y placa
        //este se mostrara para  el filtro de grua
        //tbl gruas
        public string noEconomico { get; set; }
        public int IdGrua { get; set; }
        public string propietarioGrua { get; set; }

        //ToDo no esta alias seria sacarlo de la tabla que cree con join de Concesionario
        public string Alias { get; set; }
        public string oficial { get; set; }
        public string folio { get; set; }


        /// <summary>
        /// Filtro por id pesiones mostrar nombre pension
        /// </summary>
        public string vehiculoPension { get; set; }

        //ToDo no esta delegacion hacer join para llegar a delegacion

        /// <summary>
        /// tbl delegacion
        /// </summary>
        public string Delegacion { get; set; }

        //ToDo no esta motivo
        //ToDo no esta inventario 

        public string servicio { get; set; }
        public string tipoUsuario { get; set; }
        public string solicitanteTel { get; set; }
        public string solicitanteEntidad { get; set; }
        public string solicitanteMunicipio { get; set; }
        public string vehiculoCalle { get; set; }
        public string vehiculoNumero { get; set; }
        public string vehiculoColonia { get; set; }

        public string vehiculoEntidad { get; set; }
        public string vehiculoMunicipio { get; set; }

        public string vehiculoInterseccion { get; set; }

        public int idVehiculo { get; set; }
        public int idInfraccion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public string actualizadoPor { get; set; }
        public string estatus { get; set; }
        //public int IdDependencia { get; set; }

    }
}
