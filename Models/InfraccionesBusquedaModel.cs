using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesBusquedaModel
    {
        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string folioInfraccion { get; set; } = null!;

        /// <summary>
        /// tblcatEstatusInfraccion ddl
        /// </summary>
        public int? IdEstatus { get; set; }

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string placas { get; set; } = null!;

        /// <summary>
        /// tblcatTipoCortesia ddl Pendiente
        /// </summary>
        public int? IdTipoCortesia { get; set; }

        /// <summary>
        /// tblcatDependencias ddl
        /// </summary>
        public int? IdDependencia { get; set; }

        /// <summary>
        /// tblcatGarantias ddl
        /// </summary>
        public int? IdGarantia { get; set; }

        /// <summary>
        /// tblcatDelegaciones ddl
        /// </summary>
        public int? IdDelegacion { get; set; }

        /// <summary>
        /// tblInfracciones campo fechaInfraccion
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// tblInfracciones campo fechaInfraccion
        /// </summary>
        public DateTime FechaFin { get; set; }

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string Propietario { get; set; } = null!;

        /// <summary>
        /// pediente aun no se que tabla
        /// </summary>
        public string NumeroLicencia { get; set; } = null!;

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string Conductor { get; set; } = null!;

        /// <summary>
        /// tblVehiculo pediente posible tabla vehiculo
        /// </summary>
        public string NumeroEconomico { get; set; } = null!;

        public List<InfraccionesModel> ListInfracciones { get; set; }

    }
}
