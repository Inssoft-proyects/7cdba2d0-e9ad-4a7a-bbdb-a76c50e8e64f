using GuanajuatoAdminUsuarios.Entity;
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
    public class SalariosMinimosController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 940;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListSalariosModel = GetSalarios();

            return View(ListSalariosModel);
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
            var ListSalariosModel = GetSalarios();
            //return View("IndexModal");
            return View("Index", ListSalariosModel);
        }

        [HttpPost]
        public ActionResult AgregarSalarioPacial()
        {
            int IdModulo = 941;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                return PartialView("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public ActionResult EditarSalarioParcial(int IdSalario)
        {
            int IdModulo = 942;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var salariosModel = GetSalarioByID(IdSalario);
            return View("_Editar", salariosModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public ActionResult EliminarSalarioParcial(int IdSalario)
        {
            var salariosModel = GetSalarioByID(IdSalario);
            return View("_Eliminar", salariosModel);
        }

        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.CatSalariosMinimos.ToList(), "IdSalario", "Salario");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(SalariosMinimosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Salario");
            if (ModelState.IsValid)
            {


                CreateSalario(model);
                var ListSalariosModel = GetSalarios();
                return Json(ListSalariosModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult UpdatePartialSalarioModal(SalariosMinimosModel model)
        {
            bool switchSalarios = Request.Form["salariosSwitch"].Contains("true");
            model.Estatus = switchSalarios ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Salario");
            if (ModelState.IsValid)
            {
               
                UpdateSalario(model);
                var ListSalariosModel = GetSalarios();
                return Json(ListSalariosModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarSalarioModal(SalariosMinimosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("IdSalario");
            if (ModelState.IsValid)
            {


                DeleteSalario(model);
                var ListSalariosModel = GetSalarios();
                return Json(ListSalariosModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Eliminar");
        }

        public JsonResult GetMins([DataSourceRequest] DataSourceRequest request)
        {
            var ListSalariosModel = GetSalarios();

            return Json(ListSalariosModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateSalario(SalariosMinimosModel model)
        {
            CatSalariosMinimos salario = new CatSalariosMinimos();
            salario.IdSalario = model.IdSalario;
            salario.Area = model.Area;
            salario.Salario = model.Salario;
            salario.Fecha = model.Fecha;
            salario.Estatus = 1;
            salario.FechaActualizacion = DateTime.Now;
            dbContext.CatSalariosMinimos.Add(salario);
            dbContext.SaveChanges();
        }

        public void UpdateSalario(SalariosMinimosModel model)
        {
            CatSalariosMinimos salario = new CatSalariosMinimos();
            salario.IdSalario = model.IdSalario;
            salario.Area = model.Area;
            salario.Salario = model.Salario;
            salario.Fecha = model.Fecha;
            salario.Estatus = model.Estatus;
            salario.FechaActualizacion = DateTime.Now;
            dbContext.Entry(salario).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteSalario(SalariosMinimosModel model)
        {
            CatSalariosMinimos salario = new CatSalariosMinimos();
            salario.IdSalario = model.IdSalario;
            salario.Area = model.Area;
            salario.Salario = model.Salario;
            salario.FechaActualizacion = DateTime.Now;
            salario.Estatus = 0;
            salario.FechaActualizacion = DateTime.Now;
            dbContext.Entry(salario).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        private void SetDDLSalarios()
        {
            ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
            ViewBag.Salarios = new SelectList(dbContext.CatSalariosMinimos.ToList(), "IdSalario", "Salario");
        }


        public SalariosMinimosModel GetSalarioByID(int IdSalario)
        {

            var productEnitity = dbContext.CatSalariosMinimos.Find(IdSalario);

            var SalarioModel = (from salariosMinimos in dbContext.CatSalariosMinimos.ToList()
                                select new SalariosMinimosModel

                                {
                                    IdSalario = salariosMinimos.IdSalario,
                                    Area = salariosMinimos.Area,
                                    Salario = salariosMinimos.Salario,
                                    Fecha = salariosMinimos.Fecha,
                                    Estatus = salariosMinimos.Estatus,



                                }).Where(w => w.IdSalario == IdSalario).FirstOrDefault();

            return SalarioModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<SalariosMinimosModel> GetSalarios()
        {
            var ListSalariosModel = (from salariosMinimos in dbContext.CatSalariosMinimos.ToList()
                                     join estatus in dbContext.Estatus.ToList()
                                    on salariosMinimos.Estatus equals estatus.estatus
                                     where salariosMinimos.Estatus == 1

                                     select new SalariosMinimosModel
                                     {
                                         IdSalario = salariosMinimos.IdSalario,
                                         Area = salariosMinimos.Area,
                                         Salario = salariosMinimos.Salario,
                                         Fecha = salariosMinimos.Fecha,
                                         Estatus = salariosMinimos.Estatus,
                                         estatusDesc = estatus.estatusDesc,

                                     }).ToList();
            return ListSalariosModel;
        }
        #endregion



    }
}
