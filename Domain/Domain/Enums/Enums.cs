using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EstadoSala
    {
        Inactiva = 0,
        Activa = 1
    }

    public enum RolUsuario
    {
        Administrador = 0,
        Usuario = 1,
        Coordinador = 2
    }

    public enum EstadoEquipo
    {
        Disponible = 0,
        Asignado = 1,
        EnMantenimiento = 2,
        Dañado = 3,
        Baja = 4
    }

    public enum EstadoSolicitud
    {
        Pendiente = 0,
        Aprobada = 1,
        Rechazada = 2
    }

    public enum EstadoReporte
    {
        Pendiente = 0,
        EnRevision = 1,
        Resuelto = 2,
        Rechazado = 3
    }

    public enum PrioridadReporte
    {
        Baja = 0,
        Media = 1,
        Alta = 2,
        Urgente = 3
    }

    public enum TipoReporte
    {
        Equipo = 0,
        Sala = 1
    }

    public enum EstadoAsesoria
    {
        Pendiente = 0,
        EnProceso = 1,
        Resuelto = 2,
        Rechazado = 3
    }

    public enum TipoSolicitudPrestamo
    {
        Equipo = 0,
        SalaCompleta = 1
    }

}
