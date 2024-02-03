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
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class MotivosInfraccionController : BaseController
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
			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            CatMotivosInfraccionModel searchModel = new CatMotivosInfraccionModel();
            List<CatMotivosInfraccionModel> listMotivosInfraccion = _motivoInfraccionService.GetMotivos(idDependencia);

            searchModel.ListMotivosInfraccion = listMotivosInfraccion;
            return View(searchModel);
            }
 

        #region Modal Action
        public ActionResult IndexModal()
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            CatMotivosInfraccionModel searchModel = new CatMotivosInfraccionModel();
            List<CatMotivosInfraccionModel> listMotivosInfraccion = _motivoInfraccionService.GetMotivos(idDependencia);

            searchModel.ListMotivosInfraccion = listMotivosInfraccion;
            return View(searchModel);
        }


        [HttpPost]
        public ActionResult AgregarMotivoParcial()
        {
              
                    var catConcepto = _catDictionary.GetCatalog("CatConceptoInfraccion", "0");
                ViewData["CatConcepto"] = new SelectList(catConcepto.CatalogList, "Id", "Text");
                return View("_Crear");
            }
    

        public ActionResult EditarParcial(int IdCatMotivoInfraccion)
        {
			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
       
                    var motivosInfraccionsModel = _motivoInfraccionService.GetMotivoByID(IdCatMotivoInfraccion, idDependencia);
                
                var catConcepto = _catDictionary.GetCatalog("CatConceptoInfraccion", "0");
                var catSubConcepto = _catDictionary.GetCatalog("CatSubConceptoInfraccion", motivosInfraccionsModel.idConcepto+"");
                ViewData["CatConcepto"] = new SelectList(catConcepto.CatalogList, "Id", "Text");
                ViewData["CatSubConceptoInfraccion"] = new SelectList(catSubConcepto.CatalogList, "Id", "Text");
                return View("_Editar", motivosInfraccionsModel);
                 }


        public ActionResult EliminarMotivoParcial(int IdCatMotivoInfraccion)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			var motivosInfraccionsModel = _motivoInfraccionService.GetMotivoByID(IdCatMotivoInfraccion, idDependencia);
            return View("_Eliminar", motivosInfraccionsModel);
        }
        public JsonResult Categories_Read()
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			var result = new SelectList(_motivoInfraccionService.GetCatMotivos(idDependencia), "IdCatMotivoInfraccion", "Nombre");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialMotivoModal(CatMotivosInfraccionModel model)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {


                CreateMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos(idDependencia);
                return Json(ListMotivosInfraccionModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcialModal(CatMotivosInfraccionModel model)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			bool switchMotivosInfraccion = Request.Form["motivosInfraccionSwitch"].Contains("true");
            model.estatus = switchMotivosInfraccion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            { 

                UpdateMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos(idDependencia);
                return Json(ListMotivosInfraccionModel);
            }
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarMotivoParcialModal(CatMotivosInfraccionModel model)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {


                DeleteMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos(idDependencia);
                return Json(ListMotivosInfraccionModel);
            }
            return PartialView("_Eliminar");
        }

        [HttpGet]
        public ActionResult BuscarMotivoByID(int idCatMotivoInfraccion)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			CatMotivosInfraccionModel motivo = _motivoInfraccionService.GetMotivoByID(idCatMotivoInfraccion, idDependencia);
            return Json(motivo);  
        }

        public JsonResult GetMotInf([DataSourceRequest] DataSourceRequest request)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos(idDependencia);

            return Json(ListMotivosInfraccionModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateMotivo(CatMotivosInfraccionModel model)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			_motivoInfraccionService.CrearMotivo(model, idDependencia);
        }

        public void UpdateMotivo(CatMotivosInfraccionModel model)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			_motivoInfraccionService.UpdateMotivo(model, idDependencia);

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
       public ActionResult ajax_BuscarMotivos(CatMotivosInfraccionModel model)
        {

            int idDependencia = HttpContext.Session.GetInt32("IdDependencia") ?? 0;
            var ListMotivos = _motivoInfraccionService.GetMotivosBusqueda(model, idDependencia);
            if (ListMotivos.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListaMotivosInfraccion", ListMotivos);

        }


    }
}
