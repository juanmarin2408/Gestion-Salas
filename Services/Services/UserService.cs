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

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            var usuario = await _userRepository.GetByEmail(email);
            if (usuario == null || !usuario.Activo)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password);
            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Actualizar último acceso
            usuario.UltimoAcceso = DateTime.UtcNow;
            await _userRepository.Update(usuario);

            return usuario;
        }

        public async Task Register(AddUserModel model)
        {
            // ¿Existe el email?
            var existingEmail = await _userRepository.GetByEmail(model.Email);
            if (existingEmail != null)
            {
                throw new InvalidOperationException("El correo ya está registrado.");
            }

            // ¿Existe el documento?
            var existingDoc = await _userRepository.GetByDocumento(model.Documento);
            if (existingDoc != null)
            {
                throw new InvalidOperationException("El documento ya está registrado.");
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

        public async Task UpdateLastAccess(Guid userId)
        {
            var usuario = await _userRepository.GetUser(userId);
            if (usuario != null)
            {
                usuario.UltimoAcceso = DateTime.UtcNow;
                await _userRepository.Update(usuario);
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var usuario = await _userRepository.GetByEmail(email);
            return usuario != null;
        }

        public async Task Update(Guid id, AddUserModel model)
        {
            var usuario = await _userRepository.GetUser(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("El usuario no existe.");
            }

            // Verificar si el email ya está en uso por otro usuario
            var existingEmail = await _userRepository.GetByEmail(model.Email);
            if (existingEmail != null && existingEmail.Id != id)
            {
                throw new InvalidOperationException("El correo ya está registrado por otro usuario.");
            }

            // Verificar si el documento ya está en uso por otro usuario
            var existingDoc = await _userRepository.GetByDocumento(model.Documento);
            if (existingDoc != null && existingDoc.Id != id)
            {
                throw new InvalidOperationException("El documento ya está registrado por otro usuario.");
            }

            // Actualizar campos
            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.Documento = model.Documento;
            usuario.Email = model.Email;
            usuario.Rol = model.Rol;

            // Si se proporcionó una nueva contraseña, actualizarla
            if (!string.IsNullOrEmpty(model.Password))
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, model.Password);
            }

            await _userRepository.Update(usuario);
        }

        public async Task Delete(Guid id)
        {
            var usuario = await _userRepository.GetUser(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("El usuario no existe.");
            }

            await _userRepository.Delete(id);
        }

        public async Task ChangePasswordAsync(string email, string newPassword)
        {
            var usuario = await _userRepository.GetByEmail(email);
            if (usuario == null)
            {
                throw new InvalidOperationException("El correo no está registrado.");
            }

            if (!usuario.Activo)
            {
                throw new InvalidOperationException("El usuario está inactivo.");
            }

            // Hashear la nueva contraseña
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, newPassword);
            
            await _userRepository.Update(usuario);
        }
    }
}
