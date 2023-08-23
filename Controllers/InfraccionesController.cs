﻿using GuanajuatoAdminUsuarios.Interfaces;
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

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class InfraccionesController : Controller
    {
        private readonly IEstatusInfraccionService _estatusInfraccionService;
        private readonly ITipoCortesiaService _tipoCortesiaService;
        private readonly IDependencias _dependeciaService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IGarantiasService _garantiasService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly IPdfGenerator<InfraccionesModel> _pdfService;
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly HttpClient _httpClient;
        private readonly ICrearMultasTransitoClientService _crearMultasTransitoClientService ;
        private readonly AppSettings _appSettings;


        public InfraccionesController(
            IEstatusInfraccionService estatusInfraccionService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService,
            ITipoCortesiaService tipoCortesiaService, IDependencias dependeciaService, IGarantiasService garantiasService,
            IInfraccionesService infraccionesService, IPdfGenerator<InfraccionesModel> pdfService,
            ICatDictionary catDictionary,
            IVehiculosService vehiculosService,
            IPersonasService personasService,
            IHttpClientFactory httpClientFactory,
            ICrearMultasTransitoClientService crearMultasTransitoClientService,
             IOptions<AppSettings> appSettings
            )
        {
            _catDictionary = catDictionary;
            _estatusInfraccionService = estatusInfraccionService;
            _tipoCortesiaService = tipoCortesiaService;
            _dependeciaService = dependeciaService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _garantiasService = garantiasService;
            _infraccionesService = infraccionesService;
            _pdfService = pdfService;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
            _httpClient = httpClientFactory.CreateClient();
            // Configurar el cliente HTTP con la URL base del servicio
            _httpClient.BaseAddress = new Uri("https://alfasiae.guanajuato.gob.mx/RESTAdapter/");
            _crearMultasTransitoClientService = crearMultasTransitoClientService;
            _appSettings = appSettings.Value;

        }

        public IActionResult Index()
        {
            int IdModulo = 700;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                InfraccionesBusquedaModel searchModel = new InfraccionesBusquedaModel();
                List<InfraccionesModel> listInfracciones = _infraccionesService.GetAllInfracciones(idOficina);
                searchModel.ListInfracciones = listInfracciones;
                return View(searchModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult ajax_BuscarInfracciones(InfraccionesBusquedaModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var listReporteAsignacion = _infraccionesService.GetAllInfracciones(model,idOficina);
            if (listReporteAsignacion.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }
            return PartialView("_ListadoInfracciones", listReporteAsignacion);
        }

        [HttpGet]
        public FileResult CreatePdf(string data)
        {
            var model = JsonConvert.DeserializeObject<InfraccionesBusquedaModel>(data,
               new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

            model.folioInfraccion = model.folioInfraccion == string.Empty ? null : model.folioInfraccion;
            model.placas = model.placas == string.Empty ? null : model.placas;
            model.NumeroEconomico = model.NumeroEconomico == string.Empty ? null : model.NumeroEconomico;
            model.Conductor = model.Conductor == string.Empty ? null : model.Conductor;
            model.Propietario = model.Propietario == string.Empty ? null : model.Propietario;
            model.NumeroLicencia = model.NumeroLicencia == string.Empty ? null : model.NumeroLicencia;

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"folioInfraccion","Folio"},
            {"NombreConductor","Conductor"},
            {"NombrePropietario","Propietario"},
            {"fechaInfraccion","Fecha Aplicada a"},
            {"NombreGarantia","Garantía"},
            {"nombreOficina","Delegación/Oficina"}
            };
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var ListTransitoModel = _infraccionesService.GetAllInfracciones(model, idOficina);
            var result = _pdfService.CreatePdf("ReporteInfracciones", "Infracciones", 6, ColumnsNames, ListTransitoModel);
            return File(result.Item1, "application/pdf", result.Item2);
        }

        [HttpGet]
        public FileResult CreatePdfUnRegistro(int IdInfraccion)
        {
            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"folioInfraccion","Folio"},
            {"NombreConductor","Conductor"},
            {"NombrePropietario","Propietario"},
            {"fechaInfraccion","Fecha Aplicada a"},
            {"NombreGarantia","Garantía"},
            {"delegacion","Delegación/Oficina"}
            };
            var InfraccionModel = _infraccionesService.GetInfraccionById(IdInfraccion);
            var result = _pdfService.CreatePdf("ReporteInfracciones", "Infracciones", 6, ColumnsNames, InfraccionModel);
            return File(result.Item1, "application/pdf", result.Item2);
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
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
            return Json(result);
        }

        public ActionResult Crear()
        {

            var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var vehiculosList = _vehiculosService.GetAllVehiculos();
            var personasList = _personasService.GetAllPersonas();

            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatCarreteras = new SelectList(catCarreteras.CatalogList, "Id", "Text");
            ViewBag.Vehiculos = vehiculosList;
            ViewBag.Personas = personasList;
            return View(new InfraccionesModel());
        }


        public ActionResult Editar(int id)
        {

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccion2ById(id);
            model.isPropietarioConductor = model.Vehiculo.idPersona == model.idPersona;
            var catTramos = _catDictionary.GetCatalog("CatTramosByFilter", model.idCarretera.ToString());
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
            ViewBag.CatCarreteras = new SelectList(catCarreteras.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");

            return View(model);


        }

        public ActionResult EditarA(int id)
        {

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccionAccidenteById(id);
            model.isPropietarioConductor = model.Vehiculo.idPersona == model.IdPersona;
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
            ViewBag.CatCarreteras = new SelectList(catCarreteras.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");

            return View("Editar2", model);
        }


        [HttpPost]
        public ActionResult ajax_editarInfraccion(InfraccionesModel model)
        {
            int idGarantia = 0;
            if (model.idGarantia == null || model.idGarantia == 0)
            {
                model.Garantia.numPlaca = model.placasVehiculo;
                idGarantia = _infraccionesService.CrearGarantiaInfraccion(model.Garantia);
                model.idGarantia = idGarantia;
            }
            else
            {
                var result = _infraccionesService.ModificarGarantiaInfraccion(model.Garantia);
            }
            var idInfraccion = _infraccionesService.ModificarInfraccion(model);         
            return Json(new { success = true, idInfraccion = idInfraccion });   
        }

        [HttpPost]
        public ActionResult ajax_crearInfraccion(InfraccionesModel model, CrearMultasTransitoRequestModel requestMode)
        {
            var idPersonaInfraccion = _infraccionesService.CrearPersonaInfraccion((int)model.idPersona);
            model.idPersonaInfraccion = idPersonaInfraccion;
           var idInfraccion = _infraccionesService.CrearInfraccion(model);
            return Json(new { id = idInfraccion });
            //return Ok();

        }



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
            var umas = _infraccionesService.GetUmas();
            ViewBag.Umas = umas;
            ViewBag.Totales = modelList.Sum(s => s.calificacion) * umas;
            return PartialView("_ListadoMotivos", modelList);
        }

        [HttpGet]
        public ActionResult ajax_detalleVehiculo(int idVehiculo)
        {
            var model = _vehiculosService.GetVehiculoById(idVehiculo);
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
            //var model = _vehiculosService.GetVehiculoById(id);
            var model = _infraccionesService.GetInfraccion2ById(id);
            return PartialView("_Cortesia", model);
        }

        [HttpPost]
        public ActionResult ajax_UpdateCortesiaInfraccion(InfraccionesModel model)
        {
            var result = new SelectList(_estatusInfraccionService.GetEstatusInfracciones(), "idEstatusInfraccion", "estatusInfraccion");
            var Cortesia = result.Where(x => x.Text.ToUpper() == "Cortesia".ToUpper()).FirstOrDefault();
            model.idEstatusInfraccion = Convert.ToInt32(Cortesia.Value);
            var modelInf = _infraccionesService.ModificarInfraccionPorCortesia(model);
            if (modelInf == 1)
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listInfracciones = _infraccionesService.GetAllInfracciones(idOficina);
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
                // Obtener los datos de la imagen
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                // Llamar al método del servicio para guardar la imagen
                _infraccionesService.InsertarImagenEnInfraccion(imageData, idInfraccion);

                return Json(new { success = true, message = "Imagen subida exitosamente" });
            }
            else
            {
                return Json(new { success = false, message = "No se seleccionó ninguna imagen" });
            }
        }

        /* public IActionResult ServiceCrearInfraccion(int idInfraccion)
         {
             var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion);
             var unicoMotivo = infraccionBusqueda.MotivosInfraccion.FirstOrDefault();

             CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();
             crearMultasRequestModel.CR1RFC = infraccionBusqueda.folioInfraccion;
             crearMultasRequestModel.CR1APAT = infraccionBusqueda.Persona.apellidoPaterno;
             crearMultasRequestModel.CR1AMAT = infraccionBusqueda.Persona.apellidoMaterno;
             crearMultasRequestModel.CR1NAME = infraccionBusqueda.Persona.nombre;
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
             crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString();
             crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
             crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
             crearMultasRequestModel.INF_PROP = "";
             crearMultasRequestModel.NOM_INFRACTOR = infraccionBusqueda.PersonaInfraccion.nombreCompleto;
             crearMultasRequestModel.DOM_INFRACTOR = "";
             crearMultasRequestModel.NUM_PLACA = infraccionBusqueda.placasVehiculo;
             crearMultasRequestModel.DOC_GARANTIA = "4";
             crearMultasRequestModel.NOM_RESP_SOLI = "";
             crearMultasRequestModel.DOM_RESP_SOLI = "";
             crearMultasRequestModel.FOLIO_MULTA = infraccionBusqueda.folioInfraccion;
             crearMultasRequestModel.OBS_GARANT = "";
             crearMultasRequestModel.ZMOTIVO1 = unicoMotivo.catMotivo;
             crearMultasRequestModel.ZMOTIVO2 = "";
             crearMultasRequestModel.ZMOTIVO3 = "";
             var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);
             ViewBag.Pension = result;
             if (result != null && result.MT_CrearMultasTransito_res.ZTYPE == "S")
             {
                 _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, idInfraccion);

                 return Json(new { success = true });
             } else if(result != null && result.MT_CrearMultasTransito_res.ZTYPE == "E")
                 {

                 return Json(new { success = false, message = "Registro actualizado en SITTEG" });
             }

            return Json(new { success = false, message = "Ha ocurrido un error rintenta mas tarde" });

             }
         }*/





        [HttpGet]
        public JsonResult GetInfraccionesLicencia(string numLicencia, string CURP)
        {
            if (_appSettings.AllowWebServices)
            {
                var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion);
                var unicoMotivo = infraccionBusqueda.MotivosInfraccion.FirstOrDefault();
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();
                crearMultasRequestModel.CR1RFC = infraccionBusqueda.folioInfraccion;
                crearMultasRequestModel.CR1APAT = infraccionBusqueda.Persona.apellidoPaterno;
                crearMultasRequestModel.CR1AMAT = infraccionBusqueda.Persona.apellidoMaterno;
                crearMultasRequestModel.CR1NAME = infraccionBusqueda.Persona.nombre;
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
                crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString();
                crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
                crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
                crearMultasRequestModel.INF_PROP = "";
                crearMultasRequestModel.NOM_INFRACTOR = infraccionBusqueda.PersonaInfraccion.nombreCompleto;
                crearMultasRequestModel.DOM_INFRACTOR = "";
                crearMultasRequestModel.NUM_PLACA = infraccionBusqueda.placasVehiculo;
                crearMultasRequestModel.DOC_GARANTIA = "4";
                crearMultasRequestModel.NOM_RESP_SOLI = "";
                crearMultasRequestModel.DOM_RESP_SOLI = "";
                if (infraccionBusqueda != null)
                {
                    string prefijo = (idOficina == 1) ? "TTO-PEC" : (idOficina == 2) ? "TTE-M" : "";
                    crearMultasRequestModel.FOLIO_MULTA = prefijo + infraccionBusqueda.folioInfraccion;
                }
                crearMultasRequestModel.OBS_GARANT = "";
                crearMultasRequestModel.ZMOTIVO1 = unicoMotivo.catMotivo;
                crearMultasRequestModel.ZMOTIVO2 = "";
                crearMultasRequestModel.ZMOTIVO3 = "";
                var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);
                ViewBag.Pension = result;
                if (result != null && result.MT_CrearMultasTransito_res.ZTYPE == "S")
                {
                    _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, idInfraccion);

                    return Json(new { success = true });
                }
                else if (result != null && result.MT_CrearMultasTransito_res.ZTYPE == "E")
                {

                    return Json(new { success = false, message = "Registro actualizado en SITTEG" });
                }

                return Json(new { success = false, message = "Ha ocurrido un error intenta mas tarde" });
            }
            else
            {
                return Json(new { success = false, message = "Registro actualizado en SITTEG" });
            }

        }
    }
}


