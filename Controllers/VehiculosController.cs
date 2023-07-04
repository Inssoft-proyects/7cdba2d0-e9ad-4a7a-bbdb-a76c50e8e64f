using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class VehiculosController : Controller
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;

        public VehiculosController(IVehiculosService vehiculosService, ICatDictionary catDictionary,
            IPersonasService personasService
         )
        {
            _vehiculosService = vehiculosService;
            _catDictionary = catDictionary;
            _personasService = personasService;
        }

        public IActionResult Index()
        {
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            //ViewBag.PersonasFisicas= _personasService.GetAllPersonas();
            return View(vehiculoBusquedaModel);
        }

        public IActionResult Editar()
        {
            var vehiculosModel = _vehiculosService.GetAllVehiculos();
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoBusquedaModel.ListVehiculo = vehiculosModel.ToList();
            return View(vehiculoBusquedaModel);
        }

        public ActionResult EditarVehiculo(int id)
        {
            var vehiculosModel = _vehiculosService.GetVehiculoById(id);
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = vehiculosModel;
            vehiculoBusquedaModel.Vehiculo.idSubmarcaUpdated = vehiculosModel.idSubmarca;
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoBusquedaModel.isFromUpdate = true;
            vehiculosModel.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;
            return View("Index", vehiculoBusquedaModel);
        }

        public JsonResult Entidades_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Municipios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult TipoServicios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Colores_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatColores", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Marcas_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatMarcasVehiculos", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult SubMarcas_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("SubMarcas_Read", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }
      
        public JsonResult TiposVehiculo_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        [HttpPost]
        public ActionResult ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
            var vehiculosModel = _vehiculosService.GetVehiculoToAnexo(model);
            vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
            vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            return PartialView("_Create", vehiculosModel);
        }

        [HttpPost]
        public ActionResult ajax_BuscarVehiculos(VehiculoBusquedaModel model)
        {
            var vehiculosModel = _vehiculosService.GetVehiculos(model);
            return PartialView("_ListVehiculos", vehiculosModel);
        }


        [HttpPost]
        public ActionResult ajax_BuscarPersonaMoral(PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = _personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }

        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscas()
        {
            var personasFisicas = _personasService.GetAllPersonas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            var personasMoralesModel = _personasService.GetAllPersonasMorales();
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }

        [HttpGet]
        public ActionResult ajax_GetPersonaMoral(int id)
        {
            var personaModel = _personasService.GetPersonaTypeById(id);
            return PartialView("_UpdatePersonaMoral", personaModel);
        }

        [HttpPost]
        public ActionResult ajax_UpdatePersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            var personaModel = _personasService.UpdatePersonaMoral(Persona);
            var personasMoralesModel = _personasService.GetAllPersonasMorales();
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }


        [HttpPost]
        public ActionResult ajax_CrearVehiculo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            if (model.encontradoEn == (int)EstatusBusquedaVehiculo.Sitteg)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);
            }
            else if (model.encontradoEn == (int)EstatusBusquedaVehiculo.NoEncontrado)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }

            if (IdVehiculo != 0)
            {
                return Json(new { id = IdVehiculo });
            }
            else
            {
                return null;
            }
        }

    }
}
