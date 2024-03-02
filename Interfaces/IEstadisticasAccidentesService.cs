﻿using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IEstadisticasAccidentesService
    {
        public List<InfraccionesModel> GetAllInfracciones2();
        public List<BusquedaAccidentesModel> ObtenerAccidentes();
        //public List<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente();
        public IEnumerable<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel model);

        public List<CatalogModel> GetMunicipiosFilter();
        List<CatalogModel> GetCarreterasFilter();
        List<CatalogModel> GetTramosFilter();

        public IEnumerable<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model);

        //public List<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo();
    }
}
