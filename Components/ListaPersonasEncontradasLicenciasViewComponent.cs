/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Wednesday, February 28th 2024 2:13:23 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Wed Feb 28 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class ListaPersonasEncontradasLicenciasViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<PersonaLicenciaModel> listaPersonas)
        {
            return await Task.FromResult((IViewComponentResult)View("ListaPersonasEncontradasLicencias", listaPersonas));
        }
    }
}