/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 20th 2024 5:06:14 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sat Feb 24 2024
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

            ViewBag.ReporteRobo = vehiculoPlataformaService.ValidarRoboRepuve(repuveGralModel);


            var buscarEnServicios = appSettings.Value.AllowWebServicesRepuve;
            VehiculoBusquedaModel busquedaModel = new()
            {
                IdEntidadBusqueda = model.IdEntidadBusqueda,
                PlacasBusqueda = model.PlacaBusqueda,
                SerieBusqueda = model.SerieBusqueda
            };

            List<VehiculoModel> listaVehiculos = vehiculoService.GetVehiculoPropietario(busquedaModel);
            VehiculoModel vehiculoModel = new();


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
                    vehiculoModel.ErrorRepube = string.IsNullOrEmpty(vehiculoModel.placas) ? "No" : "";
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
                    placas = repuveConsGralResponse.placa,
                    serie = repuveConsGralResponse.niv_padron,
                    motor = repuveConsGralResponse.motor,
                    color = repuveConsGralResponse.color,
                    submarca = repuveConsGralResponse.submarca,
                    modelo = repuveConsGralResponse.modelo,

                    Persona = new PersonaModel(),

                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                };
                vehiculoEncontrado.ErrorRepube = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? "No" : "";

                //Se establece el origen de datos
                vehiculoEncontrado.origenDatos = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? null : "REPUVE";

                if (ViewBag.ReporteRobo)
                {
                    vehiculoEncontrado.placas = repuveGralModel.placa;
                    vehiculoEncontrado.serie = repuveGralModel.niv;
                }


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

        #region Propietarios
        /// <summary>
        /// Muestra vista para crear persona fisica
        /// </summary>
        /// <returns></returns>
        public ActionResult MostrarPersonaFisica()
        {
            var model = new PersonaModel
            {
                PersonaDireccion = new PersonaDireccionModel()
            };
            return PartialView("_PersonaFisica", model);
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
        /// <summary>
        /// Crea un nuevo registro en la bd de una persona fisica
        /// </summary>
        /// <param name="personasService"></param>
        /// <param name="Persona"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ActionResult CrearPersonaFisica([FromServices] IPersonasService personasService, PersonaModel Persona)
        {
            Persona.nombre = Persona.nombreFisico;
            Persona.apellidoMaterno = Persona.apellidoMaternoFisico;
            Persona.apellidoPaterno = Persona.apellidoPaternoFisico;
            Persona.CURP = Persona.CURPFisico;
            Persona.RFC = Persona.RFCFisico;
            Persona.numeroLicencia = Persona.numeroLicenciaFisico;
            Persona.idTipoLicencia = Persona.idTipoLicencia;
            Persona.vigenciaLicencia = Persona.vigenciaLicencia;
            Persona.PersonaDireccion.idEntidad = Persona.PersonaDireccion.idEntidadFisico;
            Persona.PersonaDireccion.idMunicipio = Persona.PersonaDireccion.idMunicipioFisico;
            Persona.PersonaDireccion.correo = Persona.PersonaDireccion.correoFisico;
            Persona.PersonaDireccion.telefono = Persona.PersonaDireccion.telefonoFisico;
            Persona.PersonaDireccion.colonia = Persona.PersonaDireccion.coloniaFisico;
            Persona.PersonaDireccion.calle = Persona.PersonaDireccion.calleFisico;
            Persona.PersonaDireccion.numero = Persona.PersonaDireccion.numeroFisico;
            Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            var IdPersonaFisica = personasService.CreatePersona(Persona);
            if (IdPersonaFisica == 0)
            {
                throw new Exception("Ocurrio un error al dar de alta la persona");
            }
            var modelList = personasService.ObterPersonaPorIDList(IdPersonaFisica); ;
            return PartialView("_PersonasFisicas", modelList);
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
