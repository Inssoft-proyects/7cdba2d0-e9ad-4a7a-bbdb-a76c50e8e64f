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

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class PersonasController : Controller
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IPersonasService _personasService;
        private readonly IHttpClientFactory _httpClientFactory;
        public PersonasController(ICatDictionary catDictionary, IPersonasService personasService, IHttpClientFactory httpClientFactory)
        {
            _catDictionary = catDictionary;
            _personasService = personasService;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");

            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            return View();
        }
        public IActionResult DetallesLicencia()
        {

            return View("_DetalleLicencia");
        }
        public async Task<IActionResult> BuscarPorParametro(PersonaModel model)
        {
            string parametros = "";
            parametros += string.IsNullOrEmpty(model.numeroLicenciaBusqueda) ? "" : "licencia="+ model.numeroLicenciaBusqueda;
            parametros += string.IsNullOrEmpty(model.CURPBusqueda) ? "" : "curp="+ model.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.RFCBusqueda) ? "" : "rfc="+ model.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.nombreBusqueda) ? "" : "nombre="+ model.nombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.apellidoPaternoBusqueda) ? "" : "primer_apellido="+ model.apellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.apellidoMaternoBusqueda) ? "" : "segundo_apellido="+ model.apellidoMaternoBusqueda + "";

            string ultimo = parametros.Substring(parametros.Length - 1);
            if(ultimo.Equals("&"))
                parametros= parametros.Substring(0, parametros.Length - 1);

            bool licenciaNoSITTEG = true;

            if (!string.IsNullOrEmpty(model.numeroLicenciaBusqueda))


            {
                // Verificar si el número de licencia no está en la base de datos
                licenciaNoSITTEG = _personasService.VerificarLicenciaSitteg(model.numeroLicenciaBusqueda);
            }
            if (licenciaNoSITTEG)
            {
                try
                { 
                    string urlServ = Request.GetDisplayUrl();
                    Uri uri = new Uri(urlServ);
                    string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                    var url = requested+$"/api/Licencias/datos_generales?"+ parametros;

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
                    return Json(new { encontrada = false, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " +ex.InnerException});
                }
            }
            
            // Si no se cumple la condición anterior, realizar la búsqueda de personas y devolver los resultados en formato JSON
            var personasList = _personasService.BusquedaPersona(model);
            return Json(new { encontrada = false, data = personasList });
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
            //var model = json.ToObject<Gruas2Model>();
            //var errors = ModelState.Values.Select(s => s.Errors);
            //if (ModelState.IsValid)
            //{
            int id = _personasService.CreatePersona(model);
            model.PersonaDireccion.idPersona = id;
            int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);

            var modelList = _personasService.GetAllPersonas();
            return PartialView("_ListadoPersonas", modelList);
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
                var modelList = _personasService.GetAllPersonas();
                //var listPadronGruas = _concesionariosService.GetAllConcesionarios();
                return PartialView("_ListadoPersonas", modelList);
            }
            return RedirectToAction("Index");
        }



        [HttpPost]

        public ActionResult GuardaDesdeServicio(LicenciaPersonaDatos personaDatos)
        {
            try
            {
                 _personasService.InsertarDesdeServicio(personaDatos);
                var datosTabla = _personasService.BuscarPersonaSoloLicencia(personaDatos.NUM_LICENCIA);

                return Json(datosTabla);
            }
            catch (Exception ex)
            {
                // Maneja el error de manera adecuada
                return Json(new { error = "Error al guardar en la base de datos: " + ex.Message });
            }
        }
    }
}



