using GuanajuatoAdminUsuarios.Framework.Catalogs;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Framework
{
	public class CatDictionary : ICatDictionary
	{
		private readonly IPadronDepositosGruasService _padronDepositosGruasService;
		private readonly IGruasService _gruasService;
		private readonly IMunicipiosService _municipiosService;
		private readonly IConcesionariosService _concesionariosService;
		private readonly ICatalogosService _catalogosService;
		private readonly IHttpContextAccessor _httpContextAccessor;


		public CatDictionary(IPadronDepositosGruasService padronDepositosGruasService,
							 IGruasService gruasService,
							 IMunicipiosService municipiosService,
							 IConcesionariosService concesionariosService,
							 ICatalogosService catalogosService,
							 IHttpContextAccessor httpContextAccessor)
		{
			_padronDepositosGruasService = padronDepositosGruasService;
			_gruasService = gruasService;
			_municipiosService = municipiosService;
			_concesionariosService = concesionariosService;
			_catalogosService = catalogosService;
			_httpContextAccessor = httpContextAccessor;
		}
		/// <summary>
		/// Obtiene la información de los catálogos internos (no se guarda en la base de datos)
		/// </summary>
		/// <typeparam name="TEnum">Nombre de catálogo</typeparam>
		/// <returns></returns>
		public string GetCatalogSystem<TEnum>(int id)
		{
			Type enumType = typeof(TEnum);
			return (from int e in Enum.GetValues(typeof(TEnum))
					where e == id
					select ((Enum)Enum.ToObject(enumType, e)).GetDescription()).FirstOrDefault();
		}

		/// <summary>
		/// Obtiene la información de los catálogos internos (no se guarda en la base de datos)
		/// </summary>
		/// <typeparam name="TEnum">Nombre de catálogo</typeparam>
		/// <returns></returns>
		public Dictionary<int, string> GetCatalogSystem<TEnum>()
		{
			Type enumType = typeof(TEnum);
			return (from int e in Enum.GetValues(typeof(TEnum))
					select new
					{
						Id = e,
						Name = ((Enum)Enum.ToObject(enumType, e)).GetDescription()
					}).ToDictionary(item => item.Id, item => item.Name);
		}
		/// <summary>
		/// Obtiene la información de los catálogos internos (no se guarda en la base de datos)
		/// </summary>
		/// <param name="value">Nombre de catálogo</param>
		/// <returns></returns>
		public SystemCatalogModel GetCatalogSystem(string name)
		{
			Type enumType = typeof(CatEnumerator).GetNestedType(name);
			if (enumType != null)
				return new SystemCatalogModel()
				{
					CatalogName = name,
					CatalogList = (from int e in Enum.GetValues(enumType)
								   select new SystemCatalogListModel
								   {
									   Id = e,
									   Text = ((Enum)Enum.ToObject(enumType, e)).GetDescription()
								   }).ToList()
				};
			else
				return null;
		}

		public SystemCatalogModel GetCatalog(string catalog, string parameter)
		{
			SystemCatalogModel catalogModel = new SystemCatalogModel();
			Guid GuidId;
			int intId;
			long longId;
			string[] campos;
			switch (catalog)
			{
				case "CatSalariosMinimos":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idSalario", "area", "salario" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catSalariosMinimos", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["salario"]),
								Text = Convert.ToString(s["area"])

							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatTipoServicio":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idCatTipoServicio", "tipoServicio" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catTipoServicio", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idCatTipoServicio"]),
								Text = Convert.ToString(s["tipoServicio"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatTiposVehiculo":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idTipoVehiculo", "tipoVehiculo" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catTiposVehiculo", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idTipoVehiculo"]),
								Text = Convert.ToString(s["tipoVehiculo"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatGeneros":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idGenero", "genero" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catGeneros", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idGenero"]),
								Text = Convert.ToString(s["genero"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatTipoLicencia":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idTipoLicencia", "tipoLicencia" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catTipoLicencia", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idTipoLicencia"]),
								Text = Convert.ToString(s["tipoLicencia"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatTipoPlaca":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idTipoPlaca", "tipoPlaca" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catTipoPlaca", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idTipoPlaca"]),
								Text = Convert.ToString(s["tipoPlaca"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatGarantias":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idGarantia", "garantia" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catGarantias", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idGarantia"]),
								Text = Convert.ToString(s["garantia"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatTramosByFilter":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idTramo", "tramo" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catTramos", campos, "idCarretera", intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idTramo"]),
									Text = Convert.ToString(s["tramo"])
								}).ToList();
					}
					break;
				case "CatTramos":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idTramo", "tramo" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catTramos", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idTramo"]),
								Text = Convert.ToString(s["tramo"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatCarreterasByFilter":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idCarretera", "carretera" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catCarreteras", campos, "idDelegacion", intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idCarretera"]),
									Text = Convert.ToString(s["carretera"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
				case "CatCarreteras":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idCarretera", "carretera" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catCarreteras", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idCarretera"]),
								Text = Convert.ToString(s["carretera"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
                case "CatConcesionarios":
                    catalogModel.CatalogName = catalog;
                    campos = new string[] { "idConcesionario", "concesionario" };
                    catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("concesionarios", campos)
                            .Select(s => new SystemCatalogListModel()
                            {
                                Id = Convert.ToInt32(s["idConcesionario"]),
                                Text = Convert.ToString(s["concesionario"])
                            })
                            .OrderBy(s => s.Text)
                            .ToList();
                    break;
                case "CatOficiales":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idOficial", "nombre", "apellidoPaterno", "apellidoMaterno" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catOficiales", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idOficial"]),
								Text = string.Concat(Convert.ToString(s["nombre"]), " ", Convert.ToString(s["apellidoPaterno"]), " ", Convert.ToString(s["apellidoMaterno"]))
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatOficialesTodos":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idOficial", "nombre", "apellidoPaterno", "apellidoMaterno" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("CatOficialesTodos", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idOficial"]),
								Text = string.Concat(Convert.ToString(s["nombre"]), " ", Convert.ToString(s["apellidoPaterno"]), " ", Convert.ToString(s["apellidoMaterno"]))
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatAllMotivosInfraccion":
					//int? idDependencia = _httpContextAccessor.HttpContext.Session.GetInt32("IdDependencia");
					//parameter += "," + idDependencia;
					//if (parameter.Contains(","))

					//{
					//	int[] valores = parameter.Split(',').Where(w => !string.IsNullOrEmpty(w)).Select(s => Convert.ToInt32(s)).ToArray();
					//	catalogModel.CatalogName = catalog;
					//	campos = new string[] { "idCatMotivoInfraccion", "nombre" };
					//	string[] parametros = new string[] { "transito" };
					//	catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catMotivosInfraccion", campos, parametros, valores)
					//			.Select(s => new SystemCatalogListModel()
					//			{
					//				Id = Convert.ToInt32(s["idCatMotivoInfraccion"]),
					//				Text = Convert.ToString(s["nombre"])
					//			})
					//			.OrderBy(s => s.Text)
					//			.ToList();
					//}

					//else
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idCatMotivoInfraccion", "nombre" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catMotivosInfraccion", campos)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idCatMotivoInfraccion"]),
									Text = Convert.ToString(s["nombre"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
				case "CatMotivosInfraccion":

					int? idDependencia1 = _httpContextAccessor.HttpContext.Session.GetInt32("IdDependencia");
					parameter += "," + idDependencia1;
					if (parameter.Contains(","))
					{
						int[] valores = parameter.Split(',').Where(w => !string.IsNullOrEmpty(w)).Select(s => Convert.ToInt32(s)).ToArray();
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idCatMotivoInfraccion", "nombre" };
						string[] parametros = new string[] { "IdSubConcepto", "transito" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catMotivosInfraccion", campos, parametros, valores)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idCatMotivoInfraccion"]),
									Text = Convert.ToString(s["nombre"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					//else if (int.TryParse(parameter, out intId))
					//{
					//	catalogModel.CatalogName = catalog;
					//	campos = new string[] { "idCatMotivoInfraccion", "nombre" };
					//	catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catMotivosInfraccion", campos, "IdSubConcepto", intId)
					//			.Select(s => new SystemCatalogListModel()
					//			{
					//				Id = Convert.ToInt32(s["idCatMotivoInfraccion"]),
					//				Text = Convert.ToString(s["nombre"])
					//			})
					//			.OrderBy(s => s.Text)
					//			.ToList();
					//}
					break;
				case "CatConceptoInfraccion":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idConcepto", "concepto" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catConceptoInfraccion", campos)
							.Select(s => new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idConcepto"]),
								Text = Convert.ToString(s["concepto"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatSubConceptoInfraccion":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idSubConcepto", "subConcepto" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catSubConceptoInfraccion", campos, "idConcepto", intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idSubConcepto"]),
									Text = Convert.ToString(s["subConcepto"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
				case "CatConcesionariosByIdDelegacion":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						catalogModel.CatalogList = _concesionariosService.GetConcesionarios2ByIdDelegacion(intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = s.idConcesionario,
									Text = s.nombre
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
				case "CatTiposGrua":
					catalogModel.CatalogName = catalog;
					catalogModel.CatalogList = _gruasService.GetTipoGruas()
							.Select(s => new SystemCatalogListModel()
							{
								Id = s.IdTipoGrua,
								Text = s.TipoGrua
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatMunicipios":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idMunicipio", "municipio" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catMunicipios", campos)
					  .Select(s =>
						  new SystemCatalogListModel()
						  {
							  Id = Convert.ToInt32(s["idMunicipio"]),
							  Text = Convert.ToString(s["municipio"])
						  })
					  .OrderBy(s => s.Text)
					  .Distinct()
					  .ToList();

					break;
				case "CatMunicipiosByEntidad":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idMunicipio", "municipio" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catMunicipios", campos, "idEntidad", intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idMunicipio"]),
									Text = Convert.ToString(s["municipio"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
				case "CatDelegaciones":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idDelegacion", "delegacion" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catDelegaciones", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idDelegacion"]),
								Text = Convert.ToString(s["delegacion"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;

				case "CatResponsablesPensiones":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idResponsable", "responsable" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catResponsablePensiones", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idResponsable"]),
								Text = Convert.ToString(s["responsable"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatClasificacionGruas":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idClasificacionGrua", "clasificacion" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catClasificacionGruas", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idClasificacionGrua"]),
								Text = Convert.ToString(s["clasificacion"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatSituacionGruas":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idSituacion", "situacion" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catSituacionGruas", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idSituacion"]),
								Text = Convert.ToString(s["situacion"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatTipoPersona":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idCatTipoPersona", "tipoPersona" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catTipoPersona", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idCatTipoPersona"]),
								Text = Convert.ToString(s["tipoPersona"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatMarcasVehiculos":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idMarcaVehiculo", "marcaVehiculo" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catMarcasVehiculos", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idMarcaVehiculo"]),
								Text = Convert.ToString(s["marcaVehiculo"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatSubmarcasByFilter":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idSubmarca", "nombreSubmarca" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catSubmarcasVehiculos", campos, "idMarcaVehiculo", intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idSubmarca"]),
									Text = Convert.ToString(s["nombreSubmarca"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
				case "CatSubmarcasVehiculos":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idSubmarca", "nombreSubmarca" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catSubmarcasVehiculos", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idSubmarca"]),
								Text = Convert.ToString(s["nombreSubmarca"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatColores":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idColor", "color" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catColores", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idColor"]),
								Text = Convert.ToString(s["color"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatEntidades":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idEntidad", "nombreEntidad" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catEntidades", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idEntidad"]),
								Text = Convert.ToString(s["nombreEntidad"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatEstatusInfraccion":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idEstatusInfraccion", "estatusInfraccion" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catEstatusInfraccion", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idEstatusInfraccion"]),
								Text = Convert.ToString(s["estatusInfraccion"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatDependencias":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idDependencia", "nombreDependencia" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catDependencias", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idDependencia"]),
								Text = Convert.ToString(s["nombreDependencia"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatClasificacionAccidentes":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idClasificacionAccidente", "nombreClasificacion" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catClasificacionAccidentes", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idClasificacionAccidente"]),
								Text = Convert.ToString(s["nombreClasificacion"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatCausasAccidentes":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idCausaAccidente", "causaAccidente" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catCausasAccidentes", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idCausaAccidente"]),
								Text = Convert.ToString(s["causaAccidente"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;
				case "CatFactoresAccidentes":
					catalogModel.CatalogName = catalog;
					campos = new string[] { "idFactorAccidente", "factorAccidente" };
					catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catFactoresAccidentes", campos)
							.Select(s =>
							new SystemCatalogListModel()
							{
								Id = Convert.ToInt32(s["idFactorAccidente"]),
								Text = Convert.ToString(s["factorAccidente"])
							})
							.OrderBy(s => s.Text)
							.ToList();
					break;

				case "CatFactoresOpcionesAccidentesByFilter":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idFactorOpcionAccidente", "factorOpcionAccidente" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catFactoresOpcionesAccidentes", campos, "idFactorAccidente", intId)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idFactorOpcionAccidente"]),
									Text = Convert.ToString(s["factorOpcionAccidente"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
                case "CatSubtipoServicioFilter":
                    if (int.TryParse(parameter, out intId))
                    {
                        catalogModel.CatalogName = catalog;
                        campos = new string[] { "idSubtipoServicio", "servicio" };
                        catalogModel.CatalogList = _catalogosService.GetGenericCatalogosByFilter("catSubtipoServicio", campos, "idTipoServicio", intId)
                                .Select(s => new SystemCatalogListModel()
                                {
                                    Id = Convert.ToInt32(s["idSubtipoServicio"]),
                                    Text = Convert.ToString(s["servicio"])
                                })
                                .OrderBy(s => s.Text)
                                .ToList();
                    }
                    break;

                case "CatAplicadoA":
					if (int.TryParse(parameter, out intId))
					{
						catalogModel.CatalogName = catalog;
						campos = new string[] { "idAplicacion", "aplicacion" };
						catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catAplicacionInfraccion", campos)
								.Select(s => new SystemCatalogListModel()
								{
									Id = Convert.ToInt32(s["idAplicacion"]),
									Text = Convert.ToString(s["aplicacion"])
								})
								.OrderBy(s => s.Text)
								.ToList();
					}
					break;
                case "CatSubtipoServicio":
                    if (int.TryParse(parameter, out intId))
                    {
                        catalogModel.CatalogName = catalog;
                        campos = new string[] { "idSubtipoServicio", "servicio" };
                        catalogModel.CatalogList = _catalogosService.GetGenericCatalogos("catSubtipoServicio", campos)
                                .Select(s => new SystemCatalogListModel()
                                {
                                    Id = Convert.ToInt32(s["idSubtipoServicio"]),
                                    Text = Convert.ToString(s["servicio"])
                                })
                                .OrderBy(s => s.Text)
                                .ToList();
                    }
                    break;

                default:
					return catalogModel;
			}
			return catalogModel;
		}
	}
}
