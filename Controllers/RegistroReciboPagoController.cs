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

    public class RegistroReciboPagoController : Controller
    {

        private readonly IRegistroReciboPagoService _registroReciboPagoService;
      
    public  RegistroReciboPagoController(IRegistroReciboPagoService registroReciboPagoService)
        {
            _registroReciboPagoService= registroReciboPagoService;
        }

    public IActionResult Index()
        {
            return View("RegistroReciboDePago") ;
        }


        [HttpPost]
        public ActionResult ObtenerInfracciones(RegistroReciboPagoModel model,string FolioInfraccion)
        {
            var ListInfraccionesModel = _registroReciboPagoService.ObtInfracciones(FolioInfraccion);
            return PartialView("_ListadoBusquedaInfraccion", ListInfraccionesModel);

        }

        public ActionResult Detalleinfraccion(RegistroReciboPagoModel model, int Id)
        {
            var ListInfraccionesModel = _registroReciboPagoService.ObtenerDetallePorId(Id);
            return PartialView("_DetalleRegistroDePago", ListInfraccionesModel);

        }
    }
   
}
