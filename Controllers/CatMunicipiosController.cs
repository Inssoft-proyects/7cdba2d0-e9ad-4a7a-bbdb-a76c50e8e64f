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
    public class CatMunicipiosController : BaseController
    {
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;




        public CatMunicipiosController(ICatMunicipiosService catMunicipiosService, ICatEntidadesService catEntidadesService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _catMunicipiosService = catMunicipiosService;
            _catEntidadesService = catEntidadesService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }
        public IActionResult Index()
        {
            int IdModulo = 912;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListMunicipiosModel = _catMunicipiosService.GetMunicipios();

            return View(ListMunicipiosModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }

        }
        [HttpPost]
        public ActionResult AgregarMunicipioModal()
        {
            int IdModulo = 913;
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
        public JsonResult Entidades_Drop()
        {
            var result = new SelectList(_catEntidadesService.ObtenerEntidades(), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinas(), "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }
        public ActionResult EditarMunicipioModal(int IdMunicipio)
        {
            int IdModulo = 914;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var municipiosModel = _catMunicipiosService.GetMunicipioByID(IdMunicipio);
            return View("_Editar", municipiosModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }



        [HttpPost]
        public ActionResult CrearMunicipioMod(CatMunicipiosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catMunicipiosService.AgregarMunicipio(model);
                var ListMunicipiosModel = _catMunicipiosService.GetMunicipios();
                return Json(ListMunicipiosModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        public ActionResult EditarMunicipioMod(CatMunicipiosModel model)
        {
            bool switchMunicipios = Request.Form["municipiosSwitch"].Contains("true");
            model.Estatus = switchMunicipios ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catMunicipiosService.EditarMunicipio(model);
                var ListMunicipiosModel = _catMunicipiosService.GetMunicipios();
                return Json(ListMunicipiosModel);
            }

            return PartialView("_Editar");
        }

        public JsonResult GetMun([DataSourceRequest] DataSourceRequest request)
        {
            var ListMunicipiosModel = _catMunicipiosService.GetMunicipios();

            return Json(ListMunicipiosModel.ToDataSourceResult(request));
        }

    }
}
