using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PensionesService : IPensionesService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public PensionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<PensionModel> GetAllPensiones(int idOficina)
        {

            List<PensionModel> ListPensiones = new List<PensionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact = @"SELECT 
                                                  p.idPension
                                                 ,p.indicador
                                                 ,p.pension
                                                 ,p.permiso
                                                 ,p.idDelegacion
                                                 ,p.idMunicipio
                                                 ,p.direccion
                                                 ,p.telefono
                                                 ,p.correo
                                                 ,p.fechaActualizacion
                                                 ,p.actualizadoPor
                                                 ,p.estatus
                                                 ,p.idResponsable
                                                 ,d.delegacion
                                                 ,m.municipio
                                                 ,cr.responsable
                                                 ,g.placas
                                                 ,c.concesionario
												 ,g.placas
												 ,c.concesionario
                                                 FROM pensiones p
                                                 INNER JOIN catDelegaciones d
                                                 on p.idDelegacion = d.idDelegacion 
                                                 AND d.estatus = 1
                                                 INNER JOIN catMunicipios m
                                                 on p.idMunicipio = m.idMunicipio 
                                                 AND m.estatus = 1
                                                 INNER JOIN catResponsablePensiones cr
                                                 on p.idResponsable = cr.idResponsable
                                                 AND cr.estatus = 1
                                                 LEFT JOIN pensionGruas pg
                                                 on p.idPension = pg.idPension
                                                 LEFT JOIN gruas g
                                                 on pg.idGrua = g.idGrua
                                                 AND g.estatus = 1
                                                 LEFT JOIN concesionarios c
                                                 on g.idConcesionario = c.idConcesionario
                                                 AND c.estatus = 1
                                                 WHERE p.estatus = 1 AND p.idDelegacion = @idOficina";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pension = new PensionModel();
                            pension.IdPension = Convert.ToInt32(reader["idPension"].ToString());
                            pension.Indicador = reader["indicador"] == System.DBNull.Value ? default(int?) : (int?)reader["indicador"];
                            pension.Pension = reader["pension"].ToString();
                            pension.Permiso = reader["permiso"].ToString();
                            pension.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            pension.IdMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            pension.Direccion = reader["direccion"].ToString();
                            pension.Telefono = reader["telefono"].ToString();
                            pension.Correo = reader["correo"].ToString();
                            pension.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            pension.ActualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            pension.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            pension.delegacion = reader["delegacion"].ToString();
                            pension.municipio = reader["municipio"].ToString();
                            pension.responsable = reader["responsable"].ToString();
                            pension.placas = reader["placas"].ToString();
                            pension.concesionario = reader["concesionario"].ToString();
                            ListPensiones.Add(pension);

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
            return ListPensiones;
        }

        public List<PensionModel> GetPensionesToGrid(string strPension, int? idOficina)
        {

            List<PensionModel> ListPensiones = new List<PensionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string SqlTransact = @"SELECT 
                            p.idPension,
                            p.indicador,
                            p.pension,
                            p.permiso,
                            p.idDelegacion,
                            p.idMunicipio,
                            p.direccion,
                            p.telefono,
                            p.correo,
                            p.fechaActualizacion,
                            p.actualizadoPor,
                            p.estatus,
                            p.idResponsable,
                            d.delegacion,
                            m.municipio,
                            cr.responsable,
                            g.placas,
                            c.concesionario
                      FROM pensiones p
                      INNER JOIN catDelegaciones d on p.idDelegacion = d.idDelegacion AND d.estatus = 1
                      INNER JOIN catMunicipios m on p.idMunicipio = m.idMunicipio AND m.estatus = 1
                      INNER JOIN catResponsablePensiones cr on p.idResponsable = cr.idResponsable AND cr.estatus = 1
                      LEFT JOIN pensionGruas pg on p.idPension = pg.idPension
                      LEFT JOIN gruas g on pg.idGrua = g.idGrua AND g.estatus = 1
                      LEFT JOIN concesionarios c on g.idConcesionario = c.idConcesionario AND c.estatus = 1
                      WHERE p.estatus = 1
                      AND p.pension LIKE @strPension";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.AddWithValue("@strPension", "%" + strPension + "%");

                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pensionModel = new PensionModel();
                            pensionModel.IdPension = reader["idPension"] != DBNull.Value ? Convert.ToInt32(reader["idPension"]) : 0;
                            pensionModel.Indicador = reader["indicador"] != DBNull.Value ? Convert.ToInt32(reader["indicador"]) : 0;
                            pensionModel.Pension = reader["pension"] != DBNull.Value ? reader["pension"].ToString() : string.Empty;
                            pensionModel.Permiso = reader["permiso"] != DBNull.Value ? reader["permiso"].ToString() : string.Empty;
                            pensionModel.IdDelegacion = reader["idDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idDelegacion"]) : 0;
                            pensionModel.IdMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                            pensionModel.Direccion = reader["direccion"] != DBNull.Value ? reader["direccion"].ToString() : string.Empty;
                            pensionModel.Telefono = reader["telefono"] != DBNull.Value ? reader["telefono"].ToString() : string.Empty;
                            pensionModel.Correo = reader["correo"] != DBNull.Value ? reader["correo"].ToString() : string.Empty;
                            pensionModel.FechaActualizacion = reader["fechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaActualizacion"]) : DateTime.MinValue;
                            pensionModel.ActualizadoPor = reader["actualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["actualizadoPor"]) : 0;
                            pensionModel.estatus = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 0;
                            pensionModel.delegacion = reader["delegacion"] != DBNull.Value ? reader["delegacion"].ToString() : string.Empty;
                            pensionModel.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;
                            pensionModel.responsable = reader["responsable"] != DBNull.Value ? reader["responsable"].ToString() : string.Empty;
                            pensionModel.placas = reader["placas"] != DBNull.Value ? reader["placas"].ToString() : string.Empty;
                            pensionModel.concesionario = reader["concesionario"] != DBNull.Value ? reader["concesionario"].ToString() : string.Empty;

                            ListPensiones.Add(pensionModel);

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
            return ListPensiones;
        }

        public List<PensionModel> GetPensionById(int idPension, int idOficina)//se omite la condicion idOficina por falta de fatos en base
        {

            List<PensionModel> ListPensiones = new List<PensionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string SqlTransact = @"SELECT 
                                                  p.idPension
                                                 ,p.indicador
                                                 ,p.pension
                                                 ,p.permiso
                                                 ,p.idDelegacion
                                                 ,p.idMunicipio
                                                 ,p.direccion
                                                 ,p.telefono
                                                 ,p.correo
                                                 ,p.fechaActualizacion
                                                 ,p.actualizadoPor
                                                 ,p.estatus
                                                 ,p.idResponsable
                                                 ,d.delegacion
                                                 ,m.municipio
                                                 ,cr.responsable
                                                 ,g.placas
                                                 ,c.concesionario
                                                 FROM pensiones p
                                                 LEFT JOIN catDelegaciones d
                                                 on p.idDelegacion = d.idDelegacion 
                                                 AND d.estatus = 1
                                                 LEFT JOIN catMunicipios m
                                                 on p.idMunicipio = m.idMunicipio 
                                                 AND m.estatus = 1
                                                 LEFT JOIN catResponsablePensiones cr
                                                 on p.idResponsable = cr.idResponsable
                                                 AND cr.estatus = 1
                                                 LEFT JOIN pensionGruas pg
                                                 on p.idPension = pg.idPension
                                                 LEFT JOIN gruas g
                                                 on pg.idGrua = g.idGrua
                                                 AND g.estatus = 1
                                                 LEFT JOIN concesionarios c
                                                 on g.idConcesionario = c.idConcesionario
                                                 AND c.estatus = 1
                                                 WHERE p.estatus = 1
                                                 AND p.idPension = @idPension";


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pension = new PensionModel();
                            pension.IdPension = Convert.ToInt32(reader["idPension"].ToString());
                            pension.Indicador = reader["indicador"] == System.DBNull.Value ? default(int?) : (int?)reader["indicador"];
                            pension.Pension = reader["pension"].ToString();
                            pension.Permiso = reader["permiso"].ToString();
                            pension.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            pension.IdMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            pension.Direccion = reader["direccion"].ToString();
                            pension.Telefono = reader["telefono"].ToString();
                            pension.Correo = reader["correo"].ToString();
                            pension.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            pension.ActualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            pension.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            pension.IdResponsable = Convert.ToInt32(reader["idResponsable"].ToString());
                            pension.delegacion = reader["delegacion"].ToString();
                            pension.municipio = reader["municipio"].ToString();
                            pension.responsable = reader["responsable"].ToString();
                            pension.placas = reader["placas"].ToString();
                            pension.concesionario = reader["concesionario"].ToString();
                            ListPensiones.Add(pension);

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
            return ListPensiones;
        }
        public List<Gruas2Model> GetGruasDisponiblesByIdPension(int idPension, int idOficina)//se omite la condicion idOficina por falta de fatos en base
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string SqlTransact = @"SELECT g.idGrua
                                                 ,g.idConcesionario
                                                 ,g.idClasificacion
                                                 ,g.idTipoGrua
                                                 ,g.idSituacion
                                                 ,g.noEconomico
                                                 ,g.placas
                                                 ,g.modelo
                                                 ,g.capacidad
                                                 ,g.fechaActualizacion
                                                 ,g.actualizadoPor
                                                 ,g.estatus
                                                 ,0 as isPension
                                                 ,cm.municipio
								                 ,c.concesionario
								                 ,ccg.clasificacion
								                 ,ctg.TipoGrua
								                 ,csg.situacion
                                                 FROM gruas g
                                                 INNER JOIN catClasificacionGruas ccg
								                 on g.idClasificacion = ccg.idClasificacionGrua
								                 INNER JOIN catTipoGrua ctg
								                 on g.idTipoGrua = ctg.IdTipoGrua
								                 INNER JOIN catSituacionGruas csg
								                 on g.idSituacion = csg.idSituacion
								                 INNER JOIN concesionarios c
								                 on g.idConcesionario = c.idConcesionario AND c.estatus = 1
								                 INNER JOIN catMunicipios cm
								                 on c.idMunicipio = cm.idMunicipio AND c.estatus = 1
                                                 WHERE g.idGrua not in (SELECT DISTINCT idGrua FROM pensionGruas WHERE idPension <> @idPension or idPension = idPension)
                                                 AND g.estatus = 1
                                                 UNION 
                                                 SELECT g.idGrua
                                                 ,g.idConcesionario
                                                 ,g.idClasificacion
                                                 ,g.idTipoGrua
                                                 ,g.idSituacion
                                                 ,g.noEconomico
                                                 ,g.placas
                                                 ,g.modelo
                                                 ,g.capacidad
                                                 ,g.fechaActualizacion
                                                 ,g.actualizadoPor
                                                 ,g.estatus
                                                 ,1 as isPension
                                                 ,cm.municipio
								                 ,c.concesionario
								                 ,ccg.clasificacion
								                 ,ctg.TipoGrua
								                 ,csg.situacion
                                                 FROM gruas g
                                                 INNER JOIN catClasificacionGruas ccg
								                 on g.idClasificacion = ccg.idClasificacionGrua
								                 INNER JOIN catTipoGrua ctg
								                 on g.idTipoGrua = ctg.IdTipoGrua
								                 INNER JOIN catSituacionGruas csg
								                 on g.idSituacion = csg.idSituacion
								                 INNER JOIN concesionarios c
								                 on g.idConcesionario = c.idConcesionario AND c.estatus = 1
								                 INNER JOIN catMunicipios cm
								                 on c.idMunicipio = cm.idMunicipio AND c.estatus = 1
                                                 WHERE g.idGrua in (SELECT DISTINCT idGrua FROM pensionGruas WHERE idPension = @idPension)
                                                 AND g.estatus = 1 AND c.idDelegacion = @idOficina";


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            gruasModel.concesionario = reader["concesionario"].ToString();
                            gruasModel.municipio = reader["municipio"].ToString();
                            gruasModel.clasificacion = reader["clasificacion"].ToString();
                            gruasModel.tipoGrua = reader["tipoGrua"].ToString();
                            gruasModel.situacion = reader["situacion"].ToString();
                            gruasModel.isPension = reader["isPension"].ToString() == "1";
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }
        public int CrearPension(PensionModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO pensiones VALUES(@indicador
                                                              ,@pension
                                                              ,@permiso
                                                              ,@idDelegacion
                                                              ,@idMunicipio
                                                              ,@direccion
                                                              ,@telefono
                                                              ,@correo
                                                              ,@fechaActualizacion
                                                              ,@actualizadoPor
                                                              ,@estatus
                                                              ,@idResponsable);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@indicador", SqlDbType.Int)).Value = (object)model.Indicador ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@pension", SqlDbType.NVarChar)).Value = (object)model.Pension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@permiso", SqlDbType.NVarChar)).Value = (object)model.Permiso ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@direccion", SqlDbType.NVarChar)).Value = (object)model.Direccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@telefono", SqlDbType.NVarChar)).Value = (object)model.Telefono ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@correo", SqlDbType.NVarChar)).Value = (object)model.Correo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@idResponsable", SqlDbType.Int)).Value = (object)model.IdResponsable ?? DBNull.Value;
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

        public int CrearPensionGruas(int idPension, List<int> gruas)
        {
            int result = 0;
            string strQuery = @"INSERT INTO pensionGruas VALUES(@idPension,@idGrua)";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    //connection.BeginTransaction();
                    foreach (var idGrua in gruas)
                    {
                        SqlCommand command = new SqlCommand(strQuery, connection);
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)idGrua ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
                        result += command.ExecuteNonQuery();
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

        public int EliminarPensionGruas(int idPension)
        {
            int result = 0;
            string strQuery = @"DELETE FROM pensionGruas WHERE idPension =  @idPension";
            //string strGruas = string.Join(",", gruas);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
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

        public int EditarGrua(PensionModel model)

        {
            int result = 0;
            string strQuery = @"UPDATE pensiones SET  indicador = @indicador
                                                     ,pension = @pension
                                                     ,permiso = @permiso
                                                     ,idDelegacion =  @idDelegacion
                                                     ,idMunicipio = @idMunicipio
                                                     ,direccion = @direccion
                                                     ,telefono = @telefono
                                                     ,correo = @correo
                                                     ,fechaActualizacion = @fechaActualizacion
                                                     ,actualizadoPor = @actualizadoPor
                                                     ,estatus = @estatus
                                                     ,idResponsable = @idResponsable
                                                      WHERE idPension = @idPension";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@indicador", SqlDbType.Int)).Value = (object)0 ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@permiso", SqlDbType.NVarChar)).Value = (object)model.Permiso ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@pension", SqlDbType.NVarChar)).Value = (object)model.Pension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@direccion", SqlDbType.NVarChar)).Value = (object)model.Direccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@telefono", SqlDbType.NVarChar)).Value = (object)model.Telefono ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@correo", SqlDbType.NVarChar)).Value = (object)model.Correo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@idResponsable", SqlDbType.Int)).Value = (object)model.IdResponsable ?? DBNull.Value;
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
        public List<PensionModel> GetPensionesByDelegacion(int delegacionDDValue)
        {

            List<PensionModel> ListPensiones = new List<PensionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact = @"SELECT 
                                                  p.idPension
                                                 ,p.pension
                                                 ,p.idDelegacion
                                                 ,p.fechaActualizacion
                                                 ,p.actualizadoPor
                                                 ,p.estatus
                                                 FROM pensiones p
                                                 INNER JOIN catDelegaciones d
                                                 on p.idDelegacion = d.idDelegacion 
                                                 AND d.estatus = 1
                                                 WHERE p.estatus = 1 AND p.idDelegacion = @delegacionDDValue";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@delegacionDDValue", SqlDbType.Int)).Value = (object)delegacionDDValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pension = new PensionModel();
                            pension.IdPension = Convert.ToInt32(reader["IdPension"].ToString());
                            pension.Pension = reader["Pension"].ToString();
                            pension.IdDelegacion = Convert.ToInt32(reader["IdDelegacion"].ToString());
                            pension.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            pension.ActualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            pension.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            ListPensiones.Add(pension);

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
            return ListPensiones;
        }

        public string GetPensionLogin(int idPension)
		{
			string resultadoPension = "No Asignada";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string SqlTransact = @"SELECT 
                                                 p.pension 
                                                 FROM pensiones p
                                                 WHERE p.estatus = 1
                                                 AND p.idPension = @idPension";


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
							resultadoPension = reader["pension"].ToString();

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
            return resultadoPension;
        }
    }

}
