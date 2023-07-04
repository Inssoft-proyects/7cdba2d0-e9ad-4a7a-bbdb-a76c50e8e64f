using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class PersonasController : Controller
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IPersonasService _personasService;

        public PersonasController(ICatDictionary catDictionary, IPersonasService personasService)
        {
            _catDictionary = catDictionary;
            _personasService = personasService;
        }
        public IActionResult Index()
        {
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var personasList = _personasService.GetAllPersonas();

            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            return View(personasList);
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
                //var listPadronGruas = _concesionariosService.GetAllConcesionarios();
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
    }
}
