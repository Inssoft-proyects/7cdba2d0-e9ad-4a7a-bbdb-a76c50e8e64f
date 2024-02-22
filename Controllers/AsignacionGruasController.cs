using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Util;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class AsignacionGruasController : BaseController
    {
        private readonly IAsignacionGruasService _asignacionGruasService;
        private readonly IGruasService _gruasService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly string _rutaArchivo;

        public AsignacionGruasController(IAsignacionGruasService asignacionGruasService, IGruasService gruasService, IBitacoraService bitacoraServices, IConfiguration configuration)

        {
            _asignacionGruasService = asignacionGruasService;
            _gruasService = gruasService;
            _bitacoraServices = bitacoraServices;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivoInventarioDeposito");
        }
        public IActionResult Index(string folio)
        {
           
                //var resultadoSolicitudes = _asignacionGruasService.ObtenerTodasSolicitudes();
                var q = User.FindFirst(CustomClaims.Nombre).Value;
                ViewBag.FolioSolicitud= folio ?? "";
                return View();                    
        }
        public IActionResult ajax_BuscarSolicitudes(AsignacionGruaModel model)
        {
       
                var resultadoSolicitudes = _asignacionGruasService.BuscarSolicitudes(model);

                return Json(resultadoSolicitudes);
            }
            public IActionResult DatosGruas(string iSo, int iPg,int idDeposito)
        {
            HttpContext.Session.SetString("iSo", iSo);
            HttpContext.Session.SetInt32("iPg", iPg);

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = HttpContext.Session.GetInt32("IdDependencia") ?? 0;

            var solicitud = _asignacionGruasService.BuscarSolicitudPord(iSo, idOficina, idDependencia);
            HttpContext.Session.SetInt32("idDeposito", solicitud.IdDeposito==0?idDeposito: solicitud.IdDeposito);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            //var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            return View("capturaGruas", solicitud);
        }
        public IActionResult GruasAsignadasTabla([DataSourceRequest] DataSourceRequest request)
        {
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);
            return Json(DatosTabla.ToDataSourceResult(request));
        }

            public IActionResult ModalInfracciones()
        {
            return PartialView("_ModalInf");
        }

        public IActionResult ModalVehiculos()
        {
            return PartialView("_ModalVeh");
        }
        public IActionResult ModalAgregarGrua()
        {
            return PartialView("_ModalAgregarGrua");
        }

        public ActionResult BuscarVehiculo(string Placa, string Serie)
        {
            var SeleccionVehiculo = _asignacionGruasService.BuscarPorParametro(Placa, Serie);
            return Json(SeleccionVehiculo);
        }

        [HttpPost]
        public ActionResult ActualizarDatosVehiculo([FromBody] AsignacionGruaModel selectedRowData)

        {

            try
            {
                int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
                _asignacionGruasService.ActualizarDatos(selectedRowData, iDep);
                selectedRowData.Placa = string.IsNullOrEmpty(selectedRowData.Placa) ? "-" : selectedRowData.Placa;
                selectedRowData.Serie = string.IsNullOrEmpty(selectedRowData.Serie) ? "-" : selectedRowData.Serie;
                selectedRowData.Tarjeta = string.IsNullOrEmpty(selectedRowData.Tarjeta) ? "-" : selectedRowData.Tarjeta;
                selectedRowData.Marca = string.IsNullOrEmpty(selectedRowData.Marca) ? "-" : selectedRowData.Marca;
                selectedRowData.Submarca = string.IsNullOrEmpty(selectedRowData.Submarca) ? "-" : selectedRowData.Submarca;
                selectedRowData.Modelo = string.IsNullOrEmpty(selectedRowData.Modelo) ? "-" : selectedRowData.Modelo;
                selectedRowData.Propietario = string.IsNullOrEmpty(selectedRowData.Propietario) ? "-" : selectedRowData.Propietario;
                selectedRowData.CURP = string.IsNullOrEmpty(selectedRowData.CURP) ? "-" : selectedRowData.CURP;
                selectedRowData.RFC = string.IsNullOrEmpty(selectedRowData.RFC) ? "-" : selectedRowData.RFC;


                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosVehiculo", "Actualizar", "Update", user);
                return Ok(selectedRowData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar los datos");
            }
        }
        [HttpPost]

        public ActionResult BuscarInfracciones(string folioInfraccion)
        {
            var SeleccionVehiculo = _asignacionGruasService.ObtenerInfracciones(folioInfraccion);
            return Json(SeleccionVehiculo);
        }
        public JsonResult Gruas_Drop()
        {
            int iPg = HttpContext.Session.GetInt32("iPg") ?? 0;

            var result = new SelectList(_gruasService.GetGruaByPension(iPg), "idGrua", "noEconomico");
            return Json(result);
        }
        [HttpPost]

        public IActionResult ActualizarDatos(IFormCollection formData, int abanderamiento, int arrastre, int salvamento)
        {
            //int iSo = HttpContext.Session.GetInt32("iSo") ?? 0;
            var DatosGruas = _asignacionGruasService.EditarDatosGrua(formData, abanderamiento, arrastre, salvamento);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosGrua", "Actualizar", "Update", user);
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);
            return Json(DatosTabla);
        }
        
        [HttpPost]

        public IActionResult InsertarDatos(IFormCollection formData, int abanderamiento, int arrastre, int salvamento)
        {
            int iSo = HttpContext.Session.GetInt32("iSo") ?? 0;
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            var DatosGruas = _asignacionGruasService.UpdateDatosGrua(formData, abanderamiento, arrastre, salvamento, iDep,iSo);
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosGrua", "Insertar", "insert", user);

            return Json(DatosTabla);
        }
        public IActionResult AgregarObservaciones(AsignacionGruaModel formData)
        {
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            var DatosTabla = _asignacionGruasService.AgregarObs(formData,iDep);
            
            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_Observaciones", "Insertar", "insert", user);
            return Json(DatosTabla);
        }
        [HttpPost]
        public async Task<IActionResult> AgregarInventario(AsignacionGruaModel model)
        {
            try
            {
                if (model.MyFile != null && model.MyFile.Length > 0)
                {
                    int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

                    //Se crea el nombre del archivo del inventario
                    string nombreArchivo = _rutaArchivo + "/" + iDep + "_" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("/", "").Replace(":", "").Replace(" ", "") + System.IO.Path.GetExtension(model.MyFile.FileName);
                    //Se escribe el archivo en disco
                    using (Stream fileStream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        await model.MyFile.CopyToAsync(fileStream);
                    }

    
                    int resultado= _asignacionGruasService.InsertarInventario(nombreArchivo, iDep, model.numeroInventario);
                    if (resultado == 0)
                        return Json(new { success = false, message = "Ocurrió un error al actualizar depósito" });

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_Inventario", "Insertar", "insert", user);
                    return Json(new { success = true, message = "Imagen e información guardadas exitosamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se seleccionó ninguna imagen" });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ocurrió un error al cargar el archivo a depósito: "+ex);
                return Json(new { success = false, message = "Ocurrió un error al guardar el archivo" });
            }
        }

        public IActionResult ModalEditarGrua(int idAsignacion)
        {
            var eliminarGrua = _asignacionGruasService.ObtenerAsignacionPorId(idAsignacion);

            return PartialView("_ModalEditarGrua",eliminarGrua);
        }
        
        public IActionResult ModalEliminarGrua(int idAsignacion)
        {
            var eliminarGrua = _asignacionGruasService.ObtenerAsignacionPorId(idAsignacion);

            return PartialView("_ModalEliminarGrua");
        }

        public IActionResult EliminarGrua(int idAsignacion)
        {
            var eliminarGrua = _asignacionGruasService.EliminarGrua(idAsignacion);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idAsignacion, ip, "AsignacionGruas_Grua", "Eliminar", "delete", user);
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            return Json(DatosTabla);
        }
        
    }
}
