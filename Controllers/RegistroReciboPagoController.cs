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
using Microsoft.AspNetCore.Authorization;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class RegistroReciboPagoController : BaseController
    {

        private readonly IRegistroReciboPagoService _registroReciboPagoService;
        private readonly IConsultarDocumentoService _consultarDocumentoService;
        private readonly IBitacoraService _bitacoraServices;

        private readonly AppSettings _appSettings;


        public RegistroReciboPagoController(IRegistroReciboPagoService registroReciboPagoService, IConsultarDocumentoService consultarDocumentoService,
            IOptions<AppSettings> appSettings,IBitacoraService bitacoraService)
        {
            _registroReciboPagoService = registroReciboPagoService;
            _consultarDocumentoService = consultarDocumentoService; 
            _appSettings = appSettings.Value;
            _bitacoraServices = bitacoraService;


        }

        public IActionResult Index()
        {
          
                return View("RegistroReciboDePago");
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

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(IdInfraccion, ip, "Infraccion", "Pagar", "WS", user);



            return PartialView("RegistroReciboDePago");

        }

        public IActionResult ConsultarDocumento(string recibo,string idInfracc)
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

                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                _bitacoraServices.insertBitacora( Convert.ToInt32(idInfracc), ip, "Infraccion", "ConsultaP", "WS", user);


                return Json(result);
            }
            else
            {
                return Json(new { hasError = true, message = "Los servicios web no están habilitados." });
            }
        }

    }

}
