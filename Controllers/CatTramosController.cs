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
//using Telerik.SvgIcons;
using System;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatTramosController : BaseController
    {
        private readonly ICatTramosService _catTramosService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;


        public CatTramosController(ICatTramosService catTramosService, ICatCarreterasService catCarreterasService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _catTramosService = catTramosService;
            _catCarreterasService = catCarreterasService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }
        public IActionResult Index()
        {
               var ListTramosModel = _catTramosService.ObtenerTramos();
            ViewBag.ListadoTramos = ListTramosModel;

            return View();
        }
      

        public JsonResult Carreteras_Drop(int idDelegacion)
        {
            var tipo = 1;// Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idDelegacion).Where(x=>x.Transito == tipo), "IdCarretera", "Carretera");
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

        public JsonResult DelegacionesOficinas_Drop()
        {
            var tipo = 1;// Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().Where(x=>x.Transito == tipo), "IdDelegacion", "Delegacion");
            return Json(result);
        }

        [HttpGet]
        public ActionResult ajax_BuscarTramos(int idCarreteraFiltro)
        {
            List<CatTramosModel> ListTramos = new List<CatTramosModel>();


            ListTramos = (from tramos in _catTramosService.ObtenerTramos().ToList()
                                          // join delegacion in _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().ToList()
                                          //on oficiales.IdOficina equals delegacion.IdOficinaTransporte
                                          // join estatus in dbContext.Estatus.ToList()
                                          //on diasInhabiles.Estatus equals estatus.estatus

                                      select new CatTramosModel
                                      {
                                          IdTramo = tramos.IdTramo,
                                          Tramo = tramos.Tramo,
                                          Carretera = tramos.Carretera,
                                          IdCarretera = tramos.IdCarretera,
                                          estatusDesc = tramos.estatusDesc,
                                        // = diasInhabiles.Municipio
                                      }).ToList();


            if (idCarreteraFiltro > 0)
            {
                ListTramos = (from s in ListTramos
                              where s.IdCarretera == idCarreteraFiltro
                              select s).ToList();
            }

            return Json(ListTramos);
        }
    }
}
