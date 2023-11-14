﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
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
    public class BusquedaAccidentesController : BaseController
    {
        private readonly IBusquedaAccidentesService _busquedaAccidentesService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IOficiales _oficialesService;
        private readonly ICatEstatusReporteService _catEstatusReporteService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly IPdfGenerator<BusquedaAccidentesPDFModel> _pdfService;
        private readonly ICatDictionary _catDictionary;

        private int idOficina = 0;

        public BusquedaAccidentesController(IBusquedaAccidentesService busquedaAccidentesService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IOficiales oficialesService, IPdfGenerator<BusquedaAccidentesPDFModel> pdfService,
            ICapturaAccidentesService capturaAccidentesService,ICatDictionary catDictionary, ICatEstatusReporteService catEstatusReporteService)
        {
            _busquedaAccidentesService = busquedaAccidentesService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _oficialesService = oficialesService;
            _pdfService = pdfService;
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
													   &&(string.IsNullOrEmpty(model.folioBusqueda) || w.numeroReporte.Contains(model.folioBusqueda))
												       && (string.IsNullOrEmpty(model.placasBusqueda) || w.placa.Contains(model.placasBusqueda))
													   && (string.IsNullOrEmpty(model.propietarioBusqueda) || w.propietario.Contains(model.propietarioBusqueda))
													   && (string.IsNullOrEmpty(model.serieBusqueda) || w.serie.Contains(model.serieBusqueda))
												       && (string.IsNullOrEmpty(model.conductorBusqueda) || w.conductor.Contains(model.conductorBusqueda))
													   && ((model.FechaInicio == default(DateTime) && model.FechaFin == default(DateTime)) || (w.fecha >= model.FechaInicio && w.fecha <= model.FechaFin))
														).ToList();

                return Json(resultadoBusqueda);

            }

		
        [HttpGet]
        public FileResult CreatePdfUnRegistro(int IdAccidente)
        {
            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"numeroReporte","Folio"},
            {"municipio","Municipio"},
            {"carretera","Carretera"},
            {"nombrePropietarioCompleto","Propietario"},
            {"fecha","Fecha"},
            {"hora","Hora"},


            };
            var AccidenteModel = _busquedaAccidentesService.ObtenerAccidentePorId(IdAccidente);
            var result = _pdfService.CreatePdf("ReporteAccidente", "Accidentes", 6, ColumnsNames, AccidenteModel);
            return File(result.Item1, "application/pdf", result.Item2);
        }

        [HttpGet]
        public FileResult CreatePdf(string data)
        {
            var model = JsonConvert.DeserializeObject<BusquedaAccidentesPDFModel>(data);

            if (model.FechaInicio == null)
            {
                model.FechaInicio = DateTime.MinValue;
            }

            if (model.FechaFin == null)
            {
                model.FechaFin = DateTime.MinValue;
            }
      

            model.placa = model.placa == string.Empty ? null : model.placa;
            model.serie = model.serie == string.Empty ? null : model.serie;
            model.folio = model.folio == string.Empty ? null : model.folio;
            model.propietario = model.propietario == string.Empty ? null : model.propietario;
            model.conductor = model.conductor == string.Empty ? null : model.conductor;
            model.idDelegacion = model.idDelegacion == 0 ? null : model.idDelegacion;
            model.idCarretera = model.idCarretera == 0 ? null : model.idCarretera;
            model.idTramo = model.idTramo == 0 ? null : model.idTramo;
            model.IdOficial = model.IdOficial == 0 ? null : model.IdOficial;

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"municipio","Municipio"},
            {"tramo","Tramo"},
            {"carretera","Carretera"},
            {"kilometro","Kilometro"},
            {"fecha","Fecha" },
            {"hora","Hora" },
           
            };
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var ListTransitoModel = _busquedaAccidentesService.BusquedaAccidentes(model, idOficina);
            var result = _pdfService.CreatePdf("ReporteAccidentes", "Accidentes", 6, ColumnsNames, ListTransitoModel);
            return File(result.Item1, "application/pdf", result.Item2);
        }
       

    }
}
