using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using System.Globalization;
using GuanajuatoAdminUsuarios.Framework.Catalogs;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CortesiasNoAplicadasService : ICortesiasNoAplicadas
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IInfraccionesService _infraccionesService;
        public CortesiasNoAplicadasService(ISqlClientConnectionBD sqlClientConnectionBD, IInfraccionesService infraccionesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _infraccionesService = infraccionesService;
        }


        List<CortesiasNoAplicadasModel> ICortesiasNoAplicadas.ObtInfraccionesCortesiasNoAplicadas(string FolioInfraccion)
        {
            List<CortesiasNoAplicadasModel> ListaInfracciones = new List<CortesiasNoAplicadasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.folioInfraccion,
                                                        CONVERT(varchar,i.fechaInfraccion,103) AS fechaInfraccion, 
                                                        CONCAT(pI.nombre,' ',pI.apellidoPaterno,' ', pI.apellidoMaterno)AS Conductor,
                                                        i.placasVehiculo, v.serie,
                                                        CONCAT( pV.nombre,' ', pV.apellidoPaterno,' ', pV.apellidoMaterno)AS Propietario,
                                                        e.estatusInfraccion
                                                        FROM infracciones AS i 
                                                        LEFT JOIN vehiculos AS v ON i.idVehiculo = v.idVehiculo 
                                                        LEFT JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion 
                                                        LEFT JOIN personas AS pI ON pI.IdPersona = i.IdPersona 
                                                        LEFT JOIN personas AS pV ON pV.IdPersona = v.idPersona 
                                                        LEFT JOIN catDelegaciones cde ON cde.idDelegacion = i.idDelegacion  
                                                        WHERE folioInfraccion LIKE '%' + @FolioInfraccion + '%' AND i.infraccionCortesia = 0;", connection);

                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CortesiasNoAplicadasModel infraccion = new CortesiasNoAplicadasModel();
                            //infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.folioInfraccion = reader["folioInfraccion"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.FechaInfraccion = reader["fechaInfraccion"].ToString();
                            infraccion.Propietario = reader["Propietario"] is DBNull ? string.Empty : reader["Propietario"].ToString();
                            infraccion.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            infraccion.Conductor = reader["Conductor"] is DBNull ? string.Empty : reader["Conductor"].ToString();
                            infraccion.Estatus = reader["estatusInfraccion"].ToString();

                            ListaInfracciones.Add(infraccion);

                        }

                    }

                }
                catch (SqlException ex)
                {
                }
                finally
                {
                    connection.Close();
                }
            return ListaInfracciones;
        }

        CortesiasNoAplicadasModel ICortesiasNoAplicadas.ObtenerDetalleCortesiasNoAplicada(string id)
        {
            CortesiasNoAplicadasModel infraccion = new CortesiasNoAplicadasModel();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT 
                    i.folioInfraccion,m.municipio,cde.delegacion,
                    CONCAT(f.nombre,' ',f.apellidoPaterno,' ', f.apellidoMaterno)AS Oficial, t.tramo,
                    i.kmCarretera, CONCAT(pI.nombre,' ',pI.apellidoPaterno,' ', pI.apellidoMaterno)AS Conductor,
                    cv.nombreSubmarca, (Case When i.infraccionCortesia = 1 Then 'Aplica' 
                    ELse 'No Aplica' END ) AS infraccionCortesia, g.garantia, tp.tipoPlaca, i.placasVehiculo,
                    tl.tipoLicencia, v.tarjeta, i.monto, i.reciboPago, i.fechaPago, i.lugarPago,
                    i.oficioEnvio, i.placasVehiculo, CONCAT( pV.nombre,' ', pV.apellidoPaterno,' ', pV.apellidoMaterno)AS Propietario,
                    i.observaciones, e.estatusInfraccion,
                    CONVERT(varchar,i.fechaInfraccion,103) AS fechaInfraccion, v.serie,
                    (Case When i.estatus = 1 Then 'si' 
                    ELse 'No' END ) AS estatus
                    FROM infracciones AS i 
                    LEFT JOIN vehiculos AS v ON i.idVehiculo = v.idVehiculo 
                    LEFT JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion 
                    LEFT JOIN personas AS pI ON pI.IdPersona = i.IdPersona 
                    LEFT JOIN personas AS pV ON pV.IdPersona = v.idPersona 
                    LEFT JOIN catDelegaciones cde ON cde.idDelegacion = i.idDelegacion 
                    LEFT JOIN CatMunicipios AS m ON m.idMunicipio = i.idMunicipio
                    LEFT JOIN catOficiales As f ON f.idOficial = i.idOficial
                    LEFT JOIN catTramos AS t ON t.idTramo = i.idTramo
                    LEFT JOIN catSubmarcasVehiculos AS cv On cv.idSubmarca = v.idSubmarca
                    LEFT JOIN catGarantias AS g ON g.idGarantia = i.idGarantia
                    LEFT JOIN garantiasInfraccion gi ON gi.idGarantia = g.idGarantia
                    LEFT JOIN catTipoPlaca AS tp ON tp.idTipoPlaca = gi.idTipoPlaca
                    LEFT JOIN catTipoLicencia AS tl ON tl.idTipoLicencia = pi.idTipoLicencia
                    WHERE i.folioInfraccion like '%' + @id + '%' and i.infraccionCortesia = 0", connection);
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar)).Value = id;
                    command.CommandType = CommandType.Text;
                    using (
                        SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            infraccion.folioInfraccion = reader["folioInfraccion"].ToString();
                            infraccion.Municipio = reader["municipio"].ToString();
                            infraccion.Delegacion = reader["delegacion"].ToString();
                            infraccion.Oficial = reader["Oficial"].ToString();
                            //infraccion.Camino = reader["Camino"].ToString();
                            infraccion.Tramo = reader["tramo"].ToString();
                            infraccion.KmCarretera = reader["kmCarretera"].ToString();
                            infraccion.Conductor = reader["Conductor"].ToString();
                            infraccion.Vehiculo = reader["nombreSubmarca"].ToString();
                            //infraccion.TipoAplicacion = reader["TipoAplicacion"].ToString();
                            infraccion.TipoCortesia = reader["infraccionCortesia"].ToString();
                            //infraccion.CalificacionTotal = reader["CalificacionTotal"].ToString();
                            infraccion.TipoGarantia = reader["garantia"].ToString();
                            infraccion.TipoPlaca = reader["tipoPlaca"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.TipoLicencia = reader["tipoLicencia"].ToString();
                            //infraccion.Licencia = reader["tipoLicencia"].ToString();
                            infraccion.Tarjeta = reader["tarjeta"].ToString();
                            //infraccion.ArchivoInventario = reader["ArchivoInventario"].ToString();
                            //infraccion.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"].ToString());
                            infraccion.MontoCalificacion = reader["monto"].ToString();
                            infraccion.MontoPagado = reader["monto"].ToString();
                            infraccion.Recibo = reader["reciboPago"].ToString();
                            object fechaPagoObj = reader["fechaPago"];

                            if (fechaPagoObj != DBNull.Value && fechaPagoObj != null)
                            {
                                infraccion.FechaPago = Convert.ToDateTime(fechaPagoObj);
                            }
                            else
                            {
                                infraccion.FechaPago = DateTime.MinValue;
                            }
                            infraccion.LugarPago = reader["lugarPago"].ToString();
                            //infraccion.OficioConDonacion = reader["OficioConDonacion"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.Tarjeta = reader["tarjeta"].ToString();
                            infraccion.Propietario = reader["Propietario"].ToString();
                            infraccion.Observaciones = reader["observaciones"].ToString();
                            infraccion.Estatus = reader["estatusInfraccion"].ToString();
                            //infraccion.Capturista = reader["Capturista"].ToString();
                            infraccion.Baja = reader["estatus"].ToString();
                            infraccion.FechaInfraccion = reader["fechaInfraccion"].ToString();
                            infraccion.Serie = reader["serie"].ToString();

                        }

                    }

                }
                catch (SqlException ex)
                {
                }
                finally
                {
                    connection.Close();
                }

            return infraccion;

        }


		public CortesiasNoAplicadasModel GuardarObservacion(string folioInfraccion,string observaciones)
		
        {
			int result = 0;

			CortesiasNoAplicadasModel infraccion = new CortesiasNoAplicadasModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand("Update infracciones set observaciones = @observaciones where folioInfraccion = @folioInfraccion", connection);
					command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.NVarChar)).Value = folioInfraccion;
					command.Parameters.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar)).Value = observaciones;

					command.CommandType = CommandType.Text;
					result = command.ExecuteNonQuery();
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
			return infraccion;


		}

		
	}
}
