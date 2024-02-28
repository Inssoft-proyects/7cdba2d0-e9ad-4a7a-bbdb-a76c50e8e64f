﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using GuanajuatoAdminUsuarios.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.NetworkInformation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Azure;
using Org.BouncyCastle.Asn1.Cmp;
using System.ServiceModel.Channels;
using GuanajuatoAdminUsuarios.Util;

namespace GuanajuatoAdminUsuarios.Services
{
	public class InfraccionesService : IInfraccionesService
	{
		private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
		private readonly IVehiculosService _vehiculosService;
		private readonly IPersonasService _personasService;
		public InfraccionesService(ISqlClientConnectionBD sqlClientConnectionBD
								  , IVehiculosService vehiculosService
								  , IPersonasService personasService)
		{
			_sqlClientConnectionBD = sqlClientConnectionBD;
			_vehiculosService = vehiculosService;
			_personasService = personasService;
		}

		public List<InfraccionesModel> GetAllInfracciones(int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT DISTINCT inf.idInfraccion
                                    ,inf.idOficial
                                    ,inf.idDependencia
                                    ,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaInfraccion
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,MAX(catGar.idGarantia) AS idGarantia,catGar.garantia
                                    , estIn.estatusInfraccion
                                    ,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico 
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo 
                                    WHERE  inf.idDelegacion = @idOficina AND inf.transito = @idDependenciaPerfil AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
									,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaInfraccion
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
									,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.garantia
                                    ,estIn.estatusInfraccion
                                    ,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependenciaPerfil", SqlDbType.Int)).Value = (object)idDependenciaPerfil ?? DBNull.Value;

					//command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)model.idPersona);
							model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo == null ? "NO" : model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo == null ? "" : model.Vehiculo.Persona == null ? "" : model.Vehiculo.Persona.nombreCompleto;
							model.NombreGarantia = model.Garantia.garantia;
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}

			return modelList;
		}

		public List<InfraccionesModel> GetAllInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();

					string sqlCondiciones = "";
					sqlCondiciones += (object)model.IdGarantia == null ? "" : " inf.idGarantia=@IdGarantia AND \n";
					sqlCondiciones += (object)model.IdTipoCortesia == null ? "" : " inf.infraccionCortesia=@IdTipoCortesia AND \n";
					sqlCondiciones += (object)model.IdDelegacion == null ? "" : " del.idOficinaTransporte=@IdDelegacion AND \n";
					sqlCondiciones += (object)model.IdEstatus == null ? "" : " estIn.idEstatusInfraccion=@IdEstatus AND \n";
					sqlCondiciones += (object)model.IdDependencia == null ? "" : " dep.idDependencia=@IdDependencia AND \n";
					sqlCondiciones += (object)model.NumeroLicencia == null ? "" : " pInf.numeroLicencia =@numeroLicencia AND \n";
					sqlCondiciones += (object)model.NumeroEconomico == null ? "" : " veh.numeroEconomico =@numeroEconomico AND \n";
					sqlCondiciones += (object)model.folioInfraccion == null ? "" : " UPPER(inf.folioInfraccion) like  '%'+ @FolioInfraccion + '%' AND \n";
					sqlCondiciones += (object)model.placas == null ? "" : " UPPER(veh.placas)=@Placas AND \n";
					sqlCondiciones += (object)model.Propietario == null ? "" : "UPPER(per.nombre + ' ' + per.apellidoPaterno + ' ' + per.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Propietario + '%' AND \n";
					sqlCondiciones += (object)model.Conductor == null ? "" : "UPPER(pInf.nombre + ' ' + pInf.apellidoPaterno + ' ' + pInf.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Conductor + '%' AND \n";

					sqlCondiciones += (object)model.FechaInicio == null && (object)model.FechaFin == null ? "" : " inf.fechaInfraccion between @FechaInicio and  @FechaFin AND \n";



