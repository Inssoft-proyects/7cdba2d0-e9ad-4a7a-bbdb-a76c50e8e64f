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
        DBContextInssoft dbContext = new DBContextInssoft();
        private readonly ICatDictionary _catDictionary;

        public MotivosInfraccionController(ICatDictionary catDictionary)
        {
            _catDictionary = catDictionary;
        }

        public IActionResult Index()
        {
            int IdModulo = 944;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListMotivosInfraccionModel = GetMotivos();

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
            var ListMotivosInfraccionModel = GetMotivos();
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
                return PartialView("_Crear");
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
                var motivosInfraccionsModel = GetMotivoByID(IdCatMotivoInfraccion);
                
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
            var motivosInfraccionsModel = GetMotivoByID(IdCatMotivoInfraccion);
            return View("_Eliminar", motivosInfraccionsModel);
        }
        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.MotivosInfraccion.ToList(), "IdMotivoInfraccion", "Nombre");
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
                var ListMotivosInfraccionModel = GetMotivos();
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
                var ListMotivosInfraccionModel = GetMotivos();
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
                var ListMotivosInfraccionModel = GetMotivos();
                return PartialView("_ListaMotivosInfraccion", ListMotivosInfraccionModel);
            }
            return PartialView("_Eliminar");
        }

        [HttpGet]
        public ActionResult BuscarMotivoByID(int idCatMotivoInfraccion)
        {
            CatMotivosInfraccionModel motivo = GetMotivoByID(idCatMotivoInfraccion);
            return Json(motivo);  
        }

        public JsonResult GetMotInf([DataSourceRequest] DataSourceRequest request)
        {
            var ListMotivosInfraccionModel = GetMotivos();

            return Json(ListMotivosInfraccionModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateMotivo(CatMotivosInfraccionModel model)
        {
            CatMotivosInfraccion motivo = new CatMotivosInfraccion();
            motivo.idCatMotivoInfraccion = model.IdCatMotivoInfraccion;
            motivo.Nombre = model.Nombre;
            motivo.CalificacionMinima = model.CalificacionMinima;
            motivo.CalificacionMaxima = model.CalificacionMaxima;
            motivo.Fundamento = model.Fundamento;
            motivo.IdConcepto = model.idConcepto;
            motivo.IdSubConcepto = model.idSubConcepto;
            motivo.Estatus = 1;
            motivo.FechaActualizacion = DateTime.Now;
            dbContext.CatMotivosInfracciones.Add(motivo);
            dbContext.SaveChanges();
        }

        public void UpdateMotivo(CatMotivosInfraccionModel model)
        {
            CatMotivosInfraccion motivo = new CatMotivosInfraccion();
            motivo.idCatMotivoInfraccion = model.IdCatMotivoInfraccion;
            motivo.Nombre = model.Nombre;
            motivo.Fundamento = model.Fundamento;
            motivo.CalificacionMinima = model.CalificacionMinima;
            motivo.CalificacionMaxima = model.CalificacionMaxima;
            motivo.IdConcepto = model.idConcepto;
            motivo.IdSubConcepto = model.idSubConcepto;
            motivo.Estatus = model.estatus;
            motivo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(motivo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteMotivo(CatMotivosInfraccionModel model)
        {
            CatMotivosInfraccion motivo = new CatMotivosInfraccion();
            motivo.idCatMotivoInfraccion = model.IdCatMotivoInfraccion;
            motivo.Nombre = model.Nombre;
            motivo.Fundamento = model.Fundamento;
            motivo.CalificacionMinima = model.CalificacionMinima;
            motivo.CalificacionMaxima = model.CalificacionMaxima;
            motivo.Estatus = 0;
            motivo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(motivo).State = EntityState.Modified;
            dbContext.SaveChanges();


        }

        /* private void SetDDLColores()
         {
             ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
             ViewBag.Categories = new SelectList(dbContext.Color.ToList(), "IdColor", "color");
         }*/


        public CatMotivosInfraccionModel GetMotivoByID(int IdCatMotivoInfraccion)
        {

            var productEnitity = dbContext.CatMotivosInfracciones.Find(IdCatMotivoInfraccion);

            var motivosInfraccionModel = (from catMotivosInfraccion in dbContext.CatMotivosInfracciones.ToList()
                                          join estatus in dbContext.Estatus.ToList()
                                                on catMotivosInfraccion.Estatus equals estatus.estatus

                                          join concepto in dbContext.CatConceptosInfraccion.ToList()
                                            on catMotivosInfraccion.IdConcepto equals concepto.idConcepto

                                          join subconcepto in dbContext.CatSubConceptosInfraccion.ToList()
                                            on catMotivosInfraccion.IdSubConcepto equals subconcepto.idSubConcepto

                                          select new CatMotivosInfraccionModel

                                          {
                                              IdCatMotivoInfraccion = catMotivosInfraccion.idCatMotivoInfraccion,
                                              Nombre = catMotivosInfraccion.Nombre,
                                              Fundamento = catMotivosInfraccion.Fundamento,
                                              CalificacionMinima = catMotivosInfraccion.CalificacionMinima,
                                              CalificacionMaxima = catMotivosInfraccion.CalificacionMaxima,
                                              idConcepto = catMotivosInfraccion.IdConcepto,
                                              concepto = concepto.concepto,
                                              idSubConcepto = catMotivosInfraccion.IdSubConcepto,
                                              subConcepto = subconcepto.subConcepto,
                                              ValorEstatusMotivosInfraccion = estatus.estatusDesc == "activo" 
                                              

                                          }).Where(w => w.IdCatMotivoInfraccion == IdCatMotivoInfraccion).FirstOrDefault();

            return motivosInfraccionModel;
        }

        public List<CatMotivosInfraccionModel> GetMotivos()
        {
            var ListMotivosInfraccionModel = (from catMotivosInfraccion in dbContext.CatMotivosInfracciones.ToList()
                                              join estatus in dbContext.Estatus.ToList()
                                                on catMotivosInfraccion.Estatus equals estatus.estatus

                                              join concepto in dbContext.CatConceptosInfraccion.ToList()
                                                on catMotivosInfraccion.IdConcepto equals concepto.idConcepto

                                              join subconcepto in dbContext.CatSubConceptosInfraccion.ToList()
                                                on catMotivosInfraccion.IdSubConcepto equals subconcepto.idSubConcepto
                                              where catMotivosInfraccion.Estatus == 1

                                              select new CatMotivosInfraccionModel
                                              {
                                                  IdCatMotivoInfraccion = catMotivosInfraccion.idCatMotivoInfraccion,
                                                  Nombre = catMotivosInfraccion.Nombre,
                                                  Fundamento = catMotivosInfraccion.Fundamento,
                                                  CalificacionMinima = catMotivosInfraccion.CalificacionMinima,
                                                  CalificacionMaxima = catMotivosInfraccion.CalificacionMaxima,
                                                  idConcepto = catMotivosInfraccion.IdConcepto,
                                                  concepto = concepto.concepto,
                                                  idSubConcepto = catMotivosInfraccion.IdSubConcepto,
                                                  subConcepto = subconcepto.subConcepto,
                                                  estatus = catMotivosInfraccion.Estatus,
                                                  estatusDesc = estatus.estatusDesc,
                                              }).ToList();
            return ListMotivosInfraccionModel;
        }
        #endregion



    }
}
