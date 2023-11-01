﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class SalidaVehiculosController : BaseController
    {
        private readonly ISalidaVehiculosService _salidaVehiculosService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly IMarcasVehiculos _marcaServices;
        private readonly IPlacaServices _placaServices;



        public SalidaVehiculosController(ISalidaVehiculosService salidaVehiculosService, ICatMarcasVehiculosService catMarcasVehiculosService,
            IMarcasVehiculos marcaServices, IPlacaServices placaServices)
        {
            _salidaVehiculosService = salidaVehiculosService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _marcaServices = marcaServices;
            _placaServices = placaServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult Marcas_Read()
        {
            var result = new SelectList(_marcaServices.GetMarcas(), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }
        public JsonResult Placas_Read()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var result = new SelectList(_placaServices.GetPlacasByDelegacionId(idOficina), "IdDepositos", "Placa");
            return Json(result);
        }
        public IActionResult ajax_BusquedaIngresos(SalidaVehiculosModel model)
        {
            var listaDepositos = _salidaVehiculosService.ObtenerIngresos(model);
            return Json(listaDepositos);
        }
        public IActionResult DatosDeposito(int iDp)
        {
            HttpContext.Session.SetInt32("idDeposito", iDp);
            var infoDeposito = _salidaVehiculosService.DetallesDeposito(iDp);

            return View(infoDeposito);
        }
        public JsonResult GetGruasAsignadas([DataSourceRequest] DataSourceRequest request)
        {
            int iDp = HttpContext.Session.GetInt32("idDeposito") ?? 0; 
            var ListFactores = _salidaVehiculosService.ObtenerDatosGridGruas(iDp);

            return Json(ListFactores.ToDataSourceResult(request));
        }
        public ActionResult ModalCostosGrua(int idDeposito)
        {
            var DatosGruaSeleccionada = _salidaVehiculosService.CostosServicio(idDeposito);

            return PartialView("_ModalCostosServicio", DatosGruaSeleccionada);
        }
        public ActionResult GuardarCostos(CostosServicioModel model)
        {
            var DatosGruaSeleccionada = _salidaVehiculosService.ActualizarCostos(model);
         
            return PartialView ("_ListadoGruas");
        }
        public ActionResult GuardarDatosSalida(SalidaVehiculosModel model)
        {
            var DatosGruaSeleccionada = _salidaVehiculosService.GuardarInforSalida(model);

            return PartialView("_ListadoGruas");

        }

    }
}
