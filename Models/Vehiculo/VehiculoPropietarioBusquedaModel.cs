/*
 * Descripción:
 * Proyecto: Commons
 * Fecha de creación: Tuesday, February 20th 2024 10:36:45 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Wed Feb 21 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */
namespace GuanajuatoAdminUsuarios.Models
{
  public class VehiculoPropietarioBusquedaModel
  {
    public int IdEntidadBusqueda { get; set; }
    public string PlacaBusqueda { get; set; }
    public string SerieBusqueda { get; set; }
    public VehiculoModel Vehiculo { get; set; }

  }
}