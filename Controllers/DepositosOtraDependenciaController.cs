/*
 * Descripción:
 * Proyecto: Sistema de Infracciones y Accidentes
 * Fecha de creación: Sunday, February 18th 2024 9:40:13 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Fri Feb 23 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class DepositosOtraDependenciaController : BusquedaVehiculoPropietarioController
    {
        #region Variables
        #endregion

        #region Constructor
        public DepositosOtraDependenciaController(ICatDictionary catDictionary) : base(catDictionary) { }

        #endregion
        public IActionResult Depositos()
        {
            return View("DepositosOtraDependencia");
        }
        /// <summary>
        /// Busca los datos de un vehiculo y muestra el componente de datos
        /// </summary>
        /// <param name="vehiculosService"></param>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
         [HttpGet]
        public IActionResult MostrarDatosVehiculo(int idVehiculo)
        {
            return ViewComponent("VehiculoPropietarioDatos", new { idVehiculo });
        }
        /// <summary>
        /// Guarda un registro de un deposito asociado a otra dependencia en la bd
        /// </summary>
        /// <param name="ingresarVehiculosService"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GuardarDeposito([FromServices] IIngresarVehiculosService ingresarVehiculosService,[FromServices] IVehiculosService vehiculoService, SolicitudDepositoOtraDependenciaModel model)
        {
            int idOficina = (int)HttpContext.Session.GetInt32("IdOficina");
            int idPension = (int)HttpContext.Session.GetInt32("IdPension");

            //Se busca el vehiculo y se asigna al objeto
            model.Vehiculo = vehiculoService.GetVehiculoById(model.Vehiculo.idVehiculo);

            int idDeposito = ingresarVehiculosService.GuardarDepositoOtraDependencia(model, idOficina, idPension);

            if (idDeposito < 0)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true,redirectTo=Url.Action("Index","IngresarVehiculo") });
        }

        #region Catalogos
        public JsonResult DependenciaEnvia_Drop([FromServices] ICatDependenciaEnviaService catDependenciaEnviaService)
        {
            var result = new SelectList(catDependenciaEnviaService.ObtenerDependenciasEnviaActivas(), "id", "nombre");
            return Json(result);
        }

        public JsonResult TipoMotivoIngreso_Drop([FromServices] ICatTipoMotivoIngresoService catTipoMotivoIngresoService)
        {
            var result = new SelectList(catTipoMotivoIngresoService.ObtenerTiposMotivoIngresoActivos(), "id", "nombre");
            return Json(result);
        }

        public JsonResult Municipios_Drop2([FromServices] ICatMunicipiosService catMunicipiosService)
        {
            var result = new SelectList(catMunicipiosService.GetMunicipiosPorEntidad(CatEntidadesModel.GUANAJUATO), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Carreteras_Drop([FromServices] ICatCarreterasService catCarreterasService)
        {
            var result = new SelectList(catCarreterasService.ObtenerCarreteras(), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop([FromServices] ICatTramosService catTramosService, int carreteraDDValue)
        {
            var result = new SelectList(catTramosService.ObtenerTamosPorCarretera(carreteraDDValue), "IdTramo", "Tramo");
            return Json(result);
        }
        #endregion
    }
}