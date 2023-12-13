using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatCarreterasController : BaseController
    {
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;

        public CatCarreterasController(ICatCarreterasService catCarreterasService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _catCarreterasService = catCarreterasService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;

        }
        public IActionResult Index()
        {
            int IdModulo = 900;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListCarreterassModel = _catCarreterasService.ObtenerCarreteras();
            return View(ListCarreterassModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinas(), "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }
        [HttpPost]
        public ActionResult MostrarModalAgregarCarretera()
        {
            int IdModulo = 901;
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

        public ActionResult EditarCarreteraModal(int IdCarretera)
        {
            int IdModulo = 902;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var CarreterasModel = _catCarreterasService.ObtenerCarreteraByID(IdCarretera);
            return PartialView("_Editar", CarreterasModel);
        }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
    }

}

        [HttpPost]
        public ActionResult CrearCarreteraMod(CatCarreterasModel model)
        {

            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

                _catCarreterasService.CrearCarretera(model);
                var CarreterasModel = _catCarreterasService.ObtenerCarreteras();
                return Json(CarreterasModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarCarreteraBD(CatCarreterasModel model)
        {
            bool switchCarreteras = Request.Form["carreterasSwitch"].Contains("true");
            model.Estatus = switchCarreteras ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catCarreterasService.EditarCarretera(model);
                var ListCarreterasModel = _catCarreterasService.ObtenerCarreteras();
                return Json(ListCarreterasModel);
            }
            return PartialView("_Editar");
        }

        public JsonResult GetCarr([DataSourceRequest] DataSourceRequest request)
        {
            var ListCarreterasModel = _catCarreterasService.ObtenerCarreteras();

            return Json(ListCarreterasModel.ToDataSourceResult(request));
        }
    }
}
