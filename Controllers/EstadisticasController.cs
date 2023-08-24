using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly IEstatusInfraccionService _estatusInfraccionService;
        private readonly ITipoCortesiaService _tipoCortesiaService;
        private readonly IDependencias _dependeciaService;
        private readonly IDelegacionesService _delegacionesService;
        private readonly IGarantiasService _garantiasService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly IPdfGenerator<InfraccionesModel> _pdfService;
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;

        public EstadisticasController(
            IEstatusInfraccionService estatusInfraccionService, IDelegacionesService delegacionesService,
            ITipoCortesiaService tipoCortesiaService, IDependencias dependeciaService, IGarantiasService garantiasService,
            IInfraccionesService infraccionesService, IPdfGenerator<InfraccionesModel> pdfService,
            ICatDictionary catDictionary,
            IVehiculosService vehiculosService,
            IPersonasService personasService
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
        }
        public IActionResult Index()
        {
            int IdModulo = 707;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var modelList = _infraccionesService.GetAllInfracciones2()
                                            .SelectMany(s => s.MotivosInfraccion)
                                            .GroupBy(g => g.Nombre)
                                            .Select(s => new EstadisticaInfraccionMotivosModel() { Motivo = s.Key, Contador = s.Count() }).ToList();

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
            ViewBag.Estadisticas = modelList;

            return View();
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }

        public IActionResult ajax_BusquedaIncidenciasInfracciones(IncidenciasBusquedaModel model)
        {
            var modelList = _infraccionesService.GetAllInfracciones2()
                                                .Where(w => w.idDelegacion == (model.idDelegacion > 0 ? model.idDelegacion : w.idDelegacion)
                                                         || w.idOficial == (model.idOficial > 0 ? model.idOficial : w.idOficial)
                                                         || w.idCarretera == (model.idCarretera > 0 ? model.idCarretera : w.idCarretera)
                                                         || w.idTramo == (model.idTramo > 0 ? model.idTramo : w.idTramo)
                                                         || w.Vehiculo.idTipoVehiculo == (model.idTipoVehiculo > 0 ? model.idTipoVehiculo : w.Vehiculo.idTipoVehiculo)
                                                         || w.Vehiculo.idCatTipoServicio == (model.idTipoServicio > 0 ? model.idTipoServicio : w.Vehiculo.idCatTipoServicio)
                                                         || w.Persona.idTipoLicencia == (model.idTipoLicencia > 0 ? model.idTipoLicencia : w.Persona.idTipoLicencia)
                                                         || w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
                                                         && (w.fechaInfraccion >= model.fechaInicio && w.fechaInfraccion <= model.fechaFin))
                                                .SelectMany(s => s.MotivosInfraccion.
                                                Where(w => w.idCatMotivoInfraccion == (model.idTipoMotivo > 0 ? model.idTipoMotivo : w.idCatMotivoInfraccion)))
                                                .GroupBy(g => g.Nombre)
                                                .Select(s => new EstadisticaInfraccionMotivosModel() { Motivo = s.Key, Contador = s.Count() }).ToList();

            return PartialView("_EstadisticaInfraccionesMotivos", modelList);

        }
    }
}
