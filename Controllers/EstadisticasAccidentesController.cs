using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class EstadisticasAccidentesController : BaseController
    {
        private readonly IEstatusInfraccionService _estatusInfraccionService;
        private readonly ITipoCortesiaService _tipoCortesiaService;
        private readonly IDependencias _dependeciaService;
        private readonly IDelegacionesService _delegacionesService;
        private readonly IGarantiasService _garantiasService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly IPdfGenerator _pdfService;
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly IEstadisticasAccidentesService _estadisticasAccidentesService;

        public EstadisticasAccidentesController(
            IEstatusInfraccionService estatusInfraccionService, IDelegacionesService delegacionesService,
            ITipoCortesiaService tipoCortesiaService, IDependencias dependeciaService, IGarantiasService garantiasService,
            IInfraccionesService infraccionesService, IPdfGenerator pdfService,
            ICatDictionary catDictionary,
            IVehiculosService vehiculosService,
            IPersonasService personasService,
            IEstadisticasAccidentesService estadisticasAccidentesService

           )
        {
            _catDictionary = catDictionary;
            _estatusInfraccionService = estatusInfraccionService;
            _tipoCortesiaService = tipoCortesiaService;
            _dependeciaService = dependeciaService;
            _delegacionesService = delegacionesService;
            _garantiasService = garantiasService;
            _infraccionesService = infraccionesService;
            _pdfService = pdfService;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
            _estadisticasAccidentesService = estadisticasAccidentesService;
        }
        public IActionResult Index()
        {
            // var modelList = _infraccionesService.GetAllAccidentes2()
            //.GroupBy(g => g.municipio)
            // .Select(s => new EstadisticaAccidentesMotivosModel() { Motivo = s.Key.ToString(), Contador = s.Count() }).ToList();

            var catMotivosInfraccion = _catDictionary.GetCatalog("CatAllMotivosInfraccion", "0");
            var catTipoServicio = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var catTiposVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catTramos = _catDictionary.GetCatalog("CatTramos", "0");
            var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");
            var catClasificacionAccidentes = _catDictionary.GetCatalog("CatClasificacionAccidentes", "0");
            var catCausasAccidentes = _catDictionary.GetCatalog("CatCausasAccidentes", "0");
            var catFactoresAccidentes = _catDictionary.GetCatalog("CatFactoresAccidentes", "0");
            var catFactoresOpcionesAccidentes = _catDictionary.GetCatalog("CatFactoresOpcionesAccidentes", "0");

            ViewBag.CatMotivosInfraccion = new SelectList(catMotivosInfraccion.CatalogList, "Id", "Text");
            ViewBag.CatTipoServicio = new SelectList(catTipoServicio.CatalogList, "Id", "Text");
            ViewBag.CatTiposVehiculo = new SelectList(catTiposVehiculo.CatalogList, "Id", "Text");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
            ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "Id", "Text");
            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatCarreteras = new SelectList(catCarreteras.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");
            ViewBag.CatClasificacionAccidentes = new SelectList(catClasificacionAccidentes.CatalogList, "Id", "Text");
            ViewBag.CatCausasAccidentes = new SelectList(catCausasAccidentes.CatalogList, "Id", "Text");
            ViewBag.CatFactoresAccidentes = new SelectList(catFactoresAccidentes.CatalogList, "Id", "Text");
            ViewBag.CatFactoresOpcionesAccidentes = new SelectList(catFactoresOpcionesAccidentes.CatalogList, "Id", "Text");
            // ViewBag.Estadisticas = modelList;
             ViewBag.ListadoAccidentesPorAccidente = _estadisticasAccidentesService.AccidentesPorAccidente();
             ViewBag.ListadoAccidentesPorVehiculo = _estadisticasAccidentesService.AccidentesPorVehiculo();

            return View();
        }

        public IActionResult ajax_BusquedaAccidentes(BusquedaAccidentesModel model)
        {
            var modelList = _estadisticasAccidentesService.ObtenerAccidentes()
                                                .Where(w => w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
                                                    && w.idDelegacion == (model.idDelegacion > 0 ? model.idDelegacion : w.idDelegacion)
                                                    && w.IdOficial == (model.IdOficial > 0 ? model.IdOficial : w.IdOficial)
                                                    && w.idCarretera == (model.idCarretera > 0 ? model.idCarretera : w.idCarretera)
                                                    && w.idTramo == (model.idTramo > 0 ? model.idTramo : w.idTramo)
                                                    && w.idClasificacionAccidente == (model.idClasificacionAccidente > 0 ? model.idClasificacionAccidente : w.idClasificacionAccidente)
                                                    && w.idTipoLicencia == (model.idTipoLicencia > 0 ? model.idTipoLicencia : w.idTipoLicencia)
                                                    && w.idCausaAccidente == (model.idCausaAccidente > 0 ? model.idCausaAccidente : w.idCausaAccidente)
                                                    && w.idFactorAccidente == (model.idFactorAccidente > 0 ? model.idFactorAccidente : w.idFactorAccidente)
                                                    && w.IdTipoVehiculo == (model.IdTipoVehiculo > 0 ? model.IdTipoVehiculo : w.IdTipoVehiculo)
                                                    && w.IdTipoServicio == (model.IdTipoServicio > 0 ? model.IdTipoServicio : w.IdTipoServicio)
                                                    && w.idCausaAccidente == (model.idCausaAccidente > 0 ? model.idCausaAccidente : w.idCausaAccidente)
                                                    && w.idFactorOpcionAccidente == (model.idFactorOpcionAccidente > 0 ? model.idFactorOpcionAccidente : w.idFactorOpcionAccidente)
                                                    && ((model.FechaInicio == default(DateTime) && model.FechaFin == default(DateTime)) || (w.fecha >= model.FechaInicio && w.fecha <= model.FechaFin))
												    && ((model.hora == TimeSpan.Zero && w.hora != TimeSpan.Zero) || model.hora == w.hora)  
													).ToList();

            var lista = modelList.GroupBy(g => g.municipio).ToList();

            var lista2 = lista.
                Select(
                    s => new EstadisticaAccidentesMotivosModel()
                    {
                        Motivo = s.Key.ToString(),
                        Delegacion = s.Key.ToString(),
                        Contador = s.Count()

                    }
                    ).ToList();

            return PartialView("_EstadisticasAccidentes", lista2);

        }
       
    }
}
