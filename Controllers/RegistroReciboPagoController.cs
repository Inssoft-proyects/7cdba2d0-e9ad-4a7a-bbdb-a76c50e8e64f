using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using Kendo.Mvc.Extensions;
using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using GuanajuatoAdminUsuarios.Services;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;
using Microsoft.Extensions.Options;

namespace GuanajuatoAdminUsuarios.Controllers
{

    public class RegistroReciboPagoController : Controller
    {

        private readonly IRegistroReciboPagoService _registroReciboPagoService;
        private readonly IConsultarDocumentoService _consultarDocumentoService;
        
        private readonly AppSettings _appSettings;


        public RegistroReciboPagoController(IRegistroReciboPagoService registroReciboPagoService, IConsultarDocumentoService consultarDocumentoService,
            IOptions<AppSettings> appSettings)
        {
            _registroReciboPagoService = registroReciboPagoService;
            _consultarDocumentoService = consultarDocumentoService; 
            _appSettings = appSettings.Value;

        }

        public IActionResult Index()
        {
            int IdModulo = 705;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {
                return View("RegistroReciboDePago");
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }
        }


        [HttpPost]
        public ActionResult ObtenerInfracciones(RegistroReciboPagoModel model, string FolioInfraccion)
        {
            var ListInfraccionesModel = _registroReciboPagoService.ObtInfracciones(FolioInfraccion);
            return Json(ListInfraccionesModel);

        }

        public ActionResult Detalleinfraccion(RegistroReciboPagoModel model, int Id)
        {
            var ListInfraccionesModel = _registroReciboPagoService.ObtenerDetallePorId(Id);

           

            return PartialView("_DetalleRegistroDePago", ListInfraccionesModel);

        }
        public ActionResult GuardarReciboPago(string ReciboPago, float Monto, DateTime FechaPago, string LugarPago, int IdInfraccion)
        {
            var datosGuardados = _registroReciboPagoService.GuardarRecibo(ReciboPago, Monto, FechaPago, LugarPago, IdInfraccion);
            return PartialView("RegistroReciboDePago");

        }

        public IActionResult ConsultarDocumento(string recibo)
        {
            if (_appSettings.AllowWebServices)
            {
                RootConsultarDocumentoRequest rootRequest = new RootConsultarDocumentoRequest();
                MTConsultaDocumento mTConsultaDocumento = new MTConsultaDocumento();
                mTConsultaDocumento.PROCESO = "GENERAL";
                mTConsultaDocumento.DOCUMENTO = recibo;
                mTConsultaDocumento.USUARIO = "INNSJACOB";
                mTConsultaDocumento.PASSWORD = "123456";
                rootRequest.MT_Consulta_documento = mTConsultaDocumento;

                var endPointName = "ConsultarDocumentoEndPoint";
                var result = _consultarDocumentoService.ConsultarDocumento(rootRequest, endPointName);
                ViewBag.Pension = result;
                return Json(result);
            }
            else
            {
                return Json(new { hasError = true, message = "Los servicios web no están habilitados." });
            }
        }

    }

}
