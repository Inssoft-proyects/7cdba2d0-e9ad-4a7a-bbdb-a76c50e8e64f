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
    public class DiasInhabilesController : BaseController
    {
        private readonly IDiasInhabiles _diasInhabiles;
        private readonly ICatMunicipiosService _catMunicipiosService;


        public DiasInhabilesController(IDiasInhabiles diasInhabiles, ICatMunicipiosService catMunicipiosService)
        {
            _diasInhabiles = diasInhabiles;
            _catMunicipiosService = catMunicipiosService;
        }
        public IActionResult Index()
        {
         
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles();

            return View(ListDiasInhabilesModel);
            }
        

        ///Crear metodo de update (post)


        #region Modal Action
       
        [HttpPost]
        public ActionResult AgregarParcialDiaInhabil()
        {

            Municipios_Read();
            return PartialView("_Crear");
            }
     

        public ActionResult EditarParcial(int idDiaInhabil)
        {
          
             var diasInhabilesModel = _diasInhabiles.GetDiasById(idDiaInhabil);
            Municipios_Read();
            return View("_Editar", diasInhabilesModel);
         }
       

/*public ActionResult EliminarDiaInhabilParcial(int IdDiaInhabil)
        {
            var diasInhabilesModel = GetDiaInhabilByID(IdDiaInhabil);
            Municipios_Drop();
            return View("_Eliminar", diasInhabilesModel);
        }*/

        public JsonResult Municipios_Read()
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosGuanajuato(), "IdMunicipio", "Municipio");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(DiasInhabilesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {


                _diasInhabiles.CrearDiaInhabil(model);
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles();
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
                _diasInhabiles.EditDia(model);
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles();
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
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles();
                return Json(ListDiasInhabilesModel);
            }
            Municipios_Drop();
            //return View("Create");
            return PartialView("_Eliminar");
        }
        public JsonResult GetDiasIn([DataSourceRequest] DataSourceRequest request)
        {
            var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles();

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
            //dbContext.DiasInhabiles.Add(diaInhabil);
            //dbContext.SaveChanges();
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

            //dbContext.Entry(diaInhabil).State = EntityState.Modified;
            //dbContext.SaveChanges();

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
           // dbContext.Entry(diaInhabil).State = EntityState.Modified;
            //dbContext.SaveChanges();

        }

      

        public JsonResult Municipios_Drop()
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipios(), "IdMunicipio", "Municipio");
            ViewBag.CatMunicipios = result;
            return Json(result);
        }

        /*  public DiasInhabilesModel GetDiaInhabilByID(int IdDiaInhabil)
          {

              var productEnitity = dbContext.DiasInhabiles.Find(IdDiaInhabil);

              var diaInhabilModel = (from diasInhabiles in dbContext.DiasInhabiles.ToList()
                                     select new DiasInhabilesModel

                                     {
                                         idDiaInhabil = diasInhabiles.idDiaInhabil,
                                         fecha =  diasInhabiles.fecha,
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
          #
        */
        #endregion
        [HttpGet]
        public ActionResult ajax_BuscarDiasInhabiles(string fecha, int idMunicipio)
        {
            List<DiasInhabilesModel> ListDiasInhabilesModel = new List<DiasInhabilesModel>();
            if (!String.IsNullOrEmpty(fecha))
            {
                if (fecha.ToUpper().Contains("DAY"))
                    fecha = null;
            }

            ListDiasInhabilesModel = (from diasInhabiles in _diasInhabiles.GetDiasInhabiles().ToList()
                                          //join municipio in _catMunicipiosService.GetMunicipios().ToList()
                                          //on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                         // join estatus in dbContext.Estatus.ToList()
                                          //on diasInhabiles.Estatus equals estatus.estatus

                                          select new DiasInhabilesModel
                                          {
                                              idDiaInhabil = diasInhabiles.idDiaInhabil,
                                              fecha = diasInhabiles.fecha,
                                              idMunicipio = diasInhabiles.idMunicipio,
                                              todosMunicipiosDesc = diasInhabiles.todosMunicipiosDesc,
                                              Estatus = diasInhabiles.Estatus,
                                             // EstatusDesc = estatus.estatusDesc,
                                              Municipio = diasInhabiles.Municipio
                                          }).ToList();

            if (!String.IsNullOrEmpty(fecha) && idMunicipio > 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where Convert.ToDateTime(s.fecha) == Convert.ToDateTime(fecha)
                                          &&
                                          s.idMunicipio == idMunicipio
                                          select s).ToList();
            } else if (!String.IsNullOrEmpty(fecha) && idMunicipio == 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where Convert.ToDateTime(s.fecha) == Convert.ToDateTime(fecha)
                                          select s).ToList();
            }
            else if (String.IsNullOrEmpty(fecha) && idMunicipio > 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where s.idMunicipio == idMunicipio
                                          select s).ToList();
            }

            return Json(ListDiasInhabilesModel);
        }

    }
}
