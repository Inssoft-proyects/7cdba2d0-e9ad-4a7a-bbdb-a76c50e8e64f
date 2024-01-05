﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Kendo.Mvc.Infrastructure.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{



    [Authorize]
    public class BusquedaEspecialAccidentesController : BaseController
    {
        private readonly IBusquedaEspecialAccidentesService _busquedaEspecialAccidentesService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IOficiales _oficialesService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly IPdfGenerator _pdfService;
        private readonly ICatDictionary _catDictionary;
        private readonly ICatEstatusReporteService _catEstatusReporteService;

        private int idOficina = 0;

        public BusquedaEspecialAccidentesController(IBusquedaEspecialAccidentesService busquedaEspecialAccidentesService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IOficiales oficialesService, IPdfGenerator pdfService,
            ICapturaAccidentesService capturaAccidentesService, ICatDictionary catDictionary, ICatEstatusReporteService catEstatusReporteService)
        {
            _busquedaEspecialAccidentesService = busquedaEspecialAccidentesService;
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
            // int IdModulo = 800;
            //string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            // List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            // if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            // {
            return View();
           // }
           /* else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }*/
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
        public JsonResult EstatusReporte_Drop()
        {
            var result = new SelectList(_catEstatusReporteService.ObtenerEstatusReporte(), "idEstatusReporte", "estatusReporte");
            return Json(result);
        }
        #endregion

        public ActionResult ajax_BuscarAccidente(BusquedaEspecialAccidentesModel model)
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
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var resultadoBusqueda = _busquedaEspecialAccidentesService.BusquedaAccidentes(model, idOficina, idDependencia);
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
            var AccidenteModel = _busquedaEspecialAccidentesService.ObtenerAccidentePorId(IdAccidente);
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
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var ListTransitoModel = _busquedaEspecialAccidentesService.BusquedaAccidentes(model, idOficina, idDependencia);
            var result = _pdfService.CreatePdf("ReporteAccidentes", "Accidentes", 6, ColumnsNames, ListTransitoModel);
            return File(result.Item1, "application/pdf", result.Item2);
        }
        public ActionResult EliminarAccidente(int idAccidente)
        {
            var accidenteEliminado = _busquedaEspecialAccidentesService.EliminarSeleccionado(idAccidente);
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var resultadoBusqueda = _busquedaEspecialAccidentesService.ObtenerTodosAccidentes(idOficina, idDependencia);
            return Json(resultadoBusqueda);
        }

        public IActionResult ModalEliminarAccidente(int idAccidente, string numeroReporte)
        {

            var viewModel = new EditarFolioModel
            {
                IdAccidente = idAccidente,
                NumeroReporte = numeroReporte
            };

            return PartialView("_ModalEliminarAccidente", viewModel);
        }
        
        public IActionResult ModalEditarFolio(int idAccidente, string numeroReporte)
        {

            var viewModel = new EditarFolioModel
            {
                IdAccidente = idAccidente,
                NumeroReporte = numeroReporte
            };

            return PartialView("_ModalEditarFolio", viewModel);
        }

        public IActionResult ajax_BusquedaAccidentes(BusquedaEspecialAccidentesModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var resultadoBusqueda = _busquedaEspecialAccidentesService.ObtenerTodosAccidentes(idOficina,idDependencia)
                                                .Where(w => w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
                                                    && w.idSupervisa == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idSupervisa)
                                                    && w.idCarretera == (model.IdCarreteraBusqueda > 0 ? model.IdCarreteraBusqueda : w.idCarretera)
                                                    && w.idTramo == (model.IdTramoBusqueda > 0 ? model.IdTramoBusqueda : w.idTramo)
                                                    && w.idElabora == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idElabora)
                                                    && w.idAutoriza == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idAutoriza)
                                                    && w.idEstatusReporte == (model.idEstatusReporte > 0 ? model.idEstatusReporte : w.idEstatusReporte)
                                                    && w.IdDelegacionBusqueda == (model.IdDelegacionBusqueda > 0 ? model.IdDelegacionBusqueda : w.IdDelegacionBusqueda)
                                                    && (string.IsNullOrEmpty(model.folioBusqueda) || w.numeroReporte.Contains(model.folioBusqueda))
                                                   && (string.IsNullOrEmpty(model.placasBusqueda) || w.placa.Contains(model.placasBusqueda))
                                                   && (string.IsNullOrEmpty(model.propietarioBusqueda) || w.propietario.Contains(model.propietarioBusqueda, StringComparison.OrdinalIgnoreCase))
                                                   && (string.IsNullOrEmpty(model.serieBusqueda) || w.serie.Contains(model.serieBusqueda))
                                                   && (string.IsNullOrEmpty(model.conductorBusqueda) || w.conductor.Contains(model.conductorBusqueda, StringComparison.OrdinalIgnoreCase))
                                                   && ((model.FechaInicio == default(DateTime) && model.FechaFin == default(DateTime)) || (w.fecha >= model.FechaInicio && w.fecha <= model.FechaFin))
                                                    ).ToList();

            return Json(resultadoBusqueda);

        }




        public ActionResult UpdateFolio(string id,string folio)
        {


            var fol = _busquedaEspecialAccidentesService.UpdateFolio(id,folio);

            return Json(true);
        }



    }
}
