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

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class InfraccionesController : BaseController
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
             IOptions<AppSettings> appSettings,
            ICapturaAccidentesService capturaAccidentesService,
            ICotejarDocumentosClientService cotejarDocumentosClientService, ICatMunicipiosService catMunicipiosService, ICatEntidadesService catEntidadesService,
           IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService
            , IRepuveService repuveService
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
            _capturaAccidentesService = capturaAccidentesService;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            // Configurar el cliente HTTP con la URL base del servicio

            _crearMultasTransitoClientService = crearMultasTransitoClientService;
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
            _catMunicipiosService = catMunicipiosService;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _repuveService = repuveService;
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
            var listReporteAsignacion = _infraccionesService.GetAllInfracciones(model, idOficina);
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
            {"delegacion","Delegación/Oficina"}
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









        public ActionResult Editar(int idInfraccion, int id)
        {
            int ids = id != 0 ? id : idInfraccion;

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccion2ById(ids);
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

        public ActionResult EditarA(int idInfraccion, int id)
        {
            int ids = id != 0 ? id : idInfraccion;

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

            model.idDelegacion = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var idInfraccion = _infraccionesService.ModificarInfraccion(model);
            var idVehiculo = model.idVehiculo;
            return Json(new { success = true, idInfraccion = idInfraccion, idVehiculo = idVehiculo });
        }

        [HttpPost]
        public ActionResult ajax_crearInfraccion(InfraccionesModel model, CrearMultasTransitoRequestModel requestMode)
        {
            var idPersonaInfraccion = _infraccionesService.CrearPersonaInfraccion((int)model.idPersona);
            model.idPersonaInfraccion = idPersonaInfraccion;
            model.idEstatusInfraccion = (int)CatEnumerator.catEstatusInfraccion.EnProceso;
            model.idDelegacion = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var idInfraccion = _infraccionesService.CrearInfraccion(model);
            return Json(new { id = idInfraccion });
            //return Ok();

        }
        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
        }

        [HttpPost]
        public IActionResult ajax_BuscarVehiculo(VehiculoBusquedaModel model)

        {
            if (_appSettings.AllowWebServices)
            {
                var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
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
                        var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);

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
                                        telefonoFisico = telefonoValido,
                                        telefono = telefonoValido,
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
                            //Aqui servico repuve//////
                            //var resultSegundoServicio = await BusquedaRepuveAsync(model);
                            RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel()
                            {
                                placa = model.PlacasBusqueda,
                                niv = model.SerieBusqueda
                            };
                            var repuveConsGralResponse = _repuveService.ConsultaGeneral(repuveGralModel).FirstOrDefault();


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
                var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
                vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
                vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
                return PartialView("_Create", vehiculosModel);
            }
        }
        private int ObtenerIdMunicipioDesdeBD(string municipio)
        {
            var idMunicipio = _catMunicipiosService.obtenerIdPorNombre(municipio);
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

        public IActionResult ServiceCrearInfraccion(int idInfraccion)
        {
            if (_appSettings.AllowWebServices)
            {
                var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion);
                var unicoMotivo = infraccionBusqueda.MotivosInfraccion?.FirstOrDefault();
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();
                crearMultasRequestModel.CR1RFC = infraccionBusqueda.Persona.RFC;
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
                crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString("F2");
                crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
                crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
                crearMultasRequestModel.INF_PROP = "";
                crearMultasRequestModel.NOM_INFRACTOR = infraccionBusqueda.PersonaInfraccion.nombreCompleto;
                crearMultasRequestModel.DOM_INFRACTOR = infraccionBusqueda.Persona.PersonaDireccion.calle + " " + infraccionBusqueda.Persona.PersonaDireccion.numero + ", " + infraccionBusqueda.Persona.PersonaDireccion.colonia;
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
                crearMultasRequestModel.ZMOTIVO1 = unicoMotivo.Motivo;
                crearMultasRequestModel.ZMOTIVO2 = "";
                crearMultasRequestModel.ZMOTIVO3 = "";
                var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);
                ViewBag.Pension = result;

                if (result != null && result.MT_CrearMultasTransito_res != null && "S".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                {
                    _infraccionesService.ModificarEstatusInfraccion(idInfraccion, (int)CatEnumerator.catEstatusInfraccion.Enviada);
                    _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, idInfraccion);

                    return Json(new { success = true });
                }
                else if (result != null && result.MT_CrearMultasTransito_res != null && "E".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Registro actualizado en SITTEG", id = idInfraccion });
                }
                else if (result != null && result.MT_CrearMultasTransito_res != null && "A".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Infraccion anteriormente registrada en finanzas", id = idInfraccion });
                }
                else
                {
                    return Json(new { success = false, message = "Ha ocurrido un error intenta más tarde" });
                }
            }
            return Json(new { success = false, message = "Registro actualizado en SITTEG", id = idInfraccion });

        }

        public ActionResult ModalAgregarConductor()
        {
            return View("_ModalBusquedaPersonas");
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
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            var personasMoralesModel = _personasService.GetAllPersonasMorales();
            return PartialView("_ListPersonasMorales", personasMoralesModel);
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
            int id = _personasService.CreatePersona(model);

            if (id == -1)
            {
                // El registro ya existe, muestra un mensaje de error al usuario
                return Json(new { success = false, message = "El registro yaexiste, revise los datos ingresados." });
            }
            else
            {
                // La inserción se realizó correctamente
                model.PersonaDireccion.idPersona = id;
                int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);


                var modelList = _personasService.GetAllPersonas();
                return PartialView("_ListadoPersonas", modelList);
            }
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaFisica(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            var IdPersonaFisica = _personasService.CreatePersona(Persona);
            if (IdPersonaFisica == 0)
            {
                throw new Exception("Ocurrio un error al dar de alta la persona");
            }
            var personasFisicasModel = _personasService.GetAllPersonasFisicas();
            return PartialView("_PersonasFisicas", personasFisicasModel);
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
            var personasMoralesModel = _personasService.GetAllPersonasMorales();
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }

        //TODO: ejemplo crear vehiculo por service de guanajuato
        [HttpPost]
        public ActionResult ajax_CrearVehiculo_Ejemplo(VehiculoModel model)
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

    }
}

