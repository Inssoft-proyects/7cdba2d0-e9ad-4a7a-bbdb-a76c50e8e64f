using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class EstadisticasController : BaseController
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
        private readonly IEstadisticasService _estadisticasService;

        public EstadisticasController(
            IEstatusInfraccionService estatusInfraccionService, IDelegacionesService delegacionesService,
            ITipoCortesiaService tipoCortesiaService, IDependencias dependeciaService, IGarantiasService garantiasService,
            IInfraccionesService infraccionesService, IPdfGenerator<InfraccionesModel> pdfService,
            ICatDictionary catDictionary,
            IVehiculosService vehiculosService,
            IPersonasService personasService, 
            IEstadisticasService estadisticasService
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
            _estadisticasService = estadisticasService;
        }
        public IActionResult Index()
        {
            int IdModulo = 707;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                var modelList = _infraccionesService.GetAllEstadisticasInfracciones(idOficina);
                var modelListProMotivos = _infraccionesService.GetAllMotivosPorInfraccion(idOficina);
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
                ViewBag.GridPorMotivos = modelListProMotivos;


                var modelGridInfracciones = _infraccionesService.GetAllInfraccionesEstadisticasGrid();

                ViewBag.GridInfracciones = modelGridInfracciones;

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
            var modelList = _estadisticasService.GetAllInfraccionesEstadisticas(model)
                .SelectMany(s => s.MotivosInfraccion
                .Where(w => w.idCatMotivoInfraccion == (model.idTipoMotivo > 0 ? model.idTipoMotivo : w.idCatMotivoInfraccion)))
                .GroupBy(g => g.Nombre)
                .Select(s => new EstadisticaInfraccionMotivosModel() { Motivo = s.Key, Contador = s.Count() }).ToList();

            return PartialView("_EstadisticaInfraccionesMotivos", modelList);

        }

        public IActionResult ajax_BusquedaParaMotivos(IncidenciasBusquedaModel model)
        {
            var modelMotivosList = _estadisticasService.GetAllInfraccionesEstadisticas(model)
                .SelectMany(s => s.MotivosInfraccion)
                .GroupBy(g => g.idInfraccion)
                .GroupBy(g => g.Count())
                .Where(group => group.Key > 0) // Filtra grupos con ContadorMotivos > 0
                .Select(s => new EstadisticaInfraccionMotivosModel
                {
                    NumeroMotivos = s.Key,  // Número de motivos
                    ContadorMotivos = s.Count(),  // Cantidad de infracciones con ese número de motivos
                    ResultadoMultiplicacion = s.Key * s.Count()  // Nuevo parámetro con la multiplicación
                }).ToList();

            // Resto del código para calcular totales y agregar la instancia de totales a modelList

            return PartialView("_GridPorMotivos", modelMotivosList);
        }





    }
}
