using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatHospitalesService : ICatHospitalesService
    {
        
            private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
            public CatHospitalesService(ISqlClientConnectionBD sqlClientConnectionBD)
            {
                _sqlClientConnectionBD = sqlClientConnectionBD;
            }

            public List<CatHospitalesModel> GetHospitales()
            {
                //
                List<CatHospitalesModel> ListaHospitales = new List<CatHospitalesModel>();

                using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                    try

                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("SELECT h.*, e.estatusDesc, m.Municipio FROM catHospitales AS h INNER JOIN estatus AS e ON h.estatus = e.estatus INNER JOIN catMunicipios AS m ON h.idMunicipio = m.idMunicipio;", connection);
                        command.CommandType = CommandType.Text;
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                            CatHospitalesModel hospital = new CatHospitalesModel();
                            hospital.IdHospital = Convert.ToInt32(reader["IdHospital"].ToString());
                            hospital.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            hospital.NombreHospital = reader["NombreHospital"].ToString();
                            hospital.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            hospital.Municipio = reader["Municipio"].ToString();
                            hospital.estatusDesc = reader["estatusDesc"].ToString();
                            hospital.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            //hospital.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());


                            ListaHospitales.Add(hospital);

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
                return ListaHospitales;


            }


        }

    }



