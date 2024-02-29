/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 27th 2024 12:36:56 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Thu Feb 29 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class ListaPersonasEncontradasViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<PersonaModel> listaPersonas)
        {
            return await Task.FromResult((IViewComponentResult)View("ListaPersonasEncontradas", listaPersonas ));
        }
    }
}