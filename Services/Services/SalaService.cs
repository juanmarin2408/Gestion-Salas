using AutoMapper;
using Domain;
using Domain.Enums;
using Infrastructure.Repositories;
using Services.Models.SalaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class SalaService : ISalaService
    {
        private readonly ISalaRepository _salaRepository;
        private readonly IMapper _mapper;

        public SalaService(ISalaRepository salaRepository, IMapper mapper)
        {
            _salaRepository = salaRepository;
            _mapper = mapper;
        }

        public async Task<IList<SalaModel>> GetSalas()
        {
            var salas = await _salaRepository.GetSalas();
            return salas.Select(s => new SalaModel
            {
                Id = s.Id,
                Numero = s.Numero,
                Capacidad = s.Capacidad,
                Ubicacion = s.Ubicacion,
                Estado = s.Estado,
                UsuarioId = s.UsuarioId,
                UsuarioNombre = s.Usuario != null ? $"{s.Usuario.Nombre} {s.Usuario.Apellido}" : null,
                TotalEquipos = s.Equipos?.Count ?? 0
            }).ToList();
        }

        public async Task<SalaModel?> GetSala(Guid id)
        {
            var sala = await _salaRepository.GetSala(id);
            if (sala == null) return null;

            return new SalaModel
            {
                Id = sala.Id,
                Numero = sala.Numero,
                Capacidad = sala.Capacidad,
                Ubicacion = sala.Ubicacion,
                Estado = sala.Estado,
                UsuarioId = sala.UsuarioId,
                UsuarioNombre = sala.Usuario != null ? $"{sala.Usuario.Nombre} {sala.Usuario.Apellido}" : null,
                TotalEquipos = sala.Equipos?.Count ?? 0
            };
        }

        public async Task<SalaModel?> GetSalaByNumero(string numero)
        {
            var sala = await _salaRepository.GetSalaByNumero(numero);
            if (sala == null) return null;

            return _mapper.Map<SalaModel>(sala);
        }

        public async Task<IList<SalaModel>> GetSalasByEstado(EstadoSala estado)
        {
            var salas = await _salaRepository.GetSalasByEstado(estado);
            return _mapper.Map<IList<SalaModel>>(salas);
        }

        public async Task<IList<SalaModel>> GetSalasByUsuarioId(Guid usuarioId)
        {
            var salas = await _salaRepository.GetSalasByUsuarioId(usuarioId);
            return _mapper.Map<IList<SalaModel>>(salas);
        }

        public async Task Create(AddSalaModel model)
        {
            var existing = await _salaRepository.GetSalaByNumero(model.Numero);
            if (existing != null)
            {
                throw new InvalidOperationException("Ya existe una sala con ese número.");
            }

            var sala = _mapper.Map<Sala>(model);
            sala.Id = Guid.NewGuid();

            await _salaRepository.Save(sala);
        }

        public async Task Update(Guid id, AddSalaModel model)
        {
            var sala = await _salaRepository.GetSala(id);
            if (sala == null)
            {
                throw new InvalidOperationException("La sala no existe.");
            }

            // Verificar si el número ya está en uso por otra sala
            var existing = await _salaRepository.GetSalaByNumero(model.Numero);
            if (existing != null && existing.Id != id)
            {
                throw new InvalidOperationException("Ya existe otra sala con ese número.");
            }

            sala.Numero = model.Numero;
            sala.Capacidad = model.Capacidad;
            sala.Ubicacion = model.Ubicacion;
            sala.Estado = model.Estado;
            sala.UsuarioId = model.UsuarioId;

            await _salaRepository.Update(sala);
        }

        public async Task Delete(Guid id)
        {
            var sala = await _salaRepository.GetSala(id);
            if (sala == null)
            {
                throw new InvalidOperationException("La sala no existe.");
            }

            await _salaRepository.Delete(id);
        }
    }
}

