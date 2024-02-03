using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class PensionesController : BaseController
    {
        #region DPIServices

        private readonly IPensionesService _pensionesService;
        private readonly ICatDictionary _catDictionary;
        private readonly IBitacoraService _bitacoraServices;
        public PensionesController(
            ICatDictionary catDictionary,
            IPensionesService pensionesService,
            IBitacoraService bitacoraServices)
        {
            _pensionesService = pensionesService;
            _catDictionary = catDictionary;
            _bitacoraServices = bitacoraServices;
        }


        #endregion

        public IActionResult Index()
        {
   
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

               // List<PensionModel> pensionesList = _pensionesService.GetAllPensiones(idOficina);
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            return View();
 
        }

        [HttpGet]
        public ActionResult ajax_BuscarPensiones(string pension, int? idDelegacion)
        {
          
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                var ListPensionesModel = _pensionesService.GetPensionesToGrid(pension, idOficina);
                if (ListPensionesModel.Count == 0)
                {
                    ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
                }
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
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                int idPension = _pensionesService.CrearPension(model);
                List<Gruas2Model> gruasPensionesList = _pensionesService.GetGruasDisponiblesByIdPension(idPension, idOficina);
                model.IdPension = idPension;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

                //BITACORA
                _bitacoraServices.insertBitacora(idPension, ip, "Pension", "Insertar", "Insert", user);

                var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
                var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
                var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

                ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
                ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
                ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");

                ViewBag.ListadoGruasPensiones = gruasPensionesList;
                return Json (gruasPensionesList);

            }
            //SetDDLCategories();
            //return View("Create");
            return View(model);

        }


        [HttpGet]
        public ActionResult ajax_ModalEditarPension(int idPension)
        {
  
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                var model = _pensionesService.GetPensionById(idPension,idOficina).FirstOrDefault();
            
            var gruasPensionesList = _pensionesService.GetGruasDisponiblesByIdPension(model.IdPension,idOficina);

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
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                
                //BITACORA
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(idPension, ip, "Pension", "Actualizar", "Update", user);

                List<PensionModel> pensionesList = _pensionesService.GetAllPensiones(idOficina);
                return PartialView("_ListadoPensiones", pensionesList);
            }
            //SetDDLCategories();
            //return View("Create");
            return RedirectToAction("Index");
        }
    }
}
