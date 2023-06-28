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
        private readonly ITiposCarga _tiposCargaService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IPensionesService _pensionesService;
        private readonly ICatFormasTrasladoService _catFormasTrasladoService;



        private int lastInsertedId = 0;
        private int idVehiculoInsertado = 0; 

        public CapturaAccidentesController(ICapturaAccidentesService capturaAccidentesService, ICatMunicipiosService catMunicipiosService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatClasificacionAccidentes catClasificacionAccidentesService, ICatFactoresAccidentesService catFactoresAccidentesService, ICatFactoresOpcionesAccidentesService catFactoresOpcionesAccidentesService, ICatCausasAccidentesService catCausasAccidentesService,
            ITiposCarga tiposCargaService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IPensionesService pensionesService, ICatFormasTrasladoService catFormasTrasladoService)
        {
            _capturaAccidentesService = capturaAccidentesService;
            _catMunicipiosService = catMunicipiosService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _clasificacionAccidentesService = catClasificacionAccidentesService;
            _catFactoresAccidentesService = catFactoresAccidentesService;
            _catFactoresOpcionesAccidentesService = catFactoresOpcionesAccidentesService;
            _catCausasAccidentesService = catCausasAccidentesService;
            _tiposCargaService = tiposCargaService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _pensionesService = pensionesService;
            _catFormasTrasladoService = catFormasTrasladoService;
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

        public ActionResult CapturaAaccidente()
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
           var  AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente);
            return View("CapturaAaccidente", AccidenteSeleccionado);
        }

        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
        }
        public ActionResult MostrarModalConductor(int IdPersona)
        {
            var ListConductor = _capturaAccidentesService.ObtenerConductorPorId(IdPersona);
            return PartialView("_ModalConductor", ListConductor);
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
        public ActionResult ModalAnexo2()
        {
            return PartialView("_ModalAnexo2");
        }
        

        [HttpPost]

        public ActionResult BuscarVehiculo(string Placa, string Serie, string folio)
        {
            var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(Placa, Serie, folio);
            return Json(SeleccionVehiculo);
        }
        public JsonResult ObtVehiculosInvol([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados.ToDataSourceResult(request));
        }
        public IActionResult ActualizarAccidenteConVehiculo(int IdVehiculo, int IdPersona)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; 
                var idVehiculoInsertado = _capturaAccidentesService.ActualizarConVehiculo(IdVehiculo, idAccidente);
                HttpContext.Session.SetInt32("idVehiculoInsertado", idVehiculoInsertado);
                return Json(IdPersona);
        }


       [HttpPost]
        public IActionResult ActualizarConConductor(int IdVehiculo, int IdPersona)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var idVehiculoInsertado = _capturaAccidentesService.InsertarConductor(IdVehiculo, idAccidente,IdPersona);

            return Json(idVehiculoInsertado);
        }
        public IActionResult GuardarConductorVehiculo(int IdPersona)
        {
            int IdVehiculo = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0; // Obtener el valor de idVehiculoInsertado desde la variable de sesión
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; 
            var RegistroSeleccionado = _capturaAccidentesService.InsertarConductor(IdVehiculo, idAccidente, IdPersona);

            return Json(RegistroSeleccionado);
        }
        public ActionResult BuscarConductor(BusquedaInvolucradoModel model)
        {
            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            return Json(ListInvolucradoModel);
        }

        [HttpPost]

        public IActionResult GuardarComplementoVehiculo(CapturaAccidentesModel model)
        {
            int IdVehiculo = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0; // Obtener el valor de idVehiculoInsertado desde la variable de sesión
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var RegistroSeleccionado = _capturaAccidentesService.GuardarComplementoVehiculo(model, IdVehiculo, idAccidente);
            return RedirectToAction("ObtVehiculosInvol", "CapturaAccidentes");
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
        public ActionResult ModalEditarFactorAccidente(int IdFactorAccidente, int IdFactorOpcionAccidente)

        {
            return PartialView("_ModalEditarFactor");
        }

        public ActionResult ModalEliminarFactorAccidente(string FactorAccidente, string FactorOpcionAccidente)
        {
            ViewBag.FactorAccidente = FactorAccidente;
            ViewBag.FactorOpcionAccidente = FactorOpcionAccidente;
            return PartialView("_ModalEliminarFactor");
        }
        public ActionResult ModalCausaAccidente()
        {
            return PartialView("_ModalCausa");
        }
        public ActionResult ModalCapturaConductor()
        {
            return PartialView("_ModalCapturarConductor");
        }
        public ActionResult ModalEditarCausaAccidente(int IdCausaAccidente, int IdAccidente)
        {
            return PartialView("_ModalEditarCausa");
        }
        public ActionResult ModalEliminarCausas(int IdCausaAccidente, string CausaAccidente)
        {
            ViewBag.IdCausaAccidente = IdCausaAccidente;
            ViewBag.CausaAccidente = CausaAccidente;
            return PartialView("_ModalEliminarCausa");
        }
        
        public ActionResult ModalAgregarInvolucrado()
        {
            return PartialView("_ModalAgregarInvolucrado");
        }
        public ActionResult ModalAgregarComplemeto()
        {
            return PartialView("_ModalComplementoVehiculo");
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
        public IActionResult EliminarValorFactorYOpcion()
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
        public IActionResult EditarCausa(int IdCausaAccidente, int IdCausaAccidenteEdit)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarValorCausa(IdCausaAccidente, idAccidente, IdCausaAccidenteEdit);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(datosGrid);
        }
        public IActionResult EliminarCausaAccidente(int IdCausaAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarCausaBD(IdCausaAccidente, idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(datosGrid);
        }

        public ActionResult BuscarInvolucrado(BusquedaInvolucradoModel model)
        {
            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            return Json(ListInvolucradoModel);
        }

        public IActionResult GuardarInvolucrado(int idPersonaInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarPersonaInvolucrada(idPersonaInvolucrado, idAccidente);

            return PartialView("_ModalConductor");
        }
        public JsonResult ObtenerVehiculosInvolucrados([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);
            return Json(ListVehiculosInvolucrados.ToDataSourceResult(request));
        }
        public JsonResult Carga_Drop() 
        {
            var result = new SelectList(_tiposCargaService.GetTiposCarga(), "IdTipoCarga", "TipoCarga");
            return Json(result);
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
            return Json(result);
        }
    public JsonResult Pensiones_Drop(int delegacionDDValue)
    {
        var result = new SelectList(_pensionesService.GetPensionesByDelegacion(delegacionDDValue), "IdPension", "Pension");
        return Json(result);
    }
        public JsonResult Traslados_Drop()
        {
            var result = new SelectList(_catFormasTrasladoService.ObtenerFormasActivas(), "idFormaTraslado", "formaTraslado");
            return Json(result);
        }
        public ActionResult LLevaParteC(string descripcionCausa)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            _capturaAccidentesService.GuardarDescripcion(idAccidente, descripcionCausa);
            return View("CapturaCAccidente");
        }
            
        public ActionResult CapturaAccidenteC()
        {
            return PartialView("CapturaCAccidente");
        }
     
        

    }
}

