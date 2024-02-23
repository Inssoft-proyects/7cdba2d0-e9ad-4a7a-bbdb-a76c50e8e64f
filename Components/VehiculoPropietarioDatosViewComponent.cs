/*
 * Descripción:
 * Proyecto: Sistema de Infracciones y Accidentes
 * Fecha de creación: Tuesday, February 20th 2024 10:41:56 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Fri Feb 23 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;
namespace GuanajuatoAdminUsuarios.Components
{
    public class VehiculoPropietarioDatosViewComponent : ViewComponent
    {
        public VehiculoPropietarioDatosViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(int idVehiculo)  
       {
            //var modelo = new VehiculoPropietarioBusquedaModel();
           return await Task.FromResult((IViewComponentResult) View("VehiculoPropietarioDatos",new VehiculoModel()));  
       }  
    }
}