/*
 * Descripción:
 * Proyecto: Sistema de Infracciones y Accidentes
 * Fecha de creación: Monday, February 19th 2024 4:11:01 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Wed Feb 21 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IVehiculoPlataformaService
    {
        bool ValidarRoboRepuve(RepuveConsgralRequestModel repuveGralModel);
        VehiculoModel GetVehiculoModelFromFinanzas(RootCotejarDatosRes result);
    }
}