using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.WebUI.Controllers
{
    public class DiasInhabilesController : Controller
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 948;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListDiasInhabilesModel = GetDiasInhabiles();

            return View(ListDiasInhabilesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }

        ///Crear metodo de update (post)


        #region Modal Action
        public ActionResult IndexModal()
        {
            var ListDiasInhabilesModel = GetDiasInhabiles();
            //return View("IndexModal");
            return View("Index", ListDiasInhabilesModel);
        }

        [HttpPost]
        public ActionResult AgregarParcialDiaInhabil()
        {
            int IdModulo = 949;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                Municipios_Drop();
            return PartialView("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        public ActionResult EditarParcial(int IdDiaInhabil)
        {
            int IdModulo = 950;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var diasInhabilesModel = GetDiaInhabilByID(IdDiaInhabil);
            Municipios_Drop();
            return View("_Editar", diasInhabilesModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        public ActionResult EliminarDiaInhabilParcial(int IdDiaInhabil)
        {
            var diasInhabilesModel = GetDiaInhabilByID(IdDiaInhabil);
            Municipios_Drop();
            return View("_Eliminar", diasInhabilesModel);
        }

        public JsonResult Municipios_Read()
        {
            var dataSource = new SelectList(dbContext.CatMunicipios.ToList(), "IdMunicipio", "Municipio");
            return Json(dataSource);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(DiasInhabilesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {


                CreateDiaInhabil(model);
                var ListDiasInhabilesModel = GetDiasInhabiles();
                return PartialView("_ListaDiasInhabiles", ListDiasInhabilesModel);
            }
            Municipios_Drop();
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcialModal(DiasInhabilesModel model)
        {
            bool switchDiasinhabiles = Request.Form["diasInhabilesSwitch"].Contains("true");
            model.Estatus = switchDiasinhabiles ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {
                UpdateDiaInhabil(model);
                var ListDiasInhabilesModel = GetDiasInhabiles();
                return PartialView("_ListaDiasInhabiles", ListDiasInhabilesModel);
            }
            Municipios_Drop();
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarDiaParcialModal(DiasInhabilesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {


                DeleteDiaInhabil(model);
                var ListDiasInhabilesModel = GetDiasInhabiles();
                return PartialView("_ListaDiasInhabiles", ListDiasInhabilesModel);
            }
            Municipios_Drop();
            //return View("Create");
            return PartialView("_Eliminar");
        }
        public JsonResult GetDiasIn([DataSourceRequest] DataSourceRequest request)
        {
            var ListDiasInhabilesModel = GetDiasInhabiles();

            return Json(ListDiasInhabilesModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateDiaInhabil(DiasInhabilesModel model)
        {
            DiasInhabiles diaInhabil = new DiasInhabiles();
            diaInhabil.idDiaInhabil = model.idDiaInhabil;
            diaInhabil.fecha = model.fecha;
            diaInhabil.idMunicipio = model.idMunicipio;
            diaInhabil.todosMunicipiosDesc = model.todosMunicipiosDesc;
            diaInhabil.Estatus = 1;
            diaInhabil.FechaActualizacion = DateTime.Now;
            dbContext.DiasInhabiles.Add(diaInhabil);
            dbContext.SaveChanges();
        }

        public void UpdateDiaInhabil(DiasInhabilesModel model)
        {
            DiasInhabiles diaInhabil = new DiasInhabiles();
            diaInhabil.idDiaInhabil = model.idDiaInhabil;
            diaInhabil.fecha = model.fecha;
            diaInhabil.idMunicipio = model.idMunicipio;
            diaInhabil.todosMunicipiosDesc = model.todosMunicipiosDesc;
            diaInhabil.Estatus = model.Estatus;
            diaInhabil.FechaActualizacion = DateTime.Now;

            dbContext.Entry(diaInhabil).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteDiaInhabil(DiasInhabilesModel model)
        {
            DiasInhabiles diaInhabil = new DiasInhabiles();
            diaInhabil.idDiaInhabil = model.idDiaInhabil;
            diaInhabil.fecha = model.fecha;
            diaInhabil.idMunicipio = model.idMunicipio;
            diaInhabil.todosMunicipiosDesc = model.todosMunicipiosDesc;
            diaInhabil.Estatus = 0;
            diaInhabil.FechaActualizacion = DateTime.Now;
            dbContext.Entry(diaInhabil).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

      

        public JsonResult Municipios_Drop()
        {
            var result = new SelectList(dbContext.CatMunicipios.ToList(), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public DiasInhabilesModel GetDiaInhabilByID(int IdDiaInhabil)
        {

            var productEnitity = dbContext.DiasInhabiles.Find(IdDiaInhabil);

            var diaInhabilModel = (from diasInhabiles in dbContext.DiasInhabiles.ToList()
                                   select new DiasInhabilesModel

                                   {
                                       idDiaInhabil = diasInhabiles.idDiaInhabil,
                                       fecha = diasInhabiles.fecha,
                                       idMunicipio = diasInhabiles.idMunicipio,
                                       todosMunicipiosDesc = diasInhabiles.todosMunicipiosDesc,



                                   }).Where(w => w.idDiaInhabil == IdDiaInhabil).FirstOrDefault();

            return diaInhabilModel;
        }

        public List<DiasInhabilesModel> GetDiasInhabiles()
        {
            var ListDiasInhabilesModel = (from diasInhabiles in dbContext.DiasInhabiles.ToList()
                                          join municipio in dbContext.CatMunicipios.ToList()
                                          on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                          join estatus in dbContext.Estatus.ToList()
                                          on diasInhabiles.Estatus equals estatus.estatus



                                          select new DiasInhabilesModel
                                          {
                                              idDiaInhabil = diasInhabiles.idDiaInhabil,
                                              fecha = diasInhabiles.fecha,
                                              idMunicipio = diasInhabiles.idMunicipio,
                                              todosMunicipiosDesc = diasInhabiles.todosMunicipiosDesc,
                                              Estatus = diasInhabiles.Estatus,
                                              EstatusDesc = estatus.estatusDesc,
                                              Municipio = municipio.Municipio

                                          }).ToList();
            return ListDiasInhabilesModel;
        }
        #endregion



    }
}
