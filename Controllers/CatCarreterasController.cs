using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatCarreterasController : Controller
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
            var ListCarreterassModel = _catCarreterasService.ObtenerCarreteras();
            return View(ListCarreterassModel);
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinas(), "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }
        [HttpPost]
        public ActionResult MostrarModalAgregarCarretera()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        public ActionResult EditarCarreteraModal(int IdCarretera)
        {
            var CarreterasModel = _catCarreterasService.ObtenerCarreteraByID(IdCarretera);
            return PartialView("_Editar", CarreterasModel);

        }

        [HttpPost]
        public ActionResult CrearCarreteraMod(CatCarreterasModel model)
        {

            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

                _catCarreterasService.CrearCarretera(model);
                var CarreterasModel = _catCarreterasService.ObtenerCarreteras();
                return PartialView("_ListaCarreteras", CarreterasModel);
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
                return PartialView("_ListaCarreteras", ListCarreterasModel);
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
