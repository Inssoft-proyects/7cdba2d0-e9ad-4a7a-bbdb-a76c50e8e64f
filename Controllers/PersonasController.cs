using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.UI;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.Models.LicenciaTipoRespuesta;
using Kendo.Mvc.Extensions;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class PersonasController : BaseController
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IPersonasService _personasService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly IBitacoraService _bitacoraServices;
        private static BusquedaPersonaModel perModel = new BusquedaPersonaModel();
        public PersonasController(ICatDictionary catDictionary, IPersonasService personasService, IHttpClientFactory httpClientFactory, ICatEntidadesService catEntidadesService
            , ICatMunicipiosService catMunicipiosService
, IBitacoraService bitacoraServices)
        {
            _catDictionary = catDictionary;
            _personasService = personasService;
            _httpClientFactory = httpClientFactory;
            _catEntidadesService = catEntidadesService;
            _catMunicipiosService = catMunicipiosService;
            _bitacoraServices = bitacoraServices;
        }
        public IActionResult Index()
        {
            perModel = new BusquedaPersonaModel();
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var personaModel = new BusquedaPersonaModel();
            personaModel.ListadoPersonas = new List<PersonaModel>();

            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            return View(personaModel);
        }
        public IActionResult DetallesLicencia()
        {

            return View("_DetalleLicencia");
        }
        public async Task<IActionResult> BuscarPorParametro(PersonaModel model)
        {
            //buscarConductorPaginado
            // Realizar la búsqueda de personas
            var personasList = _personasService.BusquedaPersona(model);

            // Verificar si se encontraron resultados en la búsqueda de personas
            if (personasList.Any())
            {
                return Json(new { encontrada = true, data = personasList });
            }

            // Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia
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
                return Json(new { encontrada = false, data = personasList, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }

            // Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
            return Json(new { encontrada = false, data = personasList, message = "No se encontraron resultados." });
        }

        public IActionResult GetBuscar([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
        {
            perModel = model;
            return PartialView("_ListadoPersonas", new List<PersonaModel>());
        }

        [HttpPost]
        public async Task<IActionResult> BuscarPorParametroPaginado([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
        {
            // Realizar la búsqueda de personas
            if (model.PersonaModel != null)
                perModel = model;
            else
                model = perModel;

            var findAll = false;
            var personas = new BusquedaPersonaModel();
            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            if (model.PersonaModel != null)
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
            } else
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
                
                var result = new DataSourceResult()
                {
                    Data = personas.ListadoPersonas,
                    Total = total
                };
                return Json(result);
            }

            // Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia
            return Json(new { encontrada = false, Data = "1", tipo = "sin datos", message = "busca en licencias" });



            string parametros = "";
            parametros += string.IsNullOrEmpty(model.PersonaModel.numeroLicenciaBusqueda) ? "" : "licencia=" + model.PersonaModel.numeroLicenciaBusqueda;
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
                    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);

                    return Json(new { encontrada = false, Data = respuesta.datos, tipo = respuesta.datos == null? "sin datos" : "success", message = respuesta.mensaje });
                    //return Json(respuesta);
                }
            }
            catch (Exception ex)
            {
                // En caso de errores, devolver una respuesta JSON con licencia no encontrada
                return Json(new { encontrada = false, Data = personasList, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }

            //Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
            return Json(new { encontrada = false, Data = personasList, message = "No se encontraron resultados." });
        }



        [HttpGet]
        public IActionResult ajax_ModalCrearPersona()
        {
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            // var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            // var catMunicipios = (_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");

            // ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            //ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            return PartialView("_CrearPersona", new PersonaModel());
        }
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

        [HttpPost]
        public IActionResult ajax_CrearPersona(PersonaModel model)
        {
            //var model = json.ToObject<Gruas2Model>();
            //var errors = ModelState.Values.Select(s => s.Errors);
            //if (ModelState.IsValid)
            //{
            int id = _personasService.CreatePersona(model);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //BITACORA
            _bitacoraServices.insertBitacora(id, ip, "Personas_PersonaMoral", "Insertar", "Insert", user);
            //model.PersonaDireccion.idPersona = id;
            //int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);

            var modelList = _personasService.GetPersonaById(id);
            return Json(modelList);
            //}
            //return RedirectToAction("Index");
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
                }
                else
                {
                    int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);
                }
                int id = _personasService.UpdatePersona(model);
                var modelList = _personasService.GetPersonaById((int)model.idPersona);
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

                //BITACORA
                _bitacoraServices.insertBitacora((int)model.idPersona, ip, "Personas_PersonaMoral", "Actualizar", "Update", user);
                //var listPadronGruas = _concesionariosService.GetAllConcesionarios();
                return Json(new { data = modelList });
            }
            return RedirectToAction("Index");
        }



        //  [HttpPost]

        //public ActionResult GuardaDesdeServicio(PersonaModel personaDatos)
        public async Task<IActionResult> GuardaDesdeServicio(string nombre, string apellidoPaterno, string apellidoMaterno, string CURP, string RFC, string numeroLicencia, string tipoLicencia,
                                                             string idGenero, DateTime fechaNacimiento, DateTime fechaVigencia)
        {
            try
            {
                LicenciaPersonaDatos personaDatos = new LicenciaPersonaDatos();
                personaDatos.NOMBRE = nombre;
                personaDatos.PRIMER_APELLIDO = apellidoPaterno;
                personaDatos.SEGUNDO_APELLIDO = apellidoMaterno;
                personaDatos.CURP= CURP;    
                personaDatos.RFC= RFC;
                personaDatos.NUM_LICENCIA= numeroLicencia;
                personaDatos.ID_TIPO_LICENCIA = Convert.ToInt32(tipoLicencia);
                personaDatos.ID_GENERO = Convert.ToInt32(idGenero);
                personaDatos.FECHA_NACIMIENTO = fechaNacimiento;
                personaDatos.FECHA_TERMINO_VIGENCIA = fechaVigencia;

                int idPersona = _personasService.InsertarDesdeServicio(personaDatos);
                //var datosTabla = _personasService.BuscarPersonaSoloLicencia(personaDatos.NUM_LICENCIA);
                var datosTabla = _personasService.GetPersonaById(idPersona);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(idPersona, ip, "Personas_DesdeServicio", "Insertar", "insert", user);
                
                  return Json(new { data = datosTabla });
                //return Json(new { data = "hola" });

            }
            catch (Exception ex)
            {
                // Maneja el error de manera adecuada
                return Json(new { error = "Error al guardar en la base de datos: " + ex.Message });
            }
        }

        public async Task<IActionResult> TestSesion()
        {
            return Json("session activa");
        }



        public async Task<IActionResult> PersonasEnLicencias([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
        {
           



            string parametros = "";
            parametros += string.IsNullOrEmpty(model.PersonaModel.numeroLicenciaBusqueda) ? "" : "licencia=" + model.PersonaModel.numeroLicenciaBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.CURPBusqueda) ? "" : "curp=" + model.PersonaModel.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.RFCBusqueda) ? "" : "rfc=" + model.PersonaModel.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.nombreBusqueda) ? "" : "nombre=" + model.PersonaModel.nombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.PersonaModel.apellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.PersonaModel.apellidoMaternoBusqueda  ;
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

                    List<PersonaModel> pEncontradas = new List<PersonaModel>();
                    foreach (LicenciaPersonaDatos pivote in respuesta)
                    {
                        PersonaModel pm = new PersonaModel();
                        pm.idPersona = (int)pivote.ID_PERSONA;
                        pm.nombre = pivote.NOMBRE;
                        pm.apellidoPaterno = pivote.PRIMER_APELLIDO;
                        pm.apellidoMaterno = pivote.SEGUNDO_APELLIDO;
                        pm.CURP = pivote.CURP;
                        pm.RFC = pivote.RFC;
                        pm.numeroLicencia = pivote.NUM_LICENCIA;
                        pm.tipoLicencia = pivote.TIPOLICENCIA;
                        pm.idGenero = pivote.ID_GENERO==null? 0 :(int)pivote.ID_GENERO;
                        pm.fechaNacimiento = pivote.FECHA_NACIMIENTO;
                        pm.vigenciaLicencia = pivote.FECHA_TERMINO_VIGENCIA;
                        pEncontradas.Add(pm);
                    }

                   
                    
                    return Json(pEncontradas);
                }
            }
            catch (Exception ex)
            {
                // En caso de errores, devolver una respuesta JSON con licencia no encontrada
                return Json(new { encontrada = false, Data = "", message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }

            //Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
            return Json(new { encontrada = false, Data = "", message = "No se encontraron resultados." });
        }




    }
}



