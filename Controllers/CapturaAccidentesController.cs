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
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Http;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.RESTModels;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using System.Web;
using System.Numerics;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;
using System.Globalization;
using GuanajuatoAdminUsuarios.Helpers;
using Microsoft.AspNetCore.Authorization;
using static iTextSharp.tool.xml.html.table.TableRowElement;
using Kendo.Mvc;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using GuanajuatoAdminUsuarios.Util;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CapturaAccidentesController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
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
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly IPersonasService _personasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly AppSettings _appSettings;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly IRepuveService _repuveService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatSubtipoServicio _subtipoServicio;
        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;


        private int idOficina = 0;
        private int lastInsertedId = 0;
        private int idVehiculoInsertado = 0;
        private string resultValue = string.Empty;
        private bool placaEncontrada = false;
        private List<CapturaAccidentesModel> capturaAccidenteModel = new List<CapturaAccidentesModel>();
        private static CapturaAccidentesModel perModel = new CapturaAccidentesModel();
        public CapturaAccidentesController(ICapturaAccidentesService capturaAccidentesService, ICatMunicipiosService catMunicipiosService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatClasificacionAccidentes catClasificacionAccidentesService, ICatFactoresAccidentesService catFactoresAccidentesService, ICatFactoresOpcionesAccidentesService catFactoresOpcionesAccidentesService, ICatCausasAccidentesService catCausasAccidentesService,
            ITiposCarga tiposCargaService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IPensionesService pensionesService, ICatFormasTrasladoService catFormasTrasladoService, ICatTipoInvolucradoService catTipoInvolucradoService,
            ICatEstadoVictimaService catEstadoVictimaService, ICatHospitalesService catHospitalesService, ICatInstitucionesTrasladoService catIsntitucionesTraslado, ICatAsientoService catAsientoservice, ICatCinturon catCinturon, ICatAutoridadesDisposicionService catAutoridadesDisposicionservice,
            ICatAutoridadesEntregaService catAutoridadesEntregaService, IOficiales oficialesService, ICatCiudadesService catCiudadesService, ICatAgenciasMinisterioService catAgenciasMinisterioService, ICatDictionary catDictionary, IInfraccionesService infraccionesService, IHttpClientFactory httpClientFactory,
            ICotejarDocumentosClientService cotejarDocumentosClientService, IPersonasService personasService, IVehiculosService vehiculosService, IOptions<AppSettings> appSettings,
            ICatEntidadesService catEntidadesService,
            IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService
            , IRepuveService repuveService, IBitacoraService bitacoraService,
            ICatSubtipoServicio subtipoServicio,IVehiculoPlataformaService vehiculoPlataformaService

            )
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
            _httpClientFactory = httpClientFactory;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _personasService = personasService;
            _vehiculosService = vehiculosService;
            _appSettings = appSettings.Value;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _subtipoServicio = subtipoServicio;
            _vehiculoPlataformaService = vehiculoPlataformaService;
        }
        /// <summary>
        /// //PRIMERA SECCION DE CAPTURA ACCIDENTE//////////
        /// </summary>
        public IActionResult BuscarAccidentesLista([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentes(idOficina);
            return Json(ListAccidentesModel.ToDataSourceResult(request));
        }

        public IActionResult BuscarAccidentesListaPagination([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            filterValue(request.Filters);
            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = 10;
            pagination.Filter = resultValue;

            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentesPagination(idOficina, pagination);
            request.PageSize = 10;
            var total = 0;
            if (ListAccidentesModel.Count() > 0)
                total = ListAccidentesModel.ToList().FirstOrDefault().Total;

            var result = new DataSourceResult()
            {
                Data = ListAccidentesModel,
                Total = total
            };

            return Json(result);
        }

        public IActionResult Index(CapturaAccidentesModel capturaAccidentesService, [DataSourceRequest] DataSourceRequest request)
        {

            //filterValue(request.Filters);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = 1;
            pagination.Filter = resultValue;

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentesPagination(idOficina, pagination);
            if (ListAccidentesModel.Count == 0)
            {

                return View("AgregarAccidente");

            }
            else
            {
                return View("CapturaAccidentes", ListAccidentesModel);
            }
        }


        private void filterValue(IEnumerable<IFilterDescriptor> filters)
        {
            if (filters == null)
                return;

            if (filters.Any())
            {
                foreach (var filter in filters)
                {
                    var descriptor = filter as FilterDescriptor;
                    if (descriptor != null)
                    {
                        resultValue = descriptor.Value.ToString();
                        break;
                    }
                    else if (filter is CompositeFilterDescriptor)
                    {
                        if (resultValue == "")
                            filterValue(((CompositeFilterDescriptor)filter).FilterDescriptors);
                    }
                }
            }
        }

        public ActionResult NuevoAccidente()
        {
            return View("AgregarAccidente");
        }

        public ActionResult AgregarAccidente()
        {
            return View("AgregarAccidente");
        }

        public JsonResult Entidades_Drop()
        {
            var result = new SelectList(_catEntidadesService.ObtenerEntidades(), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Municipios_Drop()
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipios(), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Municipios_Drop2()
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipios2(), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Municipios_Por_Delegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorDelegacion(idOficina), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult CarreterasPorDelegacion()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");
            return Json(result);
        }
        public JsonResult Carreteras_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var tramos = _catTramosService.ObtenerTamosPorCarretera(carreteraDDValue);

            var result = new List<SelectListItem>();

            result.AddRange(new SelectList(tramos, "IdTramo", "Tramo"));

            result.Add(new SelectListItem { Value = "1", Text = "No aplica" });
            result.Add(new SelectListItem { Value = "2", Text = "No especificado" });

            return Json(result);
        }


        public JsonResult TramosTodos_Drop(int carreteraDDValue)
        {
            var result = new SelectList(_catTramosService.ObtenerTramos(), "IdTramo", "Tramo");
            return Json(result);
        }

        public JsonResult Municipios_Por_Entidad(int entidadDDlValue)
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");
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

                var oficina = User.FindFirst(CustomClaims.Oficina).Value;
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
                string abreviaturaMunicipio = User.FindFirst(CustomClaims.AbreviaturaMunicipio).Value;
                int dependencia = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia"));

                if (abreviaturaMunicipio.IsNullOrEmpty())
                {
                    return Json(new { success = false, message = "La delegación del usuario no tiene asociado un municipio, no se puede generar el folio de la solicitud de depósito" });

                }
                lastInsertedId = _capturaAccidentesService.GuardarParte1(model, idOficina, abreviaturaMunicipio, DateTime.Now.Year, oficina);
                HttpContext.Session.SetInt32("LastInsertedId", lastInsertedId);
                return Json(new { success = true });

            }
        }

        public ActionResult CapturaAaccidente(bool? showE)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);
            ViewBag.EsSoloLectura = showE.HasValue && showE.Value;
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
        public ActionResult ModalBorraRegistro(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
        {
            return PartialView("_ModalEliminarInvolucrado");
        }
        public ActionResult ModalBorraRegistroPersona(int IdPersona, int IdAccidente, int IdInvolucrado)
        {
            return PartialView("_ModalEliminarPersonaInvolucrada");
        }

        public ActionResult MostrarModalConductor(int IdPersona, int IdVehiculo)
        {
            ViewBag.IdVehiculo = IdVehiculo;
            var ListConductor = _capturaAccidentesService.ObtenerConductorPorId(IdPersona);
            HttpContext.Session.SetInt32("idVehiculoInsertado", IdVehiculo);

            return PartialView("_ModalConductor", ListConductor);
        }

        public ActionResult ModalClasificacionAccidente()
        {
            return PartialView("_ModalClasificacion");
        }
        public ActionResult ModalEliminarClasificacion(int IdAccidente, int idClasif)
        {
            List<CapturaAccidentesModel> clasificacionesModel = _capturaAccidentesService.AccidentePorID(IdAccidente);

            clasificacionesModel[0].IdClasificacionAccidente = idClasif;


            return PartialView("_ModalEliminarClasificacion");
        }
        public ActionResult ModalAnexo2()
        {
            var vehiculoEncontrado = new VehiculoModel();
            vehiculoEncontrado.idSubmarcaUpdated = vehiculoEncontrado.idSubmarca;
            vehiculoEncontrado.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoEncontrado.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoEncontrado.encontradoEn = 3;
            return PartialView("_Create", vehiculoEncontrado);
        }

        public IActionResult EliminarInvolucradoAccidente(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
        {
            var EliminarVehiculo = _capturaAccidentesService.BorrarVehiculoAccidente(IdVehiculoInvolucrado, IdAccidente);
            var involucradoEliminado = _capturaAccidentesService.EliminarInvolucradoAcc(IdVehiculoInvolucrado, IdPropietarioInvolucrado, IdAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(involucradoEliminado, ip, "CapturaAccidente_EliminarInvolucrado", "Eliminar", "delete", user);
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados);

        }

        [HttpPost]
        public async Task<ActionResult> BuscarVehiculo2(string Placa, string Serie, string folio)
        {
            if (_appSettings.AllowWebServices)
            {
                var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(Placa, Serie, folio);

                if (SeleccionVehiculo.Count == 0)
                {
                    //string jsonPartialVehiculosByWebServices = await AbrirModalVehiculo(Placa, Serie);
                    //return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                }
                return Json(new { noResults = false, data = SeleccionVehiculo, porWebService = false });
            }
            else
            {
                var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(Placa, Serie, folio);
                return Json(new { noResults = true, data = SeleccionVehiculo, porWebService = false });

            }
        }
        public JsonResult Entidades_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }


       


        public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {

                if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                {
                    model.PlacasBusqueda = model.PlacasBusqueda.ToUpper();
                }
                if (!string.IsNullOrEmpty(model.SerieBusqueda))
                {
                    model.SerieBusqueda = model.SerieBusqueda.ToUpper();
                }



                var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);

                if (SeleccionVehiculo.Count > 0)
                {
                    return Json(new { noResults = false, data = SeleccionVehiculo });
                }
                else
                {
                    var jsonPartialVehiculosByWebServices = await AbrirModalVehiculo(model);

                    if (jsonPartialVehiculosByWebServices != null)
                    {
                        return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                    }
                    else
                    {
                        return Json(new { noResults = true, data = new { } });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud" });
            }
        }


        //public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        //{
        //    try
        //    {
        //        var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);

        //        if (SeleccionVehiculo.Count > 0)
        //        {
        //            return Json(new { noResults = false, data = SeleccionVehiculo });
        //        }
        //        else
        //        {
        //            var jsonPartialVehiculosByWebServices = await AbrirModalVehiculo(model);

        //            var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);


        //            if (SeleccionVehiculo.Count > 0)
        //            {
        //                return Json(new { noResults = false, data = SeleccionVehiculo, porWebService = false });
        //            }

        //            var jsonPartialVehiculosByWebServices = await AbrirModalVehiculo(model);
        //            return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices, porWebService = true });

        //        }
        //    }
        //    catch
        //    {

        //    }
        //}


        public async Task<string> AbrirModalVehiculo(VehiculoBusquedaModel model)
        {
            try
            {

               //Se busca el vehiculo en el padron RIAG, Finanzas y REPUVE
            VehiculoModel vehiculo = _vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model);
                return await this.RenderViewAsync("_Create", vehiculo, true);
            }
            catch (Exception ex)
            {
                Logger.Error("CapturaAccidentes - AbrirModalVehiculo: " + ex.Message);
                return null;
            }
        }


       
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
       
       

        public JsonResult ObtVehiculosInvol([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados.ToDataSourceResult(request));
        }

        public IActionResult ActualizarAccidenteConVehiculo(int IdVehiculo, int IdPersona, string Placa="", string Serie="")
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var can = _capturaAccidentesService.ActualizarConVehiculo(IdVehiculo, idAccidente, IdPersona, Placa, Serie);
            if (can == 0)
            {
                return Json(new { IdPersona = IdPersona, IdVehiculoH = IdVehiculo , error=1 });
            }

            var idVehiculoInsertado = IdVehiculo;

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_AccidenteConVehiculo", "Actualizar", "update", user);

            HttpContext.Session.SetInt32("idVehiculoInsertado", idVehiculoInsertado);
            //return Json(IdPersona);
            return Json(new { IdPersona = IdPersona, IdVehiculoH = IdVehiculo });
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

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_ConConductor", "Actualizar", "update", user);

            return Json(idVehiculoInsertado);
        }

        [HttpPost]
        public IActionResult ActualizarSinConductor(int IdVehiculo)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var personamodel = new PersonaModel();
            personamodel.nombre = "SE Ignora";
            personamodel.idCatTipoPersona = 1;
            personamodel.PersonaDireccion = new PersonaDireccionModel();

            var IdPersona = _personasService.CreatePersona(personamodel);
            var idVehiculoInsertado = _capturaAccidentesService.InsertarConductor(IdVehiculo, idAccidente, IdPersona);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_ConConductor", "Actualizar", "update", user);

            return Json(idVehiculoInsertado);
        }

        [HttpPost]
        public IActionResult ActualizarInfoAccidente(DateTime Fecha, TimeSpan Hora, int IdMunicipio, int IdCarretera, int IdTramo, int Kilometro)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var idAccidenteActualizado = _capturaAccidentesService.ActualizaInfoAccidente(idAccidente, Fecha, Hora, IdMunicipio, IdCarretera, IdTramo, Kilometro);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_InfoAccidente", "Actualizar", "update", user);

            return Json(idAccidenteActualizado);
        }

        public IActionResult GuardarConductorVehiculo(int IdPersona, int idAuto)
        {
            int IdVehiculoI = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0; // Obtener el valor de idVehiculoInsertado desde la variable de sesión
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.InsertarConductor(IdVehiculoI, idAccidente, IdPersona);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_ConductorVehiculo", "Insertar", "insert", user);

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

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_ComplementoVehiculo", "Insertar", "insert", user);

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
        public IActionResult EliminaClasificacion(int IdAccidente, int IdClasificacionAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var clasificacionEliminada = _capturaAccidentesService.ClasificacionEliminar(IdAccidente, IdClasificacionAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(clasificacionEliminada, ip, "CapturaAccidente_Clasificacion", "Eliminar", "delete", user);

            return Json(datosGrid);

        }
        ///////////////
        ///SEGUNDA SECCION CAPTURA ACCIDENTE///////////
        ///

        public ActionResult CapturaBAccidente(bool? esSoloLectura)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            string descripcionCausa = _capturaAccidentesService.ObtenerDescripcionCausaDesdeBD(idAccidente);
            ViewData["DescripcionCausa"] = descripcionCausa;
            ViewBag.EsSoloLectura = esSoloLectura ?? false;
            return View("CapturaBAccidente");
        }

        public ActionResult ModalFactorAccidente()

        {
            return PartialView("_ModalFactor");
        }
        public ActionResult ModalEditarFactorAccidente(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion)

        {
            EditarFactorOpcionModel modelo = new EditarFactorOpcionModel
            {
                IdFactorAccidente = IdFactorAccidente,
                IdFactorOpcionAccidente = IdFactorOpcionAccidente,
                IdAccidenteFactorOpcion = IdAccidenteFactorOpcion
            };
            return PartialView("_ModalEditarFactor", modelo);
        }

        public ActionResult ModalEliminarFactorAccidente(string FactorAccidente, string FactorOpcionAccidente, int IdAccidenteFactorOpcion)
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
            //var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            //var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            //ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ///ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            return PartialView("_ModalCapturarConductor");
        }
        public ActionResult ModalEditarCausaAccidente(int IdCausaAccidente, string CausaAccidente, int idAccidenteCausa)
        {
            ViewBag.IdCausaAccidente = IdCausaAccidente;
            ViewBag.CausaAccidente = CausaAccidente;
            return PartialView("_ModalEditarCausa");
        }
        public ActionResult ModalEliminarCausas(int IdCausaAccidente, string CausaAccidente, int IdAccidenteCausa)
        {
            ViewBag.IdCausaAccidente = IdCausaAccidente;
            ViewBag.CausaAccidente = CausaAccidente;
            ViewBag.IdAccidenteCausa = IdAccidenteCausa;
            return PartialView("_ModalEliminarCausa");
        }

        public ActionResult ActualizaIndiceCausaAccidente(int idAccidenteCausa, int indice, int idAccidenteCausaParent, int indiceParent)
        {
            _capturaAccidentesService.ActualizaIndiceCuasa(idAccidenteCausa, indice);
            _capturaAccidentesService.ActualizaIndiceCuasa(idAccidenteCausaParent, indiceParent);
            return Json(new { success = true });
        }

        public ActionResult ModalAgregarInvolucrado()
        {

            return PartialView("_ModalInvolucrado-Vehiculo");
        }

        //public ActionResult MostrarDetalle(string Id)
        //{
        //	var ListInfraccionesModel = _CortesiasNoAplicadasService.ObtenerDetalleCortesiasNoAplicada(Id);
        //	return PartialView("_DetalleCortesiasNoAplicadas", ListInfraccionesModel);

        //}


        public ActionResult ModalAgregarInvolucradoPersona(int IdPersona)
        {
            var listPersonasModel = _capturaAccidentesService.ObtenerDetallePersona(IdPersona);
            return PartialView("_ModalInvolucrado-Vehiculo-Persona", listPersonasModel);
        }
        public ActionResult NuevoInvolucradoPersona(int IdPersona, int idAccidente, int IdInvolucrado)
        {
            var listPersonasModel = _capturaAccidentesService.DatosInvolucradoEdicion(IdPersona, idAccidente, IdInvolucrado);
            return PartialView("_ModalEditarInvolucrado", listPersonasModel);
        }

        [HttpGet]
        public IActionResult SubmodalBuscarInvolucrado()
        {
            BusquedaInvolucradoModel model = new BusquedaInvolucradoModel();
            //var ListInvolucradoModel = _personasService.GetAllPersonasFisicasPagination(pagination);
            //ViewBag.ModeInvolucrado = ListInvolucradoModel;

            return PartialView("_ModalAgregarInvolucrado");
        }

        [HttpGet]
        public IActionResult SubmodalBuscarInvolucradoPersona()
        {
            BusquedaInvolucradoModel model = new BusquedaInvolucradoModel();
            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            ViewBag.ModeInvolucrado = ListInvolucradoModel;

            return PartialView("_ModalAgregarInvolucradoPersona");
        }

	
		public IActionResult AbrirModalBuscarInvolucrado()
		{
			BusquedaInvolucradoModel model = new BusquedaInvolucradoModel();



			return PartialView("BusquedaPersonaFisica");
		}
		public IActionResult SinInvolucrado()
		{
			int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
			var personamodel = new PersonaModel();
			personamodel.nombre = "SE Ignora";
			personamodel.idCatTipoPersona = 1;
			personamodel.PersonaDireccion = new PersonaDireccionModel();

			var nuevaPersona = _personasService.CreatePersona(personamodel);
            personamodel.idPersona = nuevaPersona;

			return Json (personamodel);
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
        public IActionResult EditarValorFactorYOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarFactorOpcion(IdFactorAccidente, IdFactorOpcionAccidente, IdAccidenteFactorOpcion);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }
        [HttpPost]
        public IActionResult EliminarValorFactorYOpcion(int IdAccidenteFactorOpcion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarValorFactorYOpcion(IdAccidenteFactorOpcion);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_ValorFactorYOpcion", "Eliminar", "delete", user);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }

        public IActionResult AgregarCausaNuevo(int IdCausaAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorCausa(IdCausaAccidente, idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_CausaNuevo", "Insertar", "insert", user);
            return Json(datosGrid);
        }
        public IActionResult EditarCausa(int IdCausaAccidente, int idAccidenteCausa)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarValorCausa(IdCausaAccidente, idAccidenteCausa);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(datosGrid);
        }
        public IActionResult EliminarCausaAccidente(int IdCausaAccidente, int idAccidenteCausa)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarCausaBD(IdCausaAccidente, idAccidente, idAccidenteCausa);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_CausaAccidente", "Eliminar", "delete", user);
            _capturaAccidentesService.RecalcularIndex(idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(datosGrid);
        }
        public JsonResult ObtCausasAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListCausas = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(ListCausas.ToDataSourceResult(request));
        }
        [HttpGet]
        public ActionResult BuscarInvolucrado(BusquedaInvolucradoModel model)
        {
            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            return Json(ListInvolucradoModel);
        }





        public IActionResult GuardarInvolucrado(int idPersonaInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarPersonaInvolucrada(idPersonaInvolucrado, idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_PersonaInvolucrada", "Insertar", "insert", user);
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

        public JsonResult Delegaciones_Drop2()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().Where(X => X.Transito == 1), "IdDelegacion", "Delegacion");
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
        public JsonResult AgMinisterioDelegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catAgenciasMinisterioService.ObtenerAgenciasActivasPorDelegacion(idOficina), "IdAgenciaMinisterio", "NombreAgencia");
            return Json(result);
        }

        public JsonResult AgMinisterioDelegacion_DropTodas()
        {
            //int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catAgenciasMinisterioService.ObtenerAgenciasActivas(), "IdAgenciaMinisterio", "NombreAgencia");
            return Json(result);
        }
        public JsonResult Oficiales_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var oficiales = _oficialesService.GetOficialesFiltrados(idOficina, idDependencia)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }

        public JsonResult Oficiales_DropTodos()
        {
            //int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var oficiales = _oficialesService.GetOficiales()// .GetOficialesFiltrados(idOficina, idDependencia)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }

        public JsonResult Oficiales_DropCorpTodos()
        {
            //int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var oficiales = _oficialesService.GetOficialesByCorporacion(idDependencia)// .GetOficialesFiltrados(idOficina, idDependencia)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }


        public JsonResult CambiosDDLOficiales()
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");


            var oficiales = _oficialesService.GetOficialesPorDependencia(idDependencia)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }



        public ActionResult CapturaAccidenteC(string descripcionCausa, bool rOy)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            _capturaAccidentesService.GuardarDescripcion(idAccidente, descripcionCausa);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idAccidente, ip, "CapturaAccidente_Descripcion", "Insertar", "insert", user);

            DatosAccidenteModel datosAccidente = _capturaAccidentesService.ObtenerDatosFinales(idAccidente);
            ViewBag.EsSoloLectura = rOy;
            return View("CapturaCAccidente", datosAccidente);

        }

        public ActionResult CapturaCr(int IdVehiculo, int IdInfraccion)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(IdVehiculo, idAccidente, IdInfraccion);
            var AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);

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
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var DatosAccidente = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);
            model.IdMunicipio = (int)DatosAccidente.IdMunicipio;
            model.IdCarretera = (int)DatosAccidente.IdCarretera;
            model.IdTramo = (int)DatosAccidente.IdTramo;
            model.Kilometro = DatosAccidente.Kilometro;

            DateTime fechaInfraccion = (DateTime)DatosAccidente.Fecha;
       //     string horaInfraccion = DatosAccidente.Hora.ToString("hhmm");
            


            model.fechaInfraccion = fechaInfraccion;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

                bool validarFolio = _infraccionesService.ValidarFolio(model.folioInfraccion, idDependencia);

                if (!validarFolio)
                {
                    var idInfraccion = _capturaAccidentesService.RegistrarInfraccion(model, idDependencia);
                    var idPersonaInfraccion = _infraccionesService.CrearPersonaInfraccion((int)idInfraccion, (int)model.IdPersona);
                    var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(model.IdVehiculo, idAccidente, idInfraccion);

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    _bitacoraServices.insertBitacora(idPersonaInfraccion, ip, "CapturaAccidente_PersonaInfraccion", "Insertar", "insert", user);
                    return Json(new { id = idInfraccion });
                }
                else
                {
                    return Json(new { id = 0, validacion = validarFolio });
                }
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

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(idAccidente, ip, "CapturaAccidente_Monto", "Insertar", "insert", user);

                var ListVehiculos = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);
                return PartialView("_ListaVehiculosDaños", ListVehiculos);
            }
            return PartialView("_ModalAgregarMonto");

        }

        public JsonResult ObtenerInfraccionesVehiculos([DataSourceRequest] DataSourceRequest request)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInfracciones = _capturaAccidentesService.InfraccionesVehiculosAccidete(idAccidente);

            return Json(ListVehiculosInfracciones.ToDataSourceResult(request));
        }
        public ActionResult ModalInfraccionesVehiculos(string montoCamino, string montoCarga, string montoPropietarios, string montoOtros)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var DatosMontos = _capturaAccidentesService.GuardarDatosPrevioInfraccion(idAccidente, montoCamino, montoCarga, montoPropietarios, montoOtros);

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
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInfracciones = _capturaAccidentesService.InfraccionesDeAccidente(idAccidente);

            return Json(ListInfracciones.ToDataSourceResult(request));
        }
        public IActionResult GuardarRelacionPersonaVehiculo(int IdPersona, int IdVehiculoInvolucrado,int IdInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var PersonaVehiculo = _capturaAccidentesService.RelacionPersonaVehiculo(IdPersona, idAccidente, IdVehiculoInvolucrado, IdInvolucrado);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(PersonaVehiculo, ip, "CapturaAccidente_RelacionPersonaVehiculo", "Insertar", "insert", user);

            return Json(PersonaVehiculo);
        }
        public IActionResult ActualizarInfoInvolucrado(CapturaAccidentesModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroActualizado = _capturaAccidentesService.ActualizarInvolucrado(model, idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(RegistroActualizado, ip, "CapturaAccidente_Involucrado", "Actualizar", "update", user);

            return Json(RegistroActualizado);
        }
        public JsonResult ObtInvolucradosAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);

            return Json(ListInvolucrados.ToDataSourceResult(request));
        }

        public IActionResult EliminaInvolucrado(int IdAccidente, int idPersona, int IdInvolucrado)
        {
            var eliminarInvolucrado = _capturaAccidentesService.EliminarInvolucrado(IdInvolucrado);
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(eliminarInvolucrado, ip, "CapturaAccidente_Involucrado", "Eliminar", "delete", user);

            return Json(ListInvolucrados);

        }
        public IActionResult EditarInvolucrado(CapturaAccidentesModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarInvolucrado(model);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }

        public ActionResult MostrarModalFechaHora(int IdPersona, string FechaIngreso)
        {

            var model = new FechaHoraIngresoModel
            {
                IdPersona = IdPersona,
                FechaIngreso = DateTime.ParseExact(FechaIngreso.Substring(0, 24),
                              "ddd MMM d yyyy HH:mm:ss",
                              CultureInfo.InvariantCulture),
                HoraIngreso = DateTime.ParseExact(FechaIngreso.Substring(0, 24),
                              "ddd MMM d yyyy HH:mm:ss",
                              CultureInfo.InvariantCulture).TimeOfDay
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
                _capturaAccidentesService.AgregarFechaHoraIngreso(model, idAccidente);
                var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);
                return PartialView("_ListaInvolucradosFechaYHora", ListInvolucrados);
            }
            return PartialView("_ModalFechaHora");
        }
        [HttpPost]

        public IActionResult InsertarDatos(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int convenioValue)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            _capturaAccidentesService.AgregarDatosFinales(datosAccidente, armasValue, drogasValue, valoresValue, prendasValue, otrosValue, idAccidente, convenioValue);

            return Json(datosAccidente);
        }

        public ActionResult SetLastInsertedIdEdit(bool modoSoloLectura, int idAccidente)
        {
            ViewBag.ModoSoloLectura = modoSoloLectura;
            HttpContext.Session.SetInt32("LastInsertedId", idAccidente);

            return Ok();
        }
        public IActionResult ConsultaAccidente(bool modoSoloLectura, int idAccidente)
        {
            HttpContext.Session.SetInt32("LastInsertedId", idAccidente);
            ViewBag.ModoSoloLectura = modoSoloLectura;
            return Ok();
        }
        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            //var personasMoralesModel = _personasService.GetAllPersonasMorales();
            var modelList = _personasService.ObterPersonaPorIDList(IdPersonaMoral); ;

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(IdPersonaMoral, ip, "CapturaAccidente_PersonaMoral", "Insertar", "insert", user);

            return PartialView("_ListPersonasMorales", modelList);
        }


        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscas()
        {
            var personasFisicas = _personasService.GetAllPersonasFisicas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }



        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscasPagination([DataSourceRequest] DataSourceRequest request, int id = 0)
        {
            try
            {
                filterValue(request.Filters);
                Pagination pagination = new Pagination();
                pagination.PageIndex = request.Page - 1;
                pagination.PageSize = 10;
                pagination.Filter = resultValue;

                var personasFisicas = _personasService.GetAllPersonasFisicasPagination(pagination);
                request.PageSize = 10;
                var total = 0;
                if (personasFisicas.Count() > 0)
                    total = personasFisicas.ToList().FirstOrDefault().total;

                var result = new DataSourceResult()
                {
                    Data = personasFisicas,
                    Total = total
                };

                return Json(result);
            }
            catch
            {
                return null;
            }
        }



        public IActionResult pruebapartial()
        {

            var q = PartialView("_PersonasFisicas");
            return q;
        }



        [HttpPost]
        public ActionResult ajax_BuscarPersonaMoral(PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = _personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }
        [HttpPost]
        public ActionResult ajax_CrearPersonaFisica(PersonaModel Persona)
        {
            Persona.nombre = Persona.nombreFisico;
            Persona.apellidoMaterno = Persona.apellidoMaternoFisico;
            Persona.apellidoPaterno = Persona.apellidoPaternoFisico;
            Persona.CURP = Persona.CURPFisico;
            Persona.RFC = Persona.RFCFisico;
            Persona.numeroLicencia = Persona.numeroLicenciaFisico;
            Persona.idTipoLicencia = Persona.idTipoLicencia;
            Persona.vigenciaLicencia = Persona.vigenciaLicencia;
            Persona.PersonaDireccion.idEntidad = Persona.PersonaDireccion.idEntidadFisico;
            Persona.PersonaDireccion.idMunicipio = Persona.PersonaDireccion.idMunicipioFisico;
            Persona.PersonaDireccion.correo = Persona.PersonaDireccion.correoFisico;
            Persona.PersonaDireccion.telefono = Persona.PersonaDireccion.telefonoFisico;
            Persona.PersonaDireccion.colonia = Persona.PersonaDireccion.coloniaFisico;
            Persona.PersonaDireccion.calle = Persona.PersonaDireccion.calleFisico;
            Persona.PersonaDireccion.numero = Persona.PersonaDireccion.numeroFisico;
            Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            var IdPersonaFisica = _personasService.CreatePersona(Persona);
            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(IdPersonaFisica, ip, "CapturaAccidente_PersonaFisica", "Insertar", "insert", user);

            if (IdPersonaFisica == 0)
            {
                throw new Exception("Ocurrio un error al dar de alta la persona");
            }
            var modelList = _personasService.ObterPersonaPorIDList(IdPersonaFisica); ;
            return PartialView("_PersonasFisicas", modelList);
        }

        [HttpGet]
        public ActionResult ajax_GetPersonaMoral(int id)
        {
            var personaModel = _personasService.GetPersonaTypeById(id);
            return PartialView("_UpdatePersonaMoral", personaModel);
        }

        [HttpPost]
        public ActionResult ajax_UpdatePersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            var personaModel = _personasService.UpdatePersonaMoral(Persona);
            var personaEditada = _personasService.GetPersonaTypeById((int)Persona.idPersona);

            return Json(new { data = personaEditada });
        }


        //TODO: ejemplo crear vehiculo por service de guanajuato
        [HttpPost]


        public ActionResult ajax_CrearVehiculo_Ejemplo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            if (model.idVehiculo > 0)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);
            }
            else if (model.idVehiculo <= 0)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }

            if (IdVehiculo != 0)
            {
            }
            else
            {
            }


            var resultados = _capturaAccidentesService.BuscarPorParametro(model.placas, model.serie, model.placas);

            return Json(new { data = resultados });
        }

        public ActionResult ajax_CrearVehiculo_Ejemplo2(VehiculoModel model)
        {
            // creamos a la persona sin datos
            if (model.Persona.idPersona == -1)
            {

                PersonaDireccionModel direccion = new PersonaDireccionModel();
                direccion.idEntidad = 35;
                direccion.colonia = "se ignora";
                direccion.calle = "se ignora";
                direccion.numero = "se ignora";
                PersonaModel persona = new PersonaModel();
                persona.nombre = "se ignora";
                persona.idCatTipoPersona = (int)TipoPersona.Fisica;
                persona.PersonaDireccion = direccion;
                persona.idGenero = 1;
                var IdPersonaFisica = _personasService.CreatePersona(persona);
                model.idPersona = IdPersonaFisica;
                model.Persona.idPersona = IdPersonaFisica;
                model.propietario = IdPersonaFisica.ToString();
            }
            var IdVehiculo = _vehiculosService.CreateVehiculo(model);

            if (IdVehiculo > 0)
            {

                var Placa = model.placas;
                var Serie = model.serie;
                var folio = "";
                var resultados = _capturaAccidentesService.BuscarPorParametroid(IdVehiculo.ToString());
                return Json(new { success = true, data = resultados });
            }
            else if (IdVehiculo == -1)
            {
                return Json(new { success = false, duplicate = true });
            }
            else
            {
                return Json(new { success = false, error = "Error al guardar el vehículo." });
            }
        }


                [HttpPost]
        public ActionResult ajax_CrearVehiculo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            if (model.encontradoEn == (int)EstatusBusquedaVehiculo.Sitteg)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(IdVehiculo, ip, "CapturaAccidente_Vehiculo", "Actualizar", "update", user);
            }
            else if (model.encontradoEn == (int)EstatusBusquedaVehiculo.NoEncontrado)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(IdVehiculo, ip, "CapturaAccidente_Vehiculo", "Insertar", "insert", user);
            }

            if (IdVehiculo != 0)
            {
                var resultados = _vehiculosService.GetAllVehiculos();
                return Json(new { data = resultados });
            }
            else
            {
                return null;
            }
        }

        public async Task<IActionResult> BuscarPorParametro(PersonaModel model)
        {
            // Realizar la búsqueda de personas
            var personasList = _personasService.BusquedaPersona(model);

            var personasListFormato = personasList.Select(persona => new
            {
                IdPersona = persona.idPersona,
                nombre = persona.nombre,
                apellidoPaterno = persona.apellidoPaterno,
                apellidoMaterno = persona.apellidoMaterno,
                CURP = persona.CURP,
                RFC = persona.RFC,
                fechaNacimiento = persona.fechaNacimiento,
                numeroLicencia = persona.numeroLicencia
            }).ToList();
            if (personasList.Any())
            {
                return Json(new { encontrada = true, data = personasListFormato });
            }
            string parametros = "";
            parametros += string.IsNullOrEmpty(model.numeroLicenciaBusqueda) ? "" : "licencia=" + model.numeroLicenciaBusqueda;
            parametros += string.IsNullOrEmpty(model.CURPBusqueda) ? "" : "curp=" + model.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.RFCBusqueda) ? "" : "rfc=" + model.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.nombreBusqueda) ? "" : "nombre=" + model.nombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.apellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.apellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.apellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.apellidoMaternoBusqueda;

            string ultimo = parametros.Substring(parametros.Length - 1);
            if (ultimo.Equals("&"))
                parametros = parametros.Substring(0, parametros.Length - 1);

            try
            {
                string urlServ = Request.GetDisplayUrl();
                Uri uri = new Uri(urlServ);
                string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                var url = requested + $"/api/Licencias/datos_generales?" + parametros;

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);

                    return Json(respuesta);
                }
            }
            catch (Exception ex)
            {
                // En caso de errores, devolver una respuesta JSON con licencia no encontrada
                return Json(new { encontrada = false, data = personasListFormato, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }

            // Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
            return Json(new { encontrada = false, data = personasListFormato, message = "No se encontraron resultados." });
        }

        [HttpPost]
        public ActionResult GuardaDesdeServicio(LicenciaPersonaDatos personaDatos)
        {
            try
            {
                var id = _personasService.InsertarDesdeServicio(personaDatos);
                var datosTabla = _personasService.BuscarPersonaSoloLicencia(personaDatos.NUM_LICENCIA==null?"sin numero licencia": personaDatos.NUM_LICENCIA);

                CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
                involucrado.IdPersona = (int)datosTabla.idPersona;
                involucrado.nombre = datosTabla.nombre;
                involucrado.apellidoPaterno = datosTabla.apellidoPaterno;
                involucrado.apellidoMaterno = datosTabla.apellidoMaterno;
                involucrado.RFC = datosTabla.RFC;
                involucrado.CURP = datosTabla.CURP;
                involucrado.numeroLicencia = datosTabla.numeroLicencia;

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(id, ip, "CapturaAccidente_DesdeServicio", "Insertar", "insert", user);

                return Json(involucrado);
            }
            catch (Exception ex)
            {
                // Maneja el error de manera adecuada
                return Json(new { error = "Error al guardar en la base de datos: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult ajax_CrearPersona(PersonaModel model)
        {

            int id = _personasService.CreatePersona(model);
            var modelList = _capturaAccidentesService.ObtenerConductorPorId(id);
            string formatoFecha = "dd/MM/yyyy";
            if (DateTime.TryParseExact(modelList.FormatDateNacimiento, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaNacimiento))
            {
                modelList.fechaNacimiento = fechaNacimiento;
            }
            else
            {
                modelList.fechaNacimiento = null;
            }
            var jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy", // Establece el formato de fecha deseado
                Formatting = Formatting.None // Otra configuración de serialización si es necesaria
            };

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(id, ip, "CapturaAccidente_Persona", "Insertar", "insert", user);

            // Usa JsonResult con configuración personalizada de serialización
            return new JsonResult(modelList, jsonSettings);
        }



        public JsonResult test()
        {
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var result = new SelectList(catGeneros.CatalogList, "Id", "Text");
            return Json(result);
        }

        [HttpGet]
        public IActionResult ajax_ModalEditarPersona(int id)
        {
            var model = _personasService.GetPersonaById(id);
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
            return PartialView("_EditarPersona", model);
        }

        [HttpPost]
        public IActionResult ajax_EditarPersona(PersonaModel model)
        {
            //var model = json.ToObject<Gruas2Model>();
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                if (model.PersonaDireccion.idPersona == null || model.PersonaDireccion.idPersona <= 0)
                {
                    model.PersonaDireccion.idPersona = model.idPersona;
                    int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    _bitacoraServices.insertBitacora(idDireccion, ip, "CapturaAccidente_PersonaDireccion", "Insertar", "insert", user);
                }
                else
                {
                    int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    _bitacoraServices.insertBitacora(idDireccion, ip, "CapturaAccidente_PersonaDireccion", "Actualizar", "update", user);
                }
                int id = _personasService.UpdatePersona(model);
                var modelList = _personasService.GetPersonaById((int)model.idPersona);
                var formattedModelList = new
                {
                    IdPersona = modelList.idPersona,
                    nombre = modelList.nombre,
                    apellidoPaterno = modelList.apellidoPaterno,
                    apellidoMaterno = modelList.apellidoMaterno,
                    RFC = modelList.RFC,
                    CURP = modelList.CURP,
					fechaNacimiento = modelList.fechaNacimiento != null ? ((DateTime)modelList.fechaNacimiento).ToString("dd/MM/yyyy") : "" ,                   
                    numeroLicencia = modelList.numeroLicencia,
                    entidad = modelList.PersonaDireccion.entidad,
                    municipio = modelList.PersonaDireccion.municipio,
                    calle= modelList.PersonaDireccion.calle,
                    numero = modelList.PersonaDireccion.numero,
                    colonia = modelList.PersonaDireccion.colonia,
                    telefono = modelList.PersonaDireccion.telefono,
                    correo = modelList.PersonaDireccion.correo,
                    genero = modelList.genero
                };
                return Json(new { data = formattedModelList });
            }
            return RedirectToAction("Index");
        }
        public ActionResult ModalEliminarInfraccionDelAccidente(int IdInfraccion, string FolioInfraccion)
        {
            ViewBag.FolioInfraccion = FolioInfraccion;
            return PartialView("_ModalEliminarInfraccion");
        }
        [HttpPost]
        public IActionResult ajax_EliminarRegistroInfraccion(int IdInfraccion)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarRegistroInfraccion(IdInfraccion);

            var ListInfracciones = _capturaAccidentesService.InfraccionesDeAccidente(idAccidente);

            return Json(ListInfracciones);
        }



        [HttpPost]
        public async Task<IActionResult> BuscarPorParametroPaginado([DataSourceRequest] DataSourceRequest request, CapturaAccidentesModel capturaModel)
        {
            // Realizar la búsqueda de personas
            // if (model != null)
            //    perModel = model;
            //else
            //   model = perModel;
            BusquedaPersonaModel  model= new BusquedaPersonaModel();
            PersonaModel personaM = new PersonaModel();
            personaM.CURPBusqueda =capturaModel.CURPBusqueda;
            personaM.RFCBusqueda = capturaModel.RFCBusqueda;
            personaM.nombreBusqueda= capturaModel.nombreBusqueda;
            personaM.apellidoPaternoBusqueda = capturaModel.apellidoPaternoBusqueda;
            personaM.apellidoMaternoBusqueda = capturaModel.apellidoMaternoBusqueda;
            personaM.numeroLicenciaBusqueda = capturaModel.numeroLicenciaBusqueda;

            model.PersonaModel = personaM;

           var findAll = false;
            var personas = new BusquedaPersonaModel();
            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            if (model != null)
            {
                
                if (model.PersonaModel.apellidoMaternoBusqueda == null &&
                    model.PersonaModel.apellidoPaternoBusqueda == null &&
                    model.PersonaModel.CURPBusqueda == null &&
                    model.PersonaModel.RFCBusqueda == null &&
                    model.PersonaModel.numeroLicenciaBusqueda == null &&
                    model.PersonaModel.nombreBusqueda == null)
                {
                    pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
                }
                else
                {
                    findAll = true;
                    pagination.PageSize = 1000000;
                }
            }
            else
            {
                pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
            }

            //model = perModel;
            var personasList = _personasService.BusquedaPersonaPagination(model, pagination);

            // Verificar si se encontraron resultados en la búsqueda de personas
            if (personasList.Any())
            {
                personas.ListadoPersonas = personasList;
                var total = 0;
                if (personasList.Count() > 0)
                    total = personasList.ToList().FirstOrDefault().total;

                //if (findAll)
                request.PageSize = pagination.PageSize;

                //      var result = new DataSourceResult()
                //        {
                //              Data = personas.ListadoPersonas,
                //                Total = total,                    
                //              };
                List<CapturaAccidentesModel> resultado = new List<CapturaAccidentesModel>(); 
                foreach (PersonaModel pivote in personas.ListadoPersonas)
                {
                    CapturaAccidentesModel capAcc = new CapturaAccidentesModel();
                    capAcc.IdPersona = (int)pivote.idPersona;
                    capAcc.nombre = pivote.nombre;
                    capAcc.apellidoPaterno = pivote.apellidoPaterno;
                    capAcc.apellidoMaterno = pivote.apellidoMaterno;
                    capAcc.TipoLicencia= pivote.tipoLicencia;
                    capAcc.numeroLicencia = pivote.numeroLicencia;
                    capAcc.CURP= pivote.CURP;
                    resultado.Add(capAcc);
                }


                return Json(new { encontrada = true, Data = resultado, tipo = "riag", message = "busqueda exitosa" });

//                
            }

            // Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia

            string parametros = "";
            parametros += string.IsNullOrEmpty(model.PersonaModel.numeroLicenciaBusqueda) ? "" : "licencia=" + model.PersonaModel.numeroLicenciaBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.CURPBusqueda) ? "" : "curp=" + model.PersonaModel.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.RFCBusqueda) ? "" : "rfc=" + model.PersonaModel.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.nombreBusqueda) ? "" : "nombre=" + model.PersonaModel.nombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.PersonaModel.apellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.PersonaModel.apellidoMaternoBusqueda;
            string ultimo = parametros.Substring(parametros.Length - 1);
            if (ultimo.Equals("&"))
                parametros = parametros.Substring(0, parametros.Length - 1);

            try
            {
                string urlServ = Request.GetDisplayUrl();
                Uri uri = new Uri(urlServ);
                string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                var url = requested + $"/api/Licencias/datos_generales?" + parametros;

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    //    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);
                    // var personasEncontradas = content.
                    //  return Json(new { encontrada = false, Data = respuesta.datos, tipo = respuesta.datos == null ? "sin datos" : "success", message = respuesta.mensaje });


                    List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

                    List<CapturaAccidentesModel> resultado = new List<CapturaAccidentesModel>();
                    foreach (LicenciaPersonaDatos pivote in respuesta)
                    {
                        CapturaAccidentesModel capAcc = new CapturaAccidentesModel();
                        capAcc.nombre = pivote.NOMBRE;
                        capAcc.apellidoPaterno = pivote.PRIMER_APELLIDO;
                        capAcc.apellidoMaterno = pivote.SEGUNDO_APELLIDO;
                        capAcc.TipoLicencia = pivote.TIPOLICENCIA; ;
                        capAcc.numeroLicencia = pivote.NUM_LICENCIA;
                        capAcc.CURP = pivote.CURP;
                        resultado.Add(capAcc);
                    }



                  //  return Json(pEncontradas);
                    return Json(new { encontrada = true, Data = resultado, tipo = "success", message = "busqueda exitosa" });

                }
            }
            catch (Exception ex)
            {
                // En caso de errores, devolver una respuesta JSON con licencia no encontrada
                return Json(new { encontrada = false, Data = "", message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }

            
            //                

            return Json(new { encontrada = false, Data = "1", tipo = "sin datos", message = "busca en licencias" });


        }
        public async Task<IActionResult> CrearvehiculoSinPlaca()
        {
            try
            {
                //var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);




                var jsonPartialVehiculosByWebServices = await ajax_CrearVehiculoSinPlacasVehiculo();

                return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });


            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }
        private async Task<string> ajax_CrearVehiculoSinPlacasVehiculo()
        {

            var models = new VehiculoModel();
            models.Persona = new PersonaModel();
            models.Persona.PersonaDireccion = new PersonaDireccionModel();
            models.PersonasFisicas = new List<PersonaModel>();
            models.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            models.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            models.placas = "";
            models.serie = "";
            models.RepuveRobo = new RepuveRoboModel();
            var result = await this.RenderViewAsync2("CapturaAccidentes", models);
            return result;
        }

    }
}

