using Microsoft.AspNetCore.Mvc;
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

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatEntidadesController : Controller
    {

        private readonly ICatEntidadesService _catEntidadesService;

        public CatEntidadesController(ICatEntidadesService catEntidadesService)
        {
            _catEntidadesService = catEntidadesService;
        }
        public IActionResult Index()
        {
            var ListEntidadesModel = _catEntidadesService.ObtenerEntidades();

            return View(ListEntidadesModel);

        }
        [HttpPost]
        public ActionResult MostrarModalAgregarEntidad()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        public ActionResult EditarEntidadModal(int idEntidad)
        {
            var EntidadesModel = _catEntidadesService.ObtenerEntidadesByID(idEntidad);
            return PartialView("_Editar", EntidadesModel);

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
                return PartialView("_ListaEntidades", ListEntidadesModel);
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
                return PartialView("_ListaEntidades", ListEntidadesModel);
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
