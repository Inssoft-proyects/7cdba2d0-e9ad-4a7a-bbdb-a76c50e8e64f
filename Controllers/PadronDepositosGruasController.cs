using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using static GuanajuatoAdminUsuarios.Models.PadronDepositosGruasModel;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class PadronDepositosGruasController : Controller
    {


        private readonly IPadronDepositosGruasService _padronDepositosGruasService;
        private readonly IGruasService _gruasService;
        private readonly IMunicipiosService _municipiosService;
        private readonly IConcesionariosService _concesionariosService;
        private readonly ICatDictionary _catDictionary;

        public PadronDepositosGruasController(IPadronDepositosGruasService padronDepositosGruasService,
             IGruasService gruasService, IMunicipiosService municipiosService, IConcesionariosService concesionariosService
            , ICatDictionary catDictionary
            )
        {
            _padronDepositosGruasService = padronDepositosGruasService;
            _gruasService = gruasService;
            _municipiosService = municipiosService;
            _concesionariosService = concesionariosService;
            _catDictionary = catDictionary;
        }


        public IActionResult Index()
        {
            PadronDepositosGruasBusquedaModel searchModel = new PadronDepositosGruasBusquedaModel();
            List<PadronDepositosGruasModel> listPadronDepositosGruas = _padronDepositosGruasService.GetAllPadronDepositosGruas();
            searchModel.ListPadronDepositosGruas = listPadronDepositosGruas;
            return View(searchModel);
        }

        [HttpPost]
        public ActionResult ajax_BuscarPadron(PadronDepositosGruasBusquedaModel model)
        {
            var ListPadronDepositosGruas = _padronDepositosGruasService.GetPadronDepositosGruas(model);
            if (ListPadronDepositosGruas.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron grúas que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListadoPadron", ListPadronDepositosGruas);

        }

        public JsonResult Municipios_Read()
        {
            var CatMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(CatMunicipios.CatalogList, "Id", "Text");
            return Json(result);
        }
        
        public JsonResult Concesionarios_Read()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_concesionariosService.GetConcesionarios(idOficina), "IdConcesionario", "Concesionario");
            return Json(result);
        }

        public JsonResult Deposito_Read()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_padronDepositosGruasService.GetPensiones(idOficina), "IdPension", "Pension");
            return Json(result);
        }

        public JsonResult TipoGrua_Read()
        {
            var result = new SelectList(_gruasService.GetTipoGruas(), "IdTipoGrua", "TipoGrua");
            return Json(result);
        }

        public List<PadronDepositosGruasModel> GetDepositos()
        {
            var modelList = _padronDepositosGruasService.GetAllPadronDepositosGruas();

            var indices = modelList
                         .Select((s, i) => new { index = i, item = s })
                         .GroupBy(grp => grp.item.IdPension)
                         //.Where(w => !string.IsNullOrEmpty(w.))
                         .SelectMany(sm => sm.Select(s => s.index))
                         .ToList();

            var ListaAgrupada = modelList.Select((s, i) => new { index = i, items = s })
                             .GroupBy(x => indices.FirstOrDefault(r => r > x.index))
                             .Select(s => s.Select(ss => ss.items).ToList())
                             .ToList();

            List<PadronDepositosGruasModel> ListItems = new List<PadronDepositosGruasModel>();
            List<PensionPadronModel> padronPension = new List<PensionPadronModel>();
            foreach (var item in ListaAgrupada)
            {

                if (item.Count() > 1)
                {
                    foreach (var itemInside in item)
                    {
                        PensionPadronModel pension = new PensionPadronModel();

                        pension.IdPension = itemInside.IdPension;
                        pension.Pension = itemInside.Pension;
                        pension.Telefono = itemInside.Telefono;
                        pension.Direccion = itemInside.Direccion;
                        pension.IdMunicipio = itemInside.IdMunicipio;
                        padronPension.Add(pension);
                    }

                    var one = item.FirstOrDefault();
                    one.Pensiones = padronPension;
                    ListItems.Add(one);
                }
                else
                {
                    ListItems.Add(item.First());
                }
            }
            return ListItems;

        }

    }
}
