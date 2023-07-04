using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PensionesService : IPensionesService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public PensionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<PensionModel> GetAllPensiones()
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
                                                 WHERE p.estatus = 1";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
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

        public List<PensionModel> GetPensionesToGrid(string strPension, int? idDelegacion)
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
                                                 WHERE p.estatus = 1
                                                 AND p.pension = {0}
                                                 AND p.idDelegacion = {1}";

                    string strWherePension = !string.IsNullOrEmpty(strPension) ? string.Format("'{0}'", strPension) : "p.pension";
                    string strWhereDelegacion = idDelegacion != null ? idDelegacion.ToString() : "p.idDelegacion";
                    SqlTransact = string.Format(SqlTransact, strWherePension, strWhereDelegacion);

                    SqlCommand command = new SqlCommand(SqlTransact, connection);

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pensionModel = new PensionModel();
                            pensionModel.IdPension = Convert.ToInt32(reader["idPension"].ToString());
                            pensionModel.Indicador = Convert.ToInt32(reader["indicador"].ToString());
                            pensionModel.Pension = reader["pension"].ToString();
                            pensionModel.Permiso = reader["permiso"].ToString();
                            pensionModel.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            pensionModel.IdMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            pensionModel.Direccion = reader["direccion"].ToString();
                            pensionModel.Telefono = reader["telefono"].ToString();
                            pensionModel.Correo = reader["correo"].ToString();
                            pensionModel.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            pensionModel.ActualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            pensionModel.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            pensionModel.delegacion = reader["delegacion"].ToString();
                            pensionModel.municipio = reader["municipio"].ToString();
                            pensionModel.responsable = reader["responsable"].ToString();
                            pensionModel.placas = reader["placas"].ToString();
                            pensionModel.concesionario = reader["concesionario"].ToString();
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

        public List<PensionModel> GetPensionById(int idPension)
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
                                                 WHERE p.estatus = 1
                                                 AND p.idPension = @idPension";


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
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
        public List<Gruas2Model> GetGruasDisponiblesByIdPension(int idPension)
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
                                                 AND g.estatus = 1";


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;
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
    }
}
