﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class ReporteAsignacionServiciosController : Controller
    {
        private readonly IPadronDepositosGruasService _padronDepositosGruasService;
        private readonly IGruasService _gruasService;
        private readonly IEventoService _eventoService;
        private readonly IReporteAsignacionService _reporteAsignacionService;
        private readonly IPdfGenerator<ReporteAsignacionModel> _pdfService;

        public ReporteAsignacionServiciosController(IPadronDepositosGruasService padronDepositosGruasService,
             IGruasService gruasService, IEventoService eventoService,
             IReporteAsignacionService reporteAsignacionService, IPdfGenerator<ReporteAsignacionModel> pdfService
            )
        {
            _padronDepositosGruasService = padronDepositosGruasService;
            _gruasService = gruasService;
            _eventoService = eventoService;
            _reporteAsignacionService = reporteAsignacionService;
            _pdfService = pdfService;
        }
        public IActionResult Index()
        {
            ReporteAsignacionBusquedaModel searchModel = new ReporteAsignacionBusquedaModel();
            List<ReporteAsignacionModel> listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones();
            searchModel.ListReporteAsignacion = listReporteAsignacion;
            return View(searchModel);
        }

        [HttpPost]
        public ActionResult ajax_BuscarReporte(ReporteAsignacionBusquedaModel model)
        {
            var listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(model);
            return PartialView("_ListadoReporteAsignacion", listReporteAsignacion);
        }

        [HttpGet]
        public FileResult CreatePdf(string data)
        {
            var model = JsonConvert.DeserializeObject<ReporteAsignacionBusquedaModel>(data,
                 new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

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
            var listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(model);
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
            var result = new SelectList(_padronDepositosGruasService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }

        public JsonResult Grua_Read()
        {
            var result = new SelectList(_gruasService.GetGruas(), "IdGrua", "Placas");
            return Json(result);
        }
    }
}
