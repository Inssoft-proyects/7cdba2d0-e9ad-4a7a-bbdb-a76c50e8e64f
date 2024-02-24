/*
 * Descripción: Servicio de Vehiculo relacionado con algun servicio de REPUVE o Finanzas
 * Proyecto: RIAG
 * Fecha de creación: Wednesday, February 21st 2024 9:56:46 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Wed Feb 21 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.AspNetCore.Mvc;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;

namespace GuanajuatoAdminUsuarios.Services
{
    public partial class VehiculoPlataformaService : IVehiculoPlataformaService
    {
        #region Variables
        private readonly IRepuveService _repuveService;
        private readonly ICatMunicipiosService _municipioService;
        private readonly ICatEntidadesService _entidadService;
        private readonly IColores _colorService;
        private readonly ICatMarcasVehiculosService _marcaVehiculoService;
        private readonly ICatSubmarcasVehiculosService _submarcaVehiculoService;
        private readonly ICatDictionary _catDictionary;
        [GeneratedRegex("[0-9-]")]
        private static partial Regex MyRegex();
        #endregion

        public VehiculoPlataformaService(IRepuveService repuveService, ICatEntidadesService entidadService,ICatMunicipiosService municipioService, IColores colorService,
        ICatMarcasVehiculosService marcaVehiculoService, ICatSubmarcasVehiculosService submarcaVehiculoService, ICatDictionary catDictionary)
        {
            _repuveService = repuveService;
            _entidadService = entidadService;
            _municipioService = municipioService;
            _colorService = colorService;
            _marcaVehiculoService = marcaVehiculoService;
            _submarcaVehiculoService = submarcaVehiculoService;
            _catDictionary = catDictionary;
        }

        /// <summary>
        /// Valida si un vehiculo tiene reporte de robo en REPUVE
        /// </summary>
        /// <param name="repuveGralModel"></param>
        /// <returns></returns>

        public bool ValidarRoboRepuve(RepuveConsgralRequestModel repuveGralModel)
        {
            var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel)?.FirstOrDefault() ?? new RepuveConsRoboResponseModel();
            return repuveConsRoboResponse.estatus == "1";
        }
        #region Catalogos y Generacion de Objeto Vehiculo Finanzas

        /// <summary>
        /// Regresa objeto vehiculo a partir de un objeto de finanzas
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public VehiculoModel GetVehiculoModelFromFinanzas(RootCotejarDatosRes result)
        {
            var vehiculoEncontradoData = result.MT_CotejarDatos_res.tb_vehiculo[0];
            var vehiculoDireccionData = result.MT_CotejarDatos_res.tb_direccion[0];
            var vehiculoInterlocutorData = result.MT_CotejarDatos_res;
            var idMunicipio = !string.IsNullOrEmpty(vehiculoDireccionData.municipio)
                  ? _municipioService.obtenerIdPorNombre(vehiculoDireccionData.municipio)
                  : 0;

            var idEntidad = !string.IsNullOrEmpty(vehiculoDireccionData.entidadreg)
                ? _entidadService.obtenerIdPorEntidad(vehiculoDireccionData.entidadreg)
                : 0;

            var idColor = !string.IsNullOrEmpty(vehiculoEncontradoData.color)
                ? ObtenerIdColor(vehiculoEncontradoData.color)
                : 0;

            var idMarca = !string.IsNullOrEmpty(vehiculoEncontradoData.marca)
                ? ObtenerIdMarca(vehiculoEncontradoData.marca)
                : 0;

            var idSubmarca = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerIdSubmarca(vehiculoEncontradoData.linea)
                : 0;
            var submarcaLimpio = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerSubmarca(vehiculoEncontradoData.linea)
                : "NA";
            var telefonoValido = !string.IsNullOrEmpty(vehiculoDireccionData.telefono)
                ? LimpiarValorTelefono(vehiculoDireccionData.telefono)
                : 0;
            var cargaBool = ConvertirBool(vehiculoEncontradoData.carga);
            var generoBool = vehiculoInterlocutorData.es_per_fisica?.sexo == "2";

            var idTipo = !string.IsNullOrEmpty(vehiculoEncontradoData.categoria)
             ? ObtenerIdTipoVehiculo(vehiculoEncontradoData.categoria)
             : 0;

            var vehiculoEncontrado = new VehiculoModel
            {
                placas = vehiculoEncontradoData.no_placa,
                serie = vehiculoEncontradoData.no_serie,
                tarjeta = vehiculoEncontradoData.no_tarjeta,
                motor = vehiculoEncontradoData.no_motor,
                otros = vehiculoEncontradoData.otros,
                idColor = idColor,
                idEntidad = idEntidad,
                idMarcaVehiculo = idMarca,
                idSubmarca = idSubmarca,
                submarca = submarcaLimpio,
                idTipoVehiculo = idTipo,
                modelo = vehiculoEncontradoData.modelo,
                capacidad = vehiculoEncontradoData.numpersona,
                carga = cargaBool,

                Persona = new PersonaModel
                {
                    RFC = vehiculoInterlocutorData.Nro_rfc,
                    RFCFisico = vehiculoInterlocutorData.Nro_rfc,
                    CURPFisico = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                    nombreFisico = vehiculoInterlocutorData.es_per_fisica?.Nombre,
                    apellidoPaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                    apellidoMaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                    fechaNacimiento = vehiculoInterlocutorData.es_per_fisica?.Fecha_nacimiento,
                    generoBool = generoBool,

                    nombre = vehiculoInterlocutorData.es_per_moral?.name_org1,
                    PersonaDireccion = new PersonaDireccionModel
                    {
                        telefonoFisico = telefonoValido.ToString(),
                        telefono = telefonoValido.ToString(),
                        colonia = vehiculoDireccionData.colonia,
                        coloniaFisico = vehiculoDireccionData.colonia,
                        calleFisico = vehiculoDireccionData.calle,
                        calle = vehiculoDireccionData.calle,
                        numero = vehiculoDireccionData.nro_exterior,
                        numeroFisico = vehiculoDireccionData.nro_exterior,
                        idMunicipioFisico = idMunicipio,
                        idMunicipio = idMunicipio,
                    }
                },

                PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                {
                    PersonasMorales = new List<PersonaModel>()
                }
            };

            return vehiculoEncontrado;

        }


        private static string ObtenerSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                return submarcaLimpio;
            }

            return "NA"; // Valor predeterminado en caso de no encontrar el guión
        }

        private int ObtenerIdColor(string color)
        {
            string colorLimpio = MyRegex().Replace(color, "").Trim();
            var idColor = _colorService.obtenerIdPorColor(colorLimpio);
            return idColor;
        }
        private int ObtenerIdMarca(string marca)
        {
            string[] partes = marca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string marcaLimpio = partes[1].Trim();

                var idMarca = _marcaVehiculoService.obtenerIdPorMarca(marcaLimpio);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }

        private int ObtenerIdSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                var idSubmarca = _submarcaVehiculoService.obtenerIdPorSubmarca(submarcaLimpio);
                return idSubmarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }
        private int ObtenerIdTipoVehiculo(string categoria)
        {

            int idTipo = 0;
            var tipoVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            idTipo = tipoVehiculo.CatalogList.Where(w => categoria.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();
            return idTipo;

        }

        private static bool ConvertirBool(string carga)
        {
            bool cargaBool = false;

            if (carga.Trim() == "1.00")
            {
                cargaBool = true;
            }
            else if (carga.Trim() == "0.00")
            {
                cargaBool = false;
            }
            return cargaBool;
        }

        private static long LimpiarValorTelefono(string telefono)
        {
            telefono = telefono.Replace(" ", "");
            if (long.TryParse(telefono, out long telefonoValido))
            {
                return telefonoValido;
            }
            else
            {
                return 0; // O algún otro valor que indique que no es válido
            }
        }
        #endregion
    }
}