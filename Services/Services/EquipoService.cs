using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Services.Models.EquipoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _equipoRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly IMapper _mapper;

        public EquipoService(
            IEquipoRepository equipoRepository,
            ISalaRepository salaRepository,
            IMapper mapper)
        {
            _equipoRepository = equipoRepository;
            _salaRepository = salaRepository;
            _mapper = mapper;
        }

        public async Task<IList<EquipoModel>> GetEquipos()
        {
            var equipos = await _equipoRepository.GetEquipos();
            return equipos.Select(e => new EquipoModel
            {
                Id = e.Id,
                SalaId = e.SalaId,
                SalaNumero = e.Sala?.Numero ?? string.Empty,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<EquipoModel?> GetEquipo(Guid id)
        {
            var equipo = await _equipoRepository.GetEquipo(id);
            if (equipo == null) return null;

            return new EquipoModel
            {
                Id = equipo.Id,
                SalaId = equipo.SalaId,
                SalaNumero = equipo.Sala?.Numero ?? string.Empty,
                Estado = equipo.Estado
            };
        }

        public async Task<IList<EquipoModel>> GetEquiposBySalaId(Guid salaId)
        {
            var equipos = await _equipoRepository.GetEquiposBySalaId(salaId);
            return equipos.Select(e => new EquipoModel
            {
                Id = e.Id,
                SalaId = e.SalaId,
                SalaNumero = e.Sala?.Numero ?? string.Empty,
                Estado = e.Estado
            }).ToList();
        }

        public async Task<IList<EquipoModel>> GetEquiposByEstado(EstadoEquipo estado)
        {
            var equipos = await _equipoRepository.GetEquiposByEstado(estado);
            return equipos.Select(e => new EquipoModel
            {
                Id = e.Id,
                SalaId = e.SalaId,
                SalaNumero = e.Sala?.Numero ?? string.Empty,
                Estado = e.Estado
            }).ToList();
        }

        public async Task Create(AddEquipoModel model)
        {
            // Verificar que la sala existe
            var sala = await _salaRepository.GetSala(model.SalaId);
            if (sala == null)
            {
                throw new InvalidOperationException("La sala especificada no existe.");
            }

            var equipo = _mapper.Map<Equipo>(model);
            equipo.Id = Guid.NewGuid();

            await _equipoRepository.Save(equipo);
        }

        public async Task Update(Guid id, AddEquipoModel model)
        {
            var equipo = await _equipoRepository.GetEquipo(id);
            if (equipo == null)
            {
                throw new InvalidOperationException("El equipo no existe.");
            }

            // Verificar que la sala existe si se cambió
            if (equipo.SalaId != model.SalaId)
            {
                var sala = await _salaRepository.GetSala(model.SalaId);
                if (sala == null)
                {
                    throw new InvalidOperationException("La sala especificada no existe.");
                }
            }

            equipo.SalaId = model.SalaId;
            equipo.Estado = model.Estado;

            await _equipoRepository.Update(equipo);
        }

        public async Task Delete(Guid id)
        {
            var equipo = await _equipoRepository.GetEquipo(id);
            if (equipo == null)
            {
                throw new InvalidOperationException("El equipo no existe.");
            }

            await _equipoRepository.Delete(id);
        }
    }
}

