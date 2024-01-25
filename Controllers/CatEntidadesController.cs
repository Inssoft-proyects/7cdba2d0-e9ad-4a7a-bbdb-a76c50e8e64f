﻿using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatEntidadesController : BaseController
    {

        private readonly ICatEntidadesService _catEntidadesService;

        public CatEntidadesController(ICatEntidadesService catEntidadesService)
        {
            _catEntidadesService = catEntidadesService;
        }
        public IActionResult Index()
        {
            int IdModulo = 1041;
            string listaPermisosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaPermisos = JsonConvert.DeserializeObject<List<int>>(listaPermisosJson);
            if (listaPermisos != null && listaPermisos.Contains(IdModulo))
            {
                var ListEntidadesModel = _catEntidadesService.ObtenerEntidades();

            return View(ListEntidadesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }

        }
        [HttpPost]
        public ActionResult MostrarModalAgregarEntidad()
        {
            int IdModulo = 1043;
            string listaPermisosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaPermisos = JsonConvert.DeserializeObject<List<int>>(listaPermisosJson);
            if (listaPermisos != null && listaPermisos.Contains(IdModulo))
            {
                return PartialView("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        public ActionResult EditarEntidadModal(int idEntidad)
        {
            int IdModulo = 1045;
            string listaPermisosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaPermisos = JsonConvert.DeserializeObject<List<int>>(listaPermisosJson);
            if (listaPermisos != null && listaPermisos.Contains(IdModulo))
            {
                var EntidadesModel = _catEntidadesService.ObtenerEntidadesByID(idEntidad);
            return PartialView("_Editar", EntidadesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }

        }

        [HttpPost]
        public ActionResult AgregarEntidad(CatEntidadesModel model)
        {
           
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("idEntidad");
            if (ModelState.IsValid)
            {


                _catEntidadesService.CrearEntidad(model);
                var ListEntidadesModel = _catEntidadesService.ObtenerEntidades();
                return Json(ListEntidadesModel);
            }

            return PartialView("_Crear");
        
        
        }

        public ActionResult EditarEntidadBD(CatEntidadesModel model)
        {
            bool switchEntidades = Request.Form["entidadesSwitch"].Contains("true");
            model.estatus = switchEntidades ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catEntidadesService.EditarEntidad(model);
                var ListEntidadesModel = _catEntidadesService.ObtenerEntidades();
                return Json(ListEntidadesModel);
            }
            return PartialView("_Editar");
        }
        public JsonResult GetEnt([DataSourceRequest] DataSourceRequest request)
        {
            var ListEntidadesModel = _catEntidadesService.ObtenerEntidades();

            return Json(ListEntidadesModel.ToDataSourceResult(request));
        }
    }
}
