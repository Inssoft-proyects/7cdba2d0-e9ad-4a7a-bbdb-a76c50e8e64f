using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;


namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class TransitoTransporteController : BaseController
    {
        private readonly ITransitoTransporteService _transitoTransporteService;
        private readonly IDependencias _dependeciaService;
        private readonly IGruasService _gruasService;
        private readonly IPdfGenerator _pdfService;
        private readonly ICatDictionary _catDictionary;

        public TransitoTransporteController(ITransitoTransporteService transitoTransporteService,
            IDependencias dependeciaService, IGruasService gruasService,
            IPdfGenerator pdfService, ICatDictionary catDictionary
            )
        {
            _transitoTransporteService = transitoTransporteService;
            _dependeciaService = dependeciaService;
            _gruasService = gruasService;
            _pdfService = pdfService;
            _catDictionary = catDictionary;
        }

        public IActionResult Index()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            TransitoTransporteBusquedaModel searchModel = new TransitoTransporteBusquedaModel();
            List<TransitoTransporteModel> listTransitoTransporte = _transitoTransporteService.GetAllTransitoTransporte(idOficina);
            searchModel.ListTransitoTransporte = listTransitoTransporte;
            return View(searchModel);
        }

        [HttpGet]
        public FileResult CreatePdf(string data)
        {
            var model = JsonConvert.DeserializeObject<TransitoTransporteBusquedaModel>(data);
            if (model.FechaIngreso == null)
            {
                model.FechaIngreso = DateTime.MinValue;
            }

            if (model.FechaIngresoFin == null)
            {
                model.FechaIngresoFin = DateTime.MinValue;
            }
            model.FolioInfraccion = model.FolioInfraccion == string.Empty ? null : model.FolioInfraccion;
            model.FolioSolicitud = model.FolioSolicitud == string.Empty ? null : model.FolioSolicitud;
            model.NumeroEconomico = model.NumeroEconomico == string.Empty ? null : model.NumeroEconomico;
            model.Placas = model.Placas == string.Empty ? null : model.Placas;
            model.Propietario = model.Propietario == string.Empty ? null : model.Propietario;

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"fullSolicitudfolioInfraccion","Fecha_evento/Folio_Solicitud/Folio_Infracción"},
            {"fullVehiculo","Vehículo"},
            {"FechaIngreso","Fecha Ingreso"},
            {"FechaLiberacion","Fecha Liberación"},
            };
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var ListTransitoModel = _transitoTransporteService.GetTransitoTransportes(model,idOficina);
            var result = _pdfService.CreatePdf("ReporteTransitoTransporte", "Tránsito Transporte", 4, ColumnsNames, ListTransitoModel);
            return File(result.Item1, "application/pdf", result.Item2);
        }

        [HttpGet]
        public FileResult CreatePdfUnRegistro(int IdDeposito)
        {
            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"fullSolicitudfolioInfraccion","Fecha_evento/Folio_Solicitud/Folio_Infracción"},
            {"fullVehiculo","Vehículo"},
            {"UbicacionVehiculo","Ubicación"},
            {"DatosGrua","Grua"},
            {"Tiempos","Tiempo Servicio"},

            };
            var TransitoModel = _transitoTransporteService.GetTransitoTransporteById(IdDeposito);
            var result = _pdfService.CreatePdf("ReporteTransitoTransporte", "Tránsito Transporte", 5, ColumnsNames, TransitoModel);
            return File(result.Item1, "application/pdf", result.Item2);
        }


        [HttpPost]
        public ActionResult ajax_BuscarTransito(TransitoTransporteBusquedaModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var ListTransitoModel = _transitoTransporteService.GetTransitoTransportes(model, idOficina);

            if (ListTransitoModel.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListadoTransitoTransporte", ListTransitoModel);
        }


        public JsonResult Delegacion_Read()
        {
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var result = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Pension_Read()
        {
            var result = new SelectList(_transitoTransporteService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }

        public JsonResult Dependencia_Read()
        {
            var CatDependencias = _catDictionary.GetCatalog("CatDependencias", "0");
            var result = new SelectList(CatDependencias.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Estatus_Read()
        {
            var directions = from EstatusTransitoTransporte d in Enum.GetValues(typeof(EstatusTransitoTransporte))
                             select new { ID = (int)d, Name = d.ToString() };
            var result = new SelectList(directions, "ID", "Name");

            //var result = new SelectList(_transitoTransporteService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }


        [HttpGet]
        public ActionResult ajax_DetailGruas(int Id)
        {
            var model = _gruasService.GetGruasConcesionariosByIdCocesionario(Id);
            return PartialView("_GruasConcesionarioDetail", model);

        }

        [HttpPost]
        public ActionResult ajax_DeleteTransito(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }                

            string[] idsList = ids.Split(",");
            var result = _transitoTransporteService.DeleteTransitoTransporte(Convert.ToInt32(idsList[0]), Convert.ToInt32(idsList[1]));
            if (result > 0)
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                List<TransitoTransporteModel> ListTransitoTransporteModel = _transitoTransporteService.GetAllTransitoTransporte(idOficina);
                return PartialView("_ListadoTransitoTransporte", ListTransitoTransporteModel);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }
        }


    }
}
