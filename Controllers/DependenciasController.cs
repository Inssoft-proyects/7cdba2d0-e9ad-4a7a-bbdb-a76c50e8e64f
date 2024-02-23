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
    public class DependenciasController : BaseController
    {
        private readonly IDependencias _catDependencias;

        public DependenciasController(IDependencias catDependencias)
        {
            _catDependencias = catDependencias;
        }
        public IActionResult Index()
        {
           
                var ListDependenciasModel = _catDependencias.GetDependencias();

            return View(ListDependenciasModel);
            }
       



        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListDependenciasModel = _catDependencias.GetDependencias();
            //return View("IndexModal");
            return View("Index", ListDependenciasModel);
        }

        [HttpPost]
        public ActionResult AgregarPacial()
        {
           
                return PartialView("_Crear");
        }
 

        [HttpPost]
        public ActionResult EditarParcial(int IdDependencia)
        {
        
                var dependenciasModel = _catDependencias.GetDependenciaById(IdDependencia);
            return PartialView("_Editar", dependenciasModel);
            }
       

        [HttpPost]
        public ActionResult EliminarParcial(int IdDependencia)
        {
            var dependenciasModel = _catDependencias.GetDependenciaById(IdDependencia);
            return PartialView("_Eliminar", dependenciasModel);
        }



    



        [HttpPost]
        public ActionResult CreatePartialModal(DependenciasModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
                //Crear el producto

                _catDependencias.SaveDependencia(model);
                var ListDependenciasModel = _catDependencias.GetDependencias();
                return Json(ListDependenciasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult UpdatePartialModal(DependenciasModel model)
        {
            bool switchDependencias = Request.Form["switchDependencias"].Contains("true");
            model.Estatus = switchDependencias ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
                //Crear el producto

                _catDependencias.UpdateDependencia(model);
                var ListDependenciasModel = _catDependencias.GetDependencias();
                return Json(ListDependenciasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

       
        public JsonResult GetDeps([DataSourceRequest] DataSourceRequest request)
        {
            var ListProuctModel = _catDependencias.GetDependencias();

            return Json(ListProuctModel.ToDataSourceResult(request));
        }




        #endregion


    }
}
