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
    public class CatAutoridadesEntregaController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //var products = dbContext.Products.ToList();
            var ListAutoridadesEntregaModel = GetAutoridadesEntrega();

            return View(ListAutoridadesEntregaModel);

        }




        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListAutoridadesEntregaModel = GetAutoridadesEntrega();
            return View("Index", ListAutoridadesEntregaModel);
        }

        [HttpPost]
        public ActionResult AgregarAutoridadEntregaModal()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        public ActionResult EditarAutoridadEntregaModal(int IdAutoridadEntrega)
        {
            var autoridadesEntregaModel = GetAutoridadEntregaByID(IdAutoridadEntrega);
            return PartialView("_Editar", autoridadesEntregaModel);
        }

        public ActionResult EliminarAutoridadEntregaModal(int IdAutoridadEntrega)
        {
            var autoridadesEntregaModel = GetAutoridadEntregaByID(IdAutoridadEntrega);
            return PartialView("_Eliminar", autoridadesEntregaModel);
        }



        [HttpPost]
        public ActionResult AgregarAutoridadEntrega(CatAutoridadesEntregaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                CrearAutoridadEntrega(model);
                var ListAutoridadesEntregaModel = GetAutoridadesEntrega();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarAutoridadEntregaMod(CatAutoridadesEntregaModel model)
        {
            bool switchAutEntrega = Request.Form["autEntregaSwitch"].Contains("true");
            model.Estatus = switchAutEntrega ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                EditarAutoridadEntrega(model);
                var ListAutoridadesEntregaModel = GetAutoridadesEntrega();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

        public ActionResult EliminarAutoridadEntregaMod(CatAutoridadesEntregaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                EliminaAutoridadEntrega(model);
                var ListAutoridadesEntregaModel = GetAutoridadesEntrega();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetAutEntrega([DataSourceRequest] DataSourceRequest request)
        {
            var ListAutoridadesEntregaModel = GetAutoridadesEntrega();

            return Json(ListAutoridadesEntregaModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearAutoridadEntrega(CatAutoridadesEntregaModel model)
        {
            CatAutoridadesEntrega autoridadEnt = new CatAutoridadesEntrega();
            autoridadEnt.IdAutoridadEntrega = model.IdAutoridadEntrega;
            autoridadEnt.AutoridadEntrega = model.AutoridadEntrega;
            autoridadEnt.Estatus = 1;
            autoridadEnt.FechaActualizacion = DateTime.Now;
            dbContext.CatAutoridadesEntrega.Add(autoridadEnt);
            dbContext.SaveChanges();
        }

        public void EditarAutoridadEntrega(CatAutoridadesEntregaModel model)
        {
            CatAutoridadesEntrega autoridadEnt = new CatAutoridadesEntrega();
            autoridadEnt.IdAutoridadEntrega = model.IdAutoridadEntrega;
            autoridadEnt.AutoridadEntrega = model.AutoridadEntrega;
            autoridadEnt.Estatus = model.Estatus;
            autoridadEnt.FechaActualizacion = DateTime.Now;
            dbContext.Entry(autoridadEnt).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminaAutoridadEntrega(CatAutoridadesEntregaModel model)
        {

            CatAutoridadesEntrega autoridadEnt = new CatAutoridadesEntrega();
            autoridadEnt.IdAutoridadEntrega = model.IdAutoridadEntrega;
            autoridadEnt.AutoridadEntrega = model.AutoridadEntrega;
            autoridadEnt.Estatus = 0;
            autoridadEnt.FechaActualizacion = DateTime.Now;
            dbContext.Entry(autoridadEnt).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatAutoridadesEntregaModel GetAutoridadEntregaByID(int IdAutoridadEntrega)
        {

            var productEnitity = dbContext.CatAutoridadesEntrega.Find(IdAutoridadEntrega);

            var autoridadesEntregaModel = (from catAutoridadesEntrega in dbContext.CatAutoridadesEntrega.ToList()
                                           select new CatAutoridadesEntregaModel

                                           {
                                               IdAutoridadEntrega = catAutoridadesEntrega.IdAutoridadEntrega,
                                               AutoridadEntrega = catAutoridadesEntrega.AutoridadEntrega,


                                           }).Where(w => w.IdAutoridadEntrega == IdAutoridadEntrega).FirstOrDefault();

            return autoridadesEntregaModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatAutoridadesEntregaModel> GetAutoridadesEntrega()
        {
            var ListAutoridadesEntregaModel = (from catAutoridadesEntrega in dbContext.CatAutoridadesEntrega.ToList()
                                               join estatus in dbContext.Estatus.ToList()
                                               on catAutoridadesEntrega.Estatus equals estatus.estatus
                                               where catAutoridadesEntrega.Estatus == 1
                                               select new CatAutoridadesEntregaModel
                                               {
                                                   IdAutoridadEntrega = catAutoridadesEntrega.IdAutoridadEntrega,
                                                   AutoridadEntrega = catAutoridadesEntrega.AutoridadEntrega,
                                                   estatusDesc = estatus.estatusDesc

                                               }).ToList();
            return ListAutoridadesEntregaModel;
        }
        #endregion



    }
}
