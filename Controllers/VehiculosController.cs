using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Util;
using iTextSharp.text;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class VehiculosController : BaseController
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly ICatTipoLicenciasService _catTipoLicenciasService;
        private readonly AppSettings _appSettings;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly ICatTiposVehiculosService _catTiposVehiculosService;
        private readonly IRepuveService _repuveService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatSubtipoServicio _subtipoServicio;

        private string resultValue = string.Empty;
        public static VehiculoBusquedaModel vehModel = new VehiculoBusquedaModel();
        public VehiculosController(IVehiculosService vehiculosService, ICatDictionary catDictionary,
            IPersonasService personasService, HttpClient httpClientFactory, IConfiguration configuration,
           ICotejarDocumentosClientService cotejarDocumentosClientService, ICatTipoLicenciasService catTipoLicenciasService,
           IOptions<AppSettings> appSettings, ICatMunicipiosService catMunicipiosService, ICatEntidadesService catEntidadesService,
           IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService,
            ICatTiposVehiculosService catTiposVehiculosService
        , IRepuveService repuveService,
            IBitacoraService bitacoraService,
            ICatSubtipoServicio subtipoServicio
            )
        {
            _vehiculosService = vehiculosService;
            _catDictionary = catDictionary;
            _personasService = personasService;
            _httpClient = httpClientFactory;
            _configuration = configuration;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _catTipoLicenciasService = catTipoLicenciasService;
            _appSettings = appSettings.Value;
            _catMunicipiosService = catMunicipiosService;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _catTiposVehiculosService = catTiposVehiculosService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _subtipoServicio = subtipoServicio;
        }

        public IActionResult Index()
        {
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            //ViewBag.PersonasFisicas= _personasService.GetAllPersonas();
            return View(vehiculoBusquedaModel);
        }

        public IActionResult Editar()
        {
           // var vehiculosModel = _vehiculosService.GetAllVehiculos();
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
           // vehiculoBusquedaModel.ListVehiculo = vehiculosModel.ToList();
            return View(vehiculoBusquedaModel);
        }

        public ActionResult EditarVehiculo(int id)
        {
          
                var vehiculosModel = _vehiculosService.GetVehiculoById(id);
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = vehiculosModel;
            vehiculoBusquedaModel.Vehiculo.idSubmarcaUpdated = vehiculosModel.idSubmarca;
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoBusquedaModel.isFromUpdate = true;
            vehiculosModel.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;


            vehiculoBusquedaModel.Vehiculo.ErrorRepube = "No";
            vehiculoBusquedaModel.Vehiculo.showclose = false;
            vehiculoBusquedaModel.Vehiculo.showSubTipo = (vehiculoBusquedaModel.Vehiculo.idCatTipoServicio == 1 || vehiculoBusquedaModel.Vehiculo.idCatTipoServicio == 5) ? true : false;



            return View("EditarVehiculo", vehiculoBusquedaModel.Vehiculo);
            }
    

        public JsonResult Entidades_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Municipios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult TipoServicios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult Municipios_Drop(int entidadDDlValue)
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Colores_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatColores", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Marcas_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatMarcasVehiculos", "0");
            var orderedList = catEntidades.CatalogList.OrderBy(item => item.Text);
            var result = new SelectList(orderedList, "Id", "Text"); return Json(result);
        }

        public JsonResult SubMarcas_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatSubmarcasVehiculos", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        public JsonResult TiposVehiculo_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult TipoLicencias_Drop()
        {
            var result = new SelectList(_catTipoLicenciasService.ObtenerTiposLicencia(), "idTipoLicencia", "tipoLicencia");
            return Json(result);
        }


        [HttpPost]
        public ActionResult ajax_BuscarVehiculo2(VehiculoBusquedaModel model)
        {
            try
            {
                Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - Request - " + JsonConvert.SerializeObject(model));
                RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel()
                {
                    placa = model.PlacasBusqueda,
                    niv = model.SerieBusqueda
                };

                //verificar si tiene robo
                Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - ConsultaRobo");
                var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel)?.FirstOrDefault() ?? new RepuveConsRoboResponseModel();
                if (repuveConsRoboResponse!=null)
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - ConsultaRobo - Response - " + JsonConvert.SerializeObject(repuveConsRoboResponse));

                ViewBag.ReporteRobo = repuveConsRoboResponse.estatus == "1";

                ///validar si esta habilitado el sp repuve
                if (_appSettings.AllowWebServices)
                {
                    //base
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - GetVehiculoToAnexo");
                    var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - GetVehiculoToAnexo - Response - " + JsonConvert.SerializeObject(vehiculosModel));

                    vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
                    vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                    vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();

                    if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                    {
                        try
                        {
                            CotejarDatosRequestModel cotejarDatosRequestModel = new CotejarDatosRequestModel();
                            cotejarDatosRequestModel.Tp_folio = "4";
                            cotejarDatosRequestModel.Folio = model.PlacasBusqueda;
                            cotejarDatosRequestModel.tp_consulta = "3";

                            var endPointName = "CotejarDatosEndPoint";
                            //finanzas
                            Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - CotejarDatos");
                            var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);
                            Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - CotejarDatos - Response - " + JsonConvert.SerializeObject(result));

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


                                //repube
                                Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - ConsultaGeneral - Repuve");
                                var repuveConsGralResponse = _repuveService.ConsultaGeneral(repuveGralModel).FirstOrDefault();
                                Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - ConsultaGeneral - Repuve - Response - " + JsonConvert.SerializeObject(repuveConsGralResponse));


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
                            Logger.Error("VehículosController - ajax_BuscarVehiculo2 - " + ex.Message);
                            return Json(new { success = false, message = "Ha ocurrido un error al comunicarse con el servicio web." });
                        }
                    }


                    //repuve
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - ConsultaGeneral");
                    var repuveConsGralResponse1 = _repuveService.ConsultaGeneral(repuveGralModel).FirstOrDefault();
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - ConsultaGeneral - Response - " + JsonConvert.SerializeObject(repuveConsGralResponse1));


                    var vehiculoEncontrado1 = new VehiculoModel
                    {
                        placas = repuveConsGralResponse1.placa,
                        serie = repuveConsGralResponse1.niv_padron,
                        //tarjeta = repuveConsGralResponse.ta,
                        motor = repuveConsGralResponse1.motor,
                        //otros = repuveConsGralResponse.
                        color = repuveConsGralResponse1.color,
                        //idEntidad = idEntidad,
                        //idMarcaVehiculo = idMarca,
                        //idSubmarca = idSubmarca,
                        submarca = repuveConsGralResponse1.submarca,
                        //idTipoVehiculo = idTipo,
                        modelo = repuveConsGralResponse1.modelo,
                        //capacidad = repuveConsGralResponse.c,
                        //carga = repuveConsGralResponse.ca,

                        Persona = new PersonaModel(),

                        PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                    };
                    return PartialView("_Create", vehiculoEncontrado1);

                }
                else
                {
                    //base
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - GetVehiculoToAnexo");
                    var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
                    Logger.Debug("VehículosController - ajax_BuscarVehiculo2 - GetVehiculoToAnexo - Response - " + JsonConvert.SerializeObject(vehiculosModel));

                    vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
                    vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                    vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
                    return PartialView("_Create", vehiculosModel);
                }
            }catch(Exception ex)
            {
                Logger.Error("VehículosController - ajax_BuscarVehiculo2 - " + ex.Message);
                return PartialView("_Create");
            }
        }



        private bool ValidarRobo(RepuveConsgralRequestModel repuveGralModel)
        {
            var estatus = false;

            var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel)?.FirstOrDefault() ?? new RepuveConsRoboResponseModel();

            estatus = repuveConsRoboResponse.estatus == "1";

            return estatus;
        }

        /// <summary>
        /// //CONSULTA SERVICIO FINANZAS PADRON ESTATAL
        /// </summary>
        /// <returns></returns>

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



        [HttpPost]
        public async Task<string> ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {

                var request = JsonConvert.SerializeObject(model);
                Logger.Debug("Vehiculos - ajax_BuscarVehiculo - Request:" + request);
                var vehiculosModel = new VehiculoModel();

                RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel(model.PlacasBusqueda, model.SerieBusqueda);
                Logger.Debug("Vehiculos - ajax_BuscarVehiculo - ValidarRobo ");
                ViewBag.ReporteRobo = ValidarRobo(repuveGralModel);

                var allowSistem = _appSettings.AllowWebServicesRepuve;

                Logger.Debug("Vehiculos - ajax_BuscarVehiculo - GetVehiculoToAnexo");
                vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
                vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
                vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();

                if (vehiculosModel.idVehiculo > 0)
                {
                    return await this.RenderViewAsync("_Create", vehiculosModel, true);

                }

                if (allowSistem)
                {
                    CotejarDatosRequestModel cotejarDatosRequestModel = new CotejarDatosRequestModel();
                    cotejarDatosRequestModel.Tp_folio = "4";
                    cotejarDatosRequestModel.Folio = model.PlacasBusqueda;
                    cotejarDatosRequestModel.tp_consulta = "3";
                    var endPointName = "CotejarDatosEndPoint";
                    Logger.Debug("Vehiculos - ajax_BuscarVehiculo - CotejarDatos");
                    var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);
                    if (result.MT_CotejarDatos_res != null && result.MT_CotejarDatos_res.Es_mensaje != null && result.MT_CotejarDatos_res.Es_mensaje.TpMens.ToString().Equals("I", StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Debug("Vehiculos - ajax_BuscarVehiculo - GetVEiculoModelFromFinanzas - Response - " + JsonConvert.SerializeObject(result));
                        vehiculosModel = GetVEiculoModelFromFinanzas(result);

                        vehiculosModel.ErrorRepube = string.IsNullOrEmpty(vehiculosModel.placas) ? "No" : "";
                        //Se establece el origen de datos
                        vehiculosModel.origenDatos = "Padrón Estatal";

                        return await this.RenderViewAsync("_Create", vehiculosModel, true);
                    }
                }

                if (allowSistem)
            {
                Logger.Debug("Vehiculos - ajax_BuscarVehiculo - ConsultaGeneral - REPUVE");
                var repuveConsGralResponse = _repuveService.ConsultaGeneral(repuveGralModel).FirstOrDefault();
                Logger.Debug(" - Response - " + JsonConvert.SerializeObject(repuveConsGralResponse));
                var idEntidad = !string.IsNullOrEmpty(repuveConsGralResponse.entidad_expide)
                      ? ObtenerIdEntidadRepuve(repuveConsGralResponse.entidad_expide)
                      : 0;
                var idColor = !string.IsNullOrEmpty(repuveConsGralResponse.color)
                    ? ObtenerIdColor(repuveConsGralResponse.color)
                    : 0;

                var idMarca = !string.IsNullOrEmpty(repuveConsGralResponse.marca_padron)
                    ? ObtenerIdMarcaRepuve(repuveConsGralResponse.marca_padron)
                    : 0;

                var idSubmarca = !string.IsNullOrEmpty(repuveConsGralResponse.submarca_padron)
                    ? ObtenerIdSubmarcaRepuve(repuveConsGralResponse.submarca_padron)
                    : 0;
                var submarcaLimpio = !string.IsNullOrEmpty(repuveConsGralResponse.submarca_padron)
                    ? ObtenerIdSubmarcaRepuve(repuveConsGralResponse.submarca_padron)
                    : 0;

                var idTipo = !string.IsNullOrEmpty(repuveConsGralResponse.tipo_vehiculo_padron)
                 ? ObtenerIdTipoVehiculo(repuveConsGralResponse.tipo_vehiculo_padron)
                 : 0;
                var idTipoServicio = !string.IsNullOrEmpty(repuveConsGralResponse.tipo_uso_padron)
                ? ObtenerIdTipoServicioRepuve(repuveConsGralResponse.tipo_uso_padron)
                : 0;
                var vehiculoEncontrado = new VehiculoModel
                {
                    placas = repuveConsGralResponse.placa,
                    serie = repuveConsGralResponse.niv_padron,
                    // numeroEconomico = repuveConsGralResponse.tnia,
                    motor = repuveConsGralResponse.motor,
                    //otros = repuveConsGralResponse.
                    idColor = idColor,
                    idEntidad = idEntidad,
                    idMarcaVehiculo = idMarca,
                    idSubmarca = idSubmarca,
                    //submarca = submarcaLimpio,
                    idTipoVehiculo = idTipo,
                    modelo = repuveConsGralResponse.modelo,
                    idCatTipoServicio = idTipoServicio,
                    //carga = repuveConsGralResponse.ca,

                    Persona = new PersonaModel(),

                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                };

                vehiculoEncontrado.ErrorRepube = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? "No" : "";

                //Se establece el origen de datos
                vehiculoEncontrado.origenDatos = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? null : "REPUVE";
                return await this.RenderViewAsync("_Create", vehiculoEncontrado, true);

            }
            else
            {
                Logger.Debug("Vehiculos - ajax_BuscarVehiculo - ConsultaGeneral - REPUVE (BANDERA DESACTIVADA)");
            }
            vehiculosModel.ErrorRepube = string.IsNullOrEmpty(vehiculosModel.placas) ? "No" : "";


            return await this.RenderViewAsync("_Create", vehiculosModel, true);
        }
            catch (Exception ex)
            {
                Logger.Error("Vehiculos - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
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

                var idSubmarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio);
                return idSubmarca;
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
        private int ObtenerIdTipoServicio(string servicio)
        {
            int servicioNumero = int.Parse(servicio.TrimStart('0'));
            var idTipoVehiculo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            var tipoServicio = idTipoVehiculo.CatalogList.FirstOrDefault(item => item.Id == servicioNumero)?.Id;

            return (int)tipoServicio;
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

        public IActionResult GetBuscarVehiculos([DataSourceRequest] DataSourceRequest request, VehiculoBusquedaModel model)
        {
            vehModel = model;
            return PartialView("_ListVehiculos", new List<VehiculoModel>());
        }
        


        [HttpPost]
        public IActionResult ajax_BuscarVehiculos([DataSourceRequest] DataSourceRequest request, VehiculoBusquedaModel model)
        {
            string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(251))
            {
                Pagination pagination = new Pagination();
                pagination.PageIndex = request.Page - 1;
                pagination.PageSize = 10;
                pagination.Filter = resultValue;

                model = vehModel;

                var vehiculosModel = _vehiculosService.GetVehiculosPagination(model,pagination);
                VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
                vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
                vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
                vehiculoBusquedaModel.ListVehiculo = vehiculosModel.ToList();
                var total = 0;
                if (vehiculoBusquedaModel.ListVehiculo.Count() > 0)
                    total = vehiculoBusquedaModel.ListVehiculo.ToList().FirstOrDefault().total;

                request.PageSize = 10;
                var result = new DataSourceResult()
                {
                    Data = vehiculoBusquedaModel.ListVehiculo,
                    Total = total
                };

                return Json(result);
            }
            else
            {
                var result = new DataSourceResult()
                {
                    Data = new List<VehiculoModel>(),
                    Total = 0
                };

                return Json(result);
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

        //public ActionResult ajax_BuscarVehiculos(VehiculoBusquedaModel model)
        //{
        //    var vehiculosModel = _vehiculosService.GetVehiculos(model);
        //    return PartialView("_ListVehiculos", vehiculosModel);
        //}



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
           var personasFisicas = _personasService.GetAllPersonasFisicas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {

            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            _bitacoraServices.insertBitacora(IdPersonaMoral, ip, "VehiculosPersonaMoral", "Insertar", "Insert", user);
            //var personasMoralesModel = _personasService.GetAllPersonasMorales();
            var modelList = _personasService.ObterPersonaPorIDList(IdPersonaMoral); ;

            return PartialView("_ListPersonasMorales", modelList);
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
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            _bitacoraServices.insertBitacora((int)Persona.idPersona, ip, "PersonaMoral", "Actualizar", "Update", user);
            return Json(new { data = personaEditada });
        }


        [HttpPost]
        public ActionResult ajax_CrearVehiculo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            bool esEdicion = false;

            if (model.idVehiculo > 0)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);
                esEdicion = true;
            }
            else if (model.idVehiculo <= 0)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }

            if (IdVehiculo != 0)
            {
                return Json(new { id = IdVehiculo, esEdicion = esEdicion });
            }
            else
            {
                return null;
            }
        }



        [HttpPost]
        public ActionResult ajax_CrearVehiculo2(VehiculoModel model)
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

        public JsonResult SubTipoServicios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatSubtipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult GetSubtipoPorTipo(int idTipoServicio)
        {
            var result = new SelectList(_subtipoServicio.GetSubtipoPorTipo(idTipoServicio), "idSubTipoServicio", "subTipoServicio");
            return Json(result);
        }

    }
}
