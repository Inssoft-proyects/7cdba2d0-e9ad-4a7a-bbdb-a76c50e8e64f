using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatMarcasVehiculosController : Controller
    {
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;

        public CatMarcasVehiculosController(ICatMarcasVehiculosService catMarcasVehiculosService)
        {
            _catMarcasVehiculosService = catMarcasVehiculosService;
        }
        public IActionResult Index()
        {
            var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcas();

            return View(ListMarcasModel);
        }
        [HttpPost]
        public ActionResult AgregarPacial()
        {
            //SetDDLDependencias();
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcial(int IdMarcaVehiculo)
        {
            var marcasVehiculosModel = _catMarcasVehiculosService.GetMarcaByID(IdMarcaVehiculo);
            return PartialView("_Editar", marcasVehiculosModel);
        }
        [HttpPost]
        public ActionResult EliminarMarcaParcial(int IdMarcaVehiculo)
        {
            var marcasVehiculosModel = _catMarcasVehiculosService.GetMarcaByID(IdMarcaVehiculo);
            return View("_Eliminar", marcasVehiculosModel);
        }

        [HttpPost]
        public IActionResult GetUpdate(int IdMarcaVehiculo)
        {
            var marcaVehiculoModel = _catMarcasVehiculosService.GetMarcaByID(IdMarcaVehiculo);
            return View(marcaVehiculoModel);
        }


        public JsonResult Categories_Read()
        {
            var result = new SelectList(_catMarcasVehiculosService.ObtenerMarcas(), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(CatMarcasVehiculosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("MarcaVehiculo");
            if (ModelState.IsValid)
            {
                //Crear el producto

                _catMarcasVehiculosService.GuardarMarca(model);
                var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcas();
                return PartialView("_ListaMarcasVehiculos", ListMarcasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        public ActionResult UpdatePartialModal(CatMarcasVehiculosModel model)
        {
            bool switchMarcas = Request.Form["MarcasSwitch"].Contains("true");
            model.Estatus = switchMarcas ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("MarcaVehiculo");
            if (ModelState.IsValid)
            {
                //Crear el producto

                _catMarcasVehiculosService.UpdateMarca(model);
                var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcas();
                return PartialView("_ListaMarcasVehiculos", ListMarcasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }



        public JsonResult GetMarca2([DataSourceRequest] DataSourceRequest request)
        {
            var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcas();

            return Json(ListMarcasModel.ToDataSourceResult(request));
        }

    }
}
