using GuanajuatoAdminUsuarios.Entity;
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
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class OficialesController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            var ListOficialesModel = _oficialesService.GetOficiales();
            ViewBag.ListadoOficiales = ListOficialesModel;

            return View();
            }
        private readonly IOficiales _oficialesService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;

        public OficialesController(IOficiales oficialesService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _oficialesService = oficialesService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }


        #region Modal Action
        public ActionResult IndexModal()
        {

            var ListOficialesModel = _oficialesService.GetOficiales();
            ViewBag.ListadoOficiales = ListOficialesModel;

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

                _oficialesService.SaveOficial(model);
                var ListOficialesModel = _oficialesService.GetOficiales();
                return Json(ListOficialesModel);
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
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
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
            var result = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
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
        [HttpGet]
        public ActionResult ajax_BuscarDelegacion(int idDelegacionFiltro)
        {
            List<CatOficialesModel> ListOfcialesDelegacion = new List<CatOficialesModel>();


            ListOfcialesDelegacion = (from oficiales in _oficialesService.GetOficiales().ToList()
                                         // join delegacion in _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().ToList()
                                          //on oficiales.IdOficina equals delegacion.IdOficinaTransporte
                                          // join estatus in dbContext.Estatus.ToList()
                                          //on diasInhabiles.Estatus equals estatus.estatus

                                      select new CatOficialesModel
                                      {
                                          IdOficial = oficiales.IdOficial,
                                          Nombre = oficiales.Nombre,
                                          ApellidoPaterno = oficiales.ApellidoPaterno,
                                          ApellidoMaterno = oficiales.ApellidoMaterno,
                                          IdOficina = oficiales.IdOficina,
                                          Estatus = oficiales.Estatus,
                                          nombreOficina = oficiales.nombreOficina,
                                          // EstatusDesc = estatus.estatusDesc,
                                          // = diasInhabiles.Municipio
                                      }).ToList();

      
             if (idDelegacionFiltro > 0)
            {
                ListOfcialesDelegacion = (from s in ListOfcialesDelegacion
                                          where s.IdOficina == idDelegacionFiltro
                                          select s).ToList();
            }

            return Json(ListOfcialesDelegacion);
        }

    }
}



