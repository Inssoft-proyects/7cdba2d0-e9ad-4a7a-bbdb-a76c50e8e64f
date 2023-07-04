using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class PensionesController : Controller
    {
        #region DPIServices

        private readonly IPensionesService _pensionesService;
        private readonly ICatDictionary _catDictionary;

        public PensionesController(
            ICatDictionary catDictionary,
            IPensionesService pensionesService)
        {
            _pensionesService = pensionesService;
            _catDictionary = catDictionary;
        }


        #endregion

        public IActionResult Index()
        {
            List<PensionModel> pensionesList = _pensionesService.GetAllPensiones();
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            return View(pensionesList);
        }

        [HttpGet]
        public ActionResult ajax_BuscarPensiones(string pension, int? idDelegacion)
        {
            var ListPensionesModel = _pensionesService.GetPensionesToGrid(pension, idDelegacion);
            return PartialView("_ListadoPensiones", ListPensionesModel);

        }


        [HttpPost]
        public ActionResult ajax_ModalCrearPension()
        {
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            return PartialView("_CrearPension", new PensionModel());
        }


        [HttpPost]
        public ActionResult ajax_CrearPension(PensionModel model)
        {
            //var errors = ModelState.Values.Select(s => s.Errors);
            //ModelState.Remove("CategoryName");
            if (ModelState.IsValid)
            {
                int idPension = _pensionesService.CrearPension(model);
                List<Gruas2Model> gruasPensionesList = _pensionesService.GetGruasDisponiblesByIdPension(idPension);
                model.IdPension = idPension;

                var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
                var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
                var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

                ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
                ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
                ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");

                ViewBag.ListadoGruasPensiones = gruasPensionesList;
                return PartialView("_EditarPension", model);
            }
            //SetDDLCategories();
            //return View("Create");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult ajax_ModalEditarPension(int idPension)
        {
            var model = _pensionesService.GetPensionById(idPension).FirstOrDefault();
            
            var gruasPensionesList = _pensionesService.GetGruasDisponiblesByIdPension(model.IdPension);

            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");

            ViewBag.ListadoGruasPensiones = gruasPensionesList;
            return PartialView("_EditarPension", model);
        }


        [HttpPost]
        public ActionResult ajax_EditarPension(PensionModel model)
        {
            if (ModelState.IsValid)
            {
                int idPension = _pensionesService.EditarGrua(model);
                int eliminaGruas = _pensionesService.EliminarPensionGruas(model.IdPension);
                if (!string.IsNullOrEmpty(model.strIdGruas))
                {
                    var strListIdGruas = model.strIdGruas.Split(',').Select(s=> Convert.ToInt32(s)).ToList();
                    int altaGruas = _pensionesService.CrearPensionGruas(model.IdPension, strListIdGruas);
                }
                List<PensionModel> pensionesList = _pensionesService.GetAllPensiones();
                return PartialView("_ListadoPensiones", pensionesList);
            }
            //SetDDLCategories();
            //return View("Create");
            return RedirectToAction("Index");
        }
    }
}
