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
    public class CatCausasAccidentesController : BaseController
    {
        private readonly ICatCausasAccidentesService _catCausasAccidentesService;

        public CatCausasAccidentesController(ICatCausasAccidentesService catCausasAccidentesService)
        {
            _catCausasAccidentesService = catCausasAccidentesService;
        }
        DBContextInssoft dbContext = new  DBContextInssoft();
        public IActionResult Index()
        {

                var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas();

            return View(ListCausasAccidentesModel);
            }
 




        #region Modal Action
        public ActionResult IndexModal()
        {
           
                var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas();
            return View("Index", ListCausasAccidentesModel);
            
        }

        [HttpPost]
        public ActionResult AgregarCausasAccidenteModal()
        {
   
                return PartialView("_Crear");
            }
  

        public ActionResult EditarCausasAccidenteModal(int IdCausaAccidente)
        {
        
                var causasAccidentesModel = _catCausasAccidentesService.ObtenerCausaByID(IdCausaAccidente);
            return PartialView("_Editar", causasAccidentesModel);
            }
   

        public ActionResult EliminarCausasAccidenteModal(int IdCausaAccidente)
        {
            var causasAccidentesModel = _catCausasAccidentesService.ObtenerCausaByID(IdCausaAccidente);
            return PartialView("_Eliminar", causasAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarCausaAccidente(CatCausasAccidentesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("CausaAccidente");
            if (ModelState.IsValid)
            {


                _catCausasAccidentesService.CrearCausa(model);
                var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas();
                return Json(ListCausasAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarCausaAccidenteMod(CatCausasAccidentesModel model)
        {
            bool switchCausasAccidentes = Request.Form["causaAccidenteSwitch"].Contains("true");
            model.Estatus = switchCausasAccidentes ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("CausaAccidente");
            if (ModelState.IsValid)
            {


                _catCausasAccidentesService.EditarCausa(model);
                var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas();
                return Json(ListCausasAccidentesModel);
            }
            return PartialView("_Editar");
        }

      
        public JsonResult GetCausas([DataSourceRequest] DataSourceRequest request)
        {
            var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas();

            return Json(ListCausasAccidentesModel.ToDataSourceResult(request));
        }




        #endregion


    }
}
