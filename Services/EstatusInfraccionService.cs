using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EstatusInfraccionService: IEstatusInfraccionService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public EstatusInfraccionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }


        public List<EstatusInfraccionModel> GetEstatusInfracciones()
        {
            List<EstatusInfraccionModel> ListEstatus = new List<EstatusInfraccionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catEstatusInfraccion where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EstatusInfraccionModel estatus = new EstatusInfraccionModel();
                            estatus.idEstatusInfraccion = Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
                            estatus.estatusInfraccion = reader["estatusInfraccion"].ToString();
                            ListEstatus.Add(estatus);
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
            return ListEstatus;
        }
    }
}
