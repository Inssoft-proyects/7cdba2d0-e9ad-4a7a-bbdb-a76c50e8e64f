using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatAutoridadesDisposicionController : BaseController
    {
        private readonly ICatAutoridadesDisposicionService _catAutoridadesDisposicionservice;

        public CatAutoridadesDisposicionController(ICatAutoridadesDisposicionService catAutoridadesDisposicionservice)
        {
            _catAutoridadesDisposicionservice = catAutoridadesDisposicionservice;
        }
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 964;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListAutoridadesDisposicionModel = GetAutoridadesDisposicion();

            return View(ListAutoridadesDisposicionModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }

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
       

        [HttpPost]
        public ActionResult AgregarAutoridadDisposicionPacial()
        {
            int IdModulo = 965;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                //SetDDLDependencias();
                return PartialView("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        public ActionResult EditarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
            int IdModulo = 966;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var autoridadesDisposicionModel = GetAutoridadDispByID(IdAutoridadDisposicion);
            return PartialView("_Editar", autoridadesDisposicionModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
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
