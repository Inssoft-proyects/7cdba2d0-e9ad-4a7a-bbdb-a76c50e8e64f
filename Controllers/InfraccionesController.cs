using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Utils;
using GuanajuatoAdminUsuarios.Framework;
using System.Linq;
using System;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.Extensions.Options;
using Kendo.Mvc.Extensions;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using System.Net.Http.Json;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Http.HttpResults;
using Kendo.Mvc.UI;
using Org.BouncyCastle.Crypto;
using Microsoft.AspNetCore.Authorization;
using GuanajuatoAdminUsuarios.Services.CustomReportsService;
using Org.BouncyCastle.Asn1.X509.SigI;
using System.Data.SqlClient;
using static System.Formats.Asn1.AsnWriter;
using iTextSharp.tool.xml.html;
using Kendo.Mvc;
using static GuanajuatoAdminUsuarios.Controllers.PDFExampleController;
using Microsoft.Extensions.Configuration;
using GuanajuatoAdminUsuarios.Util;
using System.Globalization;
using GuanajuatoAdminUsuarios.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;
using static iTextSharp.tool.xml.html.HTML;
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class InfraccionesController : BaseController
    {
        private readonly IEstatusInfraccionService _estatusInfraccionService;
        private readonly ITipoCortesiaService _tipoCortesiaService;
        private readonly IDependencias _dependeciaService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IGarantiasService _garantiasService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICrearMultasTransitoClientService _crearMultasTransitoClientService;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly IRepuveService _repuveService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly string _rutaArchivo;

        private readonly AppSettings _appSettings;
        private string resultValue = string.Empty;
        public static bool findValue { get; set; } = true;
        public static InfraccionesBusquedaModel infraModel;

        public InfraccionesController(
            IEstatusInfraccionService estatusInfraccionService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService,
            ITipoCortesiaService tipoCortesiaService, IDependencias dependeciaService, IGarantiasService garantiasService,
            IInfraccionesService infraccionesService,
            ICatDictionary catDictionary,
            IVehiculosService vehiculosService,
            IPersonasService personasService,
            IHttpClientFactory httpClientFactory,
            ICrearMultasTransitoClientService crearMultasTransitoClientService,
             IOptions<AppSettings> appSettings,
            ICapturaAccidentesService capturaAccidentesService,
            ICotejarDocumentosClientService cotejarDocumentosClientService, ICatMunicipiosService catMunicipiosService, ICatEntidadesService catEntidadesService,
           IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService
            , IRepuveService repuveService, ICatCarreterasService catCarreterasService, IBitacoraService bitacoraService, IConfiguration configuration)
        {
            _catDictionary = catDictionary;
            _estatusInfraccionService = estatusInfraccionService;
            _tipoCortesiaService = tipoCortesiaService;
            _dependeciaService = dependeciaService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _garantiasService = garantiasService;
            _infraccionesService = infraccionesService;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
            _capturaAccidentesService = capturaAccidentesService;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            // Configurar el cliente HTTP con la URL base del servicio
            _catCarreterasService = catCarreterasService;
            _crearMultasTransitoClientService = crearMultasTransitoClientService;
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
            _catMunicipiosService = catMunicipiosService;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivoInventarioInfracciones");
        }

        public IActionResult Index()
        {

            infraModel = new InfraccionesBusquedaModel();
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var x = User.FindFirst(CustomClaims.IdUsuario).Value;

            InfraccionesBusquedaModel searchModel = new InfraccionesBusquedaModel();
            List<InfraccionesModel> listInfracciones = new List<InfraccionesModel>();
            //_infraccionesService.GetAllInfracciones(idOficina);
            searchModel.ListInfracciones = listInfracciones;



            return View(searchModel);
        }





        //[HttpGet]
        //public FileResult CreatePdf(string data)
        //{
        //    var model = JsonConvert.DeserializeObject<InfraccionesBusquedaModel>(data,
        //       new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

        //    model.folioInfraccion = model.folioInfraccion == string.Empty ? null : model.folioInfraccion;
        //    model.placas = model.placas == string.Empty ? null : model.placas;
        //    model.NumeroEconomico = model.NumeroEconomico == string.Empty ? null : model.NumeroEconomico;
        //    model.Conductor = model.Conductor == string.Empty ? null : model.Conductor;
        //    model.Propietario = model.Propietario == string.Empty ? null : model.Propietario;
        //    model.NumeroLicencia = model.NumeroLicencia == string.Empty ? null : model.NumeroLicencia;

        //    Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
        //    {
        //    {"folioInfraccion","Folio"},
        //    {"NombreConductor","Conductor"},
        //    {"NombrePropietario","Propietario"},
        //    {"fechaInfraccion","Fecha Aplicada a"},
        //    {"NombreGarantia","Garantía"},
        //    {"delegacion","Delegación/Oficina"}
        //    };
        //    int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
        //    var ListTransitoModel = _infraccionesService.GetAllInfracciones(model, idOficina);
        //    var result = _pdfService.CreatePdf("ReporteInfracciones", "Infracciones", 6, ColumnsNames, ListTransitoModel);
        //    return File(result.Item1, "application/pdf", result.Item2);
        //}

        [HttpGet]
        public FileResult CreatePdfUnRegistro(int IdInfraccion)
        {


            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"folioInfraccion","Folio"},
            {"NombreConductor","Conductor"},
            {"NombrePropietario","Propietario"},
            {"fechaInfraccion","Fecha Aplicada a"},
            {"NombreGarantia","Garantía"},
            {"delegacion","Delegación/Oficina"}
            };
            var InfraccionModel = _infraccionesService.GetInfraccionReportById(IdInfraccion, idDependencia);
            var uma = _infraccionesService.getUMAValue();
            InfraccionModel.Uma = uma;
            var report = new InfraccionReportService("Infracción", "INFRACCIÓN").CreatePdf(InfraccionModel);
            return File(report.File.ToArray(), "application/pdf", report.FileName);
        }

        [HttpGet]
        public ActionResult Detail(int IdInfraccion)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Update(int IdInfraccion)
        {
            return View();
        }

        public JsonResult Estatus_Read()
        {
            var result = new SelectList(_estatusInfraccionService.GetEstatusInfracciones(), "idEstatusInfraccion", "estatusInfraccion");
            return Json(result);
        }

        public JsonResult Municipios_Read()
        {
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        public JsonResult Municipios_Por_Delegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var tt = _catMunicipiosService.GetMunicipiosPorDelegacion2(idOficina);

			tt.Add(new CatMunicipiosModel() { IdMunicipio = 1, Municipio = "No aplica" });
			tt.Add(new CatMunicipiosModel() { IdMunicipio = 2, Municipio = "No especificado" });


			var result = new SelectList(tt, "IdMunicipio", "Municipio");

			



			return Json(result);
        }
        public JsonResult Municipios_Por_Entidad(int entidadDDlValue)
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult CarreterasPorDelegacion()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "idCarretera", "carretera");
            return Json(result);
        }
        public IActionResult ajax_OmitirConductor()
        {
            var personamodel = new PersonaModel();
            personamodel.nombre = "SE Ignora";
            personamodel.idCatTipoPersona = 1;
            personamodel.PersonaDireccion=new PersonaDireccionModel();

            var result = _personasService.CreatePersona(personamodel);

            return Json(result);
        }

		public JsonResult Cortesias_Read()
        {
            //catTipoCortesia
            var result = new SelectList(_tipoCortesiaService.GetTipoCortesias(), "idTipoCortesia", "tipoCortesia");
            return Json(result);
        }

        public JsonResult Dependencias_Read()
        {
            var result = new SelectList(_dependeciaService.GetDependencias(), "IdDependencia", "NombreDependencia");
            return Json(result);
        }

        public JsonResult Garantias_Read()
        {
            //catGarantias
            var result = new SelectList(_garantiasService.GetGarantias(), "idGarantia", "garantia");
            return Json(result);
        }

        public JsonResult Delegaciones_Read()
        {

            var result2 = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos();
            result2 = result2.Where(s => !s.Delegacion.Contains("Centro de Gobierno")).ToList();


			var result = new SelectList(result2, "IdDelegacion", "Delegacion");

            

            return Json(result);
        }

        public async Task<ActionResult> Crear()
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ViewBag.CatCarreteras = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion2(idOficina), "IdCarretera", "Carretera");
            ViewBag.EditarVehiculo = false;
            ViewBag.Regreso= 1;
                       
            var q = View(new InfraccionesModel());
            return q;
        }


        public ActionResult GetAllVehiculosPagination([DataSourceRequest] DataSourceRequest request)
        {
            filterValue(request.Filters);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize!=0) ? request.PageSize :  10;
            pagination.Filter = resultValue;

            var vehiculosList = _vehiculosService.GetAllVehiculosPagination(pagination);
            var total = 0;
            if (vehiculosList.Count() > 0)
                total = vehiculosList.ToList().FirstOrDefault().total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = vehiculosList,
                Total = total
            };

            return Json(result);
        }

        public ActionResult GetAllPersonasPagination([DataSourceRequest] DataSourceRequest request)
        {
            filterValue(request.Filters);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            pagination.Filter = resultValue;

            var personasList = _personasService.GetAllPersonasPagination(pagination);
            var total = 0;
            if (personasList.Count() > 0)
                total = personasList.ToList().FirstOrDefault().total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = personasList,
                Total = total
            };

            return Json(result);
        }

        public IActionResult GetBuscarInfraccionesNormal([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaModel model)
        {
            infraModel = model;
            return PartialView("_ListadoInfracciones", new List<InfraccionesModel>());
        }

        public IActionResult GetAllBuscarInfraccionesPagination([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaModel model)

        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(461))
            {

                Pagination pagination = new Pagination();
                pagination.PageIndex = request.Page - 1;
                pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
                pagination.Filter = resultValue;

                if (infraModel == null)
                    infraModel = model;

                var listReporteAsignacion = _infraccionesService.GetAllInfraccionesPagination(infraModel, idOficina, idDependencia, pagination);
                var total = 0;
                if (listReporteAsignacion.Count() > 0)
                    total = listReporteAsignacion.ToList().FirstOrDefault().Total;

                request.PageSize = pagination.PageSize;
                var result = new DataSourceResult()
                {
                    Data = listReporteAsignacion,
                    Total = total
                };
                return Json(result);
            }
            else
            {
                var result = new DataSourceResult()
                {
                    Data = new List<InfraccionesModel>(),
                    Total = 0
                };
                return Json(result);
            }
        }


        private void filterValue(IEnumerable<IFilterDescriptor> filters)
        {
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




        public ActionResult Editar(int idInfraccion, int id, string regreso, bool? showE = false)
        {


            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            int ids = id != 0 ? id : idInfraccion;

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccion2ById(ids, idDependencia);
            model.isPropietarioConductor = model.Vehiculo.idPersona == model.idPersonaInfraccion;
            model.Vehiculo.cargaTexto = (model.Vehiculo.carga == true) ? "Si" : "No";
            model.Persona = model.Persona ?? new PersonaModel();


            var ToDepositos = _infraccionesService.ExitDesposito(id);

            HttpContext.Session.SetInt32("LastInfCapturada", id);

            ViewBag.GoDEpositos = ToDepositos;
            
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ViewBag.CatCarreteras = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");

            model.Persona.PersonaDireccion = model.Persona.PersonaDireccion ?? new PersonaDireccionModel();
            var catTramos = _catDictionary.GetCatalog("CatTramosByFilter", model.idCarretera.ToString());



			var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");
            var CatAplicadoA = _catDictionary.GetCatalog("CatAplicadoA", "0");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
            ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "Id", "Text");
            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");
            ViewBag.CatAplicadoA = new SelectList(CatAplicadoA.CatalogList, "Id", "Text");
            ViewBag.EsSoloLectura = showE.HasValue && showE.Value;
            ViewBag.EditarVehiculo = true;
			ViewBag.Regreso = regreso==null ? "0":regreso;

			if ((model.MotivosInfraccion == null || model.MotivosInfraccion.Count() == 0) || (model.idGarantia == null || model.idGarantia == 0))
            {
                HttpContext.Session.SetString("isedition", "0");
            }
            else
            {
                HttpContext.Session.SetString("isedition", "1");
            }

            return View(model);

        }

        public ActionResult EditarA(int idInfraccion, int id)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            int ids = id != 0 ? id : idInfraccion;

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccionAccidenteById(id, idDependencia);
            model.isPropietarioConductor = model.Vehiculo.idPersona == model.IdPersona;


            var ToDepositos = _infraccionesService.ExitDesposito(idInfraccion);

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ViewBag.CatCarreteras = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");


            var catTramos = _catDictionary.GetCatalog("CatTramosByFilter", model.IdCarretera.ToString());
            var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
            ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "Id", "Text");
            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");

            return View("Editar2", model);
        }


        [HttpPost]
        public ActionResult ajax_editarInfraccion(InfraccionesModel model)
        {

            var isedition = HttpContext.Session.GetString("isedition");


            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            int idGarantia = 0;
            int idInf = model.idInfraccion;
            if (model.idGarantia == null || model.idGarantia == 0)
            {
                model.Garantia.numPlaca = model.Vehiculo.placas;
                model.Garantia.numLicencia = model.PersonaInfraccion2?.numeroLicencia?? model.Garantia.numLicencia;
                model.Garantia.idTipoLicencia = model.PersonaInfraccion2?.idTipoLicencia?? model.Garantia.idTipoLicencia;
                idGarantia = _infraccionesService.CrearGarantiaInfraccion(model.Garantia, idInf);
                model.idGarantia = idGarantia;
            }
            else
            {
                model.Garantia.idGarantia = model.idGarantia;
                var result = _infraccionesService.ModificarGarantiaInfraccion(model.Garantia, idInf);
            }


            model.idDelegacion = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var idInfraccion = _infraccionesService.ModificarInfraccion(model);

            if (isedition == "0")
            {
                _bitacoraServices.insertBitacora(model.idInfraccion, ip, "EditarInfraccion", "Crear2", "create 2 infraccion", user);

            }
            else
            {
                _bitacoraServices.insertBitacora(model.idInfraccion, ip, "EditarInfraccion", "Editar", "insert Garantia", user);

            }

            var idVehiculo = model.idVehiculo;
            return Json(new { success = true, idInfraccion = idInfraccion, idVehiculo = idVehiculo });
        }

        [HttpPost]
        public ActionResult ajax_crearInfraccion(InfraccionesModel model, CrearMultasTransitoRequestModel requestMode)
        {
            
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
           

            bool validarFolio = _infraccionesService.ValidarFolio(model.folioInfraccion, idDependencia);

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);



            if (!validarFolio)
            {
                // model.idPersonaInfraccion = idPersonaInfraccion;
                model.idEstatusInfraccion = (int)CatEnumerator.catEstatusInfraccion.EnProceso;
                model.idDelegacion = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                model.fechaVencimiento = getFechaVencimiento(model.fechaInfraccion, idDependencia);
                //    model.fechaVencimiento = getFechaVencimiento(model.fechaInfraccion);



                var idInfraccion = _infraccionesService.CrearInfraccion(model, idDependencia);
                var idPersonaInfraccion = _infraccionesService.CrearPersonaInfraccion((int)idInfraccion, (int)model.idPersona);

                _bitacoraServices.insertBitacora(idInfraccion, ip, "crearInfraccion", "CREAR1", "insert", user);

                return Json(new { id = idInfraccion });
            }
            else
            {
                return Json(new { id = 0, validacion = validarFolio });
            }

        }

        [HttpPost]
        public ActionResult ajax_ValidarFolio(InfraccionesModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            bool idInfraccion = _infraccionesService.ValidarFolio(model.folioInfraccion, idDependencia);

            return Json(new { id = idInfraccion });

        }
        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
        }

        [HttpPost]
        public IActionResult ajax_BuscarVehiculo2(VehiculoBusquedaModel model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - Request - " + request);
                RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel()
                {
                    placa = model.PlacasBusqueda,
                    niv = model.SerieBusqueda
                };
                Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - ConsultaRobo");
                var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel)?.FirstOrDefault() ?? new RepuveRoboModel();
                if (repuveConsRoboResponse!=null)
                    Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - ConsultaRobo - Response - " + JsonConvert.SerializeObject(repuveConsRoboResponse));

                ViewBag.ReporteRobo = repuveConsRoboResponse.EsRobado;
                if (_appSettings.AllowWebServices)
                {
                    Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - GetVehiculoToAnexo");
                    var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
                    if (repuveConsRoboResponse != null)
                        Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - GetVehiculoToAnexo - Response - " + JsonConvert.SerializeObject(repuveConsRoboResponse));

                    vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
                    vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                    vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();

                    if (vehiculosModel.encontradoEn == 3 && !string.IsNullOrEmpty(model.PlacasBusqueda))
                    {
                        try
                        {
                            CotejarDatosRequestModel cotejarDatosRequestModel = new CotejarDatosRequestModel();
                            cotejarDatosRequestModel.Tp_folio = "4";
                            cotejarDatosRequestModel.Folio = model.PlacasBusqueda;
                            cotejarDatosRequestModel.tp_consulta = "3";

                            var endPointName = "CotejarDatosEndPoint";
                            Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - CotejarDatos");
                            var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);
                            if (result!=null)
                                Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - CotejarDatos - Response - " + JsonConvert.SerializeObject(result));

                            if (result.MT_CotejarDatos_res != null && result.MT_CotejarDatos_res.Es_mensaje != null && result.MT_CotejarDatos_res.Es_mensaje.TpMens.ToString().Equals("I", StringComparison.OrdinalIgnoreCase))
                            {
                                var vehiculoEncontradoData = result.MT_CotejarDatos_res.tb_vehiculo[0];
                                var vehiculoDireccionData = result.MT_CotejarDatos_res.tb_direccion[0];
                                var vehiculoInterlocutorData = result.MT_CotejarDatos_res;
                                var idMunicipio = !string.IsNullOrEmpty(vehiculoDireccionData.municipio)
                                      ? ObtenerIdMunicipioDesdeBD(vehiculoDireccionData.municipio)
                                      : 0;

                                var idEntidad = !string.IsNullOrEmpty(vehiculoDireccionData.entidadreg)
                                    ? ObtenerIdEntidadDesdeBD(vehiculoDireccionData.entidadreg)
                                    : 0;

                                var idColor = !string.IsNullOrEmpty(vehiculoEncontradoData.color)
                                    ? ObtenerIdColor(vehiculoEncontradoData.color)
                                    : 0;

                                var idMarca = !string.IsNullOrEmpty(vehiculoEncontradoData.marca)
                                    ? ObtenerIdMarca(vehiculoEncontradoData.marca)
                                    : 0;

                                var idSubmarca = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                                    ? ObtenerIdSubmarca(vehiculoEncontradoData.linea)
                                    : 0;
                                var submarcaLimpio = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                                    ? ObtenerSubmarca(vehiculoEncontradoData.linea)
                                    : "NA";
                                var telefonoValido = !string.IsNullOrEmpty(vehiculoDireccionData.telefono)
                                    ? LimpiarValorTelefono(vehiculoDireccionData.telefono)
                                    : 0;
                                var cargaBool = ConvertirBool(vehiculoEncontradoData.carga);
                                var generoBool = ConvertirGeneroBool(vehiculoInterlocutorData.es_per_fisica?.sexo);

                                var idTipo = !string.IsNullOrEmpty(vehiculoEncontradoData.categoria)
                                 ? ObtenerIdTipoVehiculo(vehiculoEncontradoData.categoria)
                                 : 0;

                                var vehiculoEncontrado = new VehiculoModel
                                {
                                    placas = vehiculoEncontradoData.no_placa,
                                    serie = vehiculoEncontradoData.no_serie,
                                    tarjeta = vehiculoEncontradoData.no_tarjeta,
                                    motor = vehiculoEncontradoData.no_motor,
                                    otros = vehiculoEncontradoData.otros,
                                    idColor = idColor,
                                    idEntidad = idEntidad,
                                    idMarcaVehiculo = idMarca,
                                    idSubmarca = idSubmarca,
                                    submarca = submarcaLimpio,
                                    idTipoVehiculo = idTipo,
                                    modelo = vehiculoEncontradoData.modelo,
                                    capacidad = vehiculoEncontradoData.numpersona,
                                    carga = cargaBool,

                                    Persona = new PersonaModel
                                    {
                                        RFC = vehiculoInterlocutorData.Nro_rfc,
                                        RFCFisico = vehiculoInterlocutorData.Nro_rfc,
                                        CURPFisico = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                                        nombreFisico = vehiculoInterlocutorData.es_per_fisica?.Nombre,
                                        apellidoPaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                                        apellidoMaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                                        fechaNacimiento = vehiculoInterlocutorData.es_per_fisica?.Fecha_nacimiento,
                                        generoBool = generoBool,

                                        nombre = vehiculoInterlocutorData.es_per_moral?.name_org1,
                                        PersonaDireccion = new PersonaDireccionModel
                                        {
                                            telefonoFisico = telefonoValido.ToString(),
                                            telefono = telefonoValido.ToString(),
                                            colonia = vehiculoDireccionData.colonia,
                                            coloniaFisico = vehiculoDireccionData.colonia,
                                            calleFisico = vehiculoDireccionData.calle,
                                            calle = vehiculoDireccionData.calle,
                                            numero = vehiculoDireccionData.nro_exterior,
                                            numeroFisico = vehiculoDireccionData.nro_exterior,
                                            idMunicipioFisico = idMunicipio,
                                            idMunicipio = idMunicipio,
                                        }
                                    },

                                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                                    {
                                        PersonasMorales = new List<PersonaModel>()
                                    }
                                };
                                return PartialView("_Create", vehiculoEncontrado);
                            }
                            else if (result.MT_CotejarDatos_res != null && result.MT_CotejarDatos_res.Es_mensaje != null && result.MT_CotejarDatos_res.Es_mensaje.TpMens.ToString().Equals("E", StringComparison.OrdinalIgnoreCase))
                            {
                                Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - ConsultaGeneral - REPUVE");
                                var repuveConsGralResponse = _repuveService.ConsultaGeneral(repuveGralModel).FirstOrDefault();
                                if (repuveConsGralResponse != null)
                                    Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - ConsultaGeneral - REPUVE - Response - " + JsonConvert.SerializeObject(repuveConsGralResponse));

                                var vehiculoEncontrado = new VehiculoModel
                                {
                                    placas = repuveConsGralResponse.placa,
                                    serie = repuveConsGralResponse.niv_padron,
                                    //tarjeta = repuveConsGralResponse.ta,
                                    motor = repuveConsGralResponse.motor,
                                    //otros = repuveConsGralResponse.
                                    color = repuveConsGralResponse.color,
                                    //idEntidad = idEntidad,
                                    //idMarcaVehiculo = idMarca,
                                    //idSubmarca = idSubmarca,
                                    submarca = repuveConsGralResponse.submarca,
                                    //idTipoVehiculo = idTipo,
                                    modelo = repuveConsGralResponse.modelo,
                                    //capacidad = repuveConsGralResponse.c,
                                    //carga = repuveConsGralResponse.ca,

                                    Persona = new PersonaModel(),

                                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                                };
                                return PartialView("_Create", vehiculoEncontrado);

                            }
                        }

                        catch (Exception ex)
                        {

                            return Json(new { success = false, message = "Ha ocurrido un error al comunicarse con el servicio web." });
                        }
                    }
                    return PartialView("_Create", vehiculosModel);

                }
                else
                {
                    Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - GetVehiculoToAnexo");
                    var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
                    Logger.Debug("Infracciones - ajax_BuscarVehiculo2 - GetVehiculoToAnexo - Response - " + JsonConvert.SerializeObject(vehiculosModel));

                    vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
                    vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                    vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
                    return PartialView("_Create", vehiculosModel);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Infracciones - ajax_BuscarVehiculo2 :" + ex);
                return null;
            }
        }


        private bool ValidarRobo(RepuveConsgralRequestModel repuveGralModel)
        {
            var estatus = false;

            var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel)?.FirstOrDefault() ?? new RepuveRoboModel();

            estatus = repuveConsRoboResponse.EsRobado;

            return estatus;
        }


        public VehiculoModel GetVEiculoModelFromFinanzas(RootCotejarDatosRes result)
        {
            var vehiculoEncontradoData = result.MT_CotejarDatos_res.tb_vehiculo[0];
            var vehiculoDireccionData = result.MT_CotejarDatos_res.tb_direccion[0];
            var vehiculoInterlocutorData = result.MT_CotejarDatos_res;
            var idMunicipio = !string.IsNullOrEmpty(vehiculoDireccionData.municipio)
                  ? ObtenerIdMunicipioDesdeBD(vehiculoDireccionData.municipio)
                  : 0;
     
            var idEntidad = !string.IsNullOrEmpty(vehiculoDireccionData.entidadreg)
                ? ObtenerIdEntidadDesdeBD(vehiculoDireccionData.entidadreg)
                : 0;
           

            var idColor = !string.IsNullOrEmpty(vehiculoEncontradoData.color)
                ? ObtenerIdColor(vehiculoEncontradoData.color)
                : 0;

            var idMarca = !string.IsNullOrEmpty(vehiculoEncontradoData.marca)
                ? ObtenerIdMarca(vehiculoEncontradoData.marca)
                : 0;

            var idSubmarca = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerIdSubmarca(vehiculoEncontradoData.linea)
                : 0;
            var submarcaLimpio = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerSubmarca(vehiculoEncontradoData.linea)
                : "NA";
            var telefonoValido = !string.IsNullOrEmpty(vehiculoDireccionData.telefono)
                ? LimpiarValorTelefono(vehiculoDireccionData.telefono)
                : 0;
            var cargaBool = ConvertirBool(vehiculoEncontradoData.carga);
            var generoBool = ConvertirGeneroBool(vehiculoInterlocutorData.es_per_fisica?.sexo);

            var idTipo = !string.IsNullOrEmpty(vehiculoEncontradoData.categoria)
             ? ObtenerIdTipoVehiculo(vehiculoEncontradoData.categoria)
             : 0;
            var idTipoServicio = !string.IsNullOrEmpty(vehiculoEncontradoData.servicio)
            ? ObtenerIdTipoServicio(vehiculoEncontradoData.servicio)
            : 0;
            var vehiculoEncontrado = new VehiculoModel
            {
                placas = vehiculoEncontradoData.no_placa,
                serie = vehiculoEncontradoData.no_serie,
                tarjeta = vehiculoEncontradoData.no_tarjeta,
                motor = vehiculoEncontradoData.no_motor,
                otros = vehiculoEncontradoData.otros,
                idColor = idColor,
                idEntidad = idEntidad,
                idMarcaVehiculo = idMarca,
                idSubmarca = idSubmarca,
                submarca = submarcaLimpio,
                idTipoVehiculo = idTipo,
                modelo = vehiculoEncontradoData.modelo,
                capacidad = vehiculoEncontradoData.numpersona,
                carga = cargaBool,
                idCatTipoServicio = idTipoServicio,
                idTipoPersona = vehiculoInterlocutorData.es_per_fisica != null ? 1 : 2,

                Persona = new PersonaModel
                {
                    nombreFisico = vehiculoInterlocutorData.es_per_fisica?.Nombre,
                    apellidoPaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                    apellidoMaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                    fechaNacimiento = vehiculoInterlocutorData.es_per_fisica?.Fecha_nacimiento,
                    CURPFisico = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                    generoBool = generoBool,
                    nombre = vehiculoInterlocutorData.es_per_moral?.name_org1,
                    RFC = vehiculoInterlocutorData.Nro_rfc,


                    PersonaDireccion = new PersonaDireccionModel
                    {
                       
                            telefono = vehiculoInterlocutorData.es_per_moral != null ? telefonoValido.ToString() : null,
                            telefonoFisico = vehiculoInterlocutorData.es_per_fisica != null ? telefonoValido.ToString() : null,
                            colonia = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.colonia : null,
                            coloniaFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.colonia : null,
                            calle = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.calle : null,
                            calleFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.calle : null,
                            numero = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.nro_exterior : null,
                            numeroFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.nro_exterior : null,
                            idMunicipio = vehiculoInterlocutorData.es_per_moral != null ? idMunicipio : null,
                            idMunicipioFisico = vehiculoInterlocutorData.es_per_fisica != null ? idMunicipio : null,
                            idEntidad = vehiculoInterlocutorData.es_per_moral != null ? idEntidad : null,
                            idEntidadFisico = vehiculoInterlocutorData.es_per_fisica != null ? idEntidad : null,
                    }
                },

                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                    {
                        PersonasMorales = new List<PersonaModel>()
                    }
                };

            return vehiculoEncontrado;

        }
        private int ObtenerIdMarcaRepuve(string marca)
        {
           
                string marcaLimpio = marca.Trim();

                var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio);
                return idMarca;
            

        }
        private int ObtenerIdSubmarcaRepuve(string submarca)
        {
            
                string submarcaLimpio = submarca.Trim();

                var idMarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio);
                return idMarca;
            

        }
        private int ObtenerIdTipoServicioRepuve(string servicio)
        {
            int TipoServicio = 0;

            var Tipo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            TipoServicio = Tipo.CatalogList.Where(w => servicio.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return (int)TipoServicio;
        }
        private int ObtenerIdEntidadRepuve(string entidad)
        {
            int idEntidad = 0;
            var Entidad = _catDictionary.GetCatalog("CatEntidades", "0");
            idEntidad = Entidad.CatalogList
                .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(entidad.ToLower())))
                .Select(s => s.Id)
                .FirstOrDefault();
            return (idEntidad);
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
            models.placas = "XXXXOXO";
            models.serie = "XXXXOXOhf5321";
            models.RepuveRobo = new RepuveRoboModel();
            var result = await this.RenderViewAsync2("", models);
            return result;
        }



        public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                //var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);
                var SeleccionVehiculo = _vehiculosService.BuscarPorParametro(model.PlacasBusqueda??"", model.SerieBusqueda??"", model.FolioBusqueda);

                if (SeleccionVehiculo > 0)
                {
                    var text = "";
                    var value = "";

                    if(!string.IsNullOrEmpty(model.SerieBusqueda))
                    {
                         text = "serie";
                         value = model.SerieBusqueda;

                    }
                    else if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                    {
                        text = "placas";
                        value = model.PlacasBusqueda;
                    }



                    return Json(new { noResults = false, data = value , field=text });
                }
                else
                {
                    var jsonPartialVehiculosByWebServices = await ajax_BuscarVehiculo(model);

                    if (jsonPartialVehiculosByWebServices != null)
                    {
                        return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                    }
                    else
                    {
                        return Json(new { noResults = true, data = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }

        [HttpPost]
        public async  Task<string> ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                if(!string.IsNullOrEmpty(model.PlacasBusqueda)){
                    model.PlacasBusqueda = model.PlacasBusqueda.ToUpper();
                }
                if (!string.IsNullOrEmpty(model.SerieBusqueda)){
                    model.SerieBusqueda = model.SerieBusqueda.ToUpper();
                }


                var models = _vehiculosService.GetModles(model);

                var test = await this.RenderViewAsync2("", models);


                return test;
            }
            catch (Exception ex)
            {
                Logger.Error("Infracciones - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
        }







        private int ObtenerIdMunicipioDesdeBD(string municipio)
        {
            int idMunicipio = 0;

            var municipioStr = _catDictionary.GetCatalog("CatMunicipios", "0");

            idMunicipio = municipioStr.CatalogList
                            .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(municipio.ToLower())))
                            .Select(s => s.Id)
                            .FirstOrDefault();
            return (idMunicipio);
        }

        
        private int ObtenerIdEntidadDesdeBD(string entidad)
        {
            var idEntidad = _catEntidadesService.obtenerIdPorEntidad(entidad);
            return (idEntidad);
        }
        private string ObtenerSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                return submarcaLimpio;
            }

            return "NA"; // Valor predeterminado en caso de no encontrar el guión
        }
        private bool ConvertirBool(string carga)
        {
            bool cargaBool = false;

            if (carga.Trim() == "1.00")
            {
                cargaBool = true;
            }
            else if (carga.Trim() == "0.00")
            {
                cargaBool = false;
            }
            return (cargaBool);
        }


        private int ObtenerIdColor(string color)
        {
            string colorLimpio = Regex.Replace(color, "[0-9-]", "").Trim();
            var idColor = _coloresService.obtenerIdPorColor(colorLimpio);
            return (idColor);
        }
        private int ObtenerIdMarca(string marca)
        {
            string[] partes = marca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string marcaLimpio = partes[1].Trim();

                var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }





        private int ObtenerIdSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                var idMarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }
        private int ObtenerIdTipoVehiculo(string categoria)
        {

            int idTipo = 0;

            var tipoVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");

            idTipo = tipoVehiculo.CatalogList.Where(w => categoria.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return (idTipo);

        }
        private int ObtenerIdTipoServicio(string servicio)
        {
            int servicioNumero = int.Parse(servicio.TrimStart('0'));
            var idTipoVehiculo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            var tipoServicio = idTipoVehiculo.CatalogList.FirstOrDefault(item => item.Id == servicioNumero)?.Id;

            return (int)tipoServicio; 
        }



        private bool ConvertirGeneroBool(string sexo)
        {
            if (sexo == "2")
            {
                return true;
            }
            else if (sexo == "1")
            {
                return false;
            }
            else
            {
                return false;
            }

        }

        private long LimpiarValorTelefono(string telefono)
        {
            telefono = telefono.Replace(" ", "");

            long telefonoValido;

            if (long.TryParse(telefono, out telefonoValido))
            {
                return telefonoValido;
            }
            else
            {
                return 0; // O algún otro valor que indique que no es válido
            }
        }
        /*      private async Task<VehiculoModel> BusquedaRepuveAsync(VehiculoBusquedaModel model)
              {
                  try
                  {
                      // Crear una instancia de HttpClient para el segundo servicio y configurar la BaseAddress
                      using (var httpClientRepuve = _httpClientFactory.CreateClient())
                      {
                          httpClientRepuve.BaseAddress = new Uri("http://10.16.60.71/");

                          var parametros = new
                          {
                              token = "abl185B",
                              placa = model.PlacasBusqueda,
                              niv = "",
                          };

                          // Realizar la solicitud POST al servicio
                          var response = await httpClientRepuve.PostAsJsonAsync("", parametros);
                          if (response.IsSuccessStatusCode)
                          {
                              var result = await response.Content.ReadFromJsonAsync<VehiculoModel>(); // Lee el contenido JSON y deserializa
                              return result;
                          }
                          else
                          {
                              return null;
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      // Manejar excepciones aquí
                      return null;
                  }
              }*/





        [HttpGet]
        public ActionResult ajax_ModalCrearMotivo()
        {
            var catConcepto = _catDictionary.GetCatalog("CatConceptoInfraccion", "0");
            ViewData["CatConcepto"] = new SelectList(catConcepto.CatalogList, "Id", "Text");
            //ViewBag.CatConcepto = catConcepto;
            return PartialView("_CrearMotivo", new MotivoInfraccionModel());
        }

        [HttpPost]
        public ActionResult ajax_CrearMotivos(MotivoInfraccionModel model)
        {
            var id = _infraccionesService.CrearMotivoInfraccion(model);
            var modelList = _infraccionesService.GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
            var fecha = _infraccionesService.GetDateInfraccion(model.idInfraccion);

			var umas = _infraccionesService.GetUmas(fecha);
            ViewBag.Umas = umas;
            ViewBag.Totales = modelList.Sum(s => s.calificacion) * umas;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            var isedition = HttpContext.Session.GetString("isedition");
            if (isedition == "1")
            {
                _bitacoraServices.insertBitacora(model.idInfraccion, ip, "EditarInfraccion", "Editar2", "insert", user);

            }
            return PartialView("_ListadoMotivos", modelList);
        }

        [HttpGet]
        public ActionResult ajax_detalleVehiculo(int idVehiculo)
        {
            var model = _vehiculosService.GetVehiculoById(idVehiculo);
            model.cargaTexto = (model.carga == true) ? "Si" : "No";
            return PartialView("_DetalleVehiculo", model);
        }

        [HttpGet]
        public ActionResult ajax_detallePersona(int idPersona)
        {
            var model = _personasService.GetPersonaById(idPersona);
            return PartialView("_DetallePersona", model);
        }

        [HttpGet]
        public ActionResult ajax_listadoVehiculoInfracciones()
        {
            var modelList = _vehiculosService.GetAllVehiculos();
            return PartialView("_ListadoVehiculos", modelList);
        }

        [HttpGet]
        public ActionResult ajax_listadoPersonasInfracciones()
        {
            var modelList = _personasService.GetAllPersonas();
            return PartialView("_ListadoPersonas", modelList);
        }

        [HttpGet]
        public ActionResult ajax_listadoMotivosInfracciones(int idInfraccion)
        {
            var modelList = _infraccionesService.GetMotivosInfraccionByIdInfraccion(idInfraccion);
            return PartialView("_ListadoMotivos", modelList);
        }

        public JsonResult InfraccionesEstatus_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEstatusInfraccion", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        [HttpGet]
        public ActionResult ajax_CortesiaInfraccion(int id)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            //var model = _vehiculosService.GetVehiculoById(id);
            var model = _infraccionesService.GetInfraccion2ById(id, idDependencia);
            return PartialView("_Cortesia", model);
        }

        [HttpPost]
        public ActionResult ajax_UpdateCortesiaInfraccion(InfraccionesModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var modelInf = _infraccionesService.ModificarInfraccionPorCortesia(model);
            if (modelInf == 1)
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listInfracciones = _infraccionesService.GetAllInfracciones(idOficina, idDependencia);
                return PartialView("_ListadoInfracciones", listInfracciones);
                //return Json(listInfracciones);
            }
            else
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(null);
            }

        }
        public ActionResult MostrarModalAnexar()
        {
            return PartialView("_ModalAnexar");
        }
        [HttpPost]
        public async Task<IActionResult> SubirImagen(IFormFile file, int idInfraccion)
        {
            if (file != null && file.Length > 0)
            {
                //Se crea el nombre del archivo de la garantia
                string nombreArchivo = _rutaArchivo + "/" + idInfraccion + "_" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("/", "").Replace(":", "").Replace(" ", "") + System.IO.Path.GetExtension(file.FileName);

                try
                {
                    //Se escribe el archivo en disco
                    using (Stream fileStream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    // Llamar al método del servicio para guardar la imagen
                    int resultado = _infraccionesService.InsertarImagenEnInfraccion(nombreArchivo, idInfraccion);
                    if (resultado == 0)
                        return Json(new { success = false, message = "Ocurrió un error al actualizar infracción" });

                }
                catch (Exception e)
                {
                    Logger.Error("Ocurrió un error al cargar el archivo a la infracción: "+e);
                    return Json(new { success = false, message = "Ocurrió un error al guardar el archivo" });
                }




                /*using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }*/

                return Json(new { success = true, message = "El archivó se agregó exitosamente" });
            }
            else
            {
                return Json(new { success = false, message = "Selecciona una imagen antes de continuar" });
            }

        }

        public IActionResult ServiceCrearInfraccion(int idInfraccion)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            if (_appSettings.AllowWebServices)
            {
                try
                {
                    var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion, idDependencia);
                    var unicoMotivo = infraccionBusqueda.MotivosInfraccion?.FirstOrDefault();
                    int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                    if (infraccionBusqueda.cortesiaInt == 2)
                    {
                        _infraccionesService.ModificarEstatusInfraccion(idInfraccion, (int)CatEnumerator.catEstatusInfraccion.Capturada);
                        return Json(new { success = false, message = "Infracción guardada, no enviada a finanzas", id = idInfraccion });
                    }
                    else
                    {
                        CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();
                        crearMultasRequestModel.CR1RFC = infraccionBusqueda.Persona?.RFC ?? "";
                        crearMultasRequestModel.CR1APAT = infraccionBusqueda.Persona?.apellidoPaterno ?? "";
                        crearMultasRequestModel.CR1AMAT = infraccionBusqueda.Persona?.apellidoMaterno ?? "";
                        crearMultasRequestModel.CR1NAME = infraccionBusqueda.Persona?.nombre ?? "";
                        crearMultasRequestModel.CR2NAME = "";
                        crearMultasRequestModel.CR1RAZON = "";
                        crearMultasRequestModel.CR2RAZON = "";
                        crearMultasRequestModel.CR3RAZON = "";
                        crearMultasRequestModel.CR4RAZON = "";
                        crearMultasRequestModel.BIRTHDT = "";
                        crearMultasRequestModel.CR1CALLE = infraccionBusqueda.lugarCalle;
                        crearMultasRequestModel.CR1NEXT = infraccionBusqueda.lugarNumero;
                        crearMultasRequestModel.CR1NINT = "";
                        crearMultasRequestModel.CR1ENTRE = "";
                        crearMultasRequestModel.CR2ENTRE = "";
                        crearMultasRequestModel.CR1COLONIA = infraccionBusqueda.lugarColonia;
                        crearMultasRequestModel.CR1LOCAL = "";
                        crearMultasRequestModel.CR1MPIO = infraccionBusqueda.municipio;
                        crearMultasRequestModel.CR1CP = "00000";
                        crearMultasRequestModel.CR1TELE = "";
                        crearMultasRequestModel.CR1EDO = "GTO";
                        crearMultasRequestModel.CR1EMAIL = "";
                        crearMultasRequestModel.XSEXF = "";
                        crearMultasRequestModel.XSEXM = "";
                        crearMultasRequestModel.LZONE = "";
                        crearMultasRequestModel.L_OFN_IOFICINA = "";
                        crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString("F2");
                        crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
                        crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
                        crearMultasRequestModel.INF_PROP = "";
                        crearMultasRequestModel.NOM_INFRACTOR = infraccionBusqueda.PersonaInfraccion?.nombreCompleto ?? "";
                        crearMultasRequestModel.DOM_INFRACTOR = infraccionBusqueda.Persona?.PersonaDireccion.calle ?? "" + " " + infraccionBusqueda.Persona?.PersonaDireccion.numero ?? "" + ", " + infraccionBusqueda.Persona?.PersonaDireccion.colonia ?? "";
                        crearMultasRequestModel.NUM_PLACA = infraccionBusqueda.placasVehiculo;
                        crearMultasRequestModel.DOC_GARANTIA = "4";
                        crearMultasRequestModel.NOM_RESP_SOLI = "";
                        crearMultasRequestModel.DOM_RESP_SOLI = "";
                        if (infraccionBusqueda != null)
                        {
                            string prefijo = (idDependencia == 1) ? "TTO-" : (idDependencia == 0) ? "TTE" : "";
                            crearMultasRequestModel.FOLIO_MULTA = prefijo + infraccionBusqueda.folioInfraccion;
                        }
                        crearMultasRequestModel.OBS_GARANT = "";
                        crearMultasRequestModel.ZMOTIVO1 = unicoMotivo?.Motivo ?? "";
                        crearMultasRequestModel.ZMOTIVO2 = "";
                        crearMultasRequestModel.ZMOTIVO3 = "";
                        var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);
                        ViewBag.Pension = result;
                        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                        var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

                        if (result != null && result.MT_CrearMultasTransito_res != null && "S".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            _infraccionesService.ModificarEstatusInfraccion(idInfraccion, (int)CatEnumerator.catEstatusInfraccion.Enviada);
                            _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, idInfraccion);
                            _bitacoraServices.insertBitacora(idInfraccion, ip, "EditarInfraccion", "Registrarcd", "WS", user);
                            return Json(new { success = true });
                        }
                        else if (result != null && result.MT_CrearMultasTransito_res != null && "E".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            _bitacoraServices.insertBitacora(idInfraccion, ip, "EditarInfraccion", "Registrar", "WS", user);
                            return Json(new { success = false, message = "Registro actualizado en RIAG", id = idInfraccion });


                        }
                        else if (result != null && result.MT_CrearMultasTransito_res != null && "A".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            _bitacoraServices.insertBitacora(idInfraccion, ip, "EditarInfraccion", "Registrarer", "WS", user);
                            return Json(new { success = false, message = "Infraccion anteriormente registrada en finanzas", id = idInfraccion });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Infraccion Guardada, no enviada" });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    return Json(new { success = false, message = "Ha ocurrido un error intenta más tarde" });
                }
            }
            return Json(new { success = false, message = "Infracción guardada, no enviada a finanzas", id = idInfraccion });

        }

        public ActionResult ModalAgregarConductor()
        {
            BusquedaPersonaModel model = new BusquedaPersonaModel();
            return View("_ModalBusquedaPersonas", model);
        }


        [HttpPost]
        public ActionResult ajax_BuscarVehiculos(VehiculoBusquedaModel model)
        {
            var vehiculosModel = _vehiculosService.GetVehiculos(model);
            return PartialView("_ListVehiculos", vehiculosModel);
        }


        [HttpPost]
        public ActionResult ajax_BuscarPersonaMoral(PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = _personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }

        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscas()
        {
            var personasFisicas = _personasService.GetAllPersonas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            Persona.PersonaDireccion.telefono = System.String.IsNullOrEmpty(Persona.telefono) ? null : Persona.telefono;
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            if (IdPersonaMoral == 0)
                return Json(new { success = false, message = "Ocurrió un error al procesar su solicitud." });
            else
            {
                var modelList = _personasService.ObterPersonaPorIDList(IdPersonaMoral);
                return PartialView("_ListPersonasMorales", modelList);
            }


            //var personasMoralesModel = _personasService.GetAllPersonasMorales();

        }
        [HttpGet]
        public IActionResult ajax_ModalCrearPersona()
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
            return PartialView("_CrearPersona", new PersonaModel());
        }
        [HttpPost]
        public IActionResult ajax_CrearPersona(PersonaModel model)
        {
            if (!string.IsNullOrEmpty(model.numeroLicenciaFisico)) model.numeroLicencia = model.numeroLicenciaFisico;
            if (model.idTipoLicenciaInfraccion != null) model.idTipoLicencia = model.idTipoLicenciaInfraccion;
            if (!string.IsNullOrEmpty(model.telefonoInfraccion))
            {
                model.PersonaDireccion.telefono = model.telefonoInfraccion;
            }
            if (!string.IsNullOrEmpty(model.correoInfraccion)) model.PersonaDireccion.correo = model.correoInfraccion;
            if (model.vigenciaLicenciaFisico != null)
            {
                if (Convert.ToDateTime(model.vigenciaLicenciaFisico).Year > 2000)
                    model.vigenciaLicencia = model.vigenciaLicenciaFisico;
                else
                    model.vigenciaLicencia = null;
            }

            int id = _personasService.CreatePersona(model);

            if (id == -1)
            {
                // El registro ya existe, muestra un mensaje de error al usuario
                return Json(new { success = false, message = "El registro ya existe, revise los datos ingresados." });
            }
            else if (id == 0)
                return Json(new { success = false, message = "Ocurrió un error al procesar su solicitud." });
            else
            {
                // La inserción se realizó correctamente
                model.PersonaDireccion.idPersona = id;

                // NO APLICA YA QUE PREVIAMENTE SE HABIA INSERTADO.
                //int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);


                var modelList = _personasService.GetAllPersonas();
                ViewBag.EditarVehiculo = true;
                return PartialView("_ListadoPersonas", modelList);
            }
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
        public ActionResult ajax_CrearVehiculo3(VehiculoModel model)
        {
            var IdVehiculo = _vehiculosService.CreateVehiculo(model);

            if (IdVehiculo != 0)
            {
                var resultados = _vehiculosService.GetAllVehiculos();
                return PartialView("_ListadoVehiculos", resultados);
            }
            else
            {
                return null;
            }
        }



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


            var resultados = _vehiculosService.GetAllVehiculos();
            return PartialView("_ListadoVehiculos", resultados);
        }
        public ActionResult ajax_CrearVehiculo_Ejemplo2(VehiculoModel model)
        {

            model.idEntidad = model.idEdntidad2;

            var IdVehiculo = _vehiculosService.CreateVehiculo(model);

            if (IdVehiculo != 0)
            {

                var Placa = model.placas;
                var Serie = model.serie;
                var folio = "";
                var resultados = _vehiculosService.GetVehiculoById(IdVehiculo);

                return Json(new { data = resultados });
            }
            else
            {
                return null;
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
            }
            else if (model.encontradoEn == (int)EstatusBusquedaVehiculo.NoEncontrado)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }

            if (IdVehiculo != 0)
            {
                var resultados = _vehiculosService.GetAllVehiculos();
                return Json(new { id = IdVehiculo, data = resultados });
            }
            else
            {
                return null;
            }
        }


    [HttpPost]
        public IActionResult ajax_EditarConductor(PersonaModel model)
        {
            //int id = _personasService.UpdatePersona(model);
            //int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);
            
            int id = _personasService.UpdateConductor(model);
            
            
            return Json(new { success = true });
        }


        #region Budqueda
        /************************************************************************************************/
        //BusquedaEspecial

        public IActionResult BusquedaEspecial()
        {

            var t = User.FindFirst(CustomClaims.Nombre).Value;

            return View("BusquedaEspecial");
        }



        public JsonResult Overview_GetTerritories()
        {


            var Options = new List<CatalogModel>();


            Options.Add(new CatalogModel { value = "1", text = "  En proceso" });
            Options.Add(new CatalogModel { value = "2", text = "Capturada" });
            Options.Add(new CatalogModel { value = "3", text = "Pagada" });
            Options.Add(new CatalogModel { value = "4", text = "Pagada con descuento" });
            Options.Add(new CatalogModel { value = "5", text = "Solventada" });
            Options.Add(new CatalogModel { value = "6", text = "Pagada con recargo" });
            Options.Add(new CatalogModel { value = "7", text = "Enviada" });



            return Json(Options);

        }

        public IActionResult GetDataBusquedaEspecialBit(string id)
        {

            var nombre = HttpContext.Session.GetString("Nombre");
            var result = _bitacoraServices.getBitacoraData(id, nombre);




            return Json(result);

        }
        public IActionResult GetDataBusquedaEspecial(InfraccionesBusquedaEspecialModel data)
        {

            return PartialView("_ListadoInfraccionesBusquedaEspecial");
        }




        public IActionResult test([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaEspecialModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(481))
            {
                Pagination pagination = new Pagination();
                pagination.PageIndex = request.Page - 1;
                pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
                pagination.Filter = resultValue;

                var listReporteAsignacion = _infraccionesService.GetAllInfraccionesBusquedaEspecialPagination(model, idOficina, idDependencia, pagination);
                var total = 0;
                if (listReporteAsignacion.Count() > 0)
                    total = listReporteAsignacion.ToList().FirstOrDefault().Total;

                request.PageSize = pagination.PageSize;
                var result = new DataSourceResult()
                {
                    Data = listReporteAsignacion,
                    Total = total
                };

                return Json(result);
            }
            else
            {
                var result = new DataSourceResult()
                {
                    Data = new List<InfraccionesModel>(),
                    Total = 0
                };

                return Json(result);
            }
        }


        public IActionResult Mostrar(string id)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int ids = Convert.ToInt32(id);

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccion2ById(ids, idDependencia);



            return View(model);
        }




        public IActionResult RemoveData(string id)
        {

            var model = _infraccionesService.CancelTramite(id);

            return Json(true);
        }
        public IActionResult ModalEditarCortesia(int idInfraccion)
        {

            var viewCortesiaModel = new EditarCortesiaModel
            {
                idInfraccion = idInfraccion,
            };

            return PartialView("_ModalCambiarCortesia", viewCortesiaModel);
        }
        public ActionResult UpdateCortesia(int idInfraccion, int cortesiaInt,string ObsevacionesApl)
        {


            var cambioCortesia = _infraccionesService.ActualizarEstatusCortesia(idInfraccion, cortesiaInt,ObsevacionesApl);

            return Json(cambioCortesia);
        }

        /*****************************************************************************************************/
        #endregion

        public JsonResult Entidades_Drop()
        {
            var result = new SelectList(_catEntidadesService.ObtenerEntidades(), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Municipios_Drop(int entidadDDlValue)
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");
            return Json(result);
        }


        public ActionResult UpdateFolio(string id, string folio)
        {

            var t = _infraccionesService.UpdateFolio(id, folio);

            return Json(true);
        }

        public ActionResult ModalEliminarMotivo(int idMotivoInfraccion, int idInfraccion, string Nombre)
        {
            ViewBag.idMotivoInfraccion = idMotivoInfraccion;
            ViewBag.idInfraccion = idInfraccion;
            ViewBag.Nombre = Nombre;

            return PartialView("_ModalEliminarMotivo");
        }
        [HttpPost]
        public IActionResult ajax_EliminarMotivo(int idMotivoInfraccion, int idInfraccion)
        {
            var MotivoEliminar = _infraccionesService.EliminarMotivoInfraccion(idMotivoInfraccion);

            var datosGrid = _infraccionesService.GetMotivosInfraccionByIdInfraccion(idInfraccion);

            return Json(datosGrid);
        }
        public ActionResult MostrarInfraccion(bool modoSoloLectura, int Id)
        {
            ViewBag.ModoSoloLectura = modoSoloLectura;

            return RedirectToAction("Editar", new { modoSoloLectura = true, idInfraccion = Id });
        }


        public ActionResult GetCatalogTramoFilter(FilterCatalogTramoModel filter)
        {

            var dat = _infraccionesService.GetFilterCatalog(filter);

            dat.Add(new SystemCatalogListModel() { Id = 1, Text = "No aplica" });
			dat.Add(new SystemCatalogListModel() { Id = 2, Text = "No especificado" });



			return Json(dat);

		}


        public DateTime getFechaVencimiento(DateTime fechaInfraccion, int idOficina)
        {
            int contador = 0;
            DateTime fechavigencia = fechaInfraccion;
            while (contador < 10)
            {
                fechavigencia= fechaInfraccion.AddDays(1);
                Console.WriteLine(fechavigencia.ToString("dddd"));
                if (fechavigencia.DayOfWeek.ToString() == "Sunday" || fechavigencia.DayOfWeek.ToString() == "Domingo")
                    Console.WriteLine(fechavigencia.ToString("dddd"));
                else
                {
                    if(_infraccionesService.GetDiaFestivo(idOficina, fechavigencia) ==0)
                        contador++;
                }
                //else { 
                //contador++;
                //}
                fechaInfraccion = fechavigencia;
            }


            return fechaInfraccion;
        }


	}
}

