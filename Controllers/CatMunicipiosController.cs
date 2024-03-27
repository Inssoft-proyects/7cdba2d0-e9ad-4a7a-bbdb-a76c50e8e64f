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
            //var result = new SelectList(_catEntidadesService.ObtenerEntidades(), "idEntidad", "nombreEntidad");
            var municipios = new CatMunicipiosDTO();
            var ListMunicipiosModel = _catMunicipiosService.GetMunicipiosCatalogo();
            municipios.MunicipiosModel = ListMunicipiosModel;

            return View(municipios);
        }


        [HttpPost]
        public ActionResult AgregarMunicipioModal()
        {

            return PartialView("_Crear");
        }

        public JsonResult Entidades_Drop()
        {
            var result = new SelectList(_catEntidadesService.ObtenerEntidades(), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Delegaciones_Drop()
        {
            var tipo = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesDropDown().Where(x => x.Transito == tipo), "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }
        public ActionResult EditarMunicipioModal(int IdMunicipio)
        {

            var municipiosModel = _catMunicipiosService.GetMunicipioByID(IdMunicipio);
            return View("_Editar", municipiosModel);
        }




        [HttpPost]
        public ActionResult CrearMunicipioMod(CatMunicipiosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catMunicipiosService.AgregarMunicipio(model);
                var ListMunicipiosModel = _catMunicipiosService.GetMunicipiosCatalogo();
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
                var ListMunicipiosModel = _catMunicipiosService.GetMunicipiosCatalogo();
                return Json(ListMunicipiosModel);
            }

            return PartialView("_Editar");
        }

        public JsonResult GetMun([DataSourceRequest] DataSourceRequest request, string nombre, int idEntidad, int IdOficinaTransporte)
        {
            var ListMunicipiosModel = _catMunicipiosService.GetMunicipiosCatalogo();
            if (!String.IsNullOrEmpty(nombre) && idEntidad>0 && IdOficinaTransporte>0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x=>x.Municipio.ToUpper().Contains(nombre.ToUpper()) && x.IdEntidad == idEntidad && x.IdOficinaTransporte == IdOficinaTransporte ).ToList();
            else if (!String.IsNullOrEmpty(nombre) && idEntidad == 0 && IdOficinaTransporte == 0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x => x.Municipio.ToUpper().Contains(nombre.ToUpper())).ToList();
            else if (!String.IsNullOrEmpty(nombre) && idEntidad > 0 && IdOficinaTransporte == 0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x => x.Municipio.ToUpper().Contains(nombre.ToUpper()) && x.IdEntidad == idEntidad).ToList();
            else if (String.IsNullOrEmpty(nombre) && idEntidad > 0 && IdOficinaTransporte > 0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x => x.IdEntidad == idEntidad && x.IdOficinaTransporte == IdOficinaTransporte).ToList();
            else if (!String.IsNullOrEmpty(nombre) && idEntidad == 0 && IdOficinaTransporte > 0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x => x.Municipio.ToUpper().Contains(nombre.ToUpper()) && x.IdOficinaTransporte == IdOficinaTransporte).ToList();
            else if (String.IsNullOrEmpty(nombre) && idEntidad == 0 && IdOficinaTransporte > 0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x => x.IdOficinaTransporte == IdOficinaTransporte).ToList();
            else if (String.IsNullOrEmpty(nombre) && idEntidad > 0 && IdOficinaTransporte == 0)
                ListMunicipiosModel = ListMunicipiosModel.Where(x => x.IdEntidad == idEntidad).ToList();

            return Json(ListMunicipiosModel.ToDataSourceResult(request));
        }

    }
}
