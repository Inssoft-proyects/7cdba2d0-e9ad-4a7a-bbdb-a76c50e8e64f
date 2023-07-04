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
    public class CatInstitucionesTrasladoController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //var products = dbContext.Products.ToList();
            var ListInstitucionesTrasladoModel = GetInstitucionesTraslado();

            return View(ListInstitucionesTrasladoModel);

        }




        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListInstitucionesTrasladoModel = GetInstitucionesTraslado();
            return View("Index", ListInstitucionesTrasladoModel);
        }

        [HttpPost]
        public ActionResult AgregarInstitucionTrasladoModal()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        public ActionResult EditarInstitucionTrasladoModal(int IdInstitucionTraslado)
        {
            var institucionesTrasladoModel = GetInstitucionTrasladoByID(IdInstitucionTraslado);
            return PartialView("_Editar", institucionesTrasladoModel);
        }

        public ActionResult EliminarInstitucionTrasladoModal(int IdInstitucionTraslado)
        {
            var institucionesTrasladoModel = GetInstitucionTrasladoByID(IdInstitucionTraslado);
            return PartialView("_Eliminar", institucionesTrasladoModel);
        }



        [HttpPost]
        public ActionResult AgregarInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("InstitucionTraslado");
            if (ModelState.IsValid)
            {


                CrearInstitucionTraslado(model);
                var ListInstitucionesTrasladoModel = GetInstitucionesTraslado();
                return PartialView("_ListaInstitucionesTraslado", ListInstitucionesTrasladoModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarInstitucionTrasladoMod(CatInstitucionesTrasladoModel model)
        {
            bool switchInstTraslado = Request.Form["instTrasladoSwitch"].Contains("true");
            model.Estatus = switchInstTraslado ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("InstitucionTraslado");
            if (ModelState.IsValid)
            {


                EditarInstitucionTraslado(model);
                var ListInstitucionesTrasladoModel = GetInstitucionesTraslado();
                return PartialView("_ListaInstitucionesTraslado", ListInstitucionesTrasladoModel);
            }
            return PartialView("_Editar");
        }

        public ActionResult EliminarInstitucionTrasladoMod(CatInstitucionesTrasladoModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                EliminaInstitucionTraslado(model);
                var ListInstitucionesTrasladoModel = GetInstitucionesTraslado();
                return PartialView("_ListaInstitucionesTraslado", ListInstitucionesTrasladoModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetInstTraslado([DataSourceRequest] DataSourceRequest request)
        {
            var ListInstitucionesTrasladoModel = GetInstitucionesTraslado();

            return Json(ListInstitucionesTrasladoModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
            CatInstitucionesTraslado institucion = new CatInstitucionesTraslado();
            institucion.IdInstitucionTraslado = model.IdInstitucionTraslado;
            institucion.InstitucionTraslado = model.InstitucionTraslado;
            institucion.Estatus = 1;
            institucion.FechaActualizacion = DateTime.Now;
            dbContext.CatInstitucionesTraslado.Add(institucion);
            dbContext.SaveChanges();
        }

        public void EditarInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
            CatInstitucionesTraslado institucion = new CatInstitucionesTraslado();
            institucion.IdInstitucionTraslado = model.IdInstitucionTraslado;
            institucion.InstitucionTraslado = model.InstitucionTraslado;
            institucion.Estatus = model.Estatus;
            institucion.FechaActualizacion = DateTime.Now;
            dbContext.Entry(institucion).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminaInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {

            CatInstitucionesTraslado institucion = new CatInstitucionesTraslado();
            institucion.IdInstitucionTraslado = model.IdInstitucionTraslado;
            institucion.InstitucionTraslado = model.InstitucionTraslado;
            institucion.Estatus = 0;
            institucion.FechaActualizacion = DateTime.Now;
            dbContext.Entry(institucion).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatInstitucionesTrasladoModel GetInstitucionTrasladoByID(int IdInstitucionTraslado)
        {

            var productEnitity = dbContext.CatAutoridadesEntrega.Find(IdInstitucionTraslado);

            var institucionesTrasladoModel = (from catInstitucionesTraslado in dbContext.CatInstitucionesTraslado.ToList()
                                              select new CatInstitucionesTrasladoModel

                                              {
                                                  IdInstitucionTraslado = catInstitucionesTraslado.IdInstitucionTraslado,
                                                  InstitucionTraslado = catInstitucionesTraslado.InstitucionTraslado,


                                              }).Where(w => w.IdInstitucionTraslado == IdInstitucionTraslado).FirstOrDefault();

            return institucionesTrasladoModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatInstitucionesTrasladoModel> GetInstitucionesTraslado()
        {
            var ListInstitucionesTrasladoModel = (from catInstitucionesTraslado in dbContext.CatInstitucionesTraslado.ToList()
                                                  join estatus in dbContext.Estatus.ToList()
                                                  on catInstitucionesTraslado.Estatus equals estatus.estatus
                                                  where catInstitucionesTraslado.Estatus == 1
                                                  select new CatInstitucionesTrasladoModel
                                                  {
                                                      IdInstitucionTraslado = catInstitucionesTraslado.IdInstitucionTraslado,
                                                      InstitucionTraslado = catInstitucionesTraslado.InstitucionTraslado,
                                                      estatusDesc = estatus.estatusDesc,

                                                  }).ToList();
            return ListInstitucionesTrasladoModel;
        }
        #endregion



    }
}
