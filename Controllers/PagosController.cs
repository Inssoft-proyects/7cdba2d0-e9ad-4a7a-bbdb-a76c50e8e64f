using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        IPagosInfraccionesService _PagosInfraccionesService;
        ILogTraficoService _LogTraficoService;
        public PagosController(IPagosInfraccionesService PagosInfraccionesService, ILogTraficoService LogTraficoService)
        {
            _PagosInfraccionesService = PagosInfraccionesService;
            _LogTraficoService = LogTraficoService;
        }
        // POST api/<PagosController>
        [HttpPost]
        public IActionResult Post([FromBody] InfoPagoModel InfoPago)
        {
            try
            {
                var resp = _PagosInfraccionesService.Pagar(InfoPago);
                LogTraficoModel LogModel = new LogTraficoModel();
                LogModel.jsonRequest = JsonConvert.SerializeObject(InfoPago);
                LogModel.jsonResponse = JsonConvert.SerializeObject(resp);
                LogModel.fecha = DateTime.Now;
                LogModel.valor = InfoPago.FolioInfraccion;
                LogModel.api = nameof(PagosController) + "/Post";
                _LogTraficoService.CreateLog(LogModel);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponsePagoModel() { CodigoRespuesta = 6, HasError = true, Mensaje = "Error al pagar: " + ex.Message });
            }
        }

        // PUT api/<PagosController>/5
        [HttpDelete]
        public IActionResult Delete([FromBody] ReversaPagoModel ReversaPago)
        {
            try
            {
                var resp = _PagosInfraccionesService.ReversaDePago(ReversaPago);
                LogTraficoModel LogModel = new LogTraficoModel();
                LogModel.jsonRequest = JsonConvert.SerializeObject(ReversaPago);
                LogModel.jsonResponse = JsonConvert.SerializeObject(resp);
                LogModel.fecha = DateTime.Now;
                LogModel.api = nameof(PagosController) + "/Delete";
                _LogTraficoService.CreateLog(LogModel);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponsePagoModel() { CodigoRespuesta = 6, HasError = true, Mensaje = "Error al eliminar el pago: " + ex.Message });
            }
        }
    }
}
