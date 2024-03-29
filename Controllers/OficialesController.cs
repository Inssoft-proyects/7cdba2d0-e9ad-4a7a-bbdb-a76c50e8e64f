﻿using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
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
using System.Globalization;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class OficialesController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        
        public IActionResult Index()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var ListOficialesModel = _oficialesService.GetOficialesByCorporacion(corp);
            ViewBag.ListadoOficiales = ListOficialesModel;
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");

            return View();
            }
        private readonly IOficiales _oficialesService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly ICatDictionary _catDictionary;
        public OficialesController(IOficiales oficialesService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, ICatDictionary catDictionary)
        {
            _oficialesService = oficialesService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _catDictionary = catDictionary;
        }

		public JsonResult OficialesDependencia_Drop()
		{
			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			var oficiales = _oficialesService.GetOficialesPorDependencia(idDependencia)
				.Select(o => new
				{
					IdOficial = o.IdOficial,
					NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
				});
			//oficiales = oficiales.Skip(1);
			var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

			return Json(result);
		}

        public JsonResult OficialesDependenciaActivos_Drop()
        {
            var tipo = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var oficiales = _oficialesService.GetOficialesActivosTodos(true, tipo)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            //oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }

        public JsonResult OficialesDependenciaTodos_Drop()
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var oficiales = _oficialesService.GetOficialesPorDependencia()
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            //oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }
        #region Modal Action
        public ActionResult IndexModal()
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var oficiales = _oficialesService.GetOficialesPorDependencia(idDependencia);
           ViewBag.ListadoOficiales = oficiales;

            return View("Index");
        }



        [HttpPost]
        public ActionResult AgregarOficialParcial()
        {

                Delegaciones_Drop();
            return PartialView("_Crear");
            }


        [HttpPost]
        public ActionResult EditarOficialParcial(int IdOficial)
        {

                Delegaciones_Drop();
            var oficialesModel = _oficialesService.GetOficialById(IdOficial);
            return PartialView("_Editar", oficialesModel);
            }
 

        [HttpPost]
        public ActionResult EliminarOficialParcial(int IdOficial)
        {
            Delegaciones_Drop();
            var oficialesModel = _oficialesService.GetOficialById(IdOficial);
            return View("_Eliminar", oficialesModel);
        }

        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.Oficiales.ToList(), "IdOficial", "Nombre");
            return Json(result);
        }



        [HttpPost]
        public ActionResult AgregarOficialModal(CatOficialesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {
                int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
                

                _oficialesService.SaveOficial(model,idDependencia);
                //var ListOficialesModel = _oficialesService.GetOficiales();
                return Json(1);
            }

            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarOficial(CatOficialesModel model)
        {
            bool switchOficiales = Request.Form["oficialesSwitch"].Contains("true");
            model.Estatus = switchOficiales ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {

                _oficialesService.UpdateOficial(model);
                var ListOficialesModel = _oficialesService.GetOficiales();
                return Json(ListOficialesModel);
            }

            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarOficial(OficialesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {

                DeleteOficial(model);
                var ListOficialesModel = _oficialesService.GetOficiales();
                return Json(ListOficialesModel);
            }

            return PartialView("_Eliminar");
        }

        public JsonResult GetOficialess([DataSourceRequest] DataSourceRequest request)
        {
            var ListOficialesModel = _oficialesService.GetOficiales();

            return Json(ListOficialesModel.ToDataSourceResult(request));
        }

        public JsonResult DelegacionesOficinas_Drop()
        {
            var tipo = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().Where(x=>x.Transito == tipo), "IdDelegacion", "Delegacion");
            return Json(result);
        }


        #endregion


        #region Acciones a base de datos

        public void CreateOficial(OficialesModel model)
        {
            Oficiales oficial = new Oficiales();
            oficial.IdOficial = model.IdOficial;
            oficial.Rango = model.Rango;
            oficial.Nombre = model.Nombre;
            oficial.ApellidoPaterno = model.ApellidoPaterno;
            oficial.ApellidoMaterno = model.ApellidoMaterno;
            oficial.Estatus = 1;
            oficial.FechaActualizacion = DateTime.Now;
            //oficial.IdDelegacion = model.IdDelegacion;

            dbContext.Oficiales.Add(oficial);
            dbContext.SaveChanges();
        }

        public void UpdateOficial(OficialesModel model)
        {
            Oficiales oficial = new Oficiales();
            oficial.IdOficial = model.IdOficial;
            oficial.Nombre = model.Nombre;
            oficial.ApellidoPaterno = model.ApellidoPaterno;
            oficial.ApellidoMaterno = model.ApellidoMaterno;
            oficial.Estatus = model.Estatus;
            oficial.FechaActualizacion = DateTime.Now;
            //oficial.IdDelegacion = model.IdDelegacion;

            dbContext.Entry(oficial).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteOficial(OficialesModel model)
        {
            Oficiales oficial = new Oficiales();
            oficial.IdOficial = model.IdOficial;
            oficial.Nombre = model.Nombre;
            oficial.ApellidoPaterno = model.ApellidoPaterno;
            oficial.ApellidoMaterno = model.ApellidoMaterno;
            oficial.Estatus = 0;
            oficial.FechaActualizacion = DateTime.Now;
            //oficial.IdDelegacion = model.IdDelegacion;

            dbContext.Entry(oficial).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

       

        public JsonResult Delegaciones_Drop()
        {
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            //var result = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
            var result = ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            
            return Json(result);
        }
        


        public OficialesModel GetOficialByID(int IdOficial)
        {

            var productEnitity = dbContext.Oficiales.Find(IdOficial);

            var oficialModel = (from oficiales in dbContext.Oficiales.ToList()
                                select new OficialesModel

                                {
                                    IdOficial = oficiales.IdOficial, 
                                    Rango = oficiales.Rango,
                                    Nombre = oficiales.Nombre,
                                    ApellidoPaterno = oficiales.ApellidoPaterno,
                                    ApellidoMaterno = oficiales.ApellidoMaterno,



                                }).Where(w => w.IdOficial == IdOficial).FirstOrDefault();

            return oficialModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatOficialesModel> GetOficiales()
        {
            var ListOficialessModel = (from oficiales in dbContext.Oficiales.ToList() 
                                       join estatus in dbContext.Estatus.ToList()
                                       on oficiales.Estatus equals estatus.estatus
                                       select new CatOficialesModel
                                       {
                                           IdOficial = oficiales.IdOficial,
                                           Nombre = oficiales.Nombre,
                                           ApellidoPaterno = oficiales.ApellidoPaterno,
                                           ApellidoMaterno = oficiales.ApellidoMaterno,
                                           estatusDesc = estatus.estatusDesc

                                       }).ToList();
            return ListOficialessModel;
        }
        #endregion
     //   [HttpGet]
        public ActionResult ajax_BuscarDelegacion ([DataSourceRequest] DataSourceRequest request, int idDelegacionFiltro, string nombre, string apellidoPaterno, string apellidoMaterno)
        {
            List<CatOficialesModel> ListOfcialesDelegacion = new List<CatOficialesModel>();
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            ListOfcialesDelegacion = _oficialesService.GetOficialesPorDependencia2(idDependencia);

            if (idDelegacionFiltro > 0)
            {
                ListOfcialesDelegacion = (from s in ListOfcialesDelegacion
                                          where s.IdOficina == idDelegacionFiltro
                                          select s).ToList();
            }
            if (nombre != null)
            {
                ListOfcialesDelegacion = (from s in ListOfcialesDelegacion
                                          where s.Nombre.ToUpper().Contains(nombre.ToUpper())
                                          select s).ToList();
            }
            if (apellidoPaterno != null)
            {
                ListOfcialesDelegacion = (from s in ListOfcialesDelegacion
                                          where s.ApellidoPaterno.ToUpper().Contains(apellidoPaterno.ToUpper())
                                          select s).ToList();
            }
            if (apellidoMaterno != null)
            {
                ListOfcialesDelegacion = (from s in ListOfcialesDelegacion
                                          where s.ApellidoMaterno.ToUpper().Contains(apellidoMaterno.ToUpper())
                                          select s).ToList();
            }

            return Json(ListOfcialesDelegacion.ToDataSourceResult(request));
        }

    }
}



