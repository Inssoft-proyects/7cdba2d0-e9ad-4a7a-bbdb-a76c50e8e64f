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
    public class CatFactoresAccidentesController : BaseController
    {
        private readonly ICatFactoresAccidentesService _catFactoresAccidentesService;

        public CatFactoresAccidentesController(ICatFactoresAccidentesService catFactoresAccidentesService)
        {
            _catFactoresAccidentesService = catFactoresAccidentesService;
        }
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 980;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListFactoresAccidentesModel = GetFactoresAccidentes();

            return View(ListFactoresAccidentesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }

        }




        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListFactoresAccidentesModel = GetFactoresAccidentes();
            return View("Index", ListFactoresAccidentesModel);
        }

        [HttpPost]
        public ActionResult AgregarFactoresAccidenteModal()
        {
            int IdModulo = 981;
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

        public ActionResult EditarFactoresAccidenteModal(int IdFactorAccidente)
        {
            int IdModulo = 982;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var factoresAccidentesModel = GetFactorAccidenteByID(IdFactorAccidente);
            return PartialView("_Editar", factoresAccidentesModel);
        }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
    }
}

        public ActionResult EliminarFactoresAccidenteModal(int IdFactorAccidente)
        {
            var factoresAccidentesModel = GetFactorAccidenteByID(IdFactorAccidente);
            return PartialView("_Eliminar", factoresAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarFactornAccidente(CatFactoresAccidentesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorAccidente");
            if (ModelState.IsValid)
            {


                CrearFactorAccidente(model);
                var ListFactoresAccidentesModel = GetFactoresAccidentes();
                return PartialView("_ListaFactoresAccidentes", ListFactoresAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarFactorAccidenteMod(CatFactoresAccidentesModel model)
        {
            bool switchFactores = Request.Form["factoresSwitch"].Contains("true");
            model.Estatus = switchFactores ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorAccidente");
            if (ModelState.IsValid)
            {


                EditarFactorAccidente(model);
                var ListFactoresAccidentesModel = GetFactoresAccidentes();
                return PartialView("_ListaFactoresAccidentes", ListFactoresAccidentesModel);
            }
            return PartialView("_Editar");
        }

        public ActionResult EliminarFactorAccidenteMod(CatFactoresAccidentesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorAccidente");
            if (ModelState.IsValid)
            {


                EliminaFactorAccidente(model);
                var ListFactoresAccidentesModel = GetFactoresAccidentes();
                return PartialView("_ListaFactoresAccidentes", ListFactoresAccidentesModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetFactores([DataSourceRequest] DataSourceRequest request)
        {
            var ListFactoresAccidentesModel = GetFactoresAccidentes();

            return Json(ListFactoresAccidentesModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearFactorAccidente(CatFactoresAccidentesModel model)
        {
            CatFactoresAccidentes factor = new CatFactoresAccidentes();
            factor.IdFactorAccidente = model.IdFactorAccidente;
            factor.FactorAccidente = model.FactorAccidente;
            factor.Estatus = 1;
            factor.FechaActualizacion = DateTime.Now;
            dbContext.CatFactoresAccidentes.Add(factor);
            dbContext.SaveChanges();
        }

        public void EditarFactorAccidente(CatFactoresAccidentesModel model)
        {
            CatFactoresAccidentes factor = new CatFactoresAccidentes();
            factor.IdFactorAccidente = model.IdFactorAccidente;
            factor.FactorAccidente = model.FactorAccidente;
            factor.Estatus = model.Estatus;
            factor.FechaActualizacion = DateTime.Now;
            dbContext.Entry(factor).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminaFactorAccidente(CatFactoresAccidentesModel model)
        {

            CatFactoresAccidentes factor = new CatFactoresAccidentes();
            factor.IdFactorAccidente = model.IdFactorAccidente;
            factor.FactorAccidente = model.FactorAccidente;
            factor.Estatus = 0;
            factor.FechaActualizacion = DateTime.Now;
            dbContext.Entry(factor).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatFactoresAccidentesModel GetFactorAccidenteByID(int IdFactorAccidente)
        {

            var productEnitity = dbContext.CatFactoresAccidentes.Find(IdFactorAccidente);

            var factoresAccidentesModel = (from catFactoresAccidentes in dbContext.CatFactoresAccidentes.ToList()
                                           select new CatFactoresAccidentesModel

                                           {
                                               IdFactorAccidente = catFactoresAccidentes.IdFactorAccidente,
                                               FactorAccidente = catFactoresAccidentes.FactorAccidente,


                                           }).Where(w => w.IdFactorAccidente == IdFactorAccidente).FirstOrDefault();

            return factoresAccidentesModel;
        }


        public List<CatFactoresAccidentesModel> GetFactoresAccidentes()
        {
            var ListFactoresAccidentesModel = (from catFactoresAccidentes in dbContext.CatFactoresAccidentes.ToList()
                                               join estatus in dbContext.Estatus.ToList()
                                               on catFactoresAccidentes.Estatus equals estatus.estatus
                                               select new CatFactoresAccidentesModel
                                               {
                                                   IdFactorAccidente = catFactoresAccidentes.IdFactorAccidente,
                                                   FactorAccidente = catFactoresAccidentes.FactorAccidente,
                                                   estatusDesc = estatus.estatusDesc,

                                               }).ToList();
            return ListFactoresAccidentesModel;
        }
        #endregion



    }
}
