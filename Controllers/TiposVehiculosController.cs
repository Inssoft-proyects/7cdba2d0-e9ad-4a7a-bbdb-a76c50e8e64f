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
    public class TiposVehiculosController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //var products = dbContext.Products.ToList();
            var ListTiposVehiculosModel = GetTiposVehiculos();

            return View(ListTiposVehiculosModel);

        }


        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListTiposVehiculosModel = GetTiposVehiculos();
            //return View("IndexModal");
            return View("Index", ListTiposVehiculosModel);
        }

        [HttpPost]
        public ActionResult AgregarTipoVehiculo()
        {

            //SetDDLDependencias();
            return PartialView("_Crear");
        }


        [HttpPost]
        public ActionResult EditarTipoVehiculo(int IdTipoVehiculo)
        {

            var tiposVehiculosModel = GetTipoVehiculoByID(IdTipoVehiculo);
            return View("_Editar", tiposVehiculosModel);
        }

        [HttpPost]
        public ActionResult EliminarTipoVehiculoParcial(int IdTipoVehiculo)
        {

            var tiposVehiculosModel = GetTipoVehiculoByID(IdTipoVehiculo);
            return View("_Eliminar", tiposVehiculosModel);
        }

        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.TipoVehiculos.ToList(), "IdTipoVehiculo", "TipoVehiculo");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialTipoModal(TiposVehiculosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("TipoVehiculo");
            if (ModelState.IsValid)
            {
                CreateTipoVehiculo(model);
                var ListTiposVehiculosModel = GetTiposVehiculos();
                return PartialView("_ListaTiposVehiculos", ListTiposVehiculosModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        public ActionResult UpdatePartialTipoModal(TiposVehiculosModel model)
        {
            bool switchTiposVehiculos = Request.Form["tiposVehiculoSwitch"].Contains("true");
            model.Estatus = switchTiposVehiculos ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("TipoVehiculo");
            if (ModelState.IsValid)
            {
               
                UpdateTipoVehiculo(model);
                var ListTiposVehiculosModel = GetTiposVehiculos();
                return PartialView("_ListaTiposVehiculos", ListTiposVehiculosModel);
            }

            return PartialView("_Editar");
        }

        public ActionResult EliminarPartialTipoVehiculoModal(TiposVehiculosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("TipoVehiculo");
            if (ModelState.IsValid)
            {
                DeleteTipoVehiculo(model);
                var ListTiposVehiculosModel = GetTiposVehiculos();
                return PartialView("_ListaTiposVehiculos", ListTiposVehiculosModel);
            }

            return PartialView("_Eliminar");
        }

        public JsonResult GetTipos([DataSourceRequest] DataSourceRequest request)
        {
            var ListTiposVehiculosModel = GetTiposVehiculos();

            return Json(ListTiposVehiculosModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateTipoVehiculo(TiposVehiculosModel model)
        {
            TipoVehiculos tipo = new TipoVehiculos();
            tipo.IdTipoVehiculo = model.IdTipoVehiculo;
            tipo.TipoVehiculo = model.TipoVehiculo;
            tipo.Estatus = 1;
            tipo.FechaActualizacion = DateTime.Now;
            dbContext.TipoVehiculos.Add(tipo);
            dbContext.SaveChanges();
        }

        public void UpdateTipoVehiculo(TiposVehiculosModel model)
        {
            TipoVehiculos tipo = new TipoVehiculos();
            tipo.IdTipoVehiculo = model.IdTipoVehiculo;
            tipo.TipoVehiculo = model.TipoVehiculo;
            tipo.Estatus = model.Estatus;
            tipo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(tipo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteTipoVehiculo(TiposVehiculosModel model)
        {
            TipoVehiculos tipo = new TipoVehiculos();
            tipo.IdTipoVehiculo = model.IdTipoVehiculo;
            tipo.TipoVehiculo = model.TipoVehiculo;
            tipo.Estatus = 0;
            tipo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(tipo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        private void SetDDLTiposVehiculos()
        {
            ViewBag.Categories = new SelectList(dbContext.TipoVehiculos.ToList(), "IdTipoVehiculo", "TipoVehiculo");
        }


        public TiposVehiculosModel GetTipoVehiculoByID(int IdTipoVehiculo)
        {

            var productEnitity = dbContext.TipoVehiculos.Find(IdTipoVehiculo);

            var tipoVehiculoModel = (from tiposVehiculo in dbContext.TipoVehiculos.ToList()
                                     select new TiposVehiculosModel

                                     {
                                         IdTipoVehiculo = tiposVehiculo.IdTipoVehiculo,
                                         TipoVehiculo = tiposVehiculo.TipoVehiculo,


                                     }).Where(w => w.IdTipoVehiculo == IdTipoVehiculo).FirstOrDefault();

            return tipoVehiculoModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<TiposVehiculosModel> GetTiposVehiculos()
        {
            var ListTiposVehiculosModel = (from tiposVehiculo in dbContext.TipoVehiculos.ToList()
                                           join Estatus in dbContext.Estatus.ToList()
                                           on tiposVehiculo.Estatus equals Estatus.estatus


                                           select new TiposVehiculosModel
                                           {
                                               IdTipoVehiculo = tiposVehiculo.IdTipoVehiculo,
                                               TipoVehiculo = tiposVehiculo.TipoVehiculo,
                                               Estatus = tiposVehiculo.Estatus,
                                               estatusDesc = Estatus.estatusDesc

                                           }).ToList();
            return ListTiposVehiculosModel;
        }
        #endregion



    }
}
