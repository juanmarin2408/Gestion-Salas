using AutoMapper;
using Domain;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Services.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    // 👇 AQUÍ es class, y hereda de IUserService
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<IList<UserModel>> GetUsers()
        {
            var usuarios = await _userRepository.GetUsers();
            return _mapper.Map<IList<UserModel>>(usuarios);
        }

        public async Task<UserModel?> GetUser(Guid id)
        {
            var usuario = await _userRepository.GetUser(id);
            if (usuario == null) return null;

            return _mapper.Map<UserModel>(usuario);
        }

        public async Task Register(AddUserModel model)
        {
            // ¿Existe el email?
            var existing = await _userRepository.GetByEmail(model.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("El correo ya está registrado.");
            }

            // Mapear del modelo de vista a la entidad
            var usuario = _mapper.Map<Usuario>(model);
            usuario.Id = Guid.NewGuid();
            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.UtcNow;

            // Hashear contraseña
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, model.Password);

            await _userRepository.Save(usuario);
        }
    }
}
