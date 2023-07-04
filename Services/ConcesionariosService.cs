using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ConcesionariosService : IConcesionariosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ConcesionariosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }


        public List<ConcesionariosModel> GetConcesionarios()
        {
            List<ConcesionariosModel> ListConcesionarios = new List<ConcesionariosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from Concesionarios where estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ConcesionariosModel concesionarios = new ConcesionariosModel();
                            concesionarios.IdConcesionario = Convert.ToInt32(reader["IdConcesionario"].ToString());
                            concesionarios.Concesionario = reader["Concesionario"].ToString();
                            ListConcesionarios.Add(concesionarios);
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
            return ListConcesionarios;

        }

        public Concesionarios2Model GetConcesionarioById(int idConcesionario)
        {
            List<Concesionarios2Model> ListConcesionarios = new List<Concesionarios2Model>();
            string strQuery = @"SELECT 
                                idConcesionario
                                ,concesionario
                                ,idDelegacion
                                ,idMunicipio
                                ,alias
                                ,razonSocial
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM concesionarios
                                WHERE idConcesionario = @idConcesionario AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = idConcesionario;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Concesionarios2Model concesionarios = new Concesionarios2Model();
                            concesionarios.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            concesionarios.nombre = reader["concesionario"].ToString();
                            concesionarios.idDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            concesionarios.idMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            concesionarios.alias = reader["alias"].ToString();
                            concesionarios.razonSocial = reader["razonSocial"].ToString();
                            ListConcesionarios.Add(concesionarios);
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
            return ListConcesionarios.FirstOrDefault();

        }
        public List<Concesionarios2Model> GetConcesionarios2ByIdDelegacion(int idDelegacion)
        {
            List<Concesionarios2Model> ListConcesionarios = new List<Concesionarios2Model>();
            string strQuery = @"SELECT 
                                idConcesionario
                                ,concesionario
                                ,idDelegacion
                                ,idMunicipio
                                ,alias
                                ,razonSocial
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM concesionarios
                                WHERE idDelegacion = @idDelegacion AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idDelegacion;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Concesionarios2Model concesionarios = new Concesionarios2Model();
                            concesionarios.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            concesionarios.nombre = reader["concesionario"].ToString();
                            concesionarios.idDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            concesionarios.idMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            concesionarios.alias = reader["alias"].ToString();
                            concesionarios.razonSocial = reader["razonSocial"].ToString();
                            ListConcesionarios.Add(concesionarios);
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
            return ListConcesionarios;

        }

        public int CrearConcesionario(Concesionarios2Model model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO concesionarios VALUES (@concesionario
                                                                  ,@idDelegacion
                                                                  ,@idMunicipio
                                                                  ,@alias
                                                                  ,@razonSocial
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

                    //command.Parameters.Add(new SqlParameter("idConcesionario", SqlDbType.Int)).Value = (object)model.idConcesionario;
                    command.Parameters.Add(new SqlParameter("concesionario", SqlDbType.NVarChar)).Value = (object)model.nombre;
                    command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion;
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio;
                    command.Parameters.Add(new SqlParameter("alias", SqlDbType.NVarChar)).Value = (object)model.alias;
                    command.Parameters.Add(new SqlParameter("razonSocial", SqlDbType.NVarChar)).Value = (object)model.razonSocial;
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

        public int EditarConcesionario(Concesionarios2Model model)
        {
            int result = 0;
            string strQuery = @"UPDATE concesionarios 
                                SET 
                                    concesionario = @concesionario
                                   ,idDelegacion = @idDelegacion
                                   ,idMunicipio = @idMunicipio
                                   ,alias = @alias
                                   ,razonSocial = @razonSocial
                                   ,fechaActualizacion = @fechaActualizacion
                                   ,actualizadoPor = @actualizadoPor
                                   ,estatus = @estatus
                                WHERE idConcesionario = @idConcesionario";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add(new SqlParameter("idConcesionario", SqlDbType.Int)).Value = (object)model.idConcesionario;
                    command.Parameters.Add(new SqlParameter("concesionario", SqlDbType.NVarChar)).Value = (object)model.nombre;
                    command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion;
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio;
                    command.Parameters.Add(new SqlParameter("alias", SqlDbType.NVarChar)).Value = (object)model.alias;
                    command.Parameters.Add(new SqlParameter("razonSocial", SqlDbType.NVarChar)).Value = (object)model.razonSocial;
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

        public IEnumerable<Concesionarios2Model> GetAllConcesionarios()
        {
            List<Concesionarios2Model> ListConcesionarios = new List<Concesionarios2Model>();
            string strQuery = @"SELECT 
                                 c.idConcesionario
                                ,c.concesionario
                                ,c.idDelegacion
                                ,c.idMunicipio
                                ,c.alias
                                ,c.razonSocial
                                ,c.fechaActualizacion
                                ,c.actualizadoPor
                                ,c.estatus
								,d.delegacion
								,m.municipio
                                FROM concesionarios c
								INNER JOIN catDelegaciones d
                                on c.idDelegacion = d.idDelegacion 
								AND d.estatus = 1
                                INNER JOIN catMunicipios m
                                on c.idMunicipio = m.idMunicipio 
                                AND m.estatus = 1
                                WHERE c.estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Concesionarios2Model concesionarios = new Concesionarios2Model();
                            concesionarios.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            concesionarios.nombre = reader["concesionario"].ToString();
                            concesionarios.idDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            concesionarios.idMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            concesionarios.alias = reader["alias"].ToString();
                            concesionarios.razonSocial = reader["razonSocial"].ToString();
                            concesionarios.delegacion = reader["delegacion"].ToString();
                            concesionarios.municipio = reader["municipio"].ToString();
                            ListConcesionarios.Add(concesionarios);
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
            return ListConcesionarios;
        }
    }
}
