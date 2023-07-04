using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class ConcesionariosController : Controller
    {
        private readonly IConcesionariosService _concesionariosService;
        private readonly ICatDictionary _catDictionary;

        public ConcesionariosController(IConcesionariosService concesionariosService, ICatDictionary catDictionary)
        {
            _concesionariosService = concesionariosService;
            _catDictionary = catDictionary;
        }
        public IActionResult Index()
        {
            var modelList = _concesionariosService.GetAllConcesionarios();
            return View(modelList);
        }

        [HttpGet]
        public IActionResult ajax_ModalCrearConcesionario()
        {
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            return PartialView("_CrearConcesionario", new Concesionarios2Model());
        }

        [HttpPost]
        public IActionResult ajax_CrearConcesionario(Concesionarios2Model model)
        {
            //var model = json.ToObject<Gruas2Model>();
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int index = _concesionariosService.CrearConcesionario(model);
                var listPadronGruas = _concesionariosService.GetAllConcesionarios();
                return PartialView("_ListadoConcesionarios", listPadronGruas);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ajax_ModalEditarConcesionario(int idConcesionario)
        {
            var model = _concesionariosService.GetConcesionarioById(idConcesionario);
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            return PartialView("_EditarConcesionario", model);
        }

        [HttpPost]
        public IActionResult ajax_EditarConcesionario(Concesionarios2Model model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int index = _concesionariosService.EditarConcesionario(model);
                var listPadronGruas = _concesionariosService.GetAllConcesionarios();
                return PartialView("_ListadoConcesionarios", listPadronGruas);
            }
            return RedirectToAction("Index");
        }
    }
}
