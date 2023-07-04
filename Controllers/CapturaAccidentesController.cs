using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using Kendo.Mvc.Extensions;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Controllers
{

    public class CapturaAccidentesController : Controller
    {
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly ICatClasificacionAccidentes _clasificacionAccidentesService;
        private readonly ICatFactoresAccidentesService _catFactoresAccidentesService;
        private readonly ICatFactoresOpcionesAccidentesService _catFactoresOpcionesAccidentesService;
        private readonly ICatCausasAccidentesService _catCausasAccidentesService;


        private int lastInsertedId = 0;


        public CapturaAccidentesController(ICapturaAccidentesService capturaAccidentesService, ICatMunicipiosService catMunicipiosService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatClasificacionAccidentes catClasificacionAccidentesService, ICatFactoresAccidentesService catFactoresAccidentesService,ICatFactoresOpcionesAccidentesService catFactoresOpcionesAccidentesService, ICatCausasAccidentesService catCausasAccidentesService )
        {
            _capturaAccidentesService = capturaAccidentesService;
            _catMunicipiosService = catMunicipiosService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _clasificacionAccidentesService = catClasificacionAccidentesService;
            _catFactoresAccidentesService = catFactoresAccidentesService;
            _catFactoresOpcionesAccidentesService = catFactoresOpcionesAccidentesService;
            _catCausasAccidentesService = catCausasAccidentesService;
        }
        /// <summary>
        /// //PRIMERA SECCION DE CAPTURA ACCIDENTE//////////
        /// </summary>
        public IActionResult BuscarAccidentesLista([DataSourceRequest] DataSourceRequest request)
        {
            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentes();
            return Json(ListAccidentesModel.ToDataSourceResult(request));
        }

        public IActionResult Index(CapturaAccidentesModel capturaAccidentesService)
        {
            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentes();
            if (ListAccidentesModel.Count == 0)
            {
                return View("AgregarAccidente");

            }
            else
            {
                return View("CapturaAccidentes", ListAccidentesModel);
            }
        }

        public ActionResult NuevoAccidente()
        {
            return View("AgregarAccidente");
        }

        public JsonResult Municipios_Drop()
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipios(), "IdMunicipio", "Municipio");
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

        public JsonResult Clasificacion_Drop()
        {
            var result = new SelectList(_clasificacionAccidentesService.ObtenerClasificacionesActivas(), "IdClasificacionAccidente", "NombreClasificacion");
            return Json(result);
        }

        [HttpPost]
        public ActionResult GuardarUbicacionAccidente(CapturaAccidentesModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, errors = errors });
            }
            else
            {



                lastInsertedId = _capturaAccidentesService.GuardarParte1(model);
                HttpContext.Session.SetInt32("LastInsertedId", lastInsertedId); // Almacenar lastInsertedId en la variable
                return Json(new { success = true });

            }
        }

        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
        }

        public ActionResult ModalClasificacionAccidente()
        {
            return PartialView("_ModalClasificacion");
        }
        public ActionResult ModalEliminarClasificacion(int IdAccidente)
        {
            var clasificacionesModel = _capturaAccidentesService.AccidentePorID(IdAccidente);

            return PartialView("_ModalEliminarClasificacion");
        }
       
        [HttpPost]

        public ActionResult BuscarVehiculo(string Placa, string Serie, string folio)
        {
            var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(Placa, Serie, folio);
            return Json(SeleccionVehiculo);
        }


        [HttpPost]
        public IActionResult ActualizarAccidenteConVehiculo(int IdVehiculo)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var RegistroSeleccionado = _capturaAccidentesService.ActualizarConVehiculo(IdVehiculo, idAccidente);

            return Json(RegistroSeleccionado);
        }

        [HttpPost]
        public IActionResult AgregarClasificacion(int IdClasificacionAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorClasificacion(IdClasificacionAccidente, idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);
            //lastInsertedId = 0;
            return Json(datosGrid);
        }
        [HttpPost]
        public IActionResult EliminaClasificacion(int IdAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var clasificacionEliminada = _capturaAccidentesService.ClasificacionEliminar(IdAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);

            return Json(datosGrid);

        }
        ///////////////
        ///SEGUNDA SECCION CAPTURA ACCIDENTE///////////
        ///

        public ActionResult CapturaBAccidente()
        {
            return View("CapturaBAccidente");
        }

        public ActionResult ModalFactorAccidente()
        {
            return PartialView("_ModalFactor");
        }

        public ActionResult ModalEditarFactorAccidente()
        {
            return PartialView("_ModalEditarFactor");
        }
        public ActionResult ModalEliminarFactorAccidente(string dataItem)
        {
            return PartialView("_ModalEliminarFactor");
        }
        public ActionResult ModalCausaAccidente()
        {
            return PartialView("_ModalCausa");
        }
        public ActionResult ModalEditarCausaAccidente()
        {
            return PartialView("_ModalEditarCausa");
        }
        public ActionResult ModalAgregarInvolucrado()
        {
            return PartialView("_ModalAgregarInvolucrado");
        }
        public JsonResult Factores_Drop()
        {
            var result = new SelectList(_catFactoresAccidentesService.GetFactoresAccidentesActivos(), "IdFactorAccidente", "FactorAccidente");
            return Json(result);
        }

        public JsonResult FactoresOpciones_Drop(int factorDDValue)
        {
            var result = new SelectList(_catFactoresOpcionesAccidentesService.ObtenerOpcionesPorFactor(factorDDValue), "IdFactorOpcionAccidente", "FactorOpcionAccidente");
            return Json(result);
        }

        public JsonResult Causas_Drop()
        {
            var result = new SelectList(_catCausasAccidentesService.ObtenerCausasActivas(), "IdCausaAccidente", "CausaAccidente");
            return Json(result);
        }
        [HttpPost]
        public IActionResult AgregarFactorNuevo(int IdFactorAccidente, int IdFactorOpcionAccidente,int IdAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorFactorYOpcion(IdFactorAccidente, IdFactorOpcionAccidente, idAccidente);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }
        [HttpPost]
        public IActionResult EliminarValorFactorYOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarValorFactorYOpcion(idAccidente);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }

        public IActionResult AgregarCausaNuevo(int IdCausaAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorCausa(IdCausaAccidente, idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(datosGrid);
        }
    }
}