					string SqlTransact =
							string.Format(@"SELECT MAX( inf.idInfraccion)AS idInfraccion
                                    ,MAX(inf.idOficial) AS idOficial
                                    ,MAX(inf.idDependencia) AS idDependencia
                                    ,MAX(inf.idDelegacion) AS idDelegacion
                                    ,MAX(inf.idVehiculo)  AS idVehiculo
                                    ,MAX(inf.idAplicacion) AS idAplicacion
                                    ,MAX(inf.idGarantia) AS idGarantia
                                    ,MAX (inf.idEstatusInfraccion) AS idEstatusInfraccion
                                    ,MAX(inf.idMunicipio) AS idMunicipio
                                    ,MAX (inf.idTramo) AS idTramo
                                    ,MAX(inf.idCarretera) AS idCarretera
                                    ,MAX (inf.idPersona) AS idPersona
                                    ,MAX(inf.idPersonaInfraccion) AS idPersonaInfraccion
                                    ,MAX(veh.placas) AS placasVehiculo
                                    ,MAX(inf.folioInfraccion) AS folioInfraccion
                                    ,MAX(inf.fechaInfraccion) AS fechaInfraccion
                                    ,MAX(inf.kmCarretera) as kmCarretera
                                    ,MAX (inf.observaciones) AS observaciones
                                    ,MAX (inf.lugarCalle) AS lugarCalle
                                    ,MAX(inf.lugarNumero) AS lugarNumero
                                    ,MAX(inf.lugarColonia)AS lugarColonia
                                    ,MAX(inf.lugarEntreCalle) AS lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,MAX(inf.NumTarjetaCirculacion) AS NumTarjetaCirculacion
                                    ,MAX(inf.fechaActualizacion) AS fechaActualizacion
                                    ,MAX(inf.actualizadoPor) AS actualizadoPor
                                    ,MAX(inf.estatus) AS estatus
                                    ,MAX(del.idOficinaTransporte) AS idOficinaTransporte 
									,MAX(del.nombreOficina) AS nombreOficina
									,MAX(dep.idDependencia) AS idDependencia
									,MAX(dep.nombreDependencia) as max_nombreDependencia,
				                    MAX(catGar.garantia) as garantia,
				                    MAX(estIn.estatusInfraccion) as estatusInfraccion,
				                    MAX(gar.numPlaca) as numPlaca,
				                    MAX(gar.numLicencia) as numLicencia,
				                    MAX(gar.vehiculoDocumento) as vehiculoDocumento,
				                    MAX(tipoP.idTipoPlaca) as idTipoPlaca,
				                    MAX(tipoP.tipoPlaca) as tipoPlaca,
				                    MAX(tipoL.idTipoLicencia) as idTipoLicencia,
				                    MAX(tipoL.tipoLicencia) as tipoLicencia,
				                    MAX(catOfi.nombre) as nombre,
				                    MAX(catOfi.apellidoPaterno) as apellidoPaterno,
				                    MAX(catOfi.apellidoMaterno) as apellidoMaterno,
				                    MAX(catOfi.rango) as rango,
				                    MAX(catMun.municipio) as municipio,
				                    MAX(catTra.tramo) as tramo,
				                    MAX(catCarre.carretera) as carretera,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.serie) as serie,
				                    MAX(veh.tarjeta) as tarjeta,
				                    MAX(veh.vigenciaTarjeta) as vigenciaTarjeta,
				                    MAX(veh.idTipoVehiculo) as idTipoVehiculo,
				                    MAX(veh.modelo) as modelo,
				                    MAX(veh.idColor) as idColor,
				                    MAX(veh.idEntidad) as idEntidad,
				                    MAX(veh.idCatTipoServicio) as idCatTipoServicio,
				                    MAX(veh.propietario) as propietario,
				                    MAX(veh.numeroEconomico) as numeroEconomico,
				                    MAX(per.nombre) as nombre,
				                    MAX(per.apellidoPaterno) as apellidoPaterno,
				                    MAX(per.apellidoMaterno) as apellidoMaterno,
									MAX(ca.aplicacion) as aplicacion
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo 
                                    left join personas per on veh.propietario = per.idPersona 
                                    left join personasInfracciones pInf on inf.idInfraccion = pInf.idInfraccion
									left join catAplicacionInfraccion ca on ca.idAplicacion = inf.idAplicacion
                                    where {0} inf.estatus=1 and inf.transito = @idDependenciaPerfil " +
									"GROUP BY inf.idInfraccion, inf.infraccionCortesia", sqlCondiciones);

					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependenciaPerfil", SqlDbType.Int)).Value = (object)idDependenciaPerfil ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdGarantia", SqlDbType.Int)).Value = (object)model.IdGarantia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdTipoCortesia", SqlDbType.Int)).Value = (object)model.IdTipoCortesia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdEstatus", SqlDbType.Int)).Value = (object)model.IdEstatus ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdDependencia", SqlDbType.Int)).Value = (object)model.IdDependencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Placas", SqlDbType.NVarChar)).Value = (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Propietario", SqlDbType.NVarChar)).Value = (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Conductor", SqlDbType.NVarChar)).Value = (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.DateTime)).Value = model.FechaInicio == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)model.FechaInicio;
					command.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.DateTime)).Value = model.FechaFin == DateTime.MinValue ? DateTime.Now : (object)model.FechaFin;
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel infraccionModel = new InfraccionesModel();
							infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
							infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							infraccionModel.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
							infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
							infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							infraccionModel.infraccionCortesiaValue = reader["infraccionCortesia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["infraccionCortesia"].ToString());
							infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							infraccionModel.aplicacion = reader["aplicacion"].ToString();
							//infraccionModel.Persona = _personasService.GetPersonaById((int)infraccionModel.idPersona);
							infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
							infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);

							//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

							infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
							infraccionModel.Garantia = infraccionModel.Garantia ?? new GarantiaInfraccionModel();
							infraccionModel.Garantia.garantia = infraccionModel.Garantia.garantia ?? "";
							infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
							infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							if (infraccionModel.PersonaInfraccion != null)
							{
								infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
							}
							else
							{
								infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
							}
							infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
							// infraccionModel.NombreGarantia = infraccionModel.garantia;
							infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();

							InfraccionesList.Add(infraccionModel);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			return InfraccionesList;
		}




		public List<InfraccionesModel> GetAllInfraccionesBusquedaEspecial(InfraccionesBusquedaEspecialModel model, int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{

					DateTime? fechasIni = string.IsNullOrEmpty(model.FechaInicio) ? null : DateTime.ParseExact(model.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
					DateTime? fechasFin = string.IsNullOrEmpty(model.FechaFin) ? null : DateTime.ParseExact(model.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

					connection.Open();

					string sqlCondiciones = "";

					sqlCondiciones += (object)model.folio == null ? "" : " UPPER(inf.folioInfraccion)=@FolioInfraccion AND \n";
					sqlCondiciones += (object)model.placas == null ? "" : " UPPER(veh.placas)=@Placas AND \n";
					sqlCondiciones += (object)model.oficinas == null ? "" : " del.idOficinaTransporte=@IdDelegacion AND \n";

					sqlCondiciones += (object)model.propietario == null ? "" : "UPPER(per.nombre + ' ' + per.apellidoPaterno + ' ' + per.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Propietario + '%' AND \n";
					sqlCondiciones += (object)model.conductor == null ? "" : "UPPER(pInf.nombre + ' ' + pInf.apellidoPaterno + ' ' + pInf.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Conductor + '%' AND \n";
					sqlCondiciones += (object)model.estatus == null ? "" : " inf.idEstatusInfraccion=@IdEstatus AND \n";
					sqlCondiciones += string.IsNullOrEmpty(model.noLicencia) ? "" : " pInf.numeroLicencia =@numeroLicencia AND \n";

					sqlCondiciones += string.IsNullOrEmpty(model.noEconomico) ? "" : "veh.numeroEconomico = @numeroEconomico AND \n";


					sqlCondiciones += (object)fechasIni == null && (object)fechasFin == null ? "" : " inf.fechaInfraccion between @FechaInicio and  @FechaFin AND \n";



					string SqlTransact =
							string.Format(@"SELECT MAX( inf.idInfraccion)AS idInfraccion
                                    ,MAX(inf.idOficial) AS idOficial
                                    ,MAX(inf.idDependencia) AS idDependencia
                                    ,MAX(inf.idDelegacion) AS idDelegacion
                                    ,MAX(inf.idVehiculo)  AS idVehiculo
                                    ,MAX(inf.idAplicacion) AS idAplicacion
                                    ,MAX(inf.idGarantia) AS idGarantia
                                    ,MAX (inf.idEstatusInfraccion) AS idEstatusInfraccion
                                    ,MAX(inf.idMunicipio) AS idMunicipio
                                    ,MAX (inf.idTramo) AS idTramo
                                    ,MAX(inf.idCarretera) AS idCarretera
                                    ,MAX (inf.idPersona) AS idPersona
                                    ,MAX(inf.idPersonaInfraccion) AS idPersonaInfraccion
                                    ,MAX(veh.placas) AS placasVehiculo
                                    ,MAX(inf.folioInfraccion) AS folioInfraccion
                                    ,MAX(inf.fechaInfraccion) AS fechaInfraccion
                                    ,MAX(inf.kmCarretera) as kmCarretera
                                    ,MAX (inf.observaciones) AS observaciones
                                    ,MAX (inf.lugarCalle) AS lugarCalle
                                    ,MAX(inf.lugarNumero) AS lugarNumero
                                    ,MAX(inf.lugarColonia)AS lugarColonia
                                    ,MAX(inf.lugarEntreCalle) AS lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,MAX(inf.NumTarjetaCirculacion) AS NumTarjetaCirculacion
                                    ,MAX(inf.fechaActualizacion) AS fechaActualizacion
                                    ,MAX(inf.actualizadoPor) AS actualizadoPor
                                    ,MAX(inf.estatus) AS estatus
                                    ,MAX(del.idOficinaTransporte) AS idOficinaTransporte 
									,MAX(del.nombreOficina) AS nombreOficina
									,MAX(dep.idDependencia) AS idDependencia
									,MAX(dep.nombreDependencia) as max_nombreDependencia,
				                    MAX(catGar.garantia) as garantia,
				                    MAX(estIn.estatusInfraccion) as estatusInfraccion,
				                    MAX(gar.numPlaca) as numPlaca,
				                    MAX(gar.numLicencia) as numLicencia,
				                    MAX(gar.vehiculoDocumento) as vehiculoDocumento,
				                    MAX(tipoP.idTipoPlaca) as idTipoPlaca,
				                    MAX(tipoP.tipoPlaca) as tipoPlaca,
				                    MAX(tipoL.idTipoLicencia) as idTipoLicencia,
				                    MAX(tipoL.tipoLicencia) as tipoLicencia,
				                    MAX(catOfi.nombre) as nombre,
				                    MAX(catOfi.apellidoPaterno) as apellidoPaterno,
				                    MAX(catOfi.apellidoMaterno) as apellidoMaterno,
				                    MAX(catOfi.rango) as rango,
				                    MAX(catMun.municipio) as municipio,
				                    MAX(catTra.tramo) as tramo,
				                    MAX(catCarre.carretera) as carretera,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.serie) as serie,
				                    MAX(veh.tarjeta) as tarjeta,
				                    MAX(veh.vigenciaTarjeta) as vigenciaTarjeta,
				                    MAX(veh.idTipoVehiculo) as idTipoVehiculo,
				                    MAX(veh.modelo) as modelo,
				                    MAX(veh.idColor) as idColor,
				                    MAX(veh.idEntidad) as idEntidad,
				                    MAX(veh.idCatTipoServicio) as idCatTipoServicio,
				                    MAX(veh.propietario) as propietario,
				                    MAX(veh.numeroEconomico) as numeroEconomico,
				                    MAX(per.nombre) as nombre,
				                    MAX(per.apellidoPaterno) as apellidoPaterno,
				                    MAX(per.apellidoMaterno) as apellidoMaterno
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo 
                                               left join personas per on veh.propietario = per.idPersona 
                                    left join personasInfracciones pInf on inf.idInfraccion = pInf.idInfraccion
                                    where {0} inf.estatus=1 and inf.transito = @idDependenciaPerfil and inf.idPersonaInfraccion is not null
									GROUP BY inf.idInfraccion, inf.infraccionCortesia", sqlCondiciones);

					SqlCommand command = new SqlCommand(SqlTransact, connection);


					if (!string.IsNullOrEmpty(model.folio))
						command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folio != null ? model.folio.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.placas))
						command.Parameters.Add(new SqlParameter("@Placas", SqlDbType.NVarChar)).Value = (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value;
					if (!string.IsNullOrEmpty(model.oficinas))
						command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.oficinas ?? DBNull.Value;

					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

					command.Parameters.Add(new SqlParameter("@idDependenciaPerfil", SqlDbType.Int)).Value = (object)idDependenciaPerfil ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.estatus))
						command.Parameters.Add(new SqlParameter("@IdEstatus", SqlDbType.Int)).Value = (object)model.estatus ?? DBNull.Value;
					if (!string.IsNullOrEmpty(model.propietario))
						command.Parameters.Add(new SqlParameter("@Propietario", SqlDbType.NVarChar)).Value = (object)model.propietario != null ? model.propietario.ToUpper() : DBNull.Value;
					if (!string.IsNullOrEmpty(model.conductor))
						command.Parameters.Add(new SqlParameter("@Conductor", SqlDbType.NVarChar)).Value = (object)model.conductor != null ? model.conductor.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.noLicencia))
						command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.noLicencia != null ? model.noLicencia.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.noEconomico))
						command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.noEconomico != null ? model.noEconomico.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.FechaInicio))
						command.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.DateTime)).Value = fechasIni == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)fechasIni;
					if (!string.IsNullOrEmpty(model.FechaFin))
						command.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.DateTime)).Value = fechasFin == DateTime.MinValue ? DateTime.Now : (object)fechasFin;

					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel infraccionModel = new InfraccionesModel();
							infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
							infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							infraccionModel.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
							infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
							infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							infraccionModel.Persona = _personasService.GetPersonaById((int)infraccionModel.idPersona);
							infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
							infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);
							//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

							infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
							infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
							infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							if (infraccionModel.PersonaInfraccion != null)
							{
								infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
							}
							else
							{
								infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
							}
							infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
							// infraccionModel.NombreGarantia = infraccionModel.garantia;
							infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
							infraccionModel.Total = Convert.ToInt32(reader["Total"]);

							InfraccionesList.Add(infraccionModel);

						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
					throw ex;
				}
				finally
				{
					connection.Close();
				}
			return InfraccionesList;
		}


		public List<InfraccionesModel> GetAllInfraccionesByFolioInfraccion(string FolioInfraccion)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT DISTINCT inf.idInfraccion
                                    ,inf.idOficial
                                    ,inf.idDependencia
                                    ,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaInfraccion
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,MAX(catGar.idGarantia) AS idGarantia,catGar.garantia
                                    , estIn.estatusInfraccion
                                    ,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico 
                                     FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo 
                                      WHERE  inf.folioInfraccion = @FolioInfraccion AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
									,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaInfraccion
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
									,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.garantia
                                    ,estIn.estatusInfraccion
                                    ,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.VarChar)).Value = (object)FolioInfraccion ?? DBNull.Value;

					//command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)model.idPersona);
							model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo == null ? "NO" : model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							model.NombreConductor = model.PersonaInfraccion.nombreCompleto == null ? "" : model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo == null ? "" : model.Vehiculo.Persona == null ? "" : model.Vehiculo.Persona.nombreCompleto;
							if (model.Garantia != null)
								model.NombreGarantia = model.Garantia.garantia == null ? "" : model.Garantia.garantia;
							else
								model.NombreGarantia = "";

							modelList.Add(model);
						}
					}
				}
				catch (Exception ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
					throw ex;
				}
				finally
				{
					connection.Close();
				}
			}

			return modelList;
		}

		public List<InfraccionesModel> GetAllInfraccionesByReciboPago(string ReciboPago)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT DISTINCT inf.idInfraccion
                                    ,inf.idOficial
                                    ,inf.idDependencia
                                    ,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaInfraccion
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,MAX(catGar.idGarantia) AS idGarantia,catGar.garantia
                                    , estIn.estatusInfraccion
                                    ,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico 
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo 
                                    WHERE  inf.reciboPago = @ReciboPago AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
									,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaInfraccion
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
									,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.garantia
                                    ,estIn.estatusInfraccion
                                    ,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@ReciboPago", SqlDbType.VarChar)).Value = (object)ReciboPago ?? DBNull.Value;

					//command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)model.idPersona);
							model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo == null ? "NO" : model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo == null ? "" : model.Vehiculo.Persona == null ? "" : model.Vehiculo.Persona.nombreCompleto;
							if (model.Garantia != null)
								model.NombreGarantia = model.Garantia.garantia == null ? "" : model.Garantia.garantia;
							else
								model.NombreGarantia = "";

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
					throw ex;
				}
				finally
				{
					connection.Close();
				}
			}

			return modelList;
		}



		public InfraccionesModel GetInfraccionById(int IdInfraccion, int idDependencia)
		{
			InfraccionesModel model = new InfraccionesModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					const string SqlTransact =
											@"SELECT inf.idInfraccion
                                                    ,inf.idOficial
                                                    ,inf.idDependencia
                                                    ,inf.idDelegacion
                                                    ,inf.idVehiculo
                                                    ,inf.idAplicacion
                                                    ,inf.idGarantia
                                                    ,inf.idEstatusInfraccion
                                                    ,inf.idMunicipio
                                                    ,inf.idTramo
                                                    ,inf.idCarretera
                                                    ,inf.idPersona
                                                    ,inf.idPersonaInfraccion
                                                    ,inf.placasVehiculo
                                                    ,inf.folioInfraccion
                                                    ,inf.fechaInfraccion
                                                    ,inf.kmCarretera
                                                    ,inf.observaciones
                                                    ,inf.lugarCalle
                                                    ,inf.lugarNumero
                                                    ,inf.lugarColonia
                                                    ,inf.lugarEntreCalle
                                                    ,inf.infraccionCortesia
                                                    ,inf.NumTarjetaCirculacion
                                                    ,inf.fechaActualizacion
                                                    ,inf.actualizadoPor
                                                    ,inf.estatus
                                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                                    ,gar.idGarantia,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                                    ,catMun.idMunicipio,catMun.municipio
                                                    ,catTra.idTramo,catTra.tramo
                                                    ,per.RFC,per.fechaNacimiento
                                                    ,catCarre.idCarretera,catCarre.carretera
                                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                                    ,motInf.idMotivoInfraccion,motInf.idMotivoInfraccion
                                                    ,ci.nombre
                                                    ,ci.idCatMotivoInfraccion,ci.nombre
                                                    ,catSubInf.idSubConcepto,catSubInf.subConcepto
                                                    ,catConInf.idConcepto,catConInf.concepto
                                                    FROM infracciones as inf
                                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                                    left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
												   INNER JOIN catMotivosInfraccion ci on motInf.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                                    left join personas per on inf.idPersona = per.idPersona
                                                    left join catSubConceptoInfraccion catSubInf on ci.IdSubConcepto = catSubInf.idSubConcepto
                                                    left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                                                    WHERE inf.estatus = 1 and inf.idInfraccion=@IdInfraccion and inf.transito = @idDependencia";
					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)IdInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.municipio = reader["municipio"].ToString();
							//model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.infraccionCortesiaValue = reader["infraccionCortesia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)model.idPersona);
							model.PersonaInfraccion = GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();
							model.umas = GetUmas(model.fechaInfraccion);


							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}
							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo.Persona.nombreCompleto;
							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo.Persona.nombreCompleto;

							if (model.Vehiculo.Persona.fechaNacimiento.HasValue)
							{
								model.fechaNacimiento = model.Vehiculo.Persona.fechaNacimiento.Value;
							}
							else
							{
								model.fechaNacimiento = DateTime.MinValue;
							}

							if (model.Garantia != null)
								model.NombreGarantia = String.IsNullOrEmpty(model.Garantia.garantia) ? "" : model.Garantia.garantia;
							else
								model.NombreGarantia = null;

						}
					}
				}
				catch (SqlException ex)
				{

				}
				finally
				{
					connection.Close();
				}
			return model;
		}
		public decimal getUMAValue()
		{
			decimal value = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					const string SqlTransact =
											@"select format(salario,'#.##') salario from catSalariosMinimos where idSalario=1";

					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							value = Convert.ToDecimal(reader["salario"].ToString());
						}
					}

				}
				catch (Exception ex)
				{

				}
				finally
				{
					connection.Close();
				}
			return value;
		}

		public InfraccionesReportModel GetInfraccionReportById(int IdInfraccion, int idDependencia)
		{
			InfraccionesReportModel model = new InfraccionesReportModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					const string SqlTransact =
                                            @"SELECT inf.idInfraccion
                                            ,inf.folioInfraccion 
                                            ,inf.fechaInfraccion
											,ISNULL(SUBSTRING(inf.horaInfraccion, 1, 2) + ':' + SUBSTRING(inf.horaInfraccion, 3, 2),'00:00') AS horaInfraccion
                                            ,DATEADD(DAY, 10, inf.fechaInfraccion) as fechaVencimiento
                                            ,estIn.estatusInfraccion
                                            ,CONCAT(catOfi.nombre,' ',catOfi.apellidoPaterno,' ', catOfi.apellidoMaterno) nombreOficial
                                            ,catMun.municipio
                                            ,catCarre.carretera
                                            ,catTra.tramo
                                            ,inf.kmCarretera
                                            ,inf.idPersona
                                            ,inf.idPersonaInfraccion
                                            ,CONCAT(pInf.nombre, ' ', pInf.apellidoPaterno, ' ', pInf.apellidoMaterno) as nombreConductor
                                            ,UPPER(CONCAT(dirconduct.calle,' ', dirconduct.numero, ', ',dirconduct.colonia, ', ', dirconductmuni.municipio, ', ', dirconductenti.nombreEntidad)) as domicilioConductor 
                                            ,conduct.fechaNacimiento fechaNacimientoConductor
                                            ,DATEDIFF(YEAR, conduct.fechaNacimiento, GETDATE()) edadConductor
                                            ,generoconduct.genero generoConductor
                                            ,dirconduct.telefono telefonoConductor
                                            ,pInf.numeroLicencia numLicenciaConductor
                                            ,tipolicconduct.tipoLicencia tipoLicenciaConductor
                                            ,conduct.vigenciaLicencia vencimientoLicConductor
                                            ,veh.placas
											,veh.tarjeta
                                            ,tipoveh.tipoVehiculo
                                            ,marcaveh.marcaVehiculo
                                            ,submarcaveh.nombreSubmarca
                                            ,veh.modelo
                                            ,colorveh.color
                                            ,CONCAT(propietario.nombre, ' ', propietario.apellidoPaterno, ' ', propietario.apellidoMaterno) as nombrePropietario
                                            ,UPPER(CONCAT(dirprop.calle,' ', dirprop.numero, ', ',dirprop.colonia, ', ', dirpropmuni.municipio, ', ', dirpropenti.nombreEntidad)) as domicilioPropietario 
                                            ,veh.serie
                                            ,inf.NumTarjetaCirculacion
                                            ,entidadveh.nombreEntidad
                                            ,tiposerv.tipoServicio
                                            ,veh.numeroEconomico
											,catGar.garantia
                                            ,COALESCE(inf.infraccionCortesia,0) tieneCortesia
											,cTCort.nombreCortesia
                                            --SacarMotivosInfracción
                                            --SacarGarantia
                                            --DatosPago
                                            ,COALESCE(inf.monto,'0') montoCalificacion
                                            ,COALESCE(inf.montoPagado,'0') montoPagado
                                            ,COALESCE(inf.reciboPago,'') reciboPago
                                            ,inf.oficioRevocacion oficioCondonacion
                                            ,inf.fechaPago
                                            ,inf.lugarPago
                                            ,'' concepto
                                            ,inf.idGarantia
											,inf.observaciones
											,isnull(ainf.aplicacion,'') aplicadaa
                                            FROM infracciones inf 
                                            left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                            left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                            left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                            left join catTramos catTra on inf.idTramo = catTra.idTramo
											left join catAplicacionInfraccion ainf on ainf.idAplicacion = inf.idAplicacion
                                            left join catCarreteras catCarre on inf.IdCarretera = catCarre.idCarretera
                                            left join personasInfracciones pInf on pInf.idInfraccion = inf.idInfraccion
                                            left join personas conduct on conduct.idPersona = inf.idPersonaInfraccion
			                                            left join personasDirecciones dirconduct on dirconduct.idPersona = inf.idPersonaInfraccion
			                                            left join catMunicipios dirconductmuni on dirconductmuni.idMunicipio = dirconduct.idMunicipio
			                                            left join catEntidades dirconductenti on dirconductenti.idEntidad = dirconduct.idEntidad
			                                            left join catGeneros generoconduct on generoconduct.idGenero = conduct.idGenero
			                                            left join catTipoLicencia tipolicconduct on tipolicconduct.idTipoLicencia = conduct.idTipoLicencia
                                            left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
											LEFT JOIN garantiasInfraccion garInf ON garInf.idInfraccion = inf.idInfraccion
											LEFT JOIN catGarantias catGar ON catGar.idGarantia = garInf.idCatGarantia
			                                            LEFT JOIN catTiposVehiculo tipoveh on tipoveh.idTipoVehiculo = veh.idTipoVehiculo
			                                            LEFT JOIN catMarcasVehiculos marcaveh on marcaveh.idMarcaVehiculo = veh.idMarcaVehiculo
			                                            LEFT JOIN catSubmarcasVehiculos submarcaveh on submarcaveh.idSubmarca = veh.idSubmarca
			                                            LEFT JOIN catColores colorveh on colorveh.idColor = veh.idColor
			                                            LEFT JOIN catTipoServicio tiposerv on tiposerv.idCatTipoServicio = veh.idCatTipoServicio
			                                            LEFT JOIN catEntidades entidadveh on entidadveh.idEntidad = veh.idEntidad
			                                            LEFT join personas propietario on propietario.idPersona = veh.idPersona
						                                            LEFT JOIN personasDirecciones dirprop on dirprop.idPersona = propietario.idPersona
						                                            left join catMunicipios dirpropmuni on dirpropmuni.idMunicipio = dirprop.idMunicipio
						                                            left join catEntidades dirpropenti on dirpropenti.idEntidad = dirprop.idEntidad
    															    LEFT JOIN catTipoCortesia cTCort ON  cTCort.id = inf.infraccionCortesia                                        
																	WHERE inf.estatus = 1 and inf.idInfraccion=@IdInfraccion and inf.transito = @idDependencia";

					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)IdInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? default(string) : reader["observaciones"].ToString();
							model.horaInfraccion = reader["horaInfraccion"] == System.DBNull.Value ? default(string) : reader["horaInfraccion"].ToString();


							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.folioInfraccion = reader["folioInfraccion"] == System.DBNull.Value ? string.Empty : reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.fechaVencimiento = reader["fechaVencimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaVencimiento"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"] == System.DBNull.Value ? string.Empty : reader["estatusInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"] == System.DBNull.Value ? string.Empty : reader["nombreOficial"].ToString();
							model.municipio = reader["municipio"] == System.DBNull.Value ? string.Empty : reader["municipio"].ToString();
							model.carretera = reader["carretera"] == System.DBNull.Value ? string.Empty : reader["carretera"].ToString();
							model.tramo = reader["tramo"] == System.DBNull.Value ? string.Empty : reader["tramo"].ToString();
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.nombreConductor = reader["nombreConductor"] == System.DBNull.Value ? string.Empty : reader["nombreConductor"].ToString();
							model.domicilioConductor = reader["domicilioConductor"] == System.DBNull.Value ? string.Empty : reader["domicilioConductor"].ToString();
							model.fechaNacimientoConductor = reader["fechaNacimientoConductor"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimientoConductor"].ToString());
							model.edadConductor = reader["edadConductor"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["edadConductor"].ToString());
							model.generoConductor = reader["generoConductor"] == System.DBNull.Value ? string.Empty : reader["generoConductor"].ToString();
							model.telefonoConductor = reader["telefonoConductor"] == System.DBNull.Value ? string.Empty : reader["telefonoConductor"].ToString();
							model.numLicenciaConductor = reader["numLicenciaConductor"] == System.DBNull.Value ? string.Empty : reader["numLicenciaConductor"].ToString();
							model.tipoLicenciaConductor = reader["tipoLicenciaConductor"] == System.DBNull.Value ? string.Empty : reader["tipoLicenciaConductor"].ToString();
							model.vencimientoLicConductor = reader["vencimientoLicConductor"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vencimientoLicConductor"].ToString());
							model.placas = reader["placas"] == System.DBNull.Value ? string.Empty : reader["placas"].ToString();
							model.tipoVehiculo = reader["tipoVehiculo"] == System.DBNull.Value ? string.Empty : reader["tipoVehiculo"].ToString();
							model.marcaVehiculo = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : reader["marcaVehiculo"].ToString();
							model.nombreSubmarca = reader["nombreSubmarca"] == System.DBNull.Value ? string.Empty : reader["nombreSubmarca"].ToString();
							model.modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : reader["modelo"].ToString();
							model.color = reader["color"] == System.DBNull.Value ? string.Empty : reader["color"].ToString();
							model.nombrePropietario = reader["nombrePropietario"] == System.DBNull.Value ? string.Empty : reader["nombrePropietario"].ToString();
							model.domicilioPropietario = reader["domicilioPropietario"] == System.DBNull.Value ? string.Empty : reader["domicilioPropietario"].ToString();
							model.serie = reader["serie"] == System.DBNull.Value ? string.Empty : reader["serie"].ToString();
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"] == System.DBNull.Value ? string.Empty : reader["NumTarjetaCirculacion"].ToString();
							model.nombreEntidad = reader["nombreEntidad"] == System.DBNull.Value ? string.Empty : reader["nombreEntidad"].ToString();
							model.tipoServicio = reader["tipoServicio"] == System.DBNull.Value ? string.Empty : reader["tipoServicio"].ToString();
							model.numeroEconomico = reader["numeroEconomico"] == System.DBNull.Value ? string.Empty : reader["numeroEconomico"].ToString();
							model.tieneCortesia = reader["tieneCortesia"] == System.DBNull.Value ? default(bool) : Convert.ToBoolean(Convert.ToByte(reader["tieneCortesia"].ToString()));
                            model.cortesia = reader["nombreCortesia"] == System.DBNull.Value ? string.Empty : reader["nombreCortesia"].ToString();

                            model.montoCalificacion = reader["montoCalificacion"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["montoCalificacion"].ToString());
							model.montoPagado = reader["montoPagado"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["montoPagado"].ToString());
							model.reciboPago = reader["reciboPago"] == System.DBNull.Value ? string.Empty : reader["reciboPago"].ToString();
							model.oficioCondonacion = reader["oficioCondonacion"] == System.DBNull.Value ? string.Empty : reader["oficioCondonacion"].ToString();
							model.fechaPago = reader["fechaPago"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaPago"].ToString());
							model.lugarPago = reader["lugarPago"] == System.DBNull.Value ? string.Empty : reader["lugarPago"].ToString();
							model.concepto = reader["concepto"] == System.DBNull.Value ? string.Empty : reader["concepto"].ToString();
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGarantia"].ToString());
                            model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
                            model.Garantia.tipoLicencia = reader["tipoLicenciaConductor"] == System.DBNull.Value ? string.Empty : reader["tipoLicenciaConductor"].ToString();
                            model.umas = GetUmas(model.fechaInfraccion);
							model.AplicadaA = reader["aplicadaa"].GetType() == typeof(DBNull) ? "" : reader["aplicadaa"].ToString();


							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
								model.concepto = model.MotivosInfraccion.FirstOrDefault().Concepto;
							}
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			return model;
		}


		public List<SystemCatalogListModel> GetFilterCatalog(FilterCatalogTramoModel Filters)
		{

			var result = new List<SystemCatalogListModel>();

			string strQuery = @"SELECT 
                                idTramo,
								tramo
                                FROM catTramos
                                WHERE estatus = 1 and idCarretera=@carretera ";
			strQuery = string.Format(strQuery);

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.Parameters.Add(new SqlParameter("@carretera", SqlDbType.Int)).Value = (object)Filters.idCarretera ?? DBNull.Value;



					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var item = new SystemCatalogListModel();
							item.Id = (int)reader["idTramo"];
							item.Text = (string)reader["tramo"];
							result.Add(item);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}

			return result;
		}

		public DateTime GetDateInfraccion(int idInfraccion)
		{
			var time = DateTime.Now;
			var str = @"select top 1 fechainfraccion from infracciones where idinfraccion=@idInf";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(str, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInf", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read()){
							time = (DateTime)reader["fechainfraccion"];
						}
					}
				}
				catch (Exception e)
				{

				}
				finally
				{

				}
			}
			return time;

		}


		public List<MotivosInfraccionVistaModel> GetMotivosInfraccionByIdInfraccion(int idInfraccion)
		{
			int numeroContinuo = 1;

			List<MotivosInfraccionVistaModel> modelList = new List<MotivosInfraccionVistaModel>();
			string strQuery = @"SELECT
                                m.idMotivoInfraccion
                                ,ci.nombre
                                ,ci.fundamento
                                ,m.calificacionMinima
                                ,m.calificacionMaxima
                                ,m.fechaActualizacion
                                ,m.actualizadoPor
                                ,m.estatus
                                ,m.idCatMotivosInfraccion
                                ,m.idInfraccion
                                ,m.calificacion
                                ,ci.nombre motivo
                                ,ci.IdSubConcepto
                                ,csi.subConcepto
                                ,csi.idConcepto
                                ,cci.concepto
                                FROM motivosInfraccion m
                                INNER JOIN catMotivosInfraccion ci
                                on m.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                AND ci.estatus = 1
                                LEFT JOIN catSubConceptoInfraccion csi
                                on ci.IdSubConcepto = csi.idSubConcepto
                                AND csi.estatus = 1
                                LEFT JOIN catConceptoInfraccion cci
                                on csi.idConcepto = cci.idConcepto
                                AND cci.estatus = 1
                                WHERE m.estatus = 1
                                AND idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							MotivosInfraccionVistaModel model = new MotivosInfraccionVistaModel();
							model.idMotivoInfraccion = reader["idMotivoInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMotivoInfraccion"].ToString());
							model.Nombre = reader["nombre"].ToString();
							model.Fundamento = reader["fundamento"].ToString();
							model.CalificacionMinima = reader["calificacionMinima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMinima"].ToString());
							model.CalificacionMaxima = reader["calificacionMaxima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMaxima"].ToString());
							model.calificacion = reader["calificacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["calificacion"].ToString());
							model.idCatMotivoInfraccion = reader["idCatMotivosInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivosInfraccion"].ToString());
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.IdSubConcepto = reader["IdSubConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdSubConcepto"].ToString());
							model.IdConcepto = reader["idConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idConcepto"].ToString());
							model.Motivo = reader["motivo"].ToString();
							model.SubConcepto = reader["subConcepto"].ToString();
							model.Concepto = reader["concepto"].ToString();
							//model.concepto = reader["concepto"].ToString();
							model.NumeroContinuo = numeroContinuo;
							modelList.Add(model);
							numeroContinuo++;

						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}


			return modelList;

		}

		public GarantiaInfraccionModel GetGarantiaById(int idInfraccion)
		{
			List<GarantiaInfraccionModel> modelList = new List<GarantiaInfraccionModel>();
			string strQuery = @"SELECT 
                                 g.idGarantia
                                ,g.idCatGarantia
                                ,g.idTipoPlaca
                                ,g.idTipoLicencia
                                ,g.numPlaca
                                ,g.numLicencia
                                ,g.vehiculoDocumento
                                ,g.fechaActualizacion
                                ,g.actualizadoPor
                                ,g.estatus
                                ,cg.garantia
                                ,ctp.tipoPlaca
                                ,ctl.tipoLicencia
                                FROM garantiasInfraccion g
                                INNER JOIN catGarantias cg
                                on g.idCatGarantia = cg.idGarantia
                                AND cg.estatus = 1
                                LEFT JOIN catTipoPlaca ctp
                                on g.idTipoPlaca = ctp.idTipoPlaca
                                AND ctp.estatus = 1
                                LEFT JOIN catTipoLicencia ctl
                                on g.idTipoLicencia = ctl.idTipoLicencia
                                AND ctl.estatus = 1
                                WHERE g.idInfraccion = @idInfraccion";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							GarantiaInfraccionModel model = new GarantiaInfraccionModel();
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idCatGarantia = reader["idCatGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCatGarantia"].ToString());
							model.idTipoPlaca = reader["idTipoPlaca"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoPlaca"].ToString());
							model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
							model.numPlaca = reader["numPlaca"].ToString();
							model.numLicencia = reader["numLicencia"].ToString();
							model.vehiculoDocumento = reader["vehiculoDocumento"].ToString();
							model.garantia = reader["garantia"].ToString();
							model.tipoPlaca = reader["tipoPlaca"].ToString();
							model.tipoLicencia = reader["tipoLicencia"].ToString();
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return modelList.FirstOrDefault();
		}

		public PersonaInfraccionModel GetPersonaInfraccionById(int idInfraccion)
		{
			List<PersonaInfraccionModel> modelList = new List<PersonaInfraccionModel>();
			string strQuery = @"SELECT
                                 pInf.idPersonaInfraccion
                                ,pInf.numeroLicencia
                                ,pInf.CURP
                                ,pInf.RFC
                                ,pInf.nombre
                                ,pInf.apellidoPaterno
                                ,pInf.apellidoMaterno
                                ,pInf.fechaActualizacion
                                ,pInf.actualizadoPor
                                ,pInf.estatus
                                ,pv.idPersona
                                ,inf.idPersona AS idPersonaDireccion
                                ,pInf.idCatTipoPersona
                                ,pv.fechaNacimiento
								,pv.idTipoLicencia
								,pv.numeroLicencia
								,pv.vigenciaLicencia
								,ctl.tipoLicencia,ctp.tipoPersona,cg.genero
                                FROM personasInfracciones as pInf
								LEFT JOIN infracciones As inf ON pInf.idInfraccion = inf.idInfraccion
								LEFT JOIN personas As pv ON inf.idPersona = pv.idPersona
								LEFT JOIN catTipoLicencia As ctl ON ctl.idTipoLicencia = pv.idTipoLicencia
							    LEFT JOIN catTipoPersona As ctp ON ctp.idCatTipoPersona = pv.idCatTipoPersona
                                LEFT JOIN catGeneros cg on pv.idGenero = cg.idGenero
							    WHERE pInf.estatus = 1
                                AND pInf.idInfraccion = @idInfraccion";


			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							PersonaInfraccionModel model = new PersonaInfraccionModel();
							//model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.idPersona = reader["idPersonaDireccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonaDireccion"].ToString());
							model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());

                            model.numeroLicencia = reader["numeroLicencia"].ToString();
							model.CURP = reader["CURP"].ToString();
							model.RFC = reader["RFC"].ToString();
							model.nombre = reader["nombre"].ToString();
							model.apellidoPaterno = reader["apellidoPaterno"].ToString();
							model.apellidoMaterno = reader["apellidoMaterno"].ToString();
							model.tipoLicencia = reader["tipoLicencia"].ToString();
							model.tipoPersona = reader["tipoPersona"].ToString();
							model.genero = reader["genero"].ToString();
							model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
							model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
							model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
							model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
							model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
							model.PersonaDireccion = _personasService.GetPersonaDireccionByIdPersona((int)model.idPersona);

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}


			var t = modelList.FirstOrDefault();

			var test = new PersonaInfraccionModel();

			return t ?? test;
		}

		/// <summary>
		/// Captura de persona con registro especifico para infraccion, se crea al final del proceso de creacion parte 1
		/// </summary>
		/// <param name="idPersona"></param>
		/// <returns></returns>
		public int CrearPersonaInfraccion(int idInfraccion, int idPersona)
		{
			int result = 0;
			string strQuery = @"INSERT INTO personasInfracciones(idInfraccion,numeroLicencia, CURP, RFC, nombre,apellidoPaterno, apellidoMaterno, idCatTipoPersona, fechaActualizacion, actualizadoPor, estatus)
                                SELECT @idInfraccion, numeroLicencia, CURP, RFC, nombre,apellidoPaterno, apellidoMaterno, idCatTipoPersona, @fechaActualizacion, @actualizadoPor, @estatus
                                FROM personas
                                WHERE idPersona = @idPersona;";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion;
					command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)idPersona;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.NVarChar)).Value = (object)1;
					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int CrearGarantiaInfraccion(GarantiaInfraccionModel model, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"INSERT INTO garantiasInfraccion 
								(
									idInfraccion,           
									idCatGarantia,           
									idTipoPlaca,             
									idTipoLicencia,
									numPlaca,
									numLicencia,            
									vehiculoDocumento,       
									fechaActualizacion,     
									actualizadoPor,         
									estatus                 
								)
								VALUES 
								(
									@idInfraccion,
									@idCatGarantia,
									@idTipoPlaca,
									@idTipoLicencia,
									@numPlaca,
									@numLicencia,
									@vehiculoDocumento,
									@fechaActualizacion,
									@actualizadoPor,
									@estatus
									 );SELECT SCOPE_IDENTITY()";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idCatGarantia", SqlDbType.Int)).Value = (object)model.idCatGarantia ?? DBNull.Value;
					int? idTipoPlaca = (model.idTipoPlaca != null) ? model.idTipoPlaca : 0;
					command.Parameters.Add(new SqlParameter("idTipoPlaca", SqlDbType.Int)).Value = idTipoPlaca;
					int? idTipoLicencia = (model.idTipoLicencia != null) ? model.idTipoLicencia : 0;
					command.Parameters.Add(new SqlParameter("idTipoLicencia", SqlDbType.Int)).Value = idTipoLicencia;
					command.Parameters.Add(new SqlParameter("numPlaca", SqlDbType.NVarChar)).Value = (object)model.numPlaca ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("numLicencia", SqlDbType.NVarChar)).Value = (object)model.numLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("vehiculoDocumento", SqlDbType.NVarChar)).Value = (object)model.vehiculoDocumento ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;

					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int ModificarGarantiaInfraccion(GarantiaInfraccionModel model, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE garantiasInfraccion SET  idCatGarantia = @idCatGarantia
                                                               ,idTipoPlaca = @idTipoPlaca
                                                               ,idTipoLicencia = @idTipoLicencia
                                                               ,numPlaca = @numPlaca
                                                               ,numLicencia = @numLicencia
                                                               ,vehiculoDocumento = @vehiculoDocumento
                                                               ,fechaActualizacion = @fechaActualizacion
                                                               ,actualizadoPor = @actualizadoPor
                                                               WHERE idInfraccion = @idInfraccion";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idCatGarantia", SqlDbType.Int)).Value = (object)model.idCatGarantia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idTipoPlaca", SqlDbType.Int)).Value = (object)model.idTipoPlaca ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idTipoLicencia", SqlDbType.Int)).Value = (object)model.idTipoLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("numPlaca", SqlDbType.NVarChar)).Value = (object)model.numPlaca ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("numLicencia", SqlDbType.NVarChar)).Value = (object)model.numLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("vehiculoDocumento", SqlDbType.NVarChar)).Value = (object)model.vehiculoDocumento ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("idGarantia", SqlDbType.Int)).Value = model.idGarantia;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion;

					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int CrearMotivoInfraccion(MotivoInfraccionModel model)
		{
			int result = 0;
			string strQuery = @"INSERT INTO motivosInfraccion

                                      (calificacionMinima
                                      ,calificacionMaxima
                                      ,calificacion
                                      ,fechaActualizacion
                                      ,actualizadoPor
                                      ,estatus
                                      ,idCatMotivosInfraccion
                                      ,idInfraccion
                                      ,IdConcepto
                                      ,IdSubConcepto)
                               VALUES (@calificacionMinima
                                      ,@calificacionMaxima
                                      ,@calificacion
                                      ,@fechaActualizacion
                                      ,@actualizadoPor
                                      ,@estatus
                                      ,@idCatMotivosInfraccion
                                      ,@idInfraccion
                                      ,@idConcepto
                                      ,@idSubConcepto);SELECT SCOPE_IDENTITY()";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("calificacionMinima", SqlDbType.Int)).Value = (object)model.calificacionMinima ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("calificacionMaxima", SqlDbType.Int)).Value = (object)model.calificacionMaxima ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("calificacion", SqlDbType.Int)).Value = (object)model.calificacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("idCatMotivosInfraccion", SqlDbType.Int)).Value = (object)model.idCatMotivoInfraccion;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
					command.Parameters.Add(new SqlParameter("idConcepto", SqlDbType.Int)).Value = (object)model.idConcepto;
					command.Parameters.Add(new SqlParameter("idSubConcepto", SqlDbType.Int)).Value = (object)model.IdSubConcepto;
					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int EliminarMotivoInfraccion(int idMotivoInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE motivosInfraccion
                                SET fechaActualizacion = @fechaActualizacion,
                                    actualizadoPor = @actualizadoPor,
                                    estatus = @estatus
                                WHERE idMotivoInfraccion = @idMotivoInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idMotivoInfraccion", SqlDbType.Int)).Value = (object)idMotivoInfraccion;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)0;
					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}


		public bool CancelTramite(string id)
		{
			var result = false;

			string queryString = @"update infracciones set estatus=0 where idInfraccion=@id";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(queryString, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)id ?? DBNull.Value;
					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
					result = true;
				}
				catch (Exception e)
				{

				}
				finally
				{
					connection.Close();
				}
			}


			return result;
		}




		public bool UpdateFolio(string id, string folio)
		{

			var result = false;

			string queryString = @"update infracciones set folioinfraccion=@folio where idInfraccion=@id";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(queryString, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)id ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar)).Value = (object)folio ?? DBNull.Value;
					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
					result = true;
				}
				catch (Exception e)
				{

				}
				finally
				{
					connection.Close();
				}
			}


			return result;

		}


		public InfraccionesModel GetInfraccion2ById(int idInfraccion, int idDependencia)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT inf.idInfraccion
                                      ,inf.idOficial
                                      ,inf.idDependencia
                                      ,inf.idDelegacion
                                      ,inf.idVehiculo
                                      ,inf.idAplicacion
                                      ,inf.idGarantia
                                      ,inf.idEstatusInfraccion
                                      ,inf.idMunicipio
                                      ,inf.idTramo
                                      ,inf.idCarretera
                                      ,inf.idPersona
                                      ,inf.idPersonaInfraccion
                                      ,ISNULL(inf.placasVehiculo,'') placasVehiculo 
                                      ,inf.folioInfraccion
                                      ,inf.fechaInfraccion
                                      ,inf.kmCarretera
                                      ,inf.observaciones
                                      ,inf.lugarCalle
                                      ,inf.lugarNumero
                                      ,inf.lugarColonia
                                      ,inf.lugarEntreCalle
                                      ,inf.infraccionCortesia
                                      ,ISNULL(inf.NumTarjetaCirculacion,'') NumTarjetaCirculacion
                                      ,inf.fechaActualizacion
                                      ,inf.actualizadoPor
                                      ,inf.estatus
									  ,inf.horaInfraccion	
									  ,ofi.nombre AS nombreOficial
									  ,ofi.apellidoPaterno AS apellidoPaternoOficial
								      ,ofi.apellidoMaterno AS apellidoMaternoOficial
									  ,car.carretera,tra.tramo,mun.municipio,pdir.telefono
									  ,inf.fechaVencimiento
                               FROM infracciones AS inf
							   LEFT JOIN catOficiales AS ofi ON inf.idOficial = ofi.idOficial
							   LEFT JOIN catCarreteras AS car ON inf.idCarretera = car.idCarretera
							   LEFT JOIN catTramos AS tra ON inf.idTramo = tra.idTramo
					           LEFT JOIN catMunicipios AS mun ON inf.idMunicipio = mun.idMunicipio
				               LEFT JOIN personasDirecciones AS pdir ON inf.idPersona = pdir.idPersona
                               WHERE inf.estatus = 1
                               AND inf.idInfraccion = @idInfraccion";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"].ToString();
							model.apellidoPaternoOficial = reader["apellidoPaternoOficial"].ToString();
							model.apellidoMaternoOficial = reader["apellidoMaternoOficial"].ToString();
							model.carretera = reader["carretera"].ToString();
							model.tramo = reader["tramo"].ToString();
							model.municipio = reader["municipio"].ToString();
							model.telefono = reader["telefono"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							DateTime fechaInfraccion = model.fechaInfraccion;
							string horaInfraccionString = reader["horaInfraccion"] == DBNull.Value ? null : reader["horaInfraccion"].ToString();

							TimeSpan horaInfraccionTimeSpan;

							if (horaInfraccionString != null)
							{
								horaInfraccionTimeSpan = TimeSpan.ParseExact(horaInfraccionString, "hhmm", CultureInfo.InvariantCulture);
							}
							else
							{
								horaInfraccionTimeSpan = TimeSpan.Zero;
							}

							DateTime fechaHoraInfraccion = fechaInfraccion.Date + horaInfraccionTimeSpan;

							model.fechaInfraccion = fechaHoraInfraccion;
							model.horaInfraccion = fechaHoraInfraccion;

							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();

							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							if (model.Vehiculo != null)
							{
								model.idPropitario = model.Vehiculo.idPersona;
								//if (reader["NumTarjetaCirculacion"].ToString() !="")
								//  model.Vehiculo.tarjeta =  reader["NumTarjetaCirculacion"].ToString();
								//if (reader["placasVehiculo"].ToString()!="")
								//	model.Vehiculo.placas =  reader["placasVehiculo"].ToString();
								//if (reader["fechaVencimiento"] != System.DBNull.Value)
								//	model.Vehiculo.vigenciaTarjeta = Convert.ToDateTime(reader["fechaVencimiento"]);
							}
							else
							{
								throw new Exception("Vehiculo es nulo, no se puede obtener datos.");
							};
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? false : ((int)reader["infraccionCortesia"]) == 1 ? false : true;
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)(model.idPersona ?? 0));
							model.PersonaInfraccion2 = _personasService.GetPersonaById((int)(model.idPersonaInfraccion ?? 0));
							model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.strIsPropietarioConductor = model.idPersona == null ? "-" : model.idPersona == model.idPropitario ? "Propietario" : "Conductor";
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.Garantia = model.Garantia ?? new GarantiaInfraccionModel();
							model.Garantia.garantia = model.Garantia.garantia ?? "";
							model.umas = GetUmas(model.fechaInfraccion);
							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}



			return modelList.FirstOrDefault();
		}
		public int ExitDesposito(int idInfraccion)
		{
			var result = 1;

			string strQuer = @"select count(*) result from depositos where idinfraccion=@IdInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{

					connection.Open();
					SqlCommand command = new SqlCommand(strQuer, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{

						while (reader.Read())
						{
							result = (int)reader["result"];
						}

					}


				}
				catch (Exception ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}

			return result;
		}

		public NuevaInfraccionModel GetInfraccionAccidenteById(int idInfraccion, int idDependencia)
		{
			List<NuevaInfraccionModel> modelList = new List<NuevaInfraccionModel>();
			string strQuery = @"SELECT idInfraccion
                                      ,idOficial
                                      ,idDependencia
                                      ,idDelegacion
                                      ,idVehiculo
                                      ,idAplicacion
                                      ,idGarantia
                                      ,idEstatusInfraccion
                                      ,idMunicipio
                                      ,idTramo
                                      ,idCarretera
                                      ,idPersona
                                      ,idPersonaInfraccion
                                      ,placasVehiculo
                                      ,folioInfraccion
                                      ,fechaInfraccion
                                      ,kmCarretera
                                      ,observaciones
                                      ,lugarCalle
                                      ,lugarNumero
                                      ,lugarColonia
                                      ,lugarEntreCalle
                                      ,infraccionCortesia
                                      ,NumTarjetaCirculacion
                                      ,fechaActualizacion
                                      ,actualizadoPor
                                      ,estatus
                               FROM infracciones
                               WHERE estatus = 1
                               AND idInfraccion = @idInfraccion AND transito = @idDependencia"
			;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							NuevaInfraccionModel model = new NuevaInfraccionModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.IdVehiculo = (int)(reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString()));
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.IdMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.IdTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.IdCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.Placa = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.Kilometro = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.Tarjeta = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)model.IdPersona);
							model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.IdVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.umas = GetUmas(model.fechaInfraccion);
							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}



			return modelList.FirstOrDefault();
		}


		public decimal GetUmas(DateTime? fecha=null)
		{

			fecha = fecha ?? DateTime.Now;

			decimal umas = 0M;
			string strQuery = @"SELECT top 1 salario
                               FROM catSalariosMinimos
                               WHERE estatus = 1 and fecha<=@fecha order by fecha desc"
			;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime)).Value = (object)fecha ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							umas = reader["salario"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["salario"].ToString());
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return umas;
		}

		public List<EstadisticaInfraccionMotivosModel> GetAllEstadisticasInfracciones(int idOficina, int idDependencia)
		{
			List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
			string strQuery = @"SELECT ci.nombre Motivo, COUNT(m.idMotivoInfraccion) Contador
                               FROM infracciones inf
                                INNER JOIN motivosInfraccion m
                                ON m.idInfraccion = inf.idInfraccion
                                INNER JOIN catMotivosInfraccion ci
                                on m.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                AND ci.estatus = 1
                                LEFT JOIN catSubConceptoInfraccion csi
                                on ci.IdSubConcepto = csi.idSubConcepto
                                AND csi.estatus = 1
                                LEFT JOIN catConceptoInfraccion cci
                                on csi.idConcepto = cci.idConcepto
                                AND cci.estatus = 1
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                                WHERE m.estatus = 1
                               AND inf.estatus = 1 AND inf.idDelegacion = @idOficina AND inf.transito = @idDependencia
							   group by ci.nombre"
		   ;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
							model.Contador = reader["Contador"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["Contador"].ToString());
							model.Motivo = reader["Motivo"].ToString();

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return modelList;
		}

		public List<EstadisticaInfraccionMotivosModel> GetBusquedaEstadisticasInfracciones(IncidenciasBusquedaModel modelBusqueda, int idDependencia)
		{
			string condiciones = "";

			condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
			condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
			condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
			condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
			condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
			condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
			condiciones += modelBusqueda.idSubTipoServicio.Equals(null) || modelBusqueda.idSubTipoServicio == 0 ? "" : " AND veh.idSubtipoServicio  = @idSubTipoServicio ";
			condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
			condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";
			condiciones += modelBusqueda.IdTipoCortesia.Equals(null) || modelBusqueda.IdTipoCortesia == 0 ? "" : " AND inf.infraccionCortesia=@IdTipoCortesia ";

			string condicionFecha = "";

			if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
				condiciones += " AND inf.fechaInfraccion >= @fechaInicio AND inf.fechaInfraccion  <= @fechaFin ";
			else if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin == DateTime.MinValue)
				condiciones += " AND inf.fechaInfraccion >= @fechaInicio ";
			else if (modelBusqueda.fechaInicio == DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
				condiciones += " AND inf.fechaInfraccion <= @fechaFin ";
			else
				condiciones += "";

			List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
			string strQuery = @"SELECT ci.nombre Motivo, COUNT(m.idMotivoInfraccion) Contador
                               FROM infracciones inf
                                INNER JOIN motivosInfraccion m
                                ON m.idInfraccion = inf.idInfraccion
                                INNER JOIN catMotivosInfraccion ci
                                on m.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                AND ci.estatus = 1
                                LEFT JOIN catSubConceptoInfraccion csi
                                on ci.IdSubConcepto = csi.idSubConcepto
                                AND csi.estatus = 1
                                LEFT JOIN catConceptoInfraccion cci
                                on csi.idConcepto = cci.idConcepto
                                AND cci.estatus = 1
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                                WHERE m.estatus = 1
                               AND inf.estatus = 1 AND inf.transito = @idDependencia " + condiciones + condicionFecha + @"
                               group by ci.nombre"
		   ;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					// command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
							model.Contador = reader["Contador"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["Contador"].ToString());
							model.Motivo = reader["Motivo"].ToString();

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return modelList;
		}
		public List<EstadisticaInfraccionMotivosModel> GetAllMotivosPorInfraccion(int idOficina, int idDependencia)
		{
			List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
			string strQuery = @"SELECT numeroMotivos, COUNT(idInfraccion) AS CantidadInfracciones
                                    FROM (
                                        SELECT mi.idInfraccion, COUNT(mi.idMotivoInfraccion) AS numeroMotivos
                                        FROM infracciones i
                                        LEFT JOIN motivosInfraccion mi ON i.idInfraccion = mi.idInfraccion
                                        WHERE i.idDelegacion = @idOficina AND i.transito = @idDependencia AND i.estatus = 1
                                        GROUP BY mi.idInfraccion
                                    ) AS InfraccionesConMotivos
                                    GROUP BY numeroMotivos
                                    HAVING numeroMotivos > 0;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
							model.NumeroMotivos = reader["numeroMotivos"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["numeroMotivos"].ToString());
							model.ContadorMotivos = reader["CantidadInfracciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["CantidadInfracciones"].ToString());
							model.ResultadoMultiplicacion = model.NumeroMotivos * model.ContadorMotivos;
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return modelList;
		}

		public List<EstadisticaInfraccionMotivosModel> GetAllMotivosPorInfraccionBusqueda(IncidenciasBusquedaModel modelBusqueda, int idDependencia)
		{
			List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
			string strQuery = @"SELECT numeroMotivos, COUNT(idInfraccion) AS CantidadInfracciones
                                    FROM (
                                        SELECT mi.idInfraccion, COUNT(mi.idMotivoInfraccion) AS numeroMotivos
                                        FROM infracciones i
                                        LEFT JOIN motivosInfraccion mi ON i.idInfraccion = mi.idInfraccion
                                        WHERE i.transito = @idDependencia AND i.estatus = 1
                                        GROUP BY mi.idInfraccion
                                    ) AS InfraccionesConMotivos
                                    GROUP BY numeroMotivos
                                    HAVING numeroMotivos > 0;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					//command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
							model.NumeroMotivos = reader["numeroMotivos"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["numeroMotivos"].ToString());
							model.ContadorMotivos = reader["CantidadInfracciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["CantidadInfracciones"].ToString());
							model.ResultadoMultiplicacion = model.NumeroMotivos * model.ContadorMotivos;

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return modelList;
		}
		//TODO: Borrar esta consulta ya no sirve para estadisticas
		public List<InfraccionesModel> GetAllInfracciones2()
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT inf.idInfraccion
                                      ,inf.idOficial
                                      ,inf.idDependencia
                                      ,inf.idDelegacion
                                      ,inf.idVehiculo
                                      ,inf.idAplicacion
                                      ,inf.idGarantia
                                      ,inf.idEstatusInfraccion
                                      ,inf.idMunicipio
                                      ,mun.municipio
                                      ,inf.idTramo
                                      ,inf.idCarretera
                                      ,inf.idPersona
                                      ,inf.idPersonaInfraccion
                                      ,inf.placasVehiculo
                                      ,inf.folioInfraccion
                                      ,inf.fechaInfraccion
                                      ,inf.kmCarretera
                                      ,inf.observaciones
                                      ,inf.lugarCalle
                                      ,inf.lugarNumero
                                      ,inf.lugarColonia
                                      ,inf.lugarEntreCalle
                                      ,inf.infraccionCortesia
                                      ,inf.NumTarjetaCirculacion
                                      ,inf.fechaActualizacion
                                      ,inf.actualizadoPor
                                      ,inf.estatus
                               FROM infracciones inf
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                               WHERE inf.estatus = 1"
			;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.municipio = reader["municipio"].ToString();
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaById((int)model.idPersona);
							model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.umas = GetUmas(model.fechaInfraccion);
							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}



			return modelList;
		}

		public List<InfoInfraccion> GetAllInfraccionesEstadisticasGrid(IncidenciasBusquedaModel modelBusqueda, int idDependencia)
		{
			string condiciones = "";

			condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
			condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
			condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
			condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
			condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
			condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
			condiciones += modelBusqueda.idSubTipoServicio.Equals(null) || modelBusqueda.idSubTipoServicio == 0 ? "" : " AND veh.idSubtipoServicio  = @idSubTipoServicio ";
			condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
			condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";
			condiciones += modelBusqueda.IdTipoCortesia.Equals(null) || modelBusqueda.IdTipoCortesia == 0 ? "" : " AND inf.infraccionCortesia=@IdTipoCortesia ";

			string condicionFecha = "";

			if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
				condiciones += " AND inf.fechaInfraccion >= @fechaInicio AND inf.fechaInfraccion  <= @fechaFin ";
			else if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin == DateTime.MinValue)
				condiciones += " AND inf.fechaInfraccion >= @fechaInicio ";
			else if (modelBusqueda.fechaInicio == DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
				condiciones += " AND inf.fechaInfraccion <= @fechaFin ";
			else
				condiciones += "";


			List<InfoInfraccion> modelList = new List<InfoInfraccion>();
			string strQuery = @"SELECT DISTINCT inf.folioInfraccion as Folio
                        ,inf.folioInfraccion FolioTXT
                        ,'' AS TTOTTE
						,inf.idCarretera,inf.idTramo,inf.idMunicipio
						,inf.infraccionCortesia
                        ,estIn.estatusInfraccion AS Estatus,
						   CASE 
								WHEN inf.infraccionCortesia = 1 THEN 'No Cortesia'
								WHEN inf.infraccionCortesia = 2 THEN 'Cortesia'
								WHEN inf.infraccionCortesia = 3 THEN 'No Aplicada'
								WHEN inf.infraccionCortesia = 4 THEN 'Aplicada'
								ELSE 'Otro'
							END AS TipoCortesia                  
						,del.nombreOficina Delegacion
                        ,catMun.municipio as Municipio
                        ,CONVERT(varchar, inf.fechaInfraccion, 103) AS FechaInfraccion
                        ,inf.horaInfraccion AS HoraInfraccion
                        ,'' as FechaVencimiento
                        ,catCarre.carretera as Carretera
                        ,catTra.tramo as Tramo
                        ,inf.kmCarretera AS Kilometraje
                        ,CONCAT(pers.nombre, ' ', pers.apellidoPaterno, ' ', pers.apellidoMaterno) as NombreConductor
						,pers.CURP as CURPConductor
                        ,CONVERT(varchar, pers.fechaNacimiento, 103) AS FechadeNacimientoConductor
                        ,CONCAT(dirpers.calle,' ', dirpers.numero, ' ',dirpers.colonia, ', ', dipersmuni.municipio, ', ', dirpersenti.nombreEntidad) as DomicilioConductor                        
                        ,pers.numeroLicencia as LicenciaConductor
                        ,CASE WHEN pers.idCatTipoPersona = 1 THEN 'x' ELSE '' END AS TipoPersFisica
                        ,CASE WHEN pers.idCatTipoPersona = 2 THEN 'x' ELSE '' END AS TipoPersMoral
                        ,CONCAT(persProp.nombre, ' ', persProp.apellidoPaterno, ' ', persProp.apellidoMaterno) as Propietario
                        ,CONCAT(persOfi.nombre, ' ', persOfi.apellidoPaterno, ' ', persOfi.apellidoMaterno) as OficialInfraccion
						,inf.idOficial
                        --,motInf.calificacion as CalifSalarios
                        ,inf.monto AS MontoCalif
                        ,COALESCE(inf.monto,'0') AS MontoPago
                        ,COALESCE(inf.reciboPago,'') AS ReciboPago
                        ,COALESCE(inf.fechaPago,'') AS FechaPago
                        ,veh.placas AS Placas
                        ,veh.serie AS SerieVeh
                        ,veh.tarjeta AS TarjetadeCirculacion
						,veh.idTipoVehiculo
						,veh.idCatTipoServicio
                        ,mv.marcaVehiculo AS Marca
                        ,sm.nombreSubmarca AS Submarca
                        ,veh.modelo AS Modelo
                        ,veh.numeroEconomico
						,veh.idSubtipoServicio
                        ,cc.color AS Color
						--,gar.idTipoLicencia
                        ,e.nombreEntidad AS EntidaddeReg
                        ,tv.tipoVehiculo AS TipodeVehículo
                        ,ts.tipoServicio AS TipodeServicio
                        ,ts.tipoServicio AS SubtipodeServicio
                        --,catGar.garantia AS TipoGarantia
                        ,'' AS TipoAplicacion
                        --,catMotInf.nombre AS Motivo
                        ,inf.observaciones AS MotivoDesc
                        FROM infracciones as inf
                        left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
                        left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idCatMotivoInfraccion 
                        left join personas pers on pers.idPersona = inf.idPersona
						LEFT JOIN personasDirecciones dirpers on dirpers.idPersona = pers.idPersona
						left join catMunicipios dipersmuni on dipersmuni.idMunicipio = dirpers.idMunicipio
						left join catEntidades dirpersenti on dirpersenti.idEntidad = dirpers.idEntidad
                        left join catOficiales persOfi on persOfi.idOficial = inf.idOficial
                        left join catDependencias dep on inf.idDependencia= dep.idDependencia
                        left join catDelegacionesOficinasTransporte	del on del.idOficinaTransporte = inf.idDelegacion
                        left join catEstatusInfraccion estIn on estIn.idEstatusInfraccion = inf.IdEstatusInfraccion 
                        left join catGarantias catGar on catGar.idGarantia = inf.idGarantia
                        left join garantiasInfraccion gar on gar.idCatGarantia = catGar.idGarantia
                        left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                        left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                        left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                        left join catMunicipios catMun on catMun.idMunicipio = inf.idMunicipio
                        left join catTramos catTra on inf.idTramo = catTra.idTramo
                        left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                        left join vehiculos veh on inf.idVehiculo = veh.idVehiculo 
                        left join personas persProp on persProp.idPersona = veh.propietario
                        left join catMarcasVehiculos mv ON mv.idMarcaVehiculo = veh.idMarcaVehiculo
                        left join catSubmarcasVehiculos sm ON sm.idSubmarca = veh.idSubmarca
                        left join catEntidades e ON e.idEntidad = veh.idEntidad
                        left join catColores cc ON cc.idColor = veh.idColor
                        left join catTiposVehiculo tv ON tv.idTipoVehiculo = veh.idTipoVehiculo
                        left join catTipoServicio ts ON ts.idCatTipoServicio = veh.idCatTipoServicio
                        WHERE 
                        inf.estatus = 1" + condiciones + condicionFecha;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					int numeroSecuencial = 1;

					if (!modelBusqueda.idDelegacion.Equals(null) && modelBusqueda.idDelegacion != 0)
						command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)modelBusqueda.idDelegacion ?? DBNull.Value;

					if (!modelBusqueda.idOficial.Equals(null) && modelBusqueda.idOficial != 0)
						command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)modelBusqueda.idOficial ?? DBNull.Value;

					if (!modelBusqueda.idCarretera.Equals(null) && modelBusqueda.idCarretera != 0)
						command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)modelBusqueda.idCarretera ?? DBNull.Value;

					if (!modelBusqueda.idTramo.Equals(null) && modelBusqueda.idTramo != 0)
						command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)modelBusqueda.idTramo ?? DBNull.Value;

					if (!modelBusqueda.idTipoVehiculo.Equals(null) && modelBusqueda.idTipoVehiculo != 0)
						command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;

					if (!modelBusqueda.idTipoServicio.Equals(null) && modelBusqueda.idTipoServicio != 0)
						command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

					if (!modelBusqueda.idSubTipoServicio.Equals(null) && modelBusqueda.idSubTipoServicio != 0)
						command.Parameters.Add(new SqlParameter("@idSubtipoServicio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idSubTipoServicio ?? DBNull.Value;

					if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
						command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;
					if (!modelBusqueda.IdTipoCortesia.Equals(null) && modelBusqueda.IdTipoCortesia != 0)
						command.Parameters.Add(new SqlParameter("@IdTipoCortesia", SqlDbType.Int)).Value = (object)modelBusqueda.IdTipoCortesia ?? DBNull.Value;

					if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
						command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

					if (modelBusqueda.fechaInicio != DateTime.MinValue)
						command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (object)modelBusqueda.fechaInicio ?? DBNull.Value;

					if (modelBusqueda.fechaFin != DateTime.MinValue)
						command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (object)modelBusqueda.fechaFin ?? DBNull.Value;

					command.Parameters.Add(new SqlParameter("@transito", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;


					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfoInfraccion model = new InfoInfraccion();
							model.Folio = reader["Folio"] == System.DBNull.Value ? default(string) : reader["Folio"].ToString();
							model.FolioTxt = reader["FolioTXT"] == System.DBNull.Value ? default(string) : reader["FolioTXT"].ToString();
							model.TTOTTE = reader["TTOTTE"] == System.DBNull.Value ? default(string) : reader["TTOTTE"].ToString();
							model.Estatus = reader["Estatus"] == System.DBNull.Value ? default(string) : reader["Estatus"].ToString();
							model.TipoCortesia = reader["TipoCortesia"] == System.DBNull.Value ? default(string) : reader["TipoCortesia"].ToString();
							model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
							model.Municipio = reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString();
							model.FechaInfraccion = reader["FechaInfraccion"] == System.DBNull.Value ? default(string) : reader["FechaInfraccion"].ToString();
							model.HoraInfraccion = reader["HoraInfraccion"] == System.DBNull.Value ? default(string) : reader["HoraInfraccion"].ToString();
							model.FechaVencimiento = reader["FechaVencimiento"] == System.DBNull.Value ? default(string) : reader["FechaVencimiento"].ToString();
							model.Carretera = reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString();
							model.Tramo = reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString();
							model.Kilometraje = reader["Kilometraje"] == System.DBNull.Value ? default(string) : reader["Kilometraje"].ToString();
							model.NombreConductor = reader["NombreConductor"] == System.DBNull.Value ? default(string) : reader["NombreConductor"].ToString();
							model.CURPConductor = reader["CURPConductor"] == System.DBNull.Value ? default(string) : reader["CURPConductor"].ToString();
							model.FechadeNacimiento = reader["FechadeNacimientoConductor"] == System.DBNull.Value ? default(string) : reader["FechadeNacimientoConductor"].ToString();
							model.DomicilioConductor = reader["DomicilioConductor"] == System.DBNull.Value ? default(string) : reader["DomicilioConductor"].ToString();
							model.LicenciaConductor = reader["LicenciaConductor"] == System.DBNull.Value ? default(string) : reader["LicenciaConductor"].ToString();
							model.TipoPersFisica = reader["TipoPersFisica"] == System.DBNull.Value ? default(string) : reader["TipoPersFisica"].ToString();
							model.TipoPersMoral = reader["TipoPersMoral"] == System.DBNull.Value ? default(string) : reader["TipoPersMoral"].ToString();
							model.Propietario = reader["Propietario"] == System.DBNull.Value ? default(string) : reader["Propietario"].ToString();
							model.OficialInfraccion = reader["OficialInfraccion"] == System.DBNull.Value ? default(string) : reader["OficialInfraccion"].ToString();
							//model.CalifSalarios = reader["CalifSalarios"] == System.DBNull.Value ? default(string) : reader["CalifSalarios"].ToString();
							model.MontoCalif = reader["MontoCalif"] == System.DBNull.Value ? default(string) : reader["MontoCalif"].ToString();
							model.MontoPago = reader["MontoPago"] == System.DBNull.Value ? default(string) : reader["MontoPago"].ToString();
							model.ReciboPago = reader["ReciboPago"] == System.DBNull.Value ? default(string) : reader["ReciboPago"].ToString();
							model.FechaPago = reader["FechaPago"] == System.DBNull.Value ? default(string) : reader["FechaPago"].ToString();
							model.Placas = reader["Placas"] == System.DBNull.Value ? default(string) : reader["FechaPago"].ToString();
							model.SerieVeh = reader["SerieVeh"] == System.DBNull.Value ? default(string) : reader["SerieVeh"].ToString();
							model.numeroEconomicoVeh = reader["numeroEconomico"] == System.DBNull.Value ? default(string) : reader["numeroEconomico"].ToString();
							model.TarjetadeCirculacion = reader["TarjetadeCirculacion"] == System.DBNull.Value ? default(string) : reader["TarjetadeCirculacion"].ToString();
							model.Marca = reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString();
							model.Submarca = reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString();
							model.Modelo = reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString();
							model.Color = reader["Color"] == System.DBNull.Value ? default(string) : reader["Color"].ToString();
							model.EntidaddeReg = reader["EntidaddeReg"] == System.DBNull.Value ? default(string) : reader["EntidaddeReg"].ToString();
							model.TipodeVehículo = reader["TipodeVehículo"] == System.DBNull.Value ? default(string) : reader["TipodeVehículo"].ToString();
							model.TipodeServicio = reader["TipodeServicio"] == System.DBNull.Value ? default(string) : reader["TipodeServicio"].ToString();
							model.SubtipodeServicio = reader["SubtipodeServicio"] == System.DBNull.Value ? default(string) : reader["SubtipodeServicio"].ToString();
							//model.TipoGarantia = reader["TipoGarantia"] == System.DBNull.Value ? default(string) : reader["TipoGarantia"].ToString();
							model.TipoAplicacion = reader["TipoAplicacion"] == System.DBNull.Value ? default(string) : reader["TipoAplicacion"].ToString();
							//model.Motivo = reader["Motivo"] == System.DBNull.Value ? default(string) : reader["Motivo"].ToString();
							model.MotivoDesc = reader["MotivoDesc"] == System.DBNull.Value ? default(string) : reader["MotivoDesc"].ToString();
							model.NumeroSecuencial = numeroSecuencial;
							modelList.Add(model);
							numeroSecuencial++;

						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}



			return modelList;
		}

		public List<InfraccionesModel> GetAllAccidentes2()
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT inf.idAccidente
                                      ,inf.idMunicipio
                                      ,mun.municipio
                                      ,inf.idCarretera
                                      ,inf.idTramo
                                      ,inf.kilometro
                                      ,inf.fecha
                                      ,inf.fechaActualizacion
                                      ,inf.actualizadoPor
                                      ,inf.estatus
                               FROM accidentes inf
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                               WHERE inf.estatus = 1"
			;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idAccidente"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.municipio = reader["municipio"].ToString();
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.kmCarretera = reader["kilometro"] == System.DBNull.Value ? string.Empty : reader["kilometro"].ToString();
							model.fechaInfraccion = reader["fecha"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fecha"].ToString());
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}



			return modelList;
		}

		public bool ValidarFolio(string folioInfraccion, int idDependencia)
		{
			int folio = 0;


			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();

				string query = "SELECT COUNT(*) AS Result FROM infracciones WHERE folioInfraccion = @folioInfraccion and  year(fechaInfraccion) = year(getdate())AND transito = @idDependencia";

				using (SqlCommand command = new SqlCommand(query, connection))
				{

					command.Parameters.AddWithValue("@folioInfraccion", folioInfraccion);
					command.Parameters.AddWithValue("@idDependencia", idDependencia);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{

							folio = reader["Result"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["Result"]);
						}
					}
				}
			}
			return folio > 0;
		}


		public int CrearInfraccion(InfraccionesModel model, int IdDependencia)
		{
			int result = 0;

			string strQuery = @"

											INSERT INTO infracciones
                                            (fechaInfraccion
                                            ,folioInfraccion
                                            ,idOficial
                                            ,idDelegacion
                                            ,idMunicipio
                                            ,idCarretera
                                            ,idTramo
                                            ,kmCarretera
                                            ,lugarCalle
                                            ,lugarNumero
                                            ,lugarColonia
                                            ,lugarEntreCalle
                                            ,idVehiculo
                                            ,idPersona
											,idPersonaInfraccion
                                            ,placasVehiculo
                                            ,NumTarjetaCirculacion
                                            ,idEstatusInfraccion
                                            ,fechaActualizacion
                                            ,actualizadoPor
                                            ,estatus
											,horaInfraccion
										    ,transito)
                                     VALUES (@fechaInfraccion
                                            ,@folioInfraccion
                                            ,@idOficial
                                            ,@idDelegacion
                                            ,@idMunicipio
                                            ,@idCarretera
                                            ,@idTramo
                                            ,@kmCarretera
                                            ,@lugarCalle
                                            ,@lugarNumero
                                            ,@lugarColonia
                                            ,@lugarEntreCalle
                                            ,@idVehiculo
                                            ,(select top 1 propietario from vehiculos where idvehiculo=@idVehiculo)
                                            ,@idPersonaInfraccion
                                            ,@placasVehiculo
                                            ,@NumTarjetaCirculacion
                                            ,1
                                            ,@fechaActualizacion
                                            ,@actualizadoPor
                                            ,@estatus
											,@horaInfraccion
											, " + IdDependencia + ");SELECT SCOPE_IDENTITY()";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
					DateTime fechaInfraccion = model.horaInfraccion;
					TimeSpan horaInfraccion = fechaInfraccion.TimeOfDay;

					string horaFormateada = horaInfraccion.ToString("hhmm");
					string horaInfraccionString = horaFormateada;
					command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.NVarChar)).Value = horaInfraccionString;
					command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion;
					command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial;
					command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion;
					command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio;

					command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.idCarretera;
					command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.idTramo;
					command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.NVarChar)).Value = (object)model.kmCarretera;
					command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.NVarChar)).Value = (object)model.lugarCalle == null ? "" : (object)model.lugarCalle;
					command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.NVarChar)).Value = (object)model.lugarNumero == null ? "" : (object)model.lugarNumero;
					command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.NVarChar)).Value = (object)model.lugarColonia == null ? "" : (object)model.lugarColonia;
					command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.NVarChar)).Value = (object)model.lugarEntreCalle == null ? "" : (object)model.lugarEntreCalle;

					command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo;
					//command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.idPersona;
					command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.idPersona;
					command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)(String.IsNullOrEmpty(model.placasVehiculo) ? "" : model.placasVehiculo.Trim(new Char[] { ' ', '-' }));
					command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value =
						!string.IsNullOrEmpty(model.NumTarjetaCirculacion) ? (object)model.NumTarjetaCirculacion : DBNull.Value;
					command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion;

					command.Parameters.Add(new SqlParameter("idDependencia", SqlDbType.Int)).Value = IdDependencia;

					//command.Parameters.Add(new SqlParameter("idDependencia", SqlDbType.Int)).Value = (object)model.idDependencia;
					//command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion;
					//command.Parameters.Add(new SqlParameter("idAplicacion", SqlDbType.Int)).Value = (object)model.idAplicacion;
					//command.Parameters.Add(new SqlParameter("idGarantia", SqlDbType.Int)).Value = (object)model.idGarantia;
					//command.Parameters.Add(new SqlParameter("observaciones", SqlDbType.NVarChar)).Value = (object)model.observaciones;
					//command.Parameters.Add(new SqlParameter("infraccionCortesia", SqlDbType.Bit)).Value = (object)model.infraccionCortesia;

					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;


					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
					return result;
				}
				finally
				{
					connection.Close();
				}

			}

			return result;
		}

		/// <summary>
		/// HMG 
		/// ACTUALIZACIÓN A INFRACCION Y VEHICULO
		/// </summary>
		/// <param name="model"></param>
		/// <param name="vehiculo"></param>
		/// <returns></returns>
		public int ModificarInfraccion(InfraccionesModel model)
		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_UpdateInfraccionesVehiculos", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
					command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idDependencia", SqlDbType.Int)).Value = (object)model.idDependencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idAplicacion", SqlDbType.Int)).Value = (object)model.idAplicacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idGarantia", SqlDbType.Int)).Value = (object)model.idGarantia ?? DBNull.Value;
					model.idEstatusInfraccion = (int)CatEnumerator.catEstatusInfraccion.Capturada;
					command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.idTramo ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.idCarretera ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.Vehiculo.idPersona ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.idPersonaInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.Vehiculo.placas ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.NVarChar)).Value = (object)model.kmCarretera ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("observaciones", SqlDbType.NVarChar)).Value = (object)model.observaciones ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.NVarChar)).Value = (object)model.lugarCalle ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.NVarChar)).Value = (object)model.lugarNumero ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.NVarChar)).Value = (object)model.lugarColonia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.NVarChar)).Value = (object)model.lugarEntreCalle ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("infraccionCortesia", SqlDbType.Int)).Value = (object)model.cortesiaInt ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.Vehiculo.tarjeta ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
					DateTime fechaInfraccion = model.horaInfraccion;
					TimeSpan horaInfraccion = fechaInfraccion.TimeOfDay;
					string horaFormateada = horaInfraccion.ToString("hhmm");
					string horaInfraccionString = horaFormateada;
					command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.NVarChar)).Value = horaInfraccionString;

					command.Parameters.Add(new SqlParameter("vigenciaTarjeta", SqlDbType.DateTime2)).Value = (object)model.Vehiculo.vigenciaTarjeta ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("motor", SqlDbType.NVarChar)).Value = (object)model.Vehiculo.motor ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.Vehiculo.numeroEconomico ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@otros", SqlDbType.NVarChar)).Value = (object)model.Vehiculo.otros ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@poliza", SqlDbType.NVarChar)).Value = (object)model.Vehiculo.poliza ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.Int)).Value = (object)model.Vehiculo.capacidad ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.Vehiculo.idEntidad ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = (object)model.Vehiculo.idColor ?? DBNull.Value;

					result = command.ExecuteNonQuery();
					//if (result > 0) // Si la actualización tuvo éxito
					//{
					return model.idInfraccion; // Retornar el idInfraccion
											   //}
				}
				catch (SqlException ex)
				{
					Logger.Debug("usp_UpdateInfraccionesVehiculos: " + ex.Message);
					return result;
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}


		public int ModificarInfraccionPorCortesia(InfraccionesModel model)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                       SET                                          
                                           infraccionCortesia = @infraccionCortesia
                                          ,fechaActualizacion = @fechaActualizacion
										  ,ObservacionsesApl = @ObservacionesApl
                                          WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
					command.Parameters.Add(new SqlParameter("@infraccionCortesia", SqlDbType.Int)).Value = 1;
					command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("@ObservacionesApl", SqlDbType.VarChar)).Value = (object)model.ObsevacionesApl;

					//command.Parameters.Add(new SqlParameter("@observacionesCortesia", SqlDbType.NVarChar)).Value = (object)model.observacionesCortesia ?? DBNull.Value;

					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					return result;
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}
		public int InsertarImagenEnInfraccion(string rutaInventario, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                       SET archivoInventario = @inventario
                       WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("@inventario", SqlDbType.VarChar)).Value = rutaInventario;

					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					return result;
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}
		public int GuardarReponse(CrearMultasTransitoChild MT_CrearMultasTransito_res, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                SET partner = @partner,
                                        cuenta = @cuenta,
                                        objeto = @objeto,
                                        documento = @documento,
                                        idEstatusInfraccion =@idEstatusInfraccion,
                                        fechaActualizacion = @fechaActualizacion,
                                        actualizadoPor = @actualizadoPor                             
                                WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.NVarChar)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("@partner", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.BUSINESSPARTNER == null ? "" : MT_CrearMultasTransito_res.BUSINESSPARTNER;
					command.Parameters.Add(new SqlParameter("@cuenta", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.CUENTAnmbb == null ? "" : MT_CrearMultasTransito_res.CUENTAnmbb;
					command.Parameters.Add(new SqlParameter("@objeto", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.OBJETO == null ? "" : MT_CrearMultasTransito_res.OBJETO;
					command.Parameters.Add(new SqlParameter("@documento", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.DOCUMENTNUMBER == null ? "" : MT_CrearMultasTransito_res.DOCUMENTNUMBER;
					command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = (object)7;
					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public List<InfraccionesResumen> GetInfraccionesLicencia(string numLicencia, string CURP)
		{
			List<InfraccionesResumen> modelList = new List<InfraccionesResumen>();
			string strQuery = @"SELECT inf.idInfraccion
	                                ,piin.nombre+' '+ piin.apellidoPaterno +' '+ piin.apellidoMaterno conductor
	                                ,piin.numeroLicencia
	                                ,p.CURP 
	                                ,inf.folioInfraccion
	                                ,inf.fechaInfraccion 
	                                ,estIn.estatusInfraccion 
	                                ,catOfi.nombre + ' ' + catOfi.apellidoPaterno + ' ' + catOfi.apellidoMaterno nombreOficial
	                                ,catMun.municipio  
	                                ,col.color
	                                ,cmv.marcaVehiculo
	                                ,csv.nombreSubmarca
	                                ,veh.placas
	                                ,veh.modelo
	                                ,veh.serie
	                                ,veh.tarjeta
	                                ,veh.vigenciaTarjeta   
                                FROM infracciones as inf  
                                left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion   
                                left join catOficiales catOfi on inf.idOficial = catOfi.idOficial  
                                left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                left join catEntidades catEnt on  catMun.idEntidad = catEnt.idEntidad   
                                left join vehiculos veh on inf.idVehiculo = veh.idVehiculo   
                                left join catMarcasVehiculos cmv on veh.idMarcaVehiculo = cmv.idMarcaVehiculo 
                                left join catSubmarcasVehiculos csv on veh.idSubmarca  = csv.idSubmarca 
                                LEFT join catColores col on veh.idColor = col.idColor 
                                LEFT join personasInfracciones piin ON inf.idPersonaInfraccion  = piin.idPersonaInfraccion  
                                LEFT JOIN personas p on piin.idPersonaInfraccion = p.idPersona 
                                WHERE inf.estatus = 1 and (piin.numeroLicencia =@numero_licencia OR p.CURP =@CURP)";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@numero_licencia", SqlDbType.VarChar)).Value = (object)numLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@CURP", SqlDbType.VarChar)).Value = (object)CURP ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesResumen model = new InfraccionesResumen();
							model.IdInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.conductor = reader["conductor"].ToString();
							model.numeroLicencia = reader["numeroLicencia"].ToString();
							model.CURP = reader["CURP"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"].ToString();
							model.municipio = reader["municipio"].ToString();
							model.color = reader["color"].ToString();
							model.marcaVehiculo = reader["marcaVehiculo"].ToString();
							model.nombreSubmarca = reader["nombreSubmarca"].ToString();
							model.placas = reader["placas"].ToString();
							model.modelo = reader["modelo"].ToString();
							model.serie = reader["serie"].ToString();
							model.tarjeta = reader["tarjeta"].ToString();
							model.vigenciaTarjeta = reader["vigenciaTarjeta"].ToString();

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}

			return modelList;
		}

		public int ModificarEstatusInfraccion(int idInfraccion, int idEstatusInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                       SET idEstatusInfraccion = @idEstatusInfraccion
                                       WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = idEstatusInfraccion;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;

					result = command.ExecuteNonQuery();
					if (result > 0) // Si la actualización tuvo éxito
					{
						return idInfraccion; // Retornar el idInfraccion
					}
				}
				catch (SqlException ex)
				{
					return result;
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}


		public List<InfraccionesModel> GetAllInfraccionesPagination(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("[usp_ObtieneTodasLasInfracciones]", connection))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);

						cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
						cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdGarantia", (object)model.IdGarantia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoCortesia", (object)model.IdTipoCortesia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.IdDelegacion ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdEstatus", (object)model.IdEstatus ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDependencia", (object)model.IdDependencia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Propietario", (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Conductor", (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value);

						//cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio);
						//cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin);

						cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio;
						cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin;
						cmd.CommandTimeout = 0;
						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
								infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
								infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
								infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
								infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
								infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
								infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
								infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
								infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
								infraccionModel.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
								infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
								infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
								infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
								infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
								infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
								infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
								infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
								infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"]);
								infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
								infraccionModel.aplicacion = reader["aplicacion"].ToString();
								infraccionModel.infraccionCortesiaString = reader["nombreCortesia"].ToString();

								//infraccionModel.Persona = _personasService.GetPersonaById((int)infraccionModel.idPersona);
								infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
								infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);

								//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

								infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
								infraccionModel.Garantia = infraccionModel.Garantia ?? new GarantiaInfraccionModel();
								infraccionModel.Garantia.garantia = infraccionModel.Garantia.garantia ?? "";
								infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

								if (infraccionModel.PersonaInfraccion != null)
								{
									infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
								}
								else
								{
									infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
								}
								infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
								// infraccionModel.NombreGarantia = infraccionModel.garantia;
								infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
								infraccionModel.Total = Convert.ToInt32(reader["Total"]);
								InfraccionesList.Add(infraccionModel);
							}
						}
					}
				}

				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
				return InfraccionesList;
			}
		}


		public List<InfraccionesModel> GetAllInfraccionesBusquedaEspecialPagination(InfraccionesBusquedaEspecialModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			DateTime? fechasIni = string.IsNullOrEmpty(model.FechaInicio) ? null : DateTime.ParseExact(model.FechaInicio, "d/M/yyyy", CultureInfo.InvariantCulture);
			DateTime? fechasFin = string.IsNullOrEmpty(model.FechaFin) ? null : DateTime.ParseExact(model.FechaFin, "d/M/yyyy", CultureInfo.InvariantCulture);

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("[usp_ObtieneTodasLasInfraccionesEspeciales]", connection))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);

						cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
						cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.oficinas ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdEstatus", (object)model.estatus ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.noLicencia != null ? model.noLicencia.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.noEconomico != null ? model.noEconomico.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folio != null ? model.folio.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Propietario", (object)model.propietario != null ? model.propietario.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Conductor", (object)model.conductor != null ? model.conductor.ToUpper() : DBNull.Value);
						//cmd.Parameters.AddWithValue("@FechaInicio", (fechasIni==null) ? DBNull.Value : fechasIni);
						//cmd.Parameters.AddWithValue("@FechaFin", (fechasFin==null) ? DBNull.Value : fechasFin);
						cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio;
						cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin;
						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
								infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
								infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
								infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
								infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
								infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
								infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
								infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
								infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
								infraccionModel.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
								infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
								infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
								infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
								infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
								infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
								infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
								infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
								//infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
								infraccionModel.infraccionCortesiaString = reader["nombreCortesia"] == System.DBNull.Value
								 ? default(string)
								 : reader["nombreCortesia"].ToString();
								infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
								if (infraccionModel.idPersona != null)
									infraccionModel.Persona = _personasService.GetPersonaById((int)infraccionModel.idPersona);
								infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
								infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);
								//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

								infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
								infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

								if (infraccionModel.PersonaInfraccion != null)
								{
									infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
								}
								else
								{
									infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
								}
								infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
								// infraccionModel.NombreGarantia = infraccionModel.garantia;
								infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
								infraccionModel.Total = Convert.ToInt32(reader["Total"]);

								InfraccionesList.Add(infraccionModel);

							}
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
				}
				finally
				{
					connection.Close();
				}
			}
			return InfraccionesList;
		}
		public int ActualizarEstatusCortesia(int idInfraccion, int cortesiaInt ,string observaciones)
		{
			var result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string updateQuery = "UPDATE infracciones SET infraccionCortesia = @cortesiaInt ,ObservacionsesApl=@obs WHERE idInfraccion = @idInfraccion";

					SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

					updateCommand.Parameters.AddWithValue("@idInfraccion", idInfraccion);
					updateCommand.Parameters.AddWithValue("@cortesiaInt", cortesiaInt);
					updateCommand.Parameters.AddWithValue("@obs", observaciones);
					updateCommand.ExecuteNonQuery();

					string selectQuery = "SELECT infraccionCortesia FROM infracciones WHERE idInfraccion = @idInfraccion";
					SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
					selectCommand.Parameters.AddWithValue("@idInfraccion", idInfraccion);

					object infraccionCortesia = selectCommand.ExecuteScalar();
					if (infraccionCortesia != DBNull.Value)
					{
						result = (int)infraccionCortesia; // o cualquier otro tratamiento necesario
					}
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}

				return result;

			}


		}

		public List<InfraccionesModel> GetReporteInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("[sp_ReporteInfracciones]", connection))
					{


						using (var adapter = new SqlDataAdapter(cmd))
						{
							cmd.CommandType = CommandType.StoredProcedure;

							cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
							cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdGarantia", (object)model.IdGarantia ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdTipoCortesia", (object)model.IdTipoCortesia ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.IdDelegacion ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdEstatus", (object)model.IdEstatus ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdDependencia", (object)model.IdDependencia ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@Propietario", (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@Conductor", (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value);

							//cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio);
							//cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin);

							cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio;
							cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin;

                            using SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                            while (reader.Read())
                            {

                                InfraccionesModel infraccionModel = new InfraccionesModel();
                                infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                                infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
                                infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                                infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
                                infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
                                infraccionModel.aplicacion = reader["aplicacion"].ToString();
                                infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();
                                infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();

                                //Asignacion de garantia
                                infraccionModel.Garantia = new();
                                infraccionModel.Garantia.garantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();

                                //Asignacion de conductor
                                infraccionModel.PersonaInfraccion = new();
                                infraccionModel.PersonaInfraccion.nombre = reader["nombrePI"] == System.DBNull.Value ? string.Empty : reader["nombrePI"].ToString();
                                infraccionModel.PersonaInfraccion.apellidoPaterno = reader["apellidoPaternoPI"] == System.DBNull.Value ? string.Empty : reader["apellidoPaternoPI"].ToString();
                                infraccionModel.PersonaInfraccion.apellidoMaterno = reader["apellidoMaternoPI"] == System.DBNull.Value ? string.Empty : reader["apellidoMaternoPI"].ToString();

                                //Asignacion de vehiculo
                                infraccionModel.Vehiculo = new();
                                infraccionModel.Vehiculo.placas = reader["placasVehiculo"] == System.DBNull.Value ? string.Empty : reader["placasVehiculo"].ToString();
                                infraccionModel.Vehiculo.marca = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : reader["marcaVehiculo"].ToString();
                                infraccionModel.Vehiculo.submarca = reader["submarcaVehiculo"] == System.DBNull.Value ? string.Empty : reader["submarcaVehiculo"].ToString();
                                infraccionModel.Vehiculo.modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : reader["modelo"].ToString();

                                //Asignacion de propietario del vehiculo
                                infraccionModel.Vehiculo.Persona = new();
                                infraccionModel.Vehiculo.Persona.nombre = reader["nombre"] == System.DBNull.Value ? string.Empty : reader["nombre"].ToString();
                                infraccionModel.Vehiculo.Persona.apellidoPaterno = reader["apellidoPaterno"] == System.DBNull.Value ? string.Empty : reader["apellidoPaterno"].ToString();
                                infraccionModel.Vehiculo.Persona.apellidoMaterno = reader["apellidoMaterno"] == System.DBNull.Value ? string.Empty : reader["apellidoMaterno"].ToString();

                                InfraccionesList.Add(infraccionModel);
                            }
                        }
					}
				}

				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					Logger.Error("Error al generar reporte infracciones:" + ex);
				}
				finally
				{
					connection.Close();
				}
				return InfraccionesList;
			}
		}



		public int GetDiaFestivo(int idDelegacion, DateTime fecha)
		{

           int resultado = 0;
            string strQuery = @"SELECT 1 as resultado	                                  
                                FROM diasInhabiles as f, delegaciones i  
                                WHERE f.idMunicipio = i.idMunicipio
									and i.idDelegacion = @idDelegacion
									and f.fecha = CONVERT(DATE, @fecha,103) 
								";
            
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)idDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime)).Value = (object)fecha ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            resultado = reader["resultado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["resultado"].ToString());


                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    //ex
                }
                finally
                {
                    connection.Close();
                }
            }

            return resultado;


        }
    }
}

