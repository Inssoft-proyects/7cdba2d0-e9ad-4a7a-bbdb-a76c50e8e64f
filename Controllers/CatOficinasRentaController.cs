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
    public class CatOficinasRentaController : BaseController
    {
        private readonly ICatOficinasRentaService _catOficinasRentaService;

        public CatOficinasRentaController(ICatOficinasRentaService catOficinasRentaService)
        {
            _catOficinasRentaService = catOficinasRentaService;
        }
        DBContextInssoft dbContext = new DBContextInssoft();






        #region Modal Action
        public ActionResult Index()
        {
            int IdModulo = 952;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListOficinasRentaModel = GetOficinas();

            return View("Index", ListOficinasRentaModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }

        public ActionResult Index2(int idDelegacion)
        {
            var ListOficinasRentaModel = ObtenerPorDel(idDelegacion);
            return View("Index2", ListOficinasRentaModel);
        }

        [HttpPost]
        public ActionResult AgregarOficinaRentaModal()
        {
            int IdModulo = 953;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                Delegaciones_Drop();
            return PartialView("_Crear");
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }


        public ActionResult EditarOficinaRentaModal(int IdOficinaRenta, int IdDelegacion)
        {
            int IdModulo = 954;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                Delegaciones_Drop();
            var oficinasRentaModel = GetOficinaRentaByID(IdOficinaRenta);
            return PartialView("_Editar", oficinasRentaModel);
        }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
    }
}

        public ActionResult EliminarOficinaRentaModal(int IdOficinaRenta)
        {
            Delegaciones_Drop();
            var oficinasRentaModel = GetOficinaRentaByID(IdOficinaRenta);
            return PartialView("_Eliminar", oficinasRentaModel);
        }



        [HttpPost]
        public ActionResult AgregarOficinaRenta(CatOficinasRentaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                CrearOficinaRenta(model);
                var ListOficinasRentaModel = GetOficinas();
                return Json(ListOficinasRentaModel);
            }
            Delegaciones_Drop();
            return PartialView("_Crear");
        }

        public ActionResult EditarOficinaRentaMod(CatOficinasRentaModel model)
        {
            bool switchOficinasRenta = Request.Form["oficinasRentaSwitch"].Contains("true");
            model.Estatus = switchOficinasRenta ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                EditarOficinaRenta(model);
                var ListOficinasRentaModel = GetOficinas();
                return Json(ListOficinasRentaModel);
            }
            Delegaciones_Drop();
            return PartialView("_Editar");
        }

        public ActionResult EliminarOficinasRentaMod(CatOficinasRentaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                EliminarOficinaRenta(model);
                var ListOficinasRentaModel = GetOficinas();
                return Json(ListOficinasRentaModel);
            }
            Delegaciones_Drop();
            return PartialView("_Eliminar");
        }

        public JsonResult GetOfiRent([DataSourceRequest] DataSourceRequest request)
        {
            var ListOficinasRentaModel = GetOficinas();

            return Json(ListOficinasRentaModel.ToDataSourceResult(request));
        }

        public JsonResult GetOfiPorDel([DataSourceRequest] DataSourceRequest request, int idDelegacion)
        {
            var ListOficinasRentaModel = ObtenerPorDel(idDelegacion);

            return Json(ListOficinasRentaModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearOficinaRenta(CatOficinasRentaModel model)
        {
            CatOficinasRenta oficina = new CatOficinasRenta();
            oficina.IdOficinaRenta = model.IdOficinaRenta;
            oficina.NombreOficina = model.NombreOficina;
            oficina.IdDelegacion = model.IdDelegacion;
            oficina.Estatus = 1;
            oficina.FechaActualizacion = DateTime.Now;
            dbContext.CatOficinasRenta.Add(oficina);
            dbContext.SaveChanges();
        }

        public void EditarOficinaRenta(CatOficinasRentaModel model)
        {
            CatOficinasRenta oficina = new CatOficinasRenta();
            oficina.IdOficinaRenta = model.IdOficinaRenta;
            oficina.NombreOficina = model.NombreOficina;
            oficina.IdDelegacion = model.IdDelegacion;
            oficina.Estatus = model.Estatus;
            oficina.FechaActualizacion = DateTime.Now;
            dbContext.Entry(oficina).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminarOficinaRenta(CatOficinasRentaModel model)
        {

            CatOficinasRenta oficina = new CatOficinasRenta();
            oficina.IdOficinaRenta = model.IdOficinaRenta;
            oficina.NombreOficina = model.NombreOficina;
            oficina.IdDelegacion = model.IdDelegacion;
            oficina.Estatus = 0;
            oficina.FechaActualizacion = DateTime.Now;
            dbContext.Entry(oficina).State = EntityState.Modified;
            dbContext.SaveChanges();


        }

       

        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
            return Json(result);
        }
        public CatOficinasRentaModel GetOficinaRentaByID(int IdOficinaRenta)
        {

            var productEnitity = dbContext.CatOficinasRenta.Find(IdOficinaRenta);

            var oficinasRentaModel = (from catOficinasRenta in dbContext.CatOficinasRenta.ToList()
                                      select new CatOficinasRentaModel

                                      {
                                          IdOficinaRenta = catOficinasRenta.IdOficinaRenta,
                                          NombreOficina = catOficinasRenta.NombreOficina,
                                          IdDelegacion = catOficinasRenta.IdDelegacion,


                                      }).Where(w => w.IdOficinaRenta == IdOficinaRenta).FirstOrDefault();

            return oficinasRentaModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatOficinasRentaModel> GetOficinas()
        {
            var ListOficinasRentaModel = (from catOficinasRenta in dbContext.CatOficinasRenta.ToList()
                                          join delegaciones in dbContext.Delegaciones.ToList()
                                          on catOficinasRenta.IdDelegacion equals delegaciones.IdDelegacion
                                          join estatus in dbContext.Estatus.ToList()
                                          on catOficinasRenta.Estatus equals estatus.estatus
                                          select new CatOficinasRentaModel
                                          {
                                              IdOficinaRenta = catOficinasRenta.IdOficinaRenta,
                                              NombreOficina = catOficinasRenta.NombreOficina,
                                              DelegacionDesc = delegaciones.Delegacion,
                                              IdDelegacion = delegaciones.IdDelegacion,
                                              estatusDesc = estatus.estatusDesc,


                                          }).ToList();
            return ListOficinasRentaModel;
        }

        public List<CatOficinasRentaModel> ObtenerPorDel(int IdDelegacion)
        {
            var ListaPorDelegacion = (from catOficinasRenta in dbContext.CatOficinasRenta.ToList()
                                      join delegaciones in dbContext.Delegaciones.ToList()
                                      on catOficinasRenta.IdDelegacion equals delegaciones.IdDelegacion
                                      select new CatOficinasRentaModel
                                      {
                                          IdOficinaRenta = catOficinasRenta.IdOficinaRenta,
                                          NombreOficina = catOficinasRenta.NombreOficina,
                                          DelegacionDesc = delegaciones.Delegacion,
                                          IdDelegacion = delegaciones.IdDelegacion,


                                      }).Where(w => w.IdDelegacion == IdDelegacion).ToList();
            return ListaPorDelegacion;
        }

    }
}
#endregion