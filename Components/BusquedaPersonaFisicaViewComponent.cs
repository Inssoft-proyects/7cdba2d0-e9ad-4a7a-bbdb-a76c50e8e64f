/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 27th 2024 9:57:35 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Tue Feb 27 2024
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
    public class BusquedaPersonaFisicaViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(bool isModal)
        {
            var modelo = new BusquedaPersonaModel
            {
                IsModal = isModal
            };
            return await Task.FromResult((IViewComponentResult)View("BusquedaPersonaFisica",modelo));
        }
    }
}