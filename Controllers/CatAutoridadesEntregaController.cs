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
    public class CatAutoridadesEntregaController : BaseController
    {
        private readonly ICatAutoridadesEntregaService _catAutoridadesEntregaService;

        public CatAutoridadesEntregaController(ICatAutoridadesEntregaService catAutoridadesEntregaService)
        {
            _catAutoridadesEntregaService = catAutoridadesEntregaService;
        }
        public IActionResult Index()
        {
     
                var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas();

            return View(ListAutoridadesEntregaModel);
            }
   




        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas();
            return View("Index", ListAutoridadesEntregaModel);
        }

        [HttpPost]
        public ActionResult AgregarAutoridadEntregaModal()
        {

                return PartialView("_Crear");
            }
     

        public ActionResult EditarAutoridadEntregaModal(int IdAutoridadEntrega)
        {
     
                var autoridadesEntregaModel = _catAutoridadesEntregaService.GetAutoridadesByID(IdAutoridadEntrega);
                return PartialView("_Editar", autoridadesEntregaModel);
            }
  

        public ActionResult EliminarAutoridadEntregaModal(int IdAutoridadEntrega)
        {
            var autoridadesEntregaModel = _catAutoridadesEntregaService.GetAutoridadesByID(IdAutoridadEntrega);
            return PartialView("_Eliminar", autoridadesEntregaModel);
        }



        [HttpPost]
        public ActionResult AgregarAutoridadEntrega(CatAutoridadesEntregaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                _catAutoridadesEntregaService.GuardarAutoridad(model);
                var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarAutoridadEntregaMod(CatAutoridadesEntregaModel model)
        {
            bool switchAutEntrega = Request.Form["autEntregaSwitch"].Contains("true");
            model.Estatus = switchAutEntrega ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                _catAutoridadesEntregaService.UpdateAutoridad(model);
                var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

      /*  public ActionResult EliminarAutoridadEntregaMod(CatAutoridadesEntregaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                EliminaAutoridadEntrega(model);
                var ListAutoridadesEntregaModel = GetAutoridadesEntrega();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }
            return PartialView("_Eliminar");
        }*/
        public JsonResult GetAutEntrega([DataSourceRequest] DataSourceRequest request)
        {
            var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas();

            return Json(ListAutoridadesEntregaModel.ToDataSourceResult(request));
        }




        #endregion

     
    }
}
