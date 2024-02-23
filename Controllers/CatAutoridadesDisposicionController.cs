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
   
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas();

            return View(ListAutoridadesDisposicionModel);
            }
 


        [HttpPost]
        public IActionResult Agregar(CatAutoridadesDisposicionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {
                //Crear el producto

                _catAutoridadesDisposicionservice.GuardarAutoridad(model);
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas();

                return Json(ListAutoridadesDisposicionModel);
            }
            return View("_Agregar");
        }


        [HttpGet]
        public IActionResult Editar(int IdAutoridadDisposicion)
        {
            var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion);
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


                _catAutoridadesDisposicionservice. UpdateAutoridad(model);
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas();
                return Json(ListAutoridadesDisposicionModel);
            }

            return PartialView("_Editar");
        }

        [HttpGet]
        public IActionResult Eliminar(int IdAutoridadDisposicion)
        {

            var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion);
            return View(autoridadesDisposicionModel);
        }


        [HttpPost]
        public IActionResult Eliminar(CatAutoridadesDisposicionModel autoridadesDisposicionModel)
        {
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {
                //Modificiacion del registro
               // EliminarAutoridadDisp(autoridadesDisposicionModel);
                return RedirectToAction("Index");
            }
            return View("Delete");
        }



        ///Crear metodo de update (post)


        #region Modal Action
       

        [HttpPost]
        public ActionResult AgregarAutoridadDisposicionPacial()
        {
        
                //SetDDLDependencias();
                return PartialView("_Crear");
            }
   

        public ActionResult EditarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
     
                var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion);
            return PartialView("_Editar", autoridadesDisposicionModel);
            }
  

        public ActionResult EliminarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
            var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion);
            return PartialView("_Eliminar", autoridadesDisposicionModel);
        }







        [HttpPost]
        public ActionResult CrearAutoridadDisposicionParcialModal(CatAutoridadesDisposicionModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {


                _catAutoridadesDisposicionservice.GuardarAutoridad(model);
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas();
                return PartialView("_ListaAutoridadesDisposicion", ListAutoridadesDisposicionModel);
            }

            return PartialView("_Crear");
        }

        public JsonResult GetAutDisp([DataSourceRequest] DataSourceRequest request)
        {
            var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas();

            return Json(ListAutoridadesDisposicionModel.ToDataSourceResult(request));
        }




        #endregion

    }
}
