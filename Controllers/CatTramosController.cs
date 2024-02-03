using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatTramosController : BaseController
    {
        private readonly ICatTramosService _catTramosService;
        private readonly ICatCarreterasService _catCarreterasService;


        public CatTramosController(ICatTramosService catTramosService, ICatCarreterasService catCarreterasService)
        {
            _catTramosService = catTramosService;
            _catCarreterasService = catCarreterasService;
        }
        public IActionResult Index()
        {
        
                var ListTramosModel = _catTramosService.ObtenerTramos();
                return View(ListTramosModel);
            }
      

        public JsonResult Carreteras_Drop()
        {
            var result = new SelectList(_catCarreterasService.ObtenerCarreteras(), "IdCarretera", "Carretera");
            return Json(result);
        }
        [HttpPost]
        public ActionResult MostrarModalAgregarTramo()
        {
     
                return PartialView("_Crear");
            }
     

        public ActionResult EditarTramoModal(int IdTramo)
        {
  
                var TramosModel = _catTramosService.ObtenerTramoByID(IdTramo);
            return PartialView("_Editar", TramosModel);
            }


        [HttpPost]
        public ActionResult CrearTramoMod(CatTramosModel model)
        {

            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

                _catTramosService.CrearTramo(model);
                var TramosModel = _catTramosService.ObtenerTramos();
                return Json(TramosModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarTramoBD(CatTramosModel model)
        {
            bool switchTramoss = Request.Form["tramosSwitch"].Contains("true");
            model.Estatus = switchTramoss ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catTramosService.EditarTramo(model);
                var TramosModel = _catTramosService.ObtenerTramos();
                return Json(TramosModel);
            }
            return PartialView("_Editar");
        }

        public JsonResult GetTra([DataSourceRequest] DataSourceRequest request)
        {
            var ListtramosModel = _catTramosService.ObtenerTramos();

            return Json(ListtramosModel.ToDataSourceResult(request));
        }
    }
}
