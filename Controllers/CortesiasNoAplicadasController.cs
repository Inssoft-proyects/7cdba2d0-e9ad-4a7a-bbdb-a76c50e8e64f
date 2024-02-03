using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CortesiasNoAplicadasController : BaseController
    {



        private readonly ICortesiasNoAplicadas _CortesiasNoAplicadasService;
        private readonly IConsultarDocumentoService _consultarDocumentoService;

        private readonly AppSettings _appSettings;


        public CortesiasNoAplicadasController(ICortesiasNoAplicadas CortesiasNoAplicadasService, IConsultarDocumentoService consultarDocumentoService,
            IOptions<AppSettings> appSettings)
        {
            _CortesiasNoAplicadasService = CortesiasNoAplicadasService;
            _consultarDocumentoService = consultarDocumentoService;
            _appSettings = appSettings.Value;

        }


        public IActionResult Index()
        {
          
                return View("CortesiasNoAplicadas");
            }
       

        [HttpPost]
        public ActionResult ObtenerCortesiasNoAplicadas(string FolioInfraccion)
        {
            var ListInfraccionesModel = _CortesiasNoAplicadasService.ObtInfraccionesCortesiasNoAplicadas(FolioInfraccion);
            return Json(ListInfraccionesModel);

        }

        public ActionResult MostrarDetalle(string Id)
        {
            var ListInfraccionesModel = _CortesiasNoAplicadasService.ObtenerDetalleCortesiasNoAplicada(Id);
            return PartialView("_DetalleCortesiasNoAplicadas", ListInfraccionesModel);

        }

		[HttpPost]
		public IActionResult GuardarObservaciones(string folioInfraccion, string observaciones)
		{

			var ListInfraccionesModel = _CortesiasNoAplicadasService.GuardarObservacion(folioInfraccion, observaciones);
			return View("_DetalleCortesiasNoAplicadas");
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
