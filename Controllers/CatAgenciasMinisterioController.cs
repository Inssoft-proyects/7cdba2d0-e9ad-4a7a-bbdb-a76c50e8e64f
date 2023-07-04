using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Example.WebUI.Controllers
{
    public class CatAgenciasMinisterioController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //var products = dbContext.Products.ToList();
            var ListAgenciasMinisterioModel = GetAgenciasministerio();

            return View(ListAgenciasMinisterioModel);

        }




        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListAgenciasMinisterioModel = GetAgenciasministerio();
            return View("Index", ListAgenciasMinisterioModel);
        }

        [HttpPost]
        public ActionResult AgregarAgenciaMinisterioModal()
        {
            SetDDLDelegaciones();
            return PartialView("_Crear");
        }

        public ActionResult EditarAgenciaMinisterioModal(int IdAgenciaMinisterio)
        {
            SetDDLDelegaciones();
            var agenciasMinisterioModel = GetAgenciaMinisterioByID(IdAgenciaMinisterio);
            return PartialView("_Editar", agenciasMinisterioModel);
        }

        public ActionResult EliminarAgenciaMinisterioModal(int IdAgenciaMinisterio)
        {
            SetDDLDelegaciones();
            var agenciasMinisterioModel = GetAgenciaMinisterioByID(IdAgenciaMinisterio);
            return PartialView("_Eliminar", agenciasMinisterioModel);
        }



        [HttpPost]
        public ActionResult AgregarAgenciaMinisterio(CatAgenciasMinisterioModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreAgencia");
            if (ModelState.IsValid)
            {


                CrearAgenciaMinisterio(model);
                var ListAgenciasMinisterioModel = GetAgenciasministerio();
                return PartialView("_ListaAgenciasMinisterio", ListAgenciasMinisterioModel);
            }
            SetDDLDelegaciones();
            return PartialView("_Crear");
        }

            public ActionResult EditarAgenciaMinisterioMod(CatAgenciasMinisterioModel model)
        {
            bool switchAgenciasMinisterio = Request.Form["agenciasSwitch"].Contains("true");
            model.Estatus = switchAgenciasMinisterio ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreAgencia");
            if (ModelState.IsValid)
            {


                EditarAgenciaMinisterio(model);
                var ListAgenciasMinisterioModel = GetAgenciasministerio();
                return PartialView("_ListaAgenciasMinisterio", ListAgenciasMinisterioModel);
            }
            return PartialView("_ListaAgenciasMinisterio");
        }

        public ActionResult EliminarAgenciaMinisterioMod(CatAgenciasMinisterioModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreAgencia");
            if (ModelState.IsValid)
            {


                EliminarAgenciaMinisterio(model);
                var ListAgenciasMinisterioModel = GetAgenciasministerio();
                return PartialView("_ListaAgenciasMinisterio", ListAgenciasMinisterioModel);
            }
            SetDDLDelegaciones();
            return PartialView("_Eliminar");
        }

        public JsonResult GetAgenciasM([DataSourceRequest] DataSourceRequest request)
        {
            var ListAgenciasMinisterioModel = GetAgenciasministerio();

            return Json(ListAgenciasMinisterioModel.ToDataSourceResult(request));
        }

        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
            return Json(result);
        }




        #endregion


        #region Acciones a base de datos

        public void CrearAgenciaMinisterio(CatAgenciasMinisterioModel model)
        {
            CatAgenciasMinisterio agencia = new CatAgenciasMinisterio();
            agencia.IdAgenciaMinisterio = model.IdAgenciaMinisterio;
            agencia.NombreAgencia = model.NombreAgencia;
            agencia.IdDelegacion = model.IdDelegacion;
            agencia.Estatus = 1;
            agencia.FechaActualizacion = DateTime.Now;
            dbContext.CatAgenciasMinisterio.Add(agencia);
            dbContext.SaveChanges();
        }

        public void EditarAgenciaMinisterio(CatAgenciasMinisterioModel model)
        {
            CatAgenciasMinisterio agencia = new CatAgenciasMinisterio();
            agencia.IdAgenciaMinisterio = model.IdAgenciaMinisterio;
            agencia.NombreAgencia = model.NombreAgencia;
            agencia.IdDelegacion = model.IdDelegacion;
            agencia.Estatus = model.Estatus;
            agencia.FechaActualizacion = DateTime.Now;
            dbContext.Entry(agencia).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminarAgenciaMinisterio(CatAgenciasMinisterioModel model)
        {
            CatAgenciasMinisterio agencia = new CatAgenciasMinisterio();
            agencia.IdAgenciaMinisterio = model.IdAgenciaMinisterio;
            agencia.NombreAgencia = model.NombreAgencia;
            agencia.IdDelegacion = model.IdDelegacion;
            agencia.Estatus = 0;
            agencia.FechaActualizacion = DateTime.Now;
            dbContext.Entry(agencia).State = EntityState.Modified;
            dbContext.SaveChanges();


        }

        private void SetDDLDelegaciones()
        {
            ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
            ViewBag.Delegaciones = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
        }



        public CatAgenciasMinisterioModel GetAgenciaMinisterioByID(int IdAgenciaMinisterio)
        {

            var productEnitity = dbContext.CatAgenciasMinisterio.Find(IdAgenciaMinisterio);

            var agenciasMinisterioModel = (from catAgenciasMinisterio in dbContext.CatAgenciasMinisterio.ToList()
                                           select new CatAgenciasMinisterioModel

                                           {
                                               IdAgenciaMinisterio = catAgenciasMinisterio.IdAgenciaMinisterio,
                                               NombreAgencia = catAgenciasMinisterio.NombreAgencia,
                                               IdDelegacion = catAgenciasMinisterio.IdDelegacion,


                                           }).Where(w => w.IdAgenciaMinisterio == IdAgenciaMinisterio).FirstOrDefault();

            return agenciasMinisterioModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatAgenciasMinisterioModel> GetAgenciasministerio()
        {
            var ListAgenciasMinisterioModel = (from catAgenciasMinisterio in dbContext.CatAgenciasMinisterio.ToList()
                                               join delegaciones in dbContext.Delegaciones.ToList()
                                               on catAgenciasMinisterio.IdDelegacion equals delegaciones.IdDelegacion
                                               join estatus in dbContext.Estatus.ToList()
                                               on catAgenciasMinisterio.Estatus equals estatus.estatus
                                               select new CatAgenciasMinisterioModel
                                               {
                                                   IdAgenciaMinisterio = catAgenciasMinisterio.IdAgenciaMinisterio,
                                                   NombreAgencia = catAgenciasMinisterio.NombreAgencia,
                                                   DelegacionDesc = delegaciones.Delegacion,
                                                   estatusDesc = estatus.estatusDesc,

                                               }).ToList();
            return ListAgenciasMinisterioModel;
        }
        #endregion



    }
}
