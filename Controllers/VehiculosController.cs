using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class VehiculosController : Controller
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly ICatTipoLicenciasService _catTipoLicenciasService;


        public VehiculosController(IVehiculosService vehiculosService, ICatDictionary catDictionary,
            IPersonasService personasService, HttpClient httpClientFactory, IConfiguration configuration,
           ICotejarDocumentosClientService cotejarDocumentosClientService,ICatTipoLicenciasService catTipoLicenciasService


         )
        {
            _vehiculosService = vehiculosService;
            _catDictionary = catDictionary;
            _personasService = personasService;
            _httpClient = httpClientFactory;
            _configuration = configuration;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _catTipoLicenciasService = catTipoLicenciasService;
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
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
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
            var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
            vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
            vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();

          /*  if (vehiculosModel.encontradoEn == 3 && !string.IsNullOrEmpty(model.PlacasBusqueda))
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
                    var vehiculoEncontrado = new VehiculoModel
                    {
                        placas = vehiculoEncontradoData.no_placa,
                        serie = vehiculoEncontradoData.no_serie,
                        tarjeta = vehiculoEncontradoData.no_tarjeta,
                        paisManufactura = vehiculoEncontradoData.no_motor,
                        otros = vehiculoEncontradoData.otros,

                        Persona = new PersonaModel
                        {
                            RFC = vehiculoInterlocutorData.Nro_rfc,
                            nombre = vehiculoInterlocutorData.es_per_moral?.name_org1,

                            PersonaDireccion = new PersonaDireccionModel
                            {
                                telefono = vehiculoDireccionData.telefono,
                                correo = vehiculoDireccionData.correo,
                                colonia = vehiculoDireccionData.colonia,
                                calle = vehiculoDireccionData.calle,
                                numero = vehiculoDireccionData.nro_exterior,
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

                    return PartialView("_Create", vehiculosModel);

                }
            }*/

            return PartialView("_Create", vehiculosModel); 
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
