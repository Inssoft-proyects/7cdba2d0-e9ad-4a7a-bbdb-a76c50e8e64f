using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatDelegacionesOficinasTransporteService : ICatDelegacionesOficinasTransporteService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatDelegacionesOficinasTransporteService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas()
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT del.*, e.estatusDesc,m.municipio FROM catDelegacionesOficinasTransporte AS del INNER JOIN estatus AS e ON del.estatus = e.estatus INNER JOIN catMunicipios AS m ON del.idMunicipio = m.idMunicipio;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            delegacionOficina.NombreOficina = reader["NombreOficina"].ToString();
                            delegacionOficina.JefeOficina = reader["JefeOficina"].ToString();
                            delegacionOficina.Municipio = reader["Municipio"].ToString();
                            delegacionOficina.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            delegacionOficina.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }
    }
}
