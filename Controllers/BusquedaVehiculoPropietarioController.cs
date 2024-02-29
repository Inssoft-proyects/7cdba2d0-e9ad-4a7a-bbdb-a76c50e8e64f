/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 20th 2024 5:06:14 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Thu Feb 29 2024
 * Última modificación: Thu Feb 29 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */


using System.Collections.Generic;
using System.Linq;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.IdentityModel.Tokens;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class BusquedaVehiculoPropietarioController : BaseController
    {
        #region Variables
        private readonly ICatDictionary _catDictionary;

        #endregion
        #region Constructor
        public BusquedaVehiculoPropietarioController(ICatDictionary catDictionary)
        {
            _catDictionary = catDictionary;
        }
        #endregion

        #region MostrarBusqueda
        public ActionResult MostrarBusquedaVehiculo()
        {
            VehiculoPropietarioBusquedaModel model = new()
            {
                Vehiculo = new VehiculoModel
                {
                    Persona = new PersonaModel()
                },
                IdEntidadBusqueda = CatEntidadesModel.GUANAJUATO
            };
            return PartialView("_BusquedaVehiculoPropietario", model);
        }
        #endregion

        #region Vehiculo
        [HttpPost]
        public IActionResult BuscarVehiculoEnPlataformas([FromServices] IOptions<AppSettings> appSettings, [FromServices] IRepuveService repuveService,
        [FromServices] IVehiculoPlataformaService vehiculoPlataformaService, [FromServices] IVehiculosService vehiculoService, [FromServices] ICotejarDocumentosClientService cotejarDocumentosService, VehiculoPropietarioBusquedaModel model)
        {

            RepuveConsgralRequestModel repuveGralModel = new(model.PlacaBusqueda, model.SerieBusqueda);
            VehiculoModel vehiculoModel = new();
            //Se realiza la consulta para validar si el vehiculo tiene reporte de robo
            RepuveRoboModel repuveRoboModel = new();

            try
            {
                repuveRoboModel = vehiculoPlataformaService.ValidarRoboRepuve(repuveGralModel);
            }
            catch (Exception e)
            {
                Logger.Error("Ocurrió un error al consultar robados en REPUVE:" + e);
            }


            vehiculoModel.RepuveRobo = repuveRoboModel;
            vehiculoModel.ReporteRobo = repuveRoboModel.EsRobado;



            var buscarEnServicios = appSettings.Value.AllowWebServicesRepuve;
            VehiculoBusquedaModel busquedaModel = new()
            {
                IdEntidadBusqueda = model.IdEntidadBusqueda,
                PlacasBusqueda = model.PlacaBusqueda,
                SerieBusqueda = model.SerieBusqueda
            };

            List<VehiculoModel> listaVehiculos = vehiculoService.GetVehiculoPropietario(busquedaModel);



            if (listaVehiculos.Count > 1)
            {
                var view1 = this.RenderViewAsync("_ListaVehiculos", listaVehiculos, true);
                return Json(new { listaVehiculos = true, view = view1 });

            }
            if (listaVehiculos.Count == 1)
            {

                return Json(new { listaVehiculos.FirstOrDefault().idVehiculo });
            }

            if (buscarEnServicios && !string.IsNullOrEmpty(busquedaModel.PlacasBusqueda))
            {
                CotejarDatosRequestModel cotejarDatosRequestModel = new()
                {
                    Tp_folio = "4",
                    Folio = busquedaModel.PlacasBusqueda,
                    tp_consulta = "3"
                };
                var endPointName = "CotejarDatosEndPoint";
                var result = cotejarDocumentosService.CotejarDatos(cotejarDatosRequestModel, endPointName);
                if (result.MT_CotejarDatos_res != null && result.MT_CotejarDatos_res.Es_mensaje != null && result.MT_CotejarDatos_res.Es_mensaje.TpMens.ToString().Equals("I", StringComparison.OrdinalIgnoreCase))
                {
                    vehiculoModel = vehiculoPlataformaService.GetVehiculoModelFromFinanzas(result);

                    //Se asigna el objeto de la consulta de robado a repuve
                    vehiculoModel.RepuveRobo = repuveRoboModel;
                    vehiculoModel.ReporteRobo = repuveRoboModel.EsRobado;

                    //Se establece el origen de datos
                    vehiculoModel.origenDatos = "Padrón Estatal";
                    var view2 = this.RenderViewAsync("_Vehiculo", vehiculoModel, true);
                    return Json(new { crearVehiculo = true, view = view2 });
                }
            }

            if (buscarEnServicios)
            {
                var repuveConsGralResponse = repuveService.ConsultaGeneral(repuveGralModel)?.FirstOrDefault() ?? new RepuveConsgralResponseModel();

                var vehiculoEncontrado = new VehiculoModel
                {
                    placas = repuveConsGralResponse.placa.IsNullOrEmpty() ? repuveRoboModel.Placa : repuveConsGralResponse.placa,
                    serie = repuveConsGralResponse.niv_padron.IsNullOrEmpty() ? repuveRoboModel.Niv : repuveConsGralResponse.niv_padron,
                    motor = repuveConsGralResponse.motor,
                    color = repuveConsGralResponse.color,
                    submarca = repuveConsGralResponse.submarca,
                    modelo = repuveConsGralResponse.modelo,

                    Persona = new PersonaModel(),

                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                    RepuveRobo = repuveRoboModel,
                    ReporteRobo = repuveRoboModel.EsRobado,
                    ErrorRepube = !repuveConsGralResponse.estatus.IsCaseInsensitiveEqual(RepuveConsgralResponseModel.CONSULTA_CORRECTA) ? "No" : ""
                };
                vehiculoEncontrado.ErrorRepube = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? "No" : "";

                //Se establece el origen de datos
                vehiculoEncontrado.origenDatos = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? null : "REPUVE";

                var view3 = this.RenderViewAsync("_Vehiculo", vehiculoEncontrado, true);
                return Json(new { crearVehiculo = true, view = view3 });


            }
            var view4 = this.RenderViewAsync("_Vehiculo", vehiculoModel, true);
            return Json(new { crearVehiculo = true, view = view4 });
        }
        /// <summary>
        /// Crea o actualiza un registro de un vehiculo en la bd
        /// </summary>
        /// <param name="vehiculoService"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CrearEditarVehiculo([FromServices] IVehiculosService vehiculoService, VehiculoModel model)
        {
            int IdVehiculo;
            model.propietario = model.Persona?.idPersona.ToString();
            if (model.idVehiculo > 0)
            {
                vehiculoService.UpdateVehiculo(model);
                IdVehiculo = model.idVehiculo;
            }
            else
                IdVehiculo = vehiculoService.CreateVehiculo(model);

            if (IdVehiculo <= 0)
                return Json(new { success = false });

            return Json(new { success = true, data = IdVehiculo });
        }
        #endregion

        #region Propietario, Conductor, Persona
        /// <summary>
        /// Muestra vista para crear persona fisica
        /// </summary>
        /// <returns></returns>
        public ActionResult MostrarPersonaFisica(int idPersona)
        {

            return ViewComponent("CrearPersona", new { idPersona });
        }
        /// <summary>
        /// Muestra vista para crear persona moral
        /// </summary>
        /// <returns></returns>
        public ActionResult MostrarPersonaMoral()
        {
            var model = new PersonaModel
            {
                PersonaDireccion = new PersonaDireccionModel()
            };
            return PartialView("_PersonaMoral", model);
        }

        /// <summary>
        /// Busca todas las personas morales
        /// </summary>
        /// <param name="personasService"></param>
        /// <param name="PersonaMoralBusquedaModel"></param>
        /// <returns></returns>
        public ActionResult BuscarPersonaMoral([FromServices] IPersonasService personasService, PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }

        /// <summary>
        /// Busca todas las personas fisicas
        /// </summary>
        /// <param name="personasService"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BuscarPersonasFiscas([FromServices] IPersonasService personasService)
        {
            var personasFisicas = personasService.GetAllPersonas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }

        [HttpPost]
        public IActionResult BuscarPersonaFisicaWithPaginado([FromServices] IPersonasService personasService, [DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
        {
            //Se eliminan espacios en blanco de los campos de busqueda
            model.PersonaModel ??= new();
            model.CURPBusqueda = model.CURPBusqueda?.Trim();
            model.RFCBusqueda = model.RFCBusqueda?.Trim();
            model.NombreBusqueda = model.NombreBusqueda?.Trim();
            model.ApellidoPaternoBusqueda = model.ApellidoPaternoBusqueda?.Trim();
            model.ApellidoMaternoBusqueda = model.ApellidoMaternoBusqueda?.Trim();
            model.NumeroLicenciaBusqueda = model.NumeroLicenciaBusqueda?.Trim();

            //Logger.Info("Buscar persona fisica en RIAG por :" + model);
            Pagination pagination = new()
            {
                PageIndex = request.Page - 1
            };
            if (model.PersonaModel != null)
            {
                if (model.PersonaModel.apellidoMaternoBusqueda == null &&
                    model.PersonaModel.apellidoPaternoBusqueda == null &&
                    model.PersonaModel.CURPBusqueda == null &&
                    model.PersonaModel.RFCBusqueda == null &&
                    model.PersonaModel.numeroLicenciaBusqueda == null &&
                    model.PersonaModel.nombreBusqueda == null)
                {
                    pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
                }
                else
                {
                    pagination.PageSize = 1000000;
                }
            }
            else
            {
                pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
            }

            var personasList = personasService.BuscarPersonasWithPagination(model, pagination);

            // Verificar si se encontraron resultados en la búsqueda de personas
            if (personasList.Any())
            {
                List<PersonaModel> personas = personasList;
                var total = 0;
                if (personasList.Count > 0)
                    total = personasList.ToList().FirstOrDefault().total;

                //if (findAll)
                request.PageSize = pagination.PageSize;

                /* var result = new DataSourceResult()
                 {
                     Data = personas,
                     Total = total
                 };*/
                model.ListadoPersonas = personasList;
                return Json(new { encontrada = true, result = model });
            }

            // Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia
            return Json(new { encontrada = false, tipo = "sin datos", message = "busca en licencias" });
        }

        public IActionResult MostrarListaPersonasRiagEncontradas(BusquedaPersonaModel model)
        {
            return ViewComponent("ListaPersonasEncontradas", new { listaPersonas = model.ListadoPersonas });
        }

        public IActionResult MostrarListaPersonasLicenciasEncontradas(BusquedaPersonaModel model)
        {
            return ViewComponent("ListaPersonasEncontradasLicencias", new { listaPersonas = model.ListadoPersonasLicencia });
        }

        public IActionResult GuardaPersonaLicenciasEnRiag([FromServices] IPersonasService personasService, [FromServices] IBitacoraService bitacoraServices, PersonaLicenciaModel personaLicencia)
        {

            //Se busca a la persona por licencia o curp
            int idPersona = personasService.ExistePersona(personaLicencia.NumeroLicencia, personaLicencia.Curp);

            //Si no existe la persona se inserta
            if (idPersona <= 0)
                idPersona = personasService.InsertarPersonaDeLicencias(personaLicencia);

            //Se obtienen los datos de la persona por id
            PersonaModel persona = personasService.GetPersonaById(idPersona);


            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            bitacoraServices.insertBitacora(idPersona, ip, "Personas_DesdeServicio", "Insertar", "insert", user);

            BusquedaPersonaModel modelo = new()
            {
                ListadoPersonas = new List<PersonaModel>()
            };
            modelo.ListadoPersonas.Add(persona);
            return Json(new { data = modelo });
        }
        /// <summary>
        /// Crea un nuevo registro en la bd de una persona fisica
        /// </summary>
        /// <param name="personasService"></param>
        /// <param name="Persona"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ActionResult CrearPersonaFisica([FromServices] IPersonasService personasService, PersonaModel Persona)
        {
            int IdPersonaFisica=0;
            if (Persona.idPersona > 0)
            {
                Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
                int result = personasService.UpdatePersona(Persona);
                if(result>0)
                IdPersonaFisica = (int)Persona.idPersona;
            }
            else
            {
                Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
                IdPersonaFisica = personasService.CreatePersona(Persona);
            }
            if (IdPersonaFisica == 0)
            {
                throw new Exception("Ocurrio un error al dar de alta la persona");
            }
            var modelList = personasService.ObterPersonaPorIDList(IdPersonaFisica);
            BusquedaPersonaModel model = new()
            {
                ListadoPersonas = modelList
            };
            return Json(new { success = true, data = model });
        }
        /// <summary>
        /// Crea un nuevo registro en la bd de una persona moral
        /// </summary>
        /// <param name="personasService"></param>
        /// <param name="Persona"></param>
        /// <returns></returns>
        public ActionResult CrearPersonaMoral([FromServices] IPersonasService personasService, PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            Persona.PersonaDireccion.telefono = System.String.IsNullOrEmpty(Persona.telefono) ? null : Persona.telefono;
            var IdPersonaMoral = personasService.CreatePersonaMoral(Persona);
            if (IdPersonaMoral == 0)
                return Json(new { success = false, message = "Ocurrió un error al procesar su solicitud." });
            else
            {
                var modelList = personasService.ObterPersonaPorIDList(IdPersonaMoral);
                return PartialView("_ListPersonasMorales", modelList);
            }


            //var personasMoralesModel = _personasService.GetAllPersonasMorales();

        }
        /// <summary>
        /// Busca personas en el sistema de licencias a través de un servicio web publicado
        /// </summary>
        /// <param name="_httpClientFactory"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> BuscarPersonasEnLicencias([FromServices] IHttpClientFactory _httpClientFactory, BusquedaPersonaModel model)
        {
            string parametros = "";
            parametros += string.IsNullOrEmpty(model.NumeroLicenciaBusqueda) ? "" : "licencia=" + model.NumeroLicenciaBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.CURPBusqueda) ? "" : "curp=" + model.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.RFCBusqueda) ? "" : "rfc=" + model.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.NombreBusqueda) ? "" : "nombre=" + model.NombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.ApellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.ApellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.ApellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.ApellidoMaternoBusqueda;
            string ultimo = parametros[^1..];
            if (ultimo.Equals("&"))
                parametros = parametros[..^1];

            string urlServ = Request.GetDisplayUrl();
            Uri uri = new(urlServ);
            string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

            var url = requested + $"/api/Licencias/datos_generales?" + parametros;

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();


                List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

                List<PersonaLicenciaModel> resultado = new();

                foreach (LicenciaPersonaDatos p in respuesta)
                {
                    PersonaLicenciaModel pm = new();
                    pm.ConvertirModelo(p);
                    resultado.Add(pm);
                }

                return Json(new { success = true, data = resultado });
            }

            return Json(new { success = true, message = "No se pudo conectar al servicio de licencias" });
        }


        #endregion
        #region Catalogos
        public JsonResult GetEntidades_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult SubTipoServicios_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatSubtipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult GetSubtipoPorTipo_Drop([FromServices] ICatSubtipoServicio subtipoServicio, int idTipoServicio)
        {
            var result = new SelectList(subtipoServicio.GetSubtipoPorTipo(idTipoServicio), "idSubTipoServicio", "subTipoServicio");
            return Json(result);
        }
        public JsonResult Entidades_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult TipoServicios_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult Municipios_Drop([FromServices] ICatMunicipiosService _catMunicipiosService, int entidadDDlValue)
        {
            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Colores_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatColores", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Marcas_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatMarcasVehiculos", "0");
            var orderedList = catEntidades.CatalogList.OrderBy(item => item.Text);
            var result = new SelectList(orderedList, "Id", "Text"); return Json(result);
        }

        public JsonResult SubMarcas_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatSubmarcasVehiculos", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        public JsonResult TiposVehiculo_Drop()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult TipoLicencias_Drop([FromServices] ICatTipoLicenciasService catTipoLicenciasService)
        {
            var result = new SelectList(catTipoLicenciasService.ObtenerTiposLicencia(), "idTipoLicencia", "tipoLicencia");
            return Json(result);
        }

        #endregion

    }
}
