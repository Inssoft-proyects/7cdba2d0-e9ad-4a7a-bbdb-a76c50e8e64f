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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GuanajuatoAdminUsuarios.Framework;

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
        private readonly ICatTipoInvolucradoService _catTipoInvolucradoService;
        private readonly ICatEstadoVictimaService _catEstadoVictimaService;
        private readonly ICatHospitalesService _catHospitalesService;
        private readonly ICatInstitucionesTrasladoService _catInstitucionesTraslado;
        private readonly ICatAsientoService _catAsientoservice;
        private readonly ICatCinturon _catCinturon;
        private readonly ICatAutoridadesDisposicionService _catAutoridadesDisposicionservice;
        private readonly ICatAutoridadesEntregaService _catAutoridadesEntregaService;
        private readonly IOficiales _oficialesService;
        private readonly ICatCiudadesService _catCiudadesService;
        private readonly ICatAgenciasMinisterioService _catAgenciasMinisterioService;
        private readonly ICatDictionary _catDictionary;
        private readonly IInfraccionesService _infraccionesService;




        private int lastInsertedId = 0;
        private int idVehiculoInsertado = 0;

        public CapturaAccidentesController(ICapturaAccidentesService capturaAccidentesService, ICatMunicipiosService catMunicipiosService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatClasificacionAccidentes catClasificacionAccidentesService, ICatFactoresAccidentesService catFactoresAccidentesService, ICatFactoresOpcionesAccidentesService catFactoresOpcionesAccidentesService, ICatCausasAccidentesService catCausasAccidentesService,
            ITiposCarga tiposCargaService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IPensionesService pensionesService, ICatFormasTrasladoService catFormasTrasladoService, ICatTipoInvolucradoService catTipoInvolucradoService,
            ICatEstadoVictimaService catEstadoVictimaService, ICatHospitalesService catHospitalesService, ICatInstitucionesTrasladoService catIsntitucionesTraslado, ICatAsientoService catAsientoservice, ICatCinturon catCinturon, ICatAutoridadesDisposicionService catAutoridadesDisposicionservice,
            ICatAutoridadesEntregaService catAutoridadesEntregaService, IOficiales oficialesService, ICatCiudadesService catCiudadesService, ICatAgenciasMinisterioService catAgenciasMinisterioService,ICatDictionary catDictionary, IInfraccionesService infraccionesService)
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
            _catTipoInvolucradoService = catTipoInvolucradoService;
            _catEstadoVictimaService = catEstadoVictimaService;
            _catHospitalesService = catHospitalesService;
            _catAsientoservice = catAsientoservice;
            _catInstitucionesTraslado = catIsntitucionesTraslado;
            _catCinturon = catCinturon;
            _catAutoridadesDisposicionservice = catAutoridadesDisposicionservice;
            _catAutoridadesEntregaService = catAutoridadesEntregaService;
            _oficialesService = oficialesService;
            _catCiudadesService = catCiudadesService;
            _catAgenciasMinisterioService = catAgenciasMinisterioService;
            _catDictionary = catDictionary;
            _infraccionesService = infraccionesService;
        }
        /// <summary>
        /// //PRIMERA SECCION DE CAPTURA ACCIDENTE//////////
        /// </summary>
        public JsonResult BuscarAccidentesLista([DataSourceRequest] DataSourceRequest request)
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
            var AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente);
            return View("CapturaAaccidente", AccidenteSeleccionado);
        }

        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
        }
        public ActionResult ModalDetallesVehiculo(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var vehiculoInvolucrado = _capturaAccidentesService.InvolucradoSeleccionado(idAccidente, IdVehiculoInvolucrado, IdPropietarioInvolucrado);       
            return PartialView("_ModalDetalleVehiculos", vehiculoInvolucrado);
        }
        public ActionResult ModalBorraRegistro(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado,int IdAccidente)
        {
            return PartialView("_ModalEliminarInvolucrado");
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

        public IActionResult EliminarInvolucradoAccidente(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
        {
            var involucradoEliminado = _capturaAccidentesService.EliminarInvolucradoAcc(IdVehiculoInvolucrado, IdPropietarioInvolucrado,IdAccidente);
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados);

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

        public IActionResult ActualizarAccidenteConVehiculo(int IdVehiculo, int IdPersona, string Placa, string Serie)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var idVehiculoInsertado = _capturaAccidentesService.ActualizarConVehiculo(IdVehiculo, idAccidente,IdPersona, Placa, Serie);
            HttpContext.Session.SetInt32("idVehiculoInsertado", idVehiculoInsertado);
            return Json(IdPersona);
        }
        public IActionResult RegresarModalVehiculo()
        {
            int IdVehiculo = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var idVehiculoBorrado = _capturaAccidentesService.BorrarVehiculoAccidente(IdVehiculo, idAccidente);
            return Json(idVehiculoBorrado);
        }


        [HttpPost]
        public IActionResult ActualizarConConductor(int IdVehiculo, int IdPersona)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var idVehiculoInsertado = _capturaAccidentesService.InsertarConductor(IdVehiculo, idAccidente, IdPersona);

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
        public JsonResult ObtClasificacionAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListClasificaciones = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);

            return Json(ListClasificaciones.ToDataSourceResult(request));
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
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
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

            return PartialView("_ModalInvolucrado-Vehiculo");
        }
        public ActionResult SubmodalBuscarInvolucrado()
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
        public IActionResult AgregarFactorNuevo(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidente)

        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorFactorYOpcion(IdFactorAccidente, IdFactorOpcionAccidente, idAccidente);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);
            
            return Json(datosGrid);
        }
        public JsonResult ObtFactorOpcionAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListFactores = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(ListFactores.ToDataSourceResult(request));
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
        public JsonResult ObtCausasAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListCausas = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(ListCausas.ToDataSourceResult(request));
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
        public IActionResult SeleccionInvolucrado(int idPersonaInvolucrado, string nombre)
        {
            ViewBag.nombre = nombre;

            return RedirectToAction("ModalAgregarInvolucrado");
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

        public JsonResult Tipos_Drop()
        {
            var result = new SelectList(_catTipoInvolucradoService.ObtenerTipos(), "IdTipoInvolucrado", "TipoInvolucrado");
            return Json(result);
        }

        public JsonResult EstadoVictima_Drop()
        {
            var result = new SelectList(_catEstadoVictimaService.ObtenerEstados(), "IdEstadoVictima", "EstadoVictima");
            return Json(result);
        }
        public JsonResult Hospitales_Drop()
        {
            var result = new SelectList(_catHospitalesService.GetHospitales(), "IdHospital", "NombreHospital");
            return Json(result);
        }
        public JsonResult InstTraslado_Drop()
        {
            var result = new SelectList(_catInstitucionesTraslado.ObtenerInstitucionesActivas(), "IdInstitucionTraslado", "InstitucionTraslado");
            return Json(result);
        }
        public JsonResult Asiento_Drop()
        {
            var result = new SelectList(_catAsientoservice.ObtenerAsientos(), "IdAsiento", "Asiento");
            return Json(result);
        }
        public JsonResult Cinturon_Drop()
        {
            var result = new SelectList(_catCinturon.ObtenerCinturon(), "IdCinturon", "Cinturon");
            return Json(result);
        }
        public JsonResult Disposicion_Drop()
        {
            var result = new SelectList(_catAutoridadesDisposicionservice.ObtenerAutoridadesActivas(), "IdAutoridadDisposicion", "NombreAutoridadDisposicion");
            return Json(result);
        }
        public JsonResult Entrega_Drop()
        {
            var result = new SelectList(_catAutoridadesEntregaService.ObtenerAutoridadesActivas(), "IdAutoridadEntrega", "AutoridadEntrega");
            return Json(result);
        }

        public JsonResult Ciudades_Drop()
        {
            var result = new SelectList(_catCiudadesService.ObtenerCiudadesActivas(), "IdCiudad", "Ciudad");
            return Json(result);
        }
        public JsonResult AgMinisterio_Drop()
        {
            var result = new SelectList(_catAgenciasMinisterioService.ObtenerAgenciasActivas(), "IdAgenciaMinisterio", "NombreAgencia");
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

        public ActionResult CapturaAccidenteC(string descripcionCausa)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            _capturaAccidentesService.GuardarDescripcion(idAccidente, descripcionCausa);
            return View("CapturaCAccidente");
        }
        public ActionResult CapturaCr(int IdVehiculo, int IdInfraccion )
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(IdVehiculo, idAccidente, IdInfraccion);
            return View("CapturaCAccidente");
        }
        public ActionResult MostrarModalCrearInfraccion(int IdAccidente, int IdVehiculo, int IdConductor, int IdPropietario, string Placa, string Tarjeta)
        {
            var modelo = new NuevaInfraccionModel
            {
                IdAccidente = IdAccidente,
                IdPersona = IdConductor,
                IdVehiculo = IdVehiculo,
                IdPropietario = IdPropietario,
                Placa = Placa,
                Tarjeta = Tarjeta,
                
            };

            return PartialView("_ModalCrearInfraccion", modelo);
        }
        public ActionResult MostrarModalAgregarMonto(int IdAccidente, int IdVehiculoInvolucrado, int IdPropietarioInvolucrado)
        {
            var modelo = new MontoModel
            {
                IdAccidente = IdAccidente,
                IdVehiculoInvolucrado = IdVehiculoInvolucrado,
                IdPropietarioInvolucrado = IdPropietarioInvolucrado,
            };

            return PartialView("_ModalAgregarMonto", modelo);
        }

        
        [HttpPost]
        public ActionResult ajax_CrearInfraccion(NuevaInfraccionModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var DatosAccidente = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente);
            model.IdMunicipio =(int) DatosAccidente.IdMunicipio;
            model.IdCarretera = (int)DatosAccidente.IdCarretera;
            model.IdTramo =(int) DatosAccidente.IdTramo;
            model.Kilometro = DatosAccidente.Kilometro;
            var idPersonaInfraccion = _infraccionesService.CrearPersonaInfraccion((int)model.IdPersona);
            model.idPersonaInfraccion = idPersonaInfraccion;


            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

               var idInfraccion= _capturaAccidentesService.RegistrarInfraccion(model);

                return Json(new { id = idInfraccion });
            }
            return PartialView("_ModalCrearInfraccion");

        }

        [HttpPost]
        public ActionResult AgregarMontoVehiculo(MontoModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

                _capturaAccidentesService.AgregarMontoV(model);
                int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
                var ListVehiculos = _capturaAccidentesService.VehiculosInvolucrados(idAccidente); 
                return PartialView("_ListaVehiculosDaños", ListVehiculos);
            }
            return PartialView("_ModalAgregarMonto");

        }

        public JsonResult ObtenerInfraccionesVehiculos([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInfracciones = _capturaAccidentesService.InfraccionesVehiculosAccidete(idAccidente);

            return Json(ListVehiculosInfracciones.ToDataSourceResult(request));
        }
        public ActionResult ModalInfraccionesVehiculos()
        {
            return PartialView("_ModalAsignarInfracciones");
        }
        public IActionResult VincularInfraccionAccidente(int IdVehiculo, int IdInfraccion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(IdVehiculo, idAccidente, IdInfraccion);
            return Json(InfraccionAccidente);
        }
        public JsonResult ObtInfraccionesAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInfracciones = _capturaAccidentesService.InfraccionesDeAccidente(idAccidente);

            return Json(ListInfracciones.ToDataSourceResult(request));
        }
        public IActionResult GuardarRelacionPersonaVehiculo(int IdPersona, int IdVehiculoInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var PersonaVehiculo = _capturaAccidentesService.RelacionPersonaVehiculo(IdPersona, idAccidente, IdVehiculoInvolucrado);
            return Json(PersonaVehiculo);
        }
        public IActionResult ActualizarInfoInvolucrado(CapturaAccidentesModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroActualizado = _capturaAccidentesService.ActualizarInvolucrado(model, idAccidente);
            return Json(RegistroActualizado);
        }
        public JsonResult ObtInvolucradosAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);

            return Json(ListInvolucrados.ToDataSourceResult(request));
        }
        public ActionResult MostrarModalFechaHora(int IdPersona)
        {
            var model = new FechaHoraIngresoModel
            {
                IdPersona = IdPersona,
            };


            return PartialView("_ModalFechaHora", model);
        }
        [HttpPost]
        public ActionResult AgregarFechaHora(FechaHoraIngresoModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
                _capturaAccidentesService.AgregarFechaHoraIngreso(model,idAccidente);
                var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente); 
                return PartialView("_ListaInvolucradosFechaYHora",ListInvolucrados);
            }
            return PartialView("_ModalFechaHora");
        }
        [HttpPost]

        public IActionResult InsertarDatos(DatosAccidenteModel datosAccidente,int armasValue,int drogasValue,int valoresValue,int prendasValue,int otrosValue)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            _capturaAccidentesService.AgregarDatosFinales(datosAccidente, armasValue, drogasValue, valoresValue, prendasValue, otrosValue, idAccidente);

            return Json(datosAccidente);
        }

        public ActionResult SetLastInsertedIdEdit(int idAccidente)
        {
            HttpContext.Session.SetInt32("LastInsertedId", idAccidente);
            return RedirectToAction("CapturaAaccidente");
        }
    }
}

