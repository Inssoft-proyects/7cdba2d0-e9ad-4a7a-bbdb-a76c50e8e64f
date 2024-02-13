using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class DepositosController : BaseController
    {
        private readonly IDepositosService _catDepositosService;
        private readonly ICatTiposVehiculosService _catTiposVehiculoService;
        private readonly ICatResponsablesPensiones _catResponsablesPensiones;
        private readonly IOficiales _oficialesService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatDescripcionesEventoService _descripcionesEventoService;
        private readonly ICatTipoUsuarioService _catTipoUsuarioService;
        private readonly ICatTipoMotivoAsignacionService _catTipoMotivoAsignacionService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly IPensionesService _pensionesService;
        private readonly IConcesionariosService _concesionariosService;
        private readonly IBitacoraService _bitacoraServices;


        public DepositosController(IDepositosService catDepositosService, ICatTiposVehiculosService catTiposVehiculoService, ICatResponsablesPensiones catResponsablesPensiones, IOficiales oficialesService,ICatEntidadesService catEntidadesService, ICatMunicipiosService catMunicipiosService,
            ICatDescripcionesEventoService descripcionesEventoService, ICatTipoMotivoAsignacionService catTipoMotivoAsignacionService, ICatTipoUsuarioService catTipoUsuarioService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService, IPensionesService pensionesService,
            IConcesionariosService concesionariosService, IBitacoraService bitacoraService)
        {
            _catDepositosService = catDepositosService;
            _catTiposVehiculoService = catTiposVehiculoService;
            _catResponsablesPensiones = catResponsablesPensiones;
            _oficialesService = oficialesService;
            _catEntidadesService = catEntidadesService;
            _catMunicipiosService = catMunicipiosService;
            _descripcionesEventoService = descripcionesEventoService;
            _catTipoMotivoAsignacionService = catTipoMotivoAsignacionService;
            _catTipoUsuarioService = catTipoUsuarioService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _pensionesService = pensionesService;
            _concesionariosService = concesionariosService;
            _bitacoraServices = bitacoraService;
        }
    
        public IActionResult Depositos(int? Isol)
        {      
                if (Isol.HasValue)
            {
               
                var solicitud = _catDepositosService.ObtenerSolicitudPorID(Isol.Value);
                return View(solicitud);
            }
            else
            {
                return View("Depositos");
            }       
        }
        public IActionResult Ubicacion(int Isol)
        {
            var solicitud = _catDepositosService.ObtenerSolicitudPorID(Isol);

            return View(solicitud);

        }
        public IActionResult Editar(int Isol)
        {
            var solicitud = _catDepositosService.ObtenerSolicitudPorID(Isol);

            return View("Depositos",solicitud);

        }

        public JsonResult TiposVehiculos_Drop()
        {
            var result = new SelectList(_catTiposVehiculoService.GetTiposVehiculos(), "IdTipoVehiculo", "TipoVehiculo");
            return Json(result);
        }

        public JsonResult Concecionarios_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var result = new SelectList(_concesionariosService.GetConcesionarios(idOficina), "IdConcesionario", "Concesionario");
            return Json(result);
        }
        public JsonResult Oficiales_Drop()
        {
            var oficiales = _oficialesService.GetOficialesActivos()
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = $"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}"
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }
        public JsonResult Entidades_Drop()
        {
            var result = new SelectList(_catEntidadesService.ObtenerEntidades(), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Municipios_Drop(int entidadDDlValue)
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");
            return Json(result);
        }
    
        public JsonResult Descripcion_Drop()
        {
            var result = new SelectList(_descripcionesEventoService.ObtenerDescripciones(), "idDescripcion", "descripcionEvento");
            return Json(result);
        }
        public JsonResult TiposUsuario_Drop()
        {
            var result = new SelectList(_catTipoUsuarioService.ObtenerTiposUsuario(), "idTipoUsuario", "tipoUsuario");
            return Json(result);
        }
        public JsonResult Servicios_Drop()
        {
            var result = new SelectList(_catDepositosService.ObtenerServicios(), "idServicioRequiere", "servicioRequiere");
            return Json(result);
        }
        public JsonResult Motivos_Drop()
        {
            var result = new SelectList(_catTipoMotivoAsignacionService.ObtenerMotivos(), "idTipoAsignacion", "tipoAsignacion");
            return Json(result);
        }
        public JsonResult Carreteras_Drop()
        {
            var result = new SelectList(_catCarreterasService.ObtenerCarreteras(), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var result = new SelectList(_catTramosService.ObtenerTamosPorCarretera(carreteraDDValue), "IdTramo", "Tramo");
            return Json(result);
        }
        public JsonResult Pensiones_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_pensionesService.GetAllPensiones(idOficina), "IdPension", "Pension");
            return Json(result);
        }
        public ActionResult ajax_EnviarSolicitudDeposito(int? Isol, [FromBody] SolicitudDepositoModel model)
        {
          
                if (Isol.HasValue && Isol.Value > 0)
                {
                    // Es una actualización, así que actualiza los datos en la base de datos
                    // utilizando el ID 'Isol' para identificar la solicitud existente
                    var registroActualizado = _catDepositosService.ActualizarSolicitud((int)Isol, model);

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    _bitacoraServices.insertBitacora(registroActualizado, ip, "Depositos_EnviarSolicitudDeposito", "Actualizar", "update", user);

                    return Ok(registroActualizado);

                }
                else
            {
                var nombreOficina = User.FindFirst(CustomClaims.NombreOficina).Value;
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                    var resultadoBusqueda = _catDepositosService.GuardarSolicitud(model, idOficina,nombreOficina);

                    //BITACORA
                    //var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    //var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    //_bitacoraServices.insertBitacora(resultadoBusqueda, ip, "Depositos_EnviarSolicitudDeposito", "Insertar", "insert", user);

                    return Ok(resultadoBusqueda);
                }
            }
        



            public ActionResult ajax_EnviarComplementoSolicitud(SolicitudDepositoModel model)
        {
            var complemntarRegistro = _catDepositosService.CompletarSolicitud(model);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(complemntarRegistro, ip, "Depositos_CompletarSolicitud", "Insertar", "insert", user);
            return Ok();
        }
        public ActionResult ajax_ImportarInfoInfraccion(string folioBusquedaInfraccion)
        {
            var complemntarRegistro = _catDepositosService.ImportarInfraccion(folioBusquedaInfraccion);
            return Json(complemntarRegistro);
        }
        
    }
}
