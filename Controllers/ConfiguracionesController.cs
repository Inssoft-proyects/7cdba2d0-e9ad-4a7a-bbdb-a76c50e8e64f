using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class ConfiguracionesController : BaseController
    {
       
        public ActionResult Index()
        {

			return View();
        }

		[HttpPost]
		public async Task<IActionResult> ActualizarContra(string NuevaContrasena)
		{
			var usuario = User.FindFirst(CustomClaims.IdUsuario).Value;
			try
			{
				var handler = new HttpClientHandler
				{
					ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
					{
						Console.WriteLine("SSL error skipped");
						return true;
					}
				};

				using (HttpClient client = new HttpClient(handler))
				{
					string url = $"https://10.16.157.142:9096/serviciosinfracciones/getActualizaPwd?userWS=1&claveWS=18&idUsuario={usuario}&contraseña={NuevaContrasena}";

					var ip = HttpContext.Connection.RemoteIpAddress.ToString();

					HttpResponseMessage response = await client.GetAsync(url);


					return Json(url);
				}
			}
			catch (Exception ex)
			{
				HttpContext.Session.Clear();
				return StatusCode(500, $"Error en el servidor: {ex.Message}");
			}
		}



	}
}
