using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class ReporteAsignacionServiciosController : BaseController
    {
        private readonly IPadronDepositosGruasService _padronDepositosGruasService;
        private readonly IGruasService _gruasService;
        private readonly IEventoService _eventoService;
        private readonly IReporteAsignacionService _reporteAsignacionService;
        private readonly IPdfGenerator<ReporteAsignacionModel> _pdfService;
        private readonly ICatDictionary _catDictionary;
        private readonly ITransitoTransporteService _transitoTransporteService;

        public ReporteAsignacionServiciosController(
            ITransitoTransporteService transitoTransporteService,
            IPadronDepositosGruasService padronDepositosGruasService,
             IGruasService gruasService, IEventoService eventoService,
             IReporteAsignacionService reporteAsignacionService, IPdfGenerator<ReporteAsignacionModel> pdfService
            , ICatDictionary catDictionary
            )
        {
            _padronDepositosGruasService = padronDepositosGruasService;
            _gruasService = gruasService;
            _eventoService = eventoService;
            _reporteAsignacionService = reporteAsignacionService;
            _pdfService = pdfService;
            _catDictionary = catDictionary;
            _transitoTransporteService = transitoTransporteService;
        }
        public IActionResult Index()
        {
            int IdModulo = 304;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                ReporteAsignacionBusquedaModel searchModel = new ReporteAsignacionBusquedaModel();
            List<ReporteAsignacionModel> listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(idOficina);
            searchModel.ListReporteAsignacion = listReporteAsignacion;
            return View(searchModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult ajax_BuscarReporte(ReporteAsignacionBusquedaModel model)
        {
            // Verifica si las fechas son nulas y asigna DateTime.MinValue si es el caso
            if (model.FechaInicio == null)
            {
                model.FechaInicio = DateTime.MinValue;
            }

            if (model.FechaFin == null)
            {
                model.FechaFin = DateTime.MinValue;
            }
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(model, idOficina);
            if (listReporteAsignacion.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }
            return PartialView("_ListadoReporteAsignacion", listReporteAsignacion);
        }

    

    [HttpGet]
        public FileResult CreatePdf(string data)
        {
           var model = JsonConvert.DeserializeObject<ReporteAsignacionBusquedaModel>(data);

            if (model.FechaInicio == null)
            {
                model.FechaInicio = DateTime.MinValue;
            }

            if (model.FechaFin == null)
            {
                model.FechaFin = DateTime.MinValue;
            }
            model.Evento = model.Evento == string.Empty ? null : model.Evento;

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"vehiculoCarretera","Carretera"},
            {"vehiculoTramo","Tramo"},
            {"vehiculoKm","Km"},
            {"fechaSolicitud","Fecha"},
            {"fechaLiberacion","Fecha Liberación"},
            {"evento","Evento"},
            {"fullName","Nombre Solicitante"}
            };
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(model,idOficina);
            var result = _pdfService.CreatePdf("ReporteAsignacionServicios", "Asignación de Servicios", 7, ColumnsNames, listReporteAsignacion);
            return File(result.Item1, "application/pdf", result.Item2);
        }

        public JsonResult Evento_Read()
        {
            var result = new SelectList(_eventoService.GetEventos(), "IdEvento", "Evento");
            return Json(result);
        }

        public JsonResult Pension_Read()
        {
            var result = new SelectList(_transitoTransporteService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }


        public JsonResult Grua_Read()
        {
            var result = new SelectList(_gruasService.GetGruas(), "IdGrua", "noEconomico");
            return Json(result);
        }
    }
}
