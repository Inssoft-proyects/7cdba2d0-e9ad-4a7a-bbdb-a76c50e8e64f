using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatCausasAccidentesService : ICatCausasAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatCausasAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatCausasAccidentesModel> ObtenerCausasActivas()
        {
            //
            List<CatCausasAccidentesModel> ListaCausas = new List<CatCausasAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatus FROM catCausasAccidentes AS c INNER JOIN estatus AS e ON c.estatus = e.estatus WHERE c.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCausasAccidentesModel causa = new CatCausasAccidentesModel();
                            causa.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
                            causa.CausaAccidente = reader["CausaAccidente"].ToString();
                            causa.estatusDesc = reader["estatus"].ToString();
                            causa.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            causa.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            //carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaCausas.Add(causa);

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
            return ListaCausas;


        }


    }

}

