using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
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
using Microsoft.AspNetCore.Authorization;


namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class TiposVehiculosController : BaseController
    {
        private readonly ICatTiposVehiculosService _catTiposVehiculoService;

        DBContextInssoft dbContext = new DBContextInssoft();
        public TiposVehiculosController(ICatTiposVehiculosService catTiposVehiculoService)
        {
            _catTiposVehiculoService = catTiposVehiculoService;

        }
        public IActionResult Index()
        {
            int IdModulo = 932;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListTiposVehiculosModel = _catTiposVehiculoService.GetTiposVehiculos();

            return View(ListTiposVehiculosModel);
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
            var ListTiposVehiculosModel = _catTiposVehiculoService.GetTiposVehiculos();
            //return View("IndexModal");
            return View("Index", ListTiposVehiculosModel);
        }

        [HttpPost]
        public ActionResult AgregarTipoVehiculo()
        {

            int IdModulo = 933;
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
        public ActionResult EditarTipoVehiculo(int IdTipoVehiculo)
        {
            int IdModulo = 934;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var tiposVehiculosModel = _catTiposVehiculoService.GetTipoVehiculoByID(IdTipoVehiculo);
            return View("_Editar", tiposVehiculosModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
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
                _catTiposVehiculoService.CreateTipoVehiculo(model);
                var ListTiposVehiculosModel = _catTiposVehiculoService.GetTiposVehiculos();
                return Json(ListTiposVehiculosModel);
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

                _catTiposVehiculoService.UpdateTipoVehiculo(model);
                var ListTiposVehiculosModel = _catTiposVehiculoService.GetTiposVehiculos();
                return Json(ListTiposVehiculosModel);
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
                var ListTiposVehiculosModel = _catTiposVehiculoService.GetTiposVehiculos();
                return Json(ListTiposVehiculosModel);
            }

            return PartialView("_Eliminar");
        }

        public JsonResult GetTipos([DataSourceRequest] DataSourceRequest request)
        {
            var ListTiposVehiculosModel = _catTiposVehiculoService.GetTiposVehiculos();

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
       
        #endregion



    }
}
