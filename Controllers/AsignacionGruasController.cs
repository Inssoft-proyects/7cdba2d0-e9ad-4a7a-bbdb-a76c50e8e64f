using GuanajuatoAdminUsuarios.Helpers;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class AsignacionGruasController : BaseController
    {
        private readonly IAsignacionGruasService _asignacionGruasService;
        private readonly IGruasService _gruasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly string _rutaArchivo;
        private bool editar = true;

        public AsignacionGruasController(IAsignacionGruasService asignacionGruasService, IGruasService gruasService, IBitacoraService bitacoraServices, IConfiguration configuration, 
            IVehiculosService vehiculosService, IVehiculoPlataformaService vehiculoPlataformaService)

        {
            _asignacionGruasService = asignacionGruasService;
            _gruasService = gruasService;
            _bitacoraServices = bitacoraServices;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivoInventarioDeposito");
            _vehiculosService = vehiculosService;
            _vehiculoPlataformaService = vehiculoPlataformaService;
        }
        public IActionResult Index(string folio)
        {

                GruasTodas_Drop();
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

        public IActionResult DatosGruas(int iSo,string folio, int iPg,int idDeposito)
        {
            string folioOId=iSo>0?iSo.ToString():folio;
            HttpContext.Session.SetString("iSo", folioOId);
            HttpContext.Session.SetInt32("iPg", iPg);

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = HttpContext.Session.GetInt32("IdDependencia") ?? 0;

            var solicitud = _asignacionGruasService.BuscarSolicitudPord(iSo,folio,idOficina, idDependencia);
            HttpContext.Session.SetInt32("idDeposito", solicitud.IdDeposito==0?idDeposito: solicitud.IdDeposito);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            var datosInfraccion = _asignacionGruasService.DatosInfraccionAsociada(solicitud.FolioSolicitud);

            datosInfraccion.inventarios = solicitud.inventarios;
            datosInfraccion.Nombreinventarios = solicitud.Nombreinventarios;



            return View("capturaGruas", datosInfraccion);
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
            this.editar = false;
            return PartialView("_ModalAgregarGrua");
        }

        public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                //var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);
                var SeleccionVehiculo = _vehiculosService.BuscarPorParametro(model.PlacasBusqueda ?? "", model.SerieBusqueda ?? "", model.FolioBusqueda);

                if (SeleccionVehiculo > 0)
                {
                    var text = "";
                    var value = "";

                    if (!string.IsNullOrEmpty(model.SerieBusqueda))
                    {
                        text = "serie";
                        value = model.SerieBusqueda;

                    }
                    else if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                    {
                        text = "placas";
                        value = model.PlacasBusqueda.ToUpper();
                    }



                    return Json(new { noResults = false, data = value, field = text });
                }
                else
                {
                    var jsonPartialVehiculosByWebServices = await ajax_BuscarVehiculo(model);

                    if (jsonPartialVehiculosByWebServices != null)
                    {
                        return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                    }
                    else
                    {
                        return Json(new { noResults = true, data = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }

        [HttpPost]
        public async Task<string> ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                {
                    model.PlacasBusqueda = model.PlacasBusqueda.ToUpper();
                }
                if (!string.IsNullOrEmpty(model.SerieBusqueda))
                {
                    model.SerieBusqueda = model.SerieBusqueda.ToUpper();
                }


                var models = _vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model);
                HttpContext.Session.SetInt32("IdMarcaVehiculo", models.idMarcaVehiculo);


                var test = await this.RenderViewAsync2("", models);


                return test;
            }
            catch (Exception ex)
            {
                Logger.Error("Infracciones - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
        }


        [HttpPost]
        public ActionResult ActualizarDatosVehiculo([FromBody] AsignacionGruaModel selectedRowData)

        {

            try
            {
                int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

                _asignacionGruasService.ActualizarDatos(selectedRowData, iDep);

                selectedRowData.folioInfraccion = string.IsNullOrEmpty(selectedRowData.folioInfraccion) ? "-" : selectedRowData.folioInfraccion; 
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

            var result = new SelectList(_gruasService.GetGruasByIdConcesionario(iPg), "idGrua", "noEconomico");
            return Json(result);
        }


        public JsonResult GruasTodas_Drop()
        {
            int iPg = HttpContext.Session.GetInt32("iPg") ?? 0;

            var result = new SelectList(_gruasService.GetGruas(), "IdGrua", "noEconomico");
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

        public IActionResult ActualizarDatos2(IFormCollection formData, int abanderamiento, int arrastre, int salvamento,string horaInicioInsertEdit, string horaArriboInsertEdit,string horaTerminoInsertEdit)
        {
            int iSo = HttpContext.Session.GetInt32("iSo") ?? 0;
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            int DatosGruas = _asignacionGruasService.UpdateDatosGrua(formData, abanderamiento, arrastre, salvamento, iDep,iSo, horaInicioInsertEdit, horaArriboInsertEdit, horaTerminoInsertEdit);
           
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosGrua", "Insertar", "insert", user);

            return Json(DatosTabla);
        }


        public IActionResult InsertarDatos(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, string horaInicioInsert, string horaArriboInsert, string horaTerminoInsert)
        {
            int iSo = HttpContext.Session.GetInt32("iSo") ?? 0;
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            int DatosGruas = 0;
            DatosGruas = _asignacionGruasService.InsertDatosGrua(formData, abanderamiento, arrastre, salvamento, iDep, iSo, horaInicioInsert, horaArriboInsert, horaTerminoInsert);

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
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;


            try
            {
                if (model.MyFile != null && model.MyFile.Length > 0)
                {

                    //Se crea el nombre del archivo del inventario
                    string nombreArchivo = _rutaArchivo + "/" + iDep + "_" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("/", "").Replace(":", "").Replace(" ", "") + System.IO.Path.GetExtension(model.MyFile.FileName);
                    //Se escribe el archivo en disco
                    using (Stream fileStream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        await model.MyFile.CopyToAsync(fileStream);
                    }
                    var nombre = model.MyFile.FileName;


                    int resultado= _asignacionGruasService.InsertarInventario(nombreArchivo, iDep, model.numeroInventario, nombre);
                    if (resultado == 0)
                        return Json(new { success = false, message = "Ocurrió un error al actualizar depósito" });

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    _bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_Inventario", "Insertar", "insert", user);
                    return Json(new { success = true, message = "Imagen e información guardadas exitosamente" });
                }
                else if (!string.IsNullOrEmpty(model.NombreArchivo))
                {
                    int resultado = _asignacionGruasService.InsertarInventario("", iDep, model.numeroInventario, model.NombreArchivo);
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
            this.editar = true;
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
