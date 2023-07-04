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

namespace GuanajuatoAdminUsuarios.Controllers
{

    public class CancelarInfraccionController : Controller
    {

        private readonly ICancelarInfraccionService _cancelarInfraccionService;

        public CancelarInfraccionController(ICancelarInfraccionService cancelarInfraccionService)
        {
            _cancelarInfraccionService = cancelarInfraccionService;
        }

        public IActionResult Index(CancelarInfraccionModel cancelarInfraccionService)
        {
            return View("CancelarInfraccion");
        }


        [HttpPost]
        public ActionResult ObtenerInfracciones(CancelarInfraccionModel model, string FolioInfraccion)
        {
            var ListInfraccionesModel = _cancelarInfraccionService.ObtenerInfraccionPorFolio(FolioInfraccion);

            if (ListInfraccionesModel == null || ListInfraccionesModel.Count == 0)
            {
                TempData["ErrorNoCoinciencia"] = "No se encontraron infracciones con el folio especificado.";
                return PartialView("_ListadoCancelarInfraccion"); 
            }

            return PartialView("_ListadoCancelarInfraccion", ListInfraccionesModel);
        }


        public ActionResult MostrarDetalle(CancelarInfraccionModel model, int Id)
        {
            var ListInfraccionesModel = _cancelarInfraccionService.ObtenerDetalleInfraccion(Id);
            return PartialView("_DetalleInfraccion",ListInfraccionesModel);

        }

        [HttpPost]
        public IActionResult IniciarCancelacion(CancelarInfraccionModel model, int IdInfraccion, string OficioRevocacion)
        {
           
                var ListInfraccionesModel = _cancelarInfraccionService.CancelarInfraccionBD(IdInfraccion, OficioRevocacion);
                return View("CancelarInfraccion");
        }
    }

}
