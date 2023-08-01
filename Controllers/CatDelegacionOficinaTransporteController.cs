﻿using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
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
    public class CatDelegacionesOficinasTransporteController : Controller
    {
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;



        public CatDelegacionesOficinasTransporteController(ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 988;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {

                var ListDelegacionesOficinasTModel = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinas();

            return View("Index", ListDelegacionesOficinasTModel);
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
            var ListDelegacionesOficinasTModel = GetDelegacionesOficinas();
            return View("Index", ListDelegacionesOficinasTModel);
        }

        [HttpPost]
        public ActionResult AgregarDelegacionOficinaModal()
        {
            int IdModulo = 989;
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

        public ActionResult EditarDelegacionOficinaModal(int IdOficinaTransporte)
        {
            int IdModulo = 990;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                Municipios_Drop();
            var delegacionOficinaModel = GetDelegacionOficinaByID(IdOficinaTransporte);
            return PartialView("_Editar", delegacionOficinaModel);
        }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
    }
}

        public ActionResult EliminarDelegacionOficinaModal(int IdOficinaTransporte)
        {
            Municipios_Drop();
            var delegacionOficinaModel = GetDelegacionOficinaByID(IdOficinaTransporte);
            return PartialView("_Eliminar", delegacionOficinaModel);
        }



        [HttpPost]
        public ActionResult AgregarDelegacionOficinaMod(CatDelegacionesOficinasTransporteModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                CrearDelegacionOficina(model);
                var ListDelegacionesOficinasTModel = GetDelegacionesOficinas();
                return PartialView("_ListDelegacionesOficinas", ListDelegacionesOficinasTModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarDelegacionOficinaMod(CatDelegacionesOficinasTransporteModel model)
        {
            bool switchDelegacionOficinas = Request.Form["delegacionOficinaSwitch"].Contains("true");
            model.Estatus = switchDelegacionOficinas ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                EditarDelegacionOficina(model);
                var ListDelegacionesOficinasTModel = GetDelegacionesOficinas();
                return PartialView("_ListDelegacionesOficinas", ListDelegacionesOficinasTModel);
            }
            return PartialView("_Editar");
        }

        public ActionResult EliminarDelegacionOficinaMod(CatDelegacionesOficinasTransporteModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                EliminaDelegacionOficina(model);
                var ListDelegacionesOficinasTModel = GetDelegacionesOficinas();
                return PartialView("_ListDelegacionesOficinas", ListDelegacionesOficinasTModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetDelegacionOfs([DataSourceRequest] DataSourceRequest request)
        {
            var ListDelegacionesOficinasTModel = GetDelegacionesOficinas();

            return Json(ListDelegacionesOficinasTModel.ToDataSourceResult(request));
        }

        
        public JsonResult Municipios_Drop()
        {
            var result = new SelectList(dbContext.CatMunicipios.ToList(), "IdMunicipio", "Municipio");
            return Json(result);
        }
    


    #endregion


    #region Acciones a base de datos

    public void CrearDelegacionOficina(CatDelegacionesOficinasTransporteModel model)
        {
            CatDelegacionesOficinasTransporte delOficina = new CatDelegacionesOficinasTransporte();
            delOficina.IdOficinaTransporte = model.IdOficinaTransporte;
            delOficina.NombreOficina = model.NombreOficina;
            delOficina.JefeOficina = model.JefeOficina;
            delOficina.IdMunicipio = model.IdMunicipio;
            delOficina.Estatus = 1;
            delOficina.FechaActualizacion = DateTime.Now;
            dbContext.CatDelegacionesOficinasTransporte.Add(delOficina);
            dbContext.SaveChanges();
        }

        public void EditarDelegacionOficina(CatDelegacionesOficinasTransporteModel model)
        {
            CatDelegacionesOficinasTransporte delOficina = new CatDelegacionesOficinasTransporte();
            delOficina.IdOficinaTransporte = model.IdOficinaTransporte;
            delOficina.NombreOficina = model.NombreOficina;
            delOficina.JefeOficina = model.JefeOficina;
            delOficina.IdMunicipio = model.IdMunicipio;
            delOficina.Estatus = model.Estatus;
            delOficina.FechaActualizacion = DateTime.Now;
            dbContext.Entry(delOficina).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminaDelegacionOficina(CatDelegacionesOficinasTransporteModel model)
        {
            CatDelegacionesOficinasTransporte delOficina = new CatDelegacionesOficinasTransporte();
            delOficina.IdOficinaTransporte = model.IdOficinaTransporte;
            delOficina.NombreOficina = model.NombreOficina;
            delOficina.JefeOficina = model.JefeOficina;
            delOficina.IdMunicipio = model.IdMunicipio;
            delOficina.Estatus = 0;
            delOficina.FechaActualizacion = DateTime.Now;
            dbContext.Entry(delOficina).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatDelegacionesOficinasTransporteModel GetDelegacionOficinaByID(int IdOficinaTransporte)
        {

            var productEnitity = dbContext.CatFactoresOpcionesAccidentes.Find(IdOficinaTransporte);

            var delegacionesOficinasModel = (from catDelegacionesOficinasTransporte in dbContext.CatDelegacionesOficinasTransporte.ToList()
                                             select new CatDelegacionesOficinasTransporteModel

                                             {
                                                 IdOficinaTransporte = catDelegacionesOficinasTransporte.IdOficinaTransporte,
                                                 NombreOficina = catDelegacionesOficinasTransporte.NombreOficina,
                                                 JefeOficina = catDelegacionesOficinasTransporte.JefeOficina,
                                                 IdMunicipio = catDelegacionesOficinasTransporte.IdMunicipio,
                                                 Estatus = catDelegacionesOficinasTransporte.Estatus,


                                             }).Where(w => w.IdOficinaTransporte == IdOficinaTransporte).FirstOrDefault();

            return delegacionesOficinasModel;
        }


        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas()
        {
            var ListDelegacionesOficinasModel = (from catDelegacionesOficinasTransporte in dbContext.CatDelegacionesOficinasTransporte.ToList()
                                                 join catMunicipios in dbContext.CatMunicipios.ToList()
                                                 on catDelegacionesOficinasTransporte.IdMunicipio equals catMunicipios.IdMunicipio
                                                 join estatus in dbContext.Estatus.ToList()
                                                 on catDelegacionesOficinasTransporte.Estatus equals estatus.estatus
                                                 select new CatDelegacionesOficinasTransporteModel
                                                 {
                                                     IdOficinaTransporte = catDelegacionesOficinasTransporte.IdOficinaTransporte,
                                                     NombreOficina = catDelegacionesOficinasTransporte.NombreOficina,
                                                     JefeOficina = catDelegacionesOficinasTransporte.JefeOficina,
                                                     IdMunicipio = catDelegacionesOficinasTransporte.IdMunicipio,
                                                     Municipio = catMunicipios.Municipio,
                                                     Estatus = catDelegacionesOficinasTransporte.Estatus,
                                                     estatusDesc = estatus.estatusDesc,

                                                 }).ToList();
            return ListDelegacionesOficinasModel;
        }
        #endregion



    }
}
