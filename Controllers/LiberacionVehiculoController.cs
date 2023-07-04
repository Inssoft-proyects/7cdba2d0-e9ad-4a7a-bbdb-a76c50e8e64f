using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
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
using System.Net;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class LiberacionVehiculoController : Controller
    {
        #region DPIServices

        private readonly IPlacaServices _placaServices;
        private readonly IMarcasVehiculos _marcaServices;
        private readonly ILiberacionVehiculoService _liberacionVehiculoService;

        public LiberacionVehiculoController(IPlacaServices placaServices,
            IMarcasVehiculos marcaServices, ILiberacionVehiculoService liberacionVehiculoService)
        {
            _placaServices = placaServices;
            _marcaServices = marcaServices;
            _liberacionVehiculoService = liberacionVehiculoService;
        }


        #endregion

        public IActionResult Index()
        {
            LiberacionVehiculoBusquedaModel searchModel = new LiberacionVehiculoBusquedaModel();
            List<LiberacionVehiculoModel> ListDepositos = _liberacionVehiculoService.GetAllTopDepositos();
            searchModel.ListDepositosLiberacion = ListDepositos;
            return View(searchModel);
        }


        public JsonResult Placas_Read()
        {
            var result = new SelectList(_placaServices.GetPlacasByDelegacionId(1), "IdDepositos", "Placa");
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
            var ListVehiculosModel = _liberacionVehiculoService.GetDepositos(model);
            return PartialView("_ListadoVehiculos", ListVehiculosModel);

        }

        [HttpGet]
        public ActionResult ajax_UpdateLiberacion(int Id)
        {
            var model = _liberacionVehiculoService.GetDepositoByID(Id);
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
                #region funciona

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
                #endregion
                result = _liberacionVehiculoService.UpdateDeposito(model);

            }
            catch (Exception ex)
            {

            }
            if (result > 0)
            {
                List<LiberacionVehiculoModel> ListProuctModel = _liberacionVehiculoService.GetAllTopDepositos();
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
