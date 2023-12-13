using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Kendo.Mvc.Infrastructure.Implementation;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class BusquedaAccidentesController : BaseController
    {
        private readonly IBusquedaAccidentesService _busquedaAccidentesService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IOficiales _oficialesService;
        private readonly ICatEstatusReporteService _catEstatusReporteService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly ICatDictionary _catDictionary;

        private int idOficina = 0;

        public BusquedaAccidentesController(IBusquedaAccidentesService busquedaAccidentesService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IOficiales oficialesService,
            ICapturaAccidentesService capturaAccidentesService, ICatDictionary catDictionary, ICatEstatusReporteService catEstatusReporteService)
        {
            _busquedaAccidentesService = busquedaAccidentesService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _oficialesService = oficialesService;
            _capturaAccidentesService = capturaAccidentesService;
            _catDictionary = catDictionary;
            _catEstatusReporteService = catEstatusReporteService;
        }
        #region DropDowns
        public IActionResult Index()
        {
            int IdModulo = 800;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int? idOficina = HttpContext.Session.GetInt32("IdOficina");
                BusquedaAccidentesModel modelo = new BusquedaAccidentesModel
                {
                    IdDelegacionBusqueda = idOficina ?? 0,
                };
                return View(modelo);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }
        public JsonResult GetAllAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var resultadoBusqueda = _busquedaAccidentesService.GetAllAccidentes(idOficina);

            return Json(resultadoBusqueda.ToDataSourceResult(request));
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
            return Json(result);
        }
        public JsonResult Carreteras_Drop()
        {
            var result = new SelectList(_catCarreterasService.ObtenerCarreteras(), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var result = new SelectList(_catTramosService.ObtenerTamosPorCarretera(carreteraDDValue), "IdTramo", "Tramo");
            return Json(result);
        }
        public JsonResult Oficiales_Drop()
        {
            var oficiales = _oficialesService.GetOficialesActivos()
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = $"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}"
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }
        public JsonResult EstatusAccidente_Drop()
        {
            var result = new SelectList(_catEstatusReporteService.ObtenerEstatusReporte(), "idEstatusReporte", "estatusReporte");
            return Json(result);
        }
        #endregion

        /* public ActionResult ajax_BuscarAccidente(BusquedaAccidentesModel model)
         {
             if (model.FechaInicio == null)
             {
                 model.FechaInicio = DateTime.MinValue;
             }

             if (model.FechaFin == null)
             {
                 model.FechaFin = DateTime.MinValue;
             }

             int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
             var resultadoBusqueda = _busquedaAccidentesService.BusquedaAccidentes(model, idOficina);
             return Json(resultadoBusqueda);
         }*/
        public IActionResult ajax_BusquedaAccidentes(BusquedaAccidentesModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var resultadoBusqueda = _busquedaAccidentesService.GetAllAccidentes(idOficina)
                                                .Where(w => w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
                                                    && w.idSupervisa == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idSupervisa)
                                                    && w.idCarretera == (model.IdCarreteraBusqueda > 0 ? model.IdCarreteraBusqueda : w.idCarretera)
                                                    && w.idTramo == (model.IdTramoBusqueda > 0 ? model.IdTramoBusqueda : w.idTramo)
                                                    && w.idElabora == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idElabora)
                                                    && w.idAutoriza == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idAutoriza)
                                                    && w.idEstatusReporte == (model.IdEstatusAccidente > 0 ? model.IdEstatusAccidente : w.idEstatusReporte)
                                                   && (string.IsNullOrEmpty(model.folioBusqueda) || w.numeroReporte.Contains(model.folioBusqueda))
                                                   && (string.IsNullOrEmpty(model.placasBusqueda) || w.placa.Contains(model.placasBusqueda))
                                                   && (string.IsNullOrEmpty(model.propietarioBusqueda) || w.propietario.Contains(model.propietarioBusqueda))
                                                   && (string.IsNullOrEmpty(model.serieBusqueda) || w.serie.Contains(model.serieBusqueda))
                                                   && (string.IsNullOrEmpty(model.conductorBusqueda) || w.conductor.Contains(model.conductorBusqueda))
                                                   && ((model.FechaInicio == default(DateTime) && model.FechaFin == default(DateTime)) || (w.fecha >= model.FechaInicio && w.fecha <= model.FechaFin))
                                                    ).ToList();

            return Json(resultadoBusqueda);

        }
    }
}
