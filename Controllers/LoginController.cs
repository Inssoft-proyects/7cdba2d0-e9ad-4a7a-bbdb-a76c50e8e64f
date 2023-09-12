using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
}

        [HttpPost]
        public async Task<IActionResult> ConsumirServicio(string usuario, string contrasena)
        {
            try
            {
                var usuariosPermitidos = new Dictionary<(string, string), (int, List<int>, string, string)>
        {
            { ("usuarioPec", "Pec123"), (1, new List<int> {     100, 101, 102, 103, 104, 105, 106, 200, 300, 301,
                                                                        302, 303, 304, 400, 401, 402, 500, 501, 502, 600,
                                                                        601, 602, 700, 701, 702, 703, 704, 705, 706, 707,
                                                                        708, 709, 800, 801, 802, 900, 901, 902, 903, 904,
                                                                        905, 906, 907, 908, 909, 910, 911, 912, 913, 914,
                                                                        915, 916, 917, 918, 919, 920, 921, 922, 923, 924,
                                                                        925, 926, 927, 928, 929, 930, 931, 932, 933, 934,
                                                                        935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
                                                                        945, 946, 947, 948, 949, 950, 951, 952, 953, 954,
                                                                        955, 956, 957, 958, 959, 960, 961, 962, 963, 964,
                                                                        965, 966, 967, 968, 969, 970, 971, 972, 973, 974,
                                                                        975, 976, 977, 978, 979, 980, 981, 982, 983, 984,
                                                                        985, 986, 987, 988, 989, 990, 991, 992, 993, 994,
                                                                        995, 996, 997, 998, 999 }, "UsuarioPEC", "PEC") },

            { ("usuarioMovilidad", "Mov123"), (2, new List<int> {    100, 101, 102, 103, 104, 105, 106, 200, 300, 301,
                                                                        302, 303, 304, 400, 401, 402, 500, 501, 502, 600,
                                                                        601, 602, 700, 701, 702, 703, 704, 705, 706, 707,
                                                                        708, 709, 800, 801, 802, 900, 901, 902, 903, 904,
                                                                        905, 906, 907, 908, 909, 910, 911, 912, 913, 914,
                                                                        915, 916, 917, 918, 919, 920, 921, 922, 923, 924,
                                                                        925, 926, 927, 928, 929, 930, 931, 932, 933, 934,
                                                                        935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
                                                                        945, 946, 947, 948, 949, 950, 951, 952, 953, 954,
                                                                        955, 956, 957, 958, 959, 960, 961, 962, 963, 964,
                                                                        965, 966, 967, 968, 969, 970, 971, 972, 973, 974,
                                                                        975, 976, 977, 978, 979, 980, 981, 982, 983, 984,
                                                                        985, 986, 987, 988, 989, 990, 991, 992, 993, 994,
                                                                        995, 996, 997, 998, 999 }, "UsuarioMovilidad", "Movilidad") },
 { ("usuarioPrueba", "Prueba123"), (3, new List<int> {    100, 101, 102, 103, 104, 105, 106, 200, 300, 301,
                                                                        302, 303, 304, 400, 401, 402, 500, 501, 502, 600,
                                                                        601, 601, 700, 701, 702, 703, 704, 705, 706, 707,
                                                                        905, 906, 907, 908, 909, 910, 911, 912, 913, 914,
                                                                        915, 916, 917, 918, 919, 920, 921, 922, 923, 924,
                                                                        925, 926, 927, 928, 929, 930, 931, 932, 933, 934,
                                                                        935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
                                                                        945, 946, 947, 948, 949, 950, 951, 952, 953, 954,
                                                                        955, 956, 957, }, "PruebaPermisos", "Prueba") },       
                };

                if (usuariosPermitidos.TryGetValue((usuario, contrasena), out var valoresSesion))
                {
                    var (idOficina, listaIdsPermitidos, nombre, oficina) = valoresSesion;

                    // Asignar valores a las variables de sesión
                    HttpContext.Session.SetInt32("IdOficina", idOficina);
                    HttpContext.Session.SetString("IdsPermitidos", JsonConvert.SerializeObject(listaIdsPermitidos));
                    HttpContext.Session.SetString("Nombre", nombre);
                    HttpContext.Session.SetString("Oficina", oficina);

                    return Json(JsonConvert.SerializeObject(listaIdsPermitidos));
                }
                var url = $"https://virtual.zeitek.net:9094/serviciosinfracciones/getlogin?userWS=1&claveWS=1&usuario={usuario}&contraseña={contrasena}";

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject(content);

                    if (json != null && json.Count > 0)
                    {
                        string nombre = json[0].nombre;
                        string oficina = json[0].oficina;
                        string idOficinaStr = Regex.Match(oficina, @"\d+").Value;
                        string entidad = Regex.Match(oficina, @"\d+(.+?)\|").Groups[1].Value.Trim();
                        if (int.TryParse(idOficinaStr, out int idOficina))
                        {
                            HttpContext.Session.SetInt32("IdOficina", idOficina);

                        }
                        else
                        {
                 
                        }
                        string delegacion = Regex.Match(oficina, @"\|(.+)").Groups[1].Value.Trim();

                        List<RespuestaServicio> listaRespuestas = JsonConvert.DeserializeObject<List<RespuestaServicio>>(content);
                        string vectorString = listaRespuestas.FirstOrDefault()?.Vector;
                    if (!string.IsNullOrEmpty(vectorString))
                    {
                            List<int> listaIdsPermitidos = vectorString.Split(',').Select(int.Parse).ToList();
                            string listaIdsPermitidosJson = JsonConvert.SerializeObject(listaIdsPermitidos);

                            // Guardar la lista en la variable de sesión
                            HttpContext.Session.SetString("IdsPermitidos", listaIdsPermitidosJson);
                            HttpContext.Session.SetString("Nombre", nombre);
                            HttpContext.Session.SetString("Oficina", oficina);

                            return Json(listaIdsPermitidosJson);
                        }
                    }
                }

                // En caso de respuestas inválidas o vacías, limpiar la variable de sesión
                HttpContext.Session.Remove("IdsPermitidos");
                HttpContext.Session.Remove("Nombre");
                HttpContext.Session.Remove("Oficina");

                return BadRequest("Error en la respuesta del servicio");
            }
            catch (Exception ex)
            {
                // En caso de errores, limpiar la variable de sesión y manejar el error
                HttpContext.Session.Clear();
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetIdsPermitidos()
        {
            var idsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");
            
            
            var idsPermitidos = JsonConvert.DeserializeObject<List<int>>(idsPermitidosJson) ?? new List<int>();
            return Json(idsPermitidos);
        }


    }
}
