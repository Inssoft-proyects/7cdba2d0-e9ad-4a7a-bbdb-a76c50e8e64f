using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.WebUI.Controllers
{
    public class CatAutoridadesDisposicionController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //var products = dbContext.Products.ToList();
            var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();

            return View(ListAutoridadesDisposicionModel);

        }


        [HttpPost]
        public IActionResult Agregar(CatAutoridadesDisposicionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {
                //Crear el producto

                CrearAutoridadDisp(model);
                return RedirectToAction("Index");
            }
            return View("_Agregar");
        }


        [HttpGet]
        public IActionResult Editar(int IdAutoridadDisposicion)
        {
            var autoridadesDisposicionModel = GetAutoridadDispByID(IdAutoridadDisposicion);
            return View(autoridadesDisposicionModel);
        }


        [HttpPost]
        public ActionResult Update(CatAutoridadesDisposicionModel model)
        {
            bool switchAutoridadesDisposicion = Request.Form["autoridadesDisposicionSwitch"].Contains("true");
            model.Estatus = switchAutoridadesDisposicion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {


                EditarAutoridadDisp(model);
                var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();
                return PartialView("_ListaAutoridadesDisposicion", ListAutoridadesDisposicionModel);
            }

            return PartialView("_Editar");
        }

        [HttpGet]
        public IActionResult Eliminar(int IdAutoridadDisposicion)
        {

            var autoridadesDisposicionModel = GetAutoridadDispByID(IdAutoridadDisposicion);
            return View(autoridadesDisposicionModel);
        }


        [HttpPost]
        public IActionResult Eliminar(CatAutoridadesDisposicionModel autoridadesDisposicionModel)
        {
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {
                //Modificiacion del registro
                EliminarAutoridadDisp(autoridadesDisposicionModel);
                return RedirectToAction("Index");
            }
            return View("Delete");
        }



        ///Crear metodo de update (post)


        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListAuturidadesDisposicionsModel = GetAutoridadesDisposicion();
            //return View("IndexModal");
            return View("Index", ListAuturidadesDisposicionsModel);
        }

        [HttpPost]
        public ActionResult AgregarAutoridadDisposicionPacial()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        public ActionResult EditarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
            var autoridadesDisposicionModel = GetAutoridadDispByID(IdAutoridadDisposicion);
            return PartialView("_Editar", autoridadesDisposicionModel);
        }

        public ActionResult EliminarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
            var autoridadesDisposicionModel = GetAutoridadDispByID(IdAutoridadDisposicion);
            return PartialView("_Eliminar", autoridadesDisposicionModel);
        }







        [HttpPost]
        public ActionResult CrearAutoridadDisposicionParcialModal(CatAutoridadesDisposicionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {


                CrearAutoridadDisp(model);
                var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();
                return PartialView("_ListaAutoridadesDisposicion", ListAutoridadesDisposicionModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarAutoridadDisposicionlModal(CatAutoridadesDisposicionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {


                EditarAutoridadDisp(model);
                var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();
                return PartialView("_ListaAutoridadesDisposicion", ListAutoridadesDisposicionModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

        public ActionResult EliminarAutoridadDisposicionModal(CatAutoridadesDisposicionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {


                EliminarAutoridadDisp(model);
                var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();
                return PartialView("_ListaAutoridadesDisposicion", ListAutoridadesDisposicionModel);
            }

            return PartialView("_Eliminar");
        }
        public JsonResult GetAutDisp([DataSourceRequest] DataSourceRequest request)
        {
            var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();

            return Json(ListAutoridadesDisposicionModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearAutoridadDisp(CatAutoridadesDisposicionModel model)
        {
            CatAutoridadesDisposicion autoridad = new CatAutoridadesDisposicion();
            autoridad.IdAutoridadDisposicion = model.IdAutoridadDisposicion;
            autoridad.NombreAutoridadDisposicion = model.NombreAutoridadDisposicion;
            autoridad.Estatus = 1;
            autoridad.FechaActualizacion = DateTime.Now;
            dbContext.CatAutoridadesDisposicion.Add(autoridad);
            dbContext.SaveChanges();
        }

        public void EditarAutoridadDisp(CatAutoridadesDisposicionModel model)
        {
            CatAutoridadesDisposicion autoridad = new CatAutoridadesDisposicion();
            autoridad.IdAutoridadDisposicion = model.IdAutoridadDisposicion;
            autoridad.NombreAutoridadDisposicion = model.NombreAutoridadDisposicion;
            autoridad.Estatus = model.Estatus;
            autoridad.FechaActualizacion = DateTime.Now;
            dbContext.Entry(autoridad).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminarAutoridadDisp(CatAutoridadesDisposicionModel model)
        {

            CatAutoridadesDisposicion autoridad = new CatAutoridadesDisposicion();
            autoridad.IdAutoridadDisposicion = model.IdAutoridadDisposicion;
            autoridad.NombreAutoridadDisposicion = model.NombreAutoridadDisposicion;
            autoridad.Estatus = 0;
            autoridad.FechaActualizacion = DateTime.Now;
            dbContext.Entry(autoridad).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatAutoridadesDisposicionModel GetAutoridadDispByID(int IdAutoridadDisposicion)
        {

            var productEnitity = dbContext.CatAutoridadesDisposicion.Find(IdAutoridadDisposicion);

            var autoridadesDisposicionModel = (from catAutoridadesDisposicion in dbContext.CatAutoridadesDisposicion.ToList()
                                               select new CatAutoridadesDisposicionModel

                                               {
                                                   IdAutoridadDisposicion = catAutoridadesDisposicion.IdAutoridadDisposicion,
                                                   NombreAutoridadDisposicion = catAutoridadesDisposicion.NombreAutoridadDisposicion,


                                               }).Where(w => w.IdAutoridadDisposicion == IdAutoridadDisposicion).FirstOrDefault();

            return autoridadesDisposicionModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatAutoridadesDisposicionModel> GetAutoridadesDisposicion()
        {
            var ListAutoridadesDisposicionModel = (from catAutoridadesDisposicion in dbContext.CatAutoridadesDisposicion.ToList()
                                                   join estatus in dbContext.Estatus.ToList()
                                                   on catAutoridadesDisposicion.Estatus equals estatus.estatus
                                                   select new CatAutoridadesDisposicionModel
                                                   {
                                                       IdAutoridadDisposicion = catAutoridadesDisposicion.IdAutoridadDisposicion,
                                                       NombreAutoridadDisposicion = catAutoridadesDisposicion.NombreAutoridadDisposicion,
                                                       estatusDesc = estatus.estatusDesc

                                                   }).ToList();
            return ListAutoridadesDisposicionModel;
        }
        #endregion



    }
}
