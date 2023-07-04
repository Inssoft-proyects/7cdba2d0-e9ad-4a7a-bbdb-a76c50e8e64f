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
    public class CatTramosController : Controller
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
            //SetDDLDependencias();
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
                return PartialView("_ListaTramos", TramosModel);
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
                return PartialView("_ListaTramos", TramosModel);
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
