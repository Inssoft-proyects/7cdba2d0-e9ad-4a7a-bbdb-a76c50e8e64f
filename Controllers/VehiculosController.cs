using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Controllers
{
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


        public VehiculosController(IVehiculosService vehiculosService, ICatDictionary catDictionary,
            IPersonasService personasService, HttpClient httpClientFactory, IConfiguration configuration,
           ICotejarDocumentosClientService cotejarDocumentosClientService,ICatTipoLicenciasService catTipoLicenciasService,
           IOptions<AppSettings> appSettings, ICatMunicipiosService catMunicipiosService, ICatEntidadesService catEntidadesService,
           IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService,
            ICatTiposVehiculosService catTiposVehiculosService
        , IRepuveService repuveService
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
            var vehiculosModel = _vehiculosService.GetAllVehiculos();
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoBusquedaModel.ListVehiculo = vehiculosModel.ToList();
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
            return View("Index", vehiculoBusquedaModel);
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
            var catEntidades = _catDictionary.GetCatalog("SubMarcas_Read", "0");
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
        public ActionResult ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
            if (_appSettings.AllowWebServices)
            {
                var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
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
                RepuveConsgralRequestModel repuveGralModel1 = new RepuveConsgralRequestModel()
                {
                    placa = model.PlacasBusqueda,
                    niv = model.SerieBusqueda
                };
                var repuveConsGralResponse1 = _repuveService.ConsultaGeneral(repuveGralModel1).FirstOrDefault();


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
        [HttpPost]
        public ActionResult ajax_CrearPersonaFisica(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            var IdPersonaFisica = _personasService.CreatePersona(Persona);
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
                return Json(new { id = IdVehiculo });
            }
            else
            {
                return null;
            }
        }

    }
}
