using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PlacaServices : IPlacaServices
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public PlacaServices(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Se obtendra placas de la tabla depositos
        /// </summary>
        /// <param name="DelegacionId"></param>
        /// <returns></returns>
        public List<PlacaModel> GetPlacasByDelegacionId(int DelegacionId)
        {
            List<PlacaModel> placas = new List<PlacaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from depositos where idDelegacion=@idDelegacion ", connection);
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = DelegacionId;
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PlacaModel placa = new PlacaModel();
                            placa.IdDepositos = Convert.ToInt32(reader["IdDeposito"].ToString());
                            placa.Placa = reader["Placa"].ToString();
                            placas.Add(placa);
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
            return placas;
        }

    }
}
