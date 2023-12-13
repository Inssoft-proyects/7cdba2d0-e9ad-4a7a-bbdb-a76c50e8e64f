using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{

    public class CatSubtipoServicioController : BaseController
    {
        private readonly ICatSubtipoServicio _catSubtipoServicio;
        public CatSubtipoServicioController(
           ICatSubtipoServicio catSubtipoServicio)
        {
            _catSubtipoServicio = catSubtipoServicio;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
