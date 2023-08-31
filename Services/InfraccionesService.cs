using GuanajuatoAdminUsuarios.Interfaces;
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

        public List<InfraccionesModel> GetAllInfracciones(int idOficina)
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
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
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
                                    WHERE  inf.idDelegacion = @idOficina AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
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
                            model.kmCarretera = reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.idPersona);
                            model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idPersonaInfraccion);
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
                            //model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
                            model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idGarantia);
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

        public List<InfraccionesModel> GetAllInfracciones(InfraccionesBusquedaModel model,int idOficina)
        {
            List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();

                    string sqlCondiciones = "";
                    sqlCondiciones += (object)model.IdGarantia == null ? "" : " gar.idGarantia=@IdGarantia AND \n";
                    sqlCondiciones += (object)model.IdDelegacion == null ? "" : " del.idOficinaTransporte=@IdDelegacion AND \n";
                    sqlCondiciones += (object)model.IdEstatus == null ? "" : " estIn.idEstatusInfraccion=@IdEstatus AND \n";
                    sqlCondiciones += (object)model.IdDependencia == null ? "" : " dep.idDependencia=@IdDependencia AND \n";
                    sqlCondiciones += (object)model.folioInfraccion == null ? "" : " UPPER(inf.folioInfraccion)=@FolioInfraccion AND \n";
                    sqlCondiciones += (object)model.placas == null ? "" : " UPPER(veh.placas)=@Placas AND \n";
                    sqlCondiciones += (object)model.Propietario == null ? "" : " UPPER(veh.propietario)=@Propietario AND \n";
                    sqlCondiciones += (object)model.Conductor == null ? "" : "UPPER(pInf.nombre + ' ' + pInf.apellidoPaterno + ' ' + pInf.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Conductor + '%' AND \n";

                    sqlCondiciones += (object)model.FechaInicio == null && (object)model.FechaFin == null ? "" : " inf.fechaInfraccion between @FechaInicio and  @FechaFin AND \n";
                   
                     

                    string SqlTransact =
                            string.Format(@"SELECT inf.idInfraccion
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
                                    ,veh.placas placasVehiculo
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
                                    left join personasInfracciones pInf on inf.idPersonaInfraccion = pInf.idPersonaInfraccion
                                    where {0}  inf.estatus=1", sqlCondiciones);

                    
                    



                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdGarantia", SqlDbType.Int)).Value = (object)model.IdGarantia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdEstatus", SqlDbType.Int)).Value = (object)model.IdEstatus ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdDependencia", SqlDbType.Int)).Value = (object)model.IdDependencia ?? DBNull.Value;
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
                            infraccionModel.kmCarretera = reader["kmCarretera"].ToString();
                            infraccionModel.observaciones = reader["observaciones"].ToString();
                            infraccionModel.lugarCalle = reader["lugarCalle"].ToString();
                            infraccionModel.lugarNumero = reader["lugarNumero"].ToString();
                            infraccionModel.lugarColonia = reader["lugarColonia"].ToString();
                            infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            infraccionModel.Persona = _personasService.GetPersonaById((int)infraccionModel.idPersona);
                            infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idPersonaInfraccion);
                            infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);
                            //infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);
                            infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idGarantia);
                            infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo== null? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
                            infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

                            infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
                            infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "": infraccionModel.Vehiculo.Persona.nombreCompleto;
                            infraccionModel.NombreGarantia = infraccionModel.Garantia.garantia;

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


        public InfraccionesModel GetInfraccionById(int IdInfraccion)
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
                                                    WHERE inf.estatus = 1 and inf.idInfraccion=@IdInfraccion";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)IdInfraccion ?? DBNull.Value;
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
                            model.kmCarretera = reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            model.municipio = reader["municipio"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.idPersona);
                            model.PersonaInfraccion = GetPersonaInfraccionById((int)model.idPersonaInfraccion);
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
                            model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
                            model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idGarantia);
                            model.strIsPropietarioConductor = model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
                            model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();
                            model.umas = GetUmas();

                            if (model.MotivosInfraccion.Any(w => w.calificacion != null))
                            {
                                model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
                            }
                            model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
                            model.NombrePropietario = model.Vehiculo.Persona.nombreCompleto;
                            model.fechaNacimiento = model.Vehiculo.Persona.fechaNacimiento;

                            model.NombreGarantia = model.Garantia.garantia;
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


        public List<MotivosInfraccionVistaModel> GetMotivosInfraccionByIdInfraccion(int idInfraccion)
        {
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
                            //model.concepto = reader["concepto"].ToString();
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

        public GarantiaInfraccionModel GetGarantiaById(int idGarantia)
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
                                AND g.idGarantia = @idGarantia";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGarantia", SqlDbType.Int)).Value = (object)idGarantia ?? DBNull.Value;
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
                            model.numLicencia = reader["numPlaca"].ToString();
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

        public PersonaInfraccionModel GetPersonaInfraccionById(int idPersonaInfraccion)
        {
            List<PersonaInfraccionModel> modelList = new List<PersonaInfraccionModel>();
            string strQuery = @"SELECT
                                 idPersonaInfraccion
                                ,numeroLicencia
                                ,CURP
                                ,RFC
                                ,nombre
                                ,apellidoPaterno
                                ,apellidoMaterno
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                ,idCatTipoPersona
                                FROM personasInfracciones
                                WHERE estatus = 1
                                AND idPersonaInfraccion = @idPersonaInfraccion";


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPersonaInfraccion", SqlDbType.Int)).Value = (object)idPersonaInfraccion ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaInfraccionModel model = new PersonaInfraccionModel();
                            model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
                            model.CURP = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
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

        /// <summary>
        /// Captura de persona con registro especifico para infraccion, se crea al final del proceso de creacion parte 1
        /// </summary>
        /// <param name="idPersona"></param>
        /// <returns></returns>
        public int CrearPersonaInfraccion(int idPersona)
        {
            int result = 0;
            string strQuery = @"INSERT INTO personasInfracciones(numeroLicencia, CURP, RFC, nombre,apellidoPaterno, apellidoMaterno, idCatTipoPersona, fechaActualizacion, actualizadoPor, estatus)
                                SELECT numeroLicencia, CURP, RFC, nombre,apellidoPaterno, apellidoMaterno, idCatTipoPersona, @fechaActualizacion, @actualizadoPor, @estatus
                                FROM personas
                                WHERE idPersona = @idPersona;SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
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

        public int CrearGarantiaInfraccion(GarantiaInfraccionModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO garantiasInfraccion VALUES(@idCatGarantia
                                                                      ,@idTipoPlaca
                                                                      ,@idTipoLicencia
                                                                      ,@numPlaca
                                                                      ,@numLicencia
                                                                      ,@vehiculoDocumento
                                                                      ,@fechaActualizacion
                                                                      ,@actualizadoPor
                                                                      ,@estatus);SELECT SCOPE_IDENTITY()";
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

        public int ModificarGarantiaInfraccion(GarantiaInfraccionModel model)
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
                                                               WHERE idGarantia = @idGarantia";

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
                                SET fechaActualizacion = @fechaActualizacion
                                    actualizadoPor = @actualizadoPor
                                    estatus = @estatus)
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

        public InfraccionesModel GetInfraccion2ById(int idInfraccion)
        {
            List<InfraccionesModel> modelList = new List<InfraccionesModel>();
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
                               AND idInfraccion = @idInfraccion"
            ;

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
                            model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
                            model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
                            model.placasVehiculo = reader["placasVehiculo"].ToString();
                            model.folioInfraccion = reader["folioInfraccion"].ToString();
                            model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            model.kmCarretera = reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.idPersona);
                            model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idPersonaInfraccion);
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
                            model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
                            model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idGarantia);
                            model.umas = GetUmas();
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

        public NuevaInfraccionModel GetInfraccionAccidenteById(int idInfraccion)
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
                               AND idInfraccion = @idInfraccion"
            ;

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
                            NuevaInfraccionModel model = new NuevaInfraccionModel();
                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
                            model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
                            model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
                            model.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
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
                            model.Kilometro = reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.Tarjeta = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.IdPersona);
                            model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idPersonaInfraccion);
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.IdVehiculo);
                            model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
                            model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idGarantia);
                            model.umas = GetUmas();
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


        public decimal GetUmas()
        {
            decimal umas = 0M;
            string strQuery = @"SELECT salario
                               FROM catSalariosMinimos
                               WHERE estatus = 1 AND area = 'C'"
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
                            model.kmCarretera = reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.idPersona);
                            model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idPersonaInfraccion);
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
                            model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
                            model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idGarantia);
                            model.umas = GetUmas();
                            if (model.MotivosInfraccion.Any(w=> w.calificacion != null))
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
                            model.kmCarretera = reader["kilometro"].ToString();
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


        public int CrearInfraccion(InfraccionesModel model)
        {
            int result = 0;
        
            string strQuery = @"INSERT INTO infracciones
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
                                            ,estatus)
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
                                            ,@idPersona
                                            ,@idPersonaInfraccion
                                            ,@placasVehiculo
                                            ,@NumTarjetaCirculacion
                                            ,1
                                            ,@fechaActualizacion
                                            ,@actualizadoPor
                                            ,@estatus);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
                    command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion;
                    command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial;
                    command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion; 
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio;

                    command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.idCarretera;
                    command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.idTramo;
                    command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.Int)).Value = (object)model.kmCarretera;
                    command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.NVarChar)).Value = (object)model.lugarCalle == null ? "" : (object)model.lugarCalle;
                    command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.NVarChar)).Value = (object)model.lugarNumero == null ? "" : (object)model.lugarNumero;
                    command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.NVarChar)).Value = (object)model.lugarColonia == null ? "" : (object)model.lugarColonia;
                    command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.NVarChar)).Value = (object)model.lugarEntreCalle == null ? "" : (object)model.lugarEntreCalle;

                    command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo;
                    command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.idPersona;
                    command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.idPersonaInfraccion;
                    command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.placasVehiculo.Trim(new Char[] {' ','-'});
                    command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value =
                        !string.IsNullOrEmpty(model.NumTarjetaCirculacion) ? (object)model.NumTarjetaCirculacion : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion;

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

        public int ModificarInfraccion(InfraccionesModel model)
        {
            int result = 0;
            string strQuery = @"UPDATE infracciones
                                       SET idOficial = @idOficial
                                          ,idDependencia = @idDependencia
                                          ,idDelegacion = @idDelegacion
                                          ,idVehiculo = @idVehiculo
                                          ,idAplicacion = @idAplicacion
                                          ,idEstatusInfraccion = @idEstatusInfraccion
                                          ,idMunicipio = @idMunicipio
                                          ,idTramo = @idTramo
                                          ,idCarretera = @idCarretera
                                          ,idPersona = @idPersona
                                          ,idPersonaInfraccion = @idPersonaInfraccion
                                          ,placasVehiculo = @placasVehiculo
                                          ,folioInfraccion = @folioInfraccion
                                          ,fechaInfraccion = @fechaInfraccion
                                          ,kmCarretera = @kmCarretera
                                          ,observaciones = @observaciones
                                          ,lugarCalle = @lugarCalle
                                          ,lugarNumero = @lugarNumero
                                          ,lugarColonia = @lugarColonia
                                          ,lugarEntreCalle = @lugarEntreCalle
                                          ,infraccionCortesia = @infraccionCortesia
                                          ,NumTarjetaCirculacion = @NumTarjetaCirculacion
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
                    command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
                    command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idDependencia", SqlDbType.Int)).Value = (object)model.idDependencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idAplicacion", SqlDbType.Int)).Value = (object)model.idAplicacion ?? DBNull.Value;
                    //command.Parameters.Add(new SqlParameter("idGarantia", SqlDbType.Int)).Value = (object)model.idGarantia ?? DBNull.Value;
                    model.idEstatusInfraccion = (int)CatEnumerator.catEstatusInfraccion.Capturada;
                    command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.idTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.idCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.idPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.idPersonaInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.placasVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.Int)).Value = (object)model.kmCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("observaciones", SqlDbType.NVarChar)).Value = (object)model.observaciones ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.NVarChar)).Value = (object)model.lugarCalle ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.NVarChar)).Value = (object)model.lugarNumero ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.NVarChar)).Value = (object)model.lugarColonia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.NVarChar)).Value = (object)model.lugarEntreCalle ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("infraccionCortesia", SqlDbType.Bit)).Value = (object)model.infraccionCortesia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.NumTarjetaCirculacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;


                    result = command.ExecuteNonQuery();
                    if (result > 0) // Si la actualización tuvo éxito
                    {
                        return model.idInfraccion; // Retornar el idInfraccion
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


        public int ModificarInfraccionPorCortesia(InfraccionesModel model)
        {
            int result = 0;
            string strQuery = @"UPDATE infracciones
                                       SET                                          
                                           idEstatusInfraccion = @idEstatusInfraccion
                                          ,fechaActualizacion = @fechaActualizacion
                                          ,observacionesCortesia = @observacionesCortesia
                                          WHERE idInfraccion = @idInfraccion";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
                    command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@observacionesCortesia", SqlDbType.NVarChar)).Value = (object)model.observacionesCortesia ?? DBNull.Value;

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
        public int InsertarImagenEnInfraccion(byte[] imageData, int idInfraccion)
        {
            int result = 0;
            string strQuery = @"UPDATE infracciones
                       SET inventario = @inventario
                       WHERE idInfraccion = @idInfraccion";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;
                    command.Parameters.Add(new SqlParameter("@inventario", SqlDbType.VarBinary)).Value = imageData;

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
             
            return modelList ;
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

    }
 }

