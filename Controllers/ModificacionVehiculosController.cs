using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class ModificacionVehiculosController : BaseController
    {
        public IActionResult ModificacionVehiculos()
        {
            int IdModulo = 200;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);
            if (listaIdsPermitidos != null && listaIdsPermitidos.Contains(IdModulo))
            {

                return View();
            }
            else
            {
                TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta secci�n.";
                return RedirectToAction("Principal", "Inicio", new { area = "" });
            }

        }
    }
}
