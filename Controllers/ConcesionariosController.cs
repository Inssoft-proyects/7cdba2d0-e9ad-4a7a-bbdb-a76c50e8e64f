using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class ConcesionariosController : BaseController
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
            int IdModulo = 600;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var modelList = _concesionariosService.GetAllConcesionarios(idOficina);
                return View(modelList);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }

        [HttpGet]
        public IActionResult ajax_ModalCrearConcesionario()
        {
            int IdModulo = 601;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
                var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
                ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
                ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
                return PartialView("_CrearConcesionario", new Concesionarios2Model());
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

            [HttpPost]
            public IActionResult ajax_CrearConcesionario(Concesionarios2Model model)
            {
                //var model = json.ToObject<Gruas2Model>();
                var errors = ModelState.Values.Select(s => s.Errors);
                if (ModelState.IsValid)
                {
                    int index = _concesionariosService.CrearConcesionario(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _concesionariosService.GetAllConcesionarios(idOficina);
                    return PartialView("_ListadoConcesionarios", listPadronGruas);
                }
                return RedirectToAction("Index");
            }

            [HttpGet]
            public IActionResult ajax_ModalEditarConcesionario(int idConcesionario)
            {
                int IdModulo = 602;
                string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
                List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
                if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
                {
                    var model = _concesionariosService.GetConcesionarioById(idConcesionario);
                    var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
                    var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
                    ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
                    ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
                    return PartialView("_EditarConcesionario", model);
                }
                else
                {
                    TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                    return Json(new { error = true });
                }
            }

            [HttpPost]
            public IActionResult ajax_EditarConcesionario(Concesionarios2Model model)
            {
                var errors = ModelState.Values.Select(s => s.Errors);
                if (ModelState.IsValid)
                {
                    int index = _concesionariosService.EditarConcesionario(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _concesionariosService.GetAllConcesionarios(idOficina);
                    return PartialView("_ListadoConcesionarios", listPadronGruas);
                }
                return RedirectToAction("Index");
            }
        }
    }

