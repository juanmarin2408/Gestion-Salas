using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EquipmentType
    {
        Tractor,
        Plow,
        Harvester,
        Seeder,
        Sprayer
    }

    public enum EquipmentStatus
    {
        Available,
        InUse,
        UnderMaintenance,
        Decommissioned
    }
    public enum EstadoSala
    {
        Inactiva = 0,
        Activa = 1
    }

    public enum RolUsuario
    {
        Administrador = 0,
        Usuario = 1,
        Invitado = 2
    }

    public enum EstadoEquipo
    {
        Disponible = 0,
        Asignado = 1,
        EnMantenimiento = 2,
        Dañado = 3,
        Baja = 4
    }

}
