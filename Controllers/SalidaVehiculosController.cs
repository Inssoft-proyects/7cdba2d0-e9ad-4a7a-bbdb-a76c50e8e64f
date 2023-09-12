using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class SalidaVehiculosController : Controller
    {
        private readonly ISalidaVehiculosService _salidaVehiculosService;
        public SalidaVehiculosController(ISalidaVehiculosService salidaVehiculosService)
        {
            _salidaVehiculosService = salidaVehiculosService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
