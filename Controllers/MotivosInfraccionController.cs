using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.WebUI.Controllers
{
    public class MotivosInfraccionController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //var products = dbContext.Products.ToList();
            var ListMotivosInfraccionModel = GetMotivos();

            return View(ListMotivosInfraccionModel);

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
            return PartialView("_Crear");
        }

        public ActionResult EditarParcial(int IdMotivoInfraccion)
        {
            var motivosInfraccionsModel = GetMotivoByID(IdMotivoInfraccion);
            return View("_Editar", motivosInfraccionsModel);
        }

        public ActionResult EliminarMotivoParcial(int IdMotivoInfraccion)
        {
            var motivosInfraccionsModel = GetMotivoByID(IdMotivoInfraccion);
            return View("_Eliminar", motivosInfraccionsModel);
        }
        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.MotivosInfraccion.ToList(), "IdMotivoInfraccion", "Nombre");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialMotivoModal(MotivosInfraccionModel model)
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
        public ActionResult EditarParcialModal(MotivosInfraccionModel model)
        {
            bool switchMotivosInfraccion = Request.Form["motivosInfraccionSwitch"].Contains("true");
            model.Estatus = switchMotivosInfraccion ? 1 : 0;
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
        public ActionResult EliminarMotivoParcialModal(MotivosInfraccionModel model)
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
        public JsonResult GetMotInf([DataSourceRequest] DataSourceRequest request)
        {
            var ListMotivosInfraccionModel = GetMotivos();

            return Json(ListMotivosInfraccionModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateMotivo(MotivosInfraccionModel model)
        {
            MotivosInfraccion motivo = new MotivosInfraccion();
            motivo.IdMotivoInfraccion = model.IdMotivoInfraccion;
            motivo.Nombre = model.Nombre;
            motivo.CalificacionMinima = model.CalificacionMinima;
            motivo.CalificacionMaxima = model.CalificacionMaxima;
            motivo.Fundamento = model.Fundamento;
            motivo.Estatus = 1;
            motivo.FechaActualizacion = DateTime.Now;
            dbContext.MotivosInfraccion.Add(motivo);
            dbContext.SaveChanges();
        }

        public void UpdateMotivo(MotivosInfraccionModel model)
        {
            MotivosInfraccion motivo = new MotivosInfraccion();
            motivo.IdMotivoInfraccion = model.IdMotivoInfraccion;
            motivo.Nombre = model.Nombre;
            motivo.Fundamento = model.Fundamento;
            motivo.CalificacionMinima = model.CalificacionMinima;
            motivo.CalificacionMaxima = model.CalificacionMaxima;
            motivo.Estatus = model.Estatus;
            motivo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(motivo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteMotivo(MotivosInfraccionModel model)
        {
            MotivosInfraccion motivo = new MotivosInfraccion();
            motivo.IdMotivoInfraccion = model.IdMotivoInfraccion;
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


        public MotivosInfraccionModel GetMotivoByID(int IdMotivoInfraccion)
        {

            var productEnitity = dbContext.MotivosInfraccion.Find(IdMotivoInfraccion);

            var motivosInfraccionModel = (from motivosInfraccion in dbContext.MotivosInfraccion.ToList()
                                          select new MotivosInfraccionModel

                                          {
                                              IdMotivoInfraccion = motivosInfraccion.IdMotivoInfraccion,
                                              Nombre = motivosInfraccion.Nombre,
                                              CalificacionMinima = motivosInfraccion.CalificacionMinima,
                                              CalificacionMaxima = motivosInfraccion.CalificacionMaxima,
                                              Fundamento = motivosInfraccion.Fundamento

                                          }).Where(w => w.IdMotivoInfraccion == IdMotivoInfraccion).FirstOrDefault();

            return motivosInfraccionModel;
        }


        public List<MotivosInfraccionModel> GetMotivos()
        {
            var ListMotivosInfraccionModel = (from motivosInfraccion in dbContext.MotivosInfraccion.ToList()
                                              join estatus in dbContext.Estatus.ToList()
                                              on motivosInfraccion.Estatus equals estatus.estatus
                                              where motivosInfraccion.Estatus == 1

                                              select new MotivosInfraccionModel
                                              {
                                                  IdMotivoInfraccion = motivosInfraccion.IdMotivoInfraccion,
                                                  Nombre = motivosInfraccion.Nombre,
                                                  Fundamento = motivosInfraccion.Fundamento,
                                                  CalificacionMinima = motivosInfraccion.CalificacionMinima,
                                                  CalificacionMaxima = motivosInfraccion.CalificacionMaxima,
                                                  Estatus = motivosInfraccion.Estatus,
                                                  EstatusDesc = estatus.estatusDesc,
                                              }).ToList();
            return ListMotivosInfraccionModel;
        }
        #endregion



    }
}
