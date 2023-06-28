using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
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
    public class CatClasificacionAccidentesController : Controller
    {
        private readonly ICatClasificacionAccidentes _clasificacionAccidentesService;


        public CatClasificacionAccidentesController(ICatClasificacionAccidentes clasificacionAccidentesService)
        {
            _clasificacionAccidentesService = clasificacionAccidentesService;
        }

        public IActionResult Index()
        {
            var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes();

            return View(ListClasificacionAccidentesModel);

        }

        public IActionResult OntenerParaDDL()
        {
            var ListClasificacionAccidentesModel = _clasificacionAccidentesService.ObtenerClasificacionesActivas();

            return View(ListClasificacionAccidentesModel);

        }





        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes();
            return View("Index", ListClasificacionAccidentesModel);
        }

        [HttpPost]
        public ActionResult AgregarClasificacionAccidenteModal()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        public ActionResult EditarClasificacionAccidenteModal(int IdClasificacionAccidente)
        {
            var clasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidenteByID(IdClasificacionAccidente);
            return PartialView("_Editar", clasificacionAccidentesModel);
        }

        public ActionResult EliminarClasificacionAccidenteModal(int IdClasificacionAccidente)
        {
            var clasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidenteByID(IdClasificacionAccidente);
            return PartialView("_Eliminar", clasificacionAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarClasificacionAccidente(CatClasificacionAccidentesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _clasificacionAccidentesService.CrearClasificacionAccidente(model);
                var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes();
                return PartialView("_ListaClasificacionAccidentes", ListClasificacionAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarClasificacionAccidenteMod(CatClasificacionAccidentesModel model)
        {
            bool switchClasificacion = Request.Form["clasificacionAccidentesSwitch"].Contains("true");
            model.Estatus = switchClasificacion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreClasificacion");
            if (ModelState.IsValid)
            {


                _clasificacionAccidentesService.EditarClasificacionAccidente(model);
                var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes();
                return PartialView("_ListaClasificacionAccidentes", ListClasificacionAccidentesModel);
            }
            return PartialView("_Editar");
        }

        public JsonResult GetClasAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            var ListClasificacionAccidentesModel = _clasificacionAccidentesService.ObtenerClasificacionesActivas();

            return Json(ListClasificacionAccidentesModel.ToDataSourceResult(request));
        }
    }
}




#endregion

