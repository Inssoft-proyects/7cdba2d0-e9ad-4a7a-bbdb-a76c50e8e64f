using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class BusquedaDepositoController : BaseController
    {
        private readonly IBusquedaDepositoService _busquedaDepositoService;
        private readonly IGruasService _gruasService;



        public BusquedaDepositoController(IBusquedaDepositoService busquedaDepositoService, IGruasService gruasService)
        {
            _busquedaDepositoService = busquedaDepositoService;
            _gruasService = gruasService;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public JsonResult GetAllDepositos([DataSourceRequest] DataSourceRequest request)
        {
            var listaDepositos = _busquedaDepositoService.ObtenerTodosDepositos();

            return Json(listaDepositos.ToDataSourceResult(request));
        }
        public IActionResult ajax_BusquedaDepositos(BusquedaDepositoModel model)
        {
            var listaDepositos = _busquedaDepositoService.ObtenerDepositos(model);
            return Json(listaDepositos);
        }
        public ActionResult ModalDetalleGrua(int Id)
        {
            var model = _gruasService.GetGruasConcesionariosByIdCocesionario(Id);

            return PartialView("_DetallesGrua", model);
        }
    }
}
