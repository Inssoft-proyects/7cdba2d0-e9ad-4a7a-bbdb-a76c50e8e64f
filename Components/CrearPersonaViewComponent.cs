/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Thursday, February 29th 2024 2:22:40 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sat Mar 02 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */


using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class CrearPersonaViewComponent : ViewComponent
    {
        private readonly IPersonasService _personasService;
        public CrearPersonaViewComponent(IPersonasService personasService)
        {
            _personasService = personasService;
        }
        public async Task<IViewComponentResult> InvokeAsync(PersonaModel persona)
        {
            if (persona == null)
            {
                persona = new PersonaModel();

            }
            if (persona.idPersona==null || persona.idPersona == 0)
            {
                persona.PersonaDireccion ??= new PersonaDireccionModel();
            }
            else
            {
                persona = _personasService.GetPersonaById((int)persona.idPersona);
                persona.generoBool = persona.idGenero == 1;
            }

            return await Task.FromResult((IViewComponentResult)View("CrearPersonaFisica", persona));
        }
    }
}