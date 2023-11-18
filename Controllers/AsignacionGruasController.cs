using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public AsignacionGruasController(IAsignacionGruasService asignacionGruasService, IGruasService gruasService)

        {
            _asignacionGruasService = asignacionGruasService;
            _gruasService = gruasService;
        }
        public IActionResult Index()
        {
            /* int IdModulo = 800;
             string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
             List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);*/
            //var resultadoSolicitudes = _asignacionGruasService.ObtenerTodasSolicitudes();

            return View();
        }
        public IActionResult ajax_BuscarSolicitudes(AsignacionGruaModel model)
        {
            var resultadoSolicitudes = _asignacionGruasService.BuscarSolicitudes(model);

            return Json(resultadoSolicitudes);
        }
        public IActionResult DatosGruas(int iSo, int iPg)
        {
            HttpContext.Session.SetInt32("iSo", iSo);
            HttpContext.Session.SetInt32("iPg", iPg);

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var solicitud = _asignacionGruasService.BuscarSolicitudPord(iSo, idOficina);
            HttpContext.Session.SetInt32("idDeposito", solicitud.IdDeposito);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

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

            return Json(DatosTabla);
        }
        public IActionResult AgregarObservaciones(AsignacionGruaModel formData)
        {
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            var DatosTabla = _asignacionGruasService.AgregarObs(formData,iDep);

            return Json(DatosTabla);
        }
        [HttpPost]
        public async Task<IActionResult> AgregarInventario(AsignacionGruaModel model)
        {
            try
            {
                if (model.MyFile != null && model.MyFile.Length > 0)
                {
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        model.MyFile.CopyTo(memoryStream);
                        imageData = memoryStream.ToArray();
                    }

                    int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
                    _asignacionGruasService.InsertarInventario(imageData, iDep, model.numeroInventario);

                    return Json(new { success = true, message = "Imagen e información guardadas exitosamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se seleccionó ninguna imagen" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al procesar los datos: {ex.Message}" });
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
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            return Json(DatosTabla);
        }
        
    }
}
