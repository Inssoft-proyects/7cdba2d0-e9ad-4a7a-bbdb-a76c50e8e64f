using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using System.Net;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class LiberacionVehiculoController : BaseController
    {
        #region DPIServices

        private readonly IPlacaServices _placaServices;
        private readonly IMarcasVehiculos _marcaServices;
        private readonly ILiberacionVehiculoService _liberacionVehiculoService;
        private readonly IRepuveService _repuveService;

        public LiberacionVehiculoController(IPlacaServices placaServices,
            IMarcasVehiculos marcaServices, ILiberacionVehiculoService liberacionVehiculoService, IRepuveService repuveService)
        {
            _placaServices = placaServices;
            _marcaServices = marcaServices;
            _liberacionVehiculoService = liberacionVehiculoService;
            _repuveService = repuveService;
        }


        #endregion

        public IActionResult Index()
        {
            int IdModulo = 303;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                LiberacionVehiculoBusquedaModel searchModel = new LiberacionVehiculoBusquedaModel();
                List<LiberacionVehiculoModel> ListDepositos = _liberacionVehiculoService.GetAllTopDepositos(idOficina);
                searchModel.ListDepositosLiberacion = ListDepositos;
                return View(searchModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }


        public JsonResult Placas_Read()
		{
			int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
			var result = new SelectList(_placaServices.GetPlacasByDelegacionId(idOficina), "IdDepositos", "Placa");
            return Json(result);
        }

        public JsonResult Marcas_Read()
        {
            var result = new SelectList(_marcaServices.GetMarcas(), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }

        [HttpPost]
        public ActionResult ajax_BuscarVehiculo(LiberacionVehiculoBusquedaModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var ListVehiculosModel = _liberacionVehiculoService.GetDepositos(model, idOficina);

            if (ListVehiculosModel.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron resultados que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListadoVehiculos", ListVehiculosModel);
        }


        [HttpGet]
        public ActionResult ajax_UpdateLiberacion(int Id)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var model = _liberacionVehiculoService.GetDepositoByID(Id, idOficina);
            RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel()
            {
                placa = model.Placa,
                niv = model.Serie
            };
            var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel).FirstOrDefault();
            ViewBag.ReporteRobo = repuveConsRoboResponse.estatus == 1;
            //model.FechaIngreso.ToString("dd/MM/yyyy");
            return PartialView("_UpdateLiberacion", model);

        }

        //public ActionResult UpdateLiberacion(LiberacionVehiculoModel model, IFormFile ImageAcreditacionPropiedad, IFormFile ImageAcreditacionPersonalidad, IFormFile ImageReciboPago)
        [HttpPost]
        public ActionResult UpdateLiberacion(IFormFile ImageAcreditacionPropiedad, IFormFile ImageAcreditacionPersonalidad, IFormFile ImageReciboPago, string data)
        {
            int result = 0;
            try
            {

                var model = JsonConvert.DeserializeObject<LiberacionVehiculoModel>(data);
                if (ImageAcreditacionPropiedad != null)
                {
                    using (var ms1 = new MemoryStream())
                    {
                        ImageAcreditacionPropiedad.CopyTo(ms1);
                        model.AcreditacionPropiedad = ms1.ToArray();
                    }
                }
                if (ImageAcreditacionPersonalidad != null)
                {
                    using (var ms2 = new MemoryStream())
                    {
                        ImageAcreditacionPersonalidad.CopyTo(ms2);
                        model.AcreditacionPersonalidad = ms2.ToArray();
                    }
                }
                if (ImageReciboPago != null)
                {
                    using (var ms3 = new MemoryStream())
                    {
                        ImageReciboPago.CopyTo(ms3);
                        model.ReciboPago = ms3.ToArray();
                    }
                }

                ////Prueba de que allmacena bien la imagen  
                ////var imgByte = model.AcreditacionPropiedad;
                ////return new FileContentResult(imgByte, "image/jpeg");
                result = _liberacionVehiculoService.UpdateDeposito(model);

            }
            catch (Exception ex)
            {

            }
            if (result > 0)
            {
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                List<LiberacionVehiculoModel> ListProuctModel = _liberacionVehiculoService.GetAllTopDepositos(idOficina);
                return Json(ListProuctModel);
            }
            else
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(null);
            }
        }


    }
}
