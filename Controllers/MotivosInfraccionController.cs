using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;

namespace Example.WebUI.Controllers
{
    public class MotivosInfraccionController : Controller
    {
        //DBContextInssoft dbContext = new DBContextInssoft();
        private readonly ICatDictionary _catDictionary;
        private readonly IMotivoInfraccionService _motivoInfraccionService;

        public MotivosInfraccionController(ICatDictionary catDictionary, IMotivoInfraccionService motivoInfraccionService)
        { 
            _catDictionary = catDictionary;
            _motivoInfraccionService = motivoInfraccionService;
        }

        public IActionResult Index()
        {
            int IdModulo = 944;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos();

            return View(ListMotivosInfraccionModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }

        }



        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos();
            //return View("IndexModal");
            return View("Index", ListMotivosInfraccionModel);
        }

        [HttpPost]
        public ActionResult AgregarMotivoParcial()
        {
            int IdModulo = 945;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var catConcepto = _catDictionary.GetCatalog("CatConceptoInfraccion", "0");
                ViewData["CatConcepto"] = new SelectList(catConcepto.CatalogList, "Id", "Text");
                return View("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        public ActionResult EditarParcial(int IdCatMotivoInfraccion)
        {
            int IdModulo = 946;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var motivosInfraccionsModel = _motivoInfraccionService.GetMotivoByID(IdCatMotivoInfraccion);
                
                var catConcepto = _catDictionary.GetCatalog("CatConceptoInfraccion", "0");
                var catSubConcepto = _catDictionary.GetCatalog("CatSubConceptoInfraccion", motivosInfraccionsModel.idConcepto+"");
                ViewData["CatConcepto"] = new SelectList(catConcepto.CatalogList, "Id", "Text");
                ViewData["CatSubConceptoInfraccion"] = new SelectList(catSubConcepto.CatalogList, "Id", "Text");
                return View("_Editar", motivosInfraccionsModel);
        }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
    }
}

        public ActionResult EliminarMotivoParcial(int IdCatMotivoInfraccion)
        {
            var motivosInfraccionsModel = _motivoInfraccionService.GetMotivoByID(IdCatMotivoInfraccion);
            return View("_Eliminar", motivosInfraccionsModel);
        }
        public JsonResult Categories_Read()
        {
            var result = new SelectList(_motivoInfraccionService.GetCatMotivos(), "IdCatMotivoInfraccion", "Nombre");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialMotivoModal(CatMotivosInfraccionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {


                CreateMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos();
                return PartialView("_ListaMotivosInfraccion", ListMotivosInfraccionModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcialModal(CatMotivosInfraccionModel model)
        {
            bool switchMotivosInfraccion = Request.Form["motivosInfraccionSwitch"].Contains("true");
            model.estatus = switchMotivosInfraccion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            { 

                UpdateMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos();
                return PartialView("_ListaMotivosInfraccion", ListMotivosInfraccionModel);
            }
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarMotivoParcialModal(CatMotivosInfraccionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {


                DeleteMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos();
                return PartialView("_ListaMotivosInfraccion", ListMotivosInfraccionModel);
            }
            return PartialView("_Eliminar");
        }

        [HttpGet]
        public ActionResult BuscarMotivoByID(int idCatMotivoInfraccion)
        {
            CatMotivosInfraccionModel motivo = _motivoInfraccionService.GetMotivoByID(idCatMotivoInfraccion);
            return Json(motivo);  
        }

        public JsonResult GetMotInf([DataSourceRequest] DataSourceRequest request)
        {
            var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos();

            return Json(ListMotivosInfraccionModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateMotivo(CatMotivosInfraccionModel model)
        {
            _motivoInfraccionService.CrearMotivo(model);
        }

        public void UpdateMotivo(CatMotivosInfraccionModel model)
        {
            _motivoInfraccionService.UpdateMotivo(model);

        }

        public void DeleteMotivo(CatMotivosInfraccionModel model)
        {
            _motivoInfraccionService.DeleteMotivo(model);
        }

        /* private void SetDDLColores()
         {
             ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
             ViewBag.Categories = new SelectList(dbContext.Color.ToList(), "IdColor", "color");
         }*/

        
        #endregion



    }
}
