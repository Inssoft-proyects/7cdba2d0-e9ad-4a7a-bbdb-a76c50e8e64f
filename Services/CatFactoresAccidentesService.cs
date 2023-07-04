using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatFactoresAccidentesService : ICatFactoresAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatFactoresAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatFactoresAccidentesModel> GetFactoresAccidentes()
        {
            //
            List<CatFactoresAccidentesModel> ListaFactores = new List<CatFactoresAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT f.*, e.estatus FROM catFactoresAccidentes AS f INNER JOIN estatus AS e ON c.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresAccidentesModel factor = new CatFactoresAccidentesModel();
                            factor.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            factor.FactorAccidente = reader["FactorAccidente"].ToString();
                            factor.estatusDesc = reader["estatus"].ToString();
                            factor.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            factor.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            factor.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaFactores.Add(factor);

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
            return ListaFactores;


        }

        public List<CatFactoresAccidentesModel> GetFactoresAccidentesActivos()
        {
            //
            List<CatFactoresAccidentesModel> ListaFactores = new List<CatFactoresAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT f.*, e.estatus FROM catFactoresAccidentes AS f INNER JOIN estatus AS e ON f.estatus = e.estatus WHERE f.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresAccidentesModel factor = new CatFactoresAccidentesModel();
                            factor.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            factor.FactorAccidente = reader["FactorAccidente"].ToString();
                            factor.estatusDesc = reader["estatus"].ToString();
                            factor.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            factor.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                           // factor.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaFactores.Add(factor);

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
            return ListaFactores;


        }


    }

}
