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
    public class DependenciasController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            int IdModulo = 908;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var ListDependenciasModel = GetDependencias();

            return View(ListDependenciasModel);
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
            var ListDependenciasModel = GetDependencias();
            //return View("IndexModal");
            return View("Index", ListDependenciasModel);
        }

        [HttpPost]
        public ActionResult AgregarPacial()
        {
            int IdModulo = 909;
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

        [HttpPost]
        public ActionResult EditarParcial(int IdDependencia)
        {
            int IdModulo = 910;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                var dependenciasModel = GetDependenciaByID(IdDependencia);
            return PartialView("_Editar", dependenciasModel);
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no tiene permisos suficientes para esta acción.";
                return PartialView("ErrorPartial");
            }
        }

        [HttpPost]
        public ActionResult EliminarParcial(int IdDependencia)
        {
            var dependenciasModel = GetDependenciaByID(IdDependencia);
            return PartialView("_Eliminar", dependenciasModel);
        }



        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.Dependencias.ToList(), "IdDependencia", "NombreDependencia");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(DependenciasModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
                //Crear el producto

                CreateDependencia(model);
                var ListDependenciasModel = GetDependencias();
                return PartialView("_ListaDependencias", ListDependenciasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult UpdatePartialModal(DependenciasModel model)
        {
            bool switchDependencias = Request.Form["switchDependencias"].Contains("true");
            model.Estatus = switchDependencias ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
                //Crear el producto

                UpdateDependencia(model);
                var ListDependenciasModel = GetDependencias();
                return PartialView("_ListaDependencias", ListDependenciasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult DeletePartialModal(DependenciasModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
                //Crear el producto

                DeleteDependencia(model);
                var ListDependenciasModel = GetDependencias();
                return PartialView("_ListaDependencias", ListDependenciasModel);
            }

            return PartialView("_Eliminar");
        }
        public JsonResult GetDeps([DataSourceRequest] DataSourceRequest request)
        {
            var ListProuctModel = GetDependencias();

            return Json(ListProuctModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateDependencia(DependenciasModel model)
        {
            Dependencias dependencia = new Dependencias();
            dependencia.IdDependencia = model.IdDependencia;
            dependencia.NombreDependencia = model.NombreDependencia;
            dependencia.Estatus = 1;
            dependencia.FechaActualizacion = DateTime.Now;
            dbContext.Dependencias.Add(dependencia);
            dbContext.SaveChanges();
        }

        public void UpdateDependencia(DependenciasModel model)
        {
            Dependencias dependencia = new Dependencias();
            dependencia.IdDependencia = model.IdDependencia;
            dependencia.NombreDependencia = model.NombreDependencia;
            dependencia.Estatus = model.Estatus;
            dependencia.FechaActualizacion = DateTime.Now;
            dbContext.Entry(dependencia).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteDependencia(DependenciasModel model)
        {
            Dependencias dependencia = new Dependencias();
            dependencia.IdDependencia = model.IdDependencia;
            dependencia.NombreDependencia = model.NombreDependencia;
            dependencia.Estatus = 0;
            dependencia.FechaActualizacion = DateTime.Now;
            dbContext.Entry(dependencia).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        private void SetDDLDependencias()
        {
            ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
            ViewBag.Categories = new SelectList(dbContext.Dependencias.ToList(), "CategoryId", "CategoryName");
        }


        public DependenciasModel GetDependenciaByID(int IdDependencia)
        {

            var productEnitity = dbContext.Dependencias.Find(IdDependencia);

            var dependenciaModel = (from dependencias in dbContext.Dependencias.ToList()
                                    select new DependenciasModel

                                    {
                                        IdDependencia = dependencias.IdDependencia,
                                        NombreDependencia = dependencias.NombreDependencia,


                                    }).Where(w => w.IdDependencia == IdDependencia).FirstOrDefault();

            return dependenciaModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<DependenciasModel> GetDependencias()
        {
            var ListDependenciasModel = (from dependencias in dbContext.Dependencias.ToList()
                                         join estatus in dbContext.Estatus.ToList()
                                         on dependencias.Estatus equals estatus.estatus

                                         select new DependenciasModel
                                         {
                                             IdDependencia = dependencias.IdDependencia,
                                             NombreDependencia = dependencias.NombreDependencia,
                                             Estatus= dependencias.Estatus,
                                             estatusDesc = estatus.estatusDesc,

                                         }).ToList();
            return ListDependenciasModel;
        }
        #endregion



    }
}
