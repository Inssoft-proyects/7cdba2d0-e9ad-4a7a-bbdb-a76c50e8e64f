using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class OficialesController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 920;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListOficialesModel = GetOficiales();

            return View(ListOficialesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }
        private readonly IOficiales _oficialesService;

        public OficialesController(IOficiales oficialesService)
        {
            _oficialesService = oficialesService;
        }


        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListOficialessModel = GetOficiales();
            //return View("IndexModal");
            return View("Index", ListOficialessModel);
        }

        [HttpPost]
        public ActionResult AgregarOficialParcial()
        {
            int IdModulo = 921;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                Delegaciones_Drop();
            return PartialView("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public ActionResult EditarOficialParcial(int IdOficial)
        {
            int IdModulo = 922;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                Delegaciones_Drop();
            var oficialesModel = GetOficialByID(IdOficial);
            return View("_Editar", oficialesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public ActionResult EliminarOficialParcial(int IdOficial)
        {
            Delegaciones_Drop();
            var oficialesModel = GetOficialByID(IdOficial);
            return View("_Eliminar", oficialesModel);
        }

        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.Oficiales.ToList(), "IdOficial", "Nombre");
            return Json(result);
        }



        [HttpPost]
        public ActionResult AgregarOficialModal(OficialesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {

                CreateOficial(model);
                var ListOficialesModel = GetOficiales();
                return PartialView("_ListaOficiales", ListOficialesModel);
            }

            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarOficial(OficialesModel model)
        {
            bool switchOficiales = Request.Form["oficialesSwitch"].Contains("true");
            model.Estatus = switchOficiales ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {

                UpdateOficial(model);
                var ListOficialesModel = GetOficiales();
                return PartialView("_ListaOficiales", ListOficialesModel);
            }

            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarOficial(OficialesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {

                DeleteOficial(model);
                var ListOficialesModel = GetOficiales();
                return PartialView("_ListaOficiales", ListOficialesModel);
            }

            return PartialView("_Eliminar");
        }

        public JsonResult GetOficialess([DataSourceRequest] DataSourceRequest request)
        {
            var ListOficialesModel = GetOficiales();

            return Json(ListOficialesModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateOficial(OficialesModel model)
        {
            Oficiales oficial = new Oficiales();
            oficial.IdOficial = model.IdOficial;
            oficial.Rango = model.Rango;
            oficial.Nombre = model.Nombre;
            oficial.ApellidoPaterno = model.ApellidoPaterno;
            oficial.ApellidoMaterno = model.ApellidoMaterno;
            oficial.Estatus = 1;
            oficial.FechaActualizacion = DateTime.Now;
            //oficial.IdDelegacion = model.IdDelegacion;

            dbContext.Oficiales.Add(oficial);
            dbContext.SaveChanges();
        }

        public void UpdateOficial(OficialesModel model)
        {
            Oficiales oficial = new Oficiales();
            oficial.IdOficial = model.IdOficial;
            oficial.Nombre = model.Nombre;
            oficial.ApellidoPaterno = model.ApellidoPaterno;
            oficial.ApellidoMaterno = model.ApellidoMaterno;
            oficial.Estatus = model.Estatus;
            oficial.FechaActualizacion = DateTime.Now;
            //oficial.IdDelegacion = model.IdDelegacion;

            dbContext.Entry(oficial).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteOficial(OficialesModel model)
        {
            Oficiales oficial = new Oficiales();
            oficial.IdOficial = model.IdOficial;
            oficial.Nombre = model.Nombre;
            oficial.ApellidoPaterno = model.ApellidoPaterno;
            oficial.ApellidoMaterno = model.ApellidoMaterno;
            oficial.Estatus = 0;
            oficial.FechaActualizacion = DateTime.Now;
            //oficial.IdDelegacion = model.IdDelegacion;

            dbContext.Entry(oficial).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

       

        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
            return Json(result);
        }


        public OficialesModel GetOficialByID(int IdOficial)
        {

            var productEnitity = dbContext.Oficiales.Find(IdOficial);

            var oficialModel = (from oficiales in dbContext.Oficiales.ToList()
                                select new OficialesModel

                                {
                                    IdOficial = oficiales.IdOficial, 
                                    Rango = oficiales.Rango,
                                    Nombre = oficiales.Nombre,
                                    ApellidoPaterno = oficiales.ApellidoPaterno,
                                    ApellidoMaterno = oficiales.ApellidoMaterno,



                                }).Where(w => w.IdOficial == IdOficial).FirstOrDefault();

            return oficialModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<OficialesModel> GetOficiales()
        {
            var ListOficialessModel = (from oficiales in dbContext.Oficiales.ToList() 
                                       join estatus in dbContext.Estatus.ToList()
                                       on oficiales.Estatus equals estatus.estatus
                                       where oficiales.Estatus == 1
                                       select new OficialesModel
                                       {
                                           IdOficial = oficiales.IdOficial,
                                           Nombre = oficiales.Nombre,
                                           ApellidoPaterno = oficiales.ApellidoPaterno,
                                           ApellidoMaterno = oficiales.ApellidoMaterno,
                                           estatusDesc = estatus.estatusDesc

                                       }).ToList();
            return ListOficialessModel;
        }
        #endregion



    }
}
