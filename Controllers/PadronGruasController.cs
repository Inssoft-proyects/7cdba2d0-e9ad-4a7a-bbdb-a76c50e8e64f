using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class PadronGruasController : Controller
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IGruasService _gruasService;
        private readonly IConcesionariosService _concesionariosService;
        public PadronGruasController(ICatDictionary catDictionary,
                                     IGruasService gruasService,
                                     IConcesionariosService concesionariosService)
        {
            _catDictionary = catDictionary;
            _gruasService = gruasService;
            _concesionariosService = concesionariosService;
        }
        public IActionResult Index()
        {
            int IdModulo = 500;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                IEnumerable<Gruas2Model> listGruas = _gruasService.GetAllGruas(idOficina);
                var catTipoGruas = _catDictionary.GetCatalog("CatTiposGrua", "0");
                ViewBag.CatTipoGruas = new SelectList(catTipoGruas.CatalogList, "Id", "Text");
                return View(listGruas);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return Ok();
            }
        }

        [HttpGet]
        public ActionResult ajax_BuscarGruas(string placas, string noEconomico, int? idTipoGrua)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var listPadronGruas = _gruasService.GetGruasToGrid(placas, noEconomico, idTipoGrua, idOficina);

            return PartialView("_ListadoGruas", listPadronGruas);
        }


        /// <summary>
        /// Accion que redirige a la vista
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ajax_create()
        {
            int IdModulo = 501;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catClasificacionGruas = _catDictionary.GetCatalog("CatClasificacionGruas", "0");
            var catTipoGruas = _catDictionary.GetCatalog("CatTiposGrua", "0");
            var catSituacionGruas = _catDictionary.GetCatalog("CatSituacionGruas", "0");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatClasificacionGruas = new SelectList(catClasificacionGruas.CatalogList, "Id", "Text");
            ViewBag.CatTipoGruas = new SelectList(catTipoGruas.CatalogList, "Id", "Text");
            ViewBag.CatSituacionGruas = new SelectList(catSituacionGruas.CatalogList, "Id", "Text");
            return PartialView("_CrearGrua", new Gruas2Model());
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public IActionResult ajax_createGrua(Gruas2Model model)
        {
            //var model = json.ToObject<Gruas2Model>();
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int index = _gruasService.CrearGrua(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _gruasService.GetAllGruas(idOficina);
                return PartialView("_ListadoGruas", listPadronGruas);
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Accion que redirige a la vista
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ajax_edit(int idGrua)
        {
            int IdModulo = 502;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var model = _gruasService.GetGruaById(idGrua);
                var catConcesionarios = _concesionariosService.GetConcesionarios(idOficina).Where(w => w.IdConcesionario == model.idConcesionario);
                var catDelegacione = _catDictionary.GetCatalog("CatDelegaciones", "0");
                var catClasificacionGruas = _catDictionary.GetCatalog("CatClasificacionGruas", "0");
                var catTipoGruas = _catDictionary.GetCatalog("CatTiposGrua", "0");
                var catSituacionGruas = _catDictionary.GetCatalog("CatSituacionGruas", "0");
                ViewData["CatDelegaciones"] = new SelectList(catDelegacione.CatalogList, "Id", "Text");
                ViewData["CatClasificacionGruas"] = new SelectList(catClasificacionGruas.CatalogList, "Id", "Text");
                ViewData["CatTipoGruas"] = new SelectList(catTipoGruas.CatalogList, "Id", "Text");
                ViewData["CatSituacionGruas"] = new SelectList(catSituacionGruas.CatalogList, "Id", "Text");
                ViewData["CatConcesionarios"] = new SelectList(catConcesionarios, "IdConcesionario", "Concesionario");
                return PartialView("_EditarGrua", model);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public IActionResult ajax_editGrua(Gruas2Model model)
        {
            //var model = json.ToObject<Gruas2Model>();
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int index = _gruasService.EditarGrua(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _gruasService.GetAllGruas(idOficina);
                return PartialView("_ListadoGruas", listPadronGruas);
            }
            return RedirectToAction("Index");
        }

    }
}
