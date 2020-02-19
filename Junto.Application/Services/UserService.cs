using Junto.Application.Entity;
using Junto.Application.Exceptions;
using Junto.Application.Helpers;
using Junto.Application.Models;
using Junto.Application.Repository;
using Junto.Application.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junto.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> Create(CreateUser request)
        {
            var login = StringHelper.Normalize(request.Login);
            var password = Sha256Helper.Convert(request.Password);

            if (await _userRepository.VerifyIfLoginAlreadyExists(login))
                throw new InvalidUserException("login already exists");

            var user = await _userRepository.Create(new User(login, request.Name, password));

            if (user == null)
                throw new UnexpectedUserException("cant create user");

            return Convert(user);
        }

        public async Task<IEnumerable<UserResponse>> GetAll()
        {
            var users = await _userRepository.GetAll();

            if (users == null || !users.Any())
                throw new NotFoundException("users not found");

            return users.Select(x => Convert(x));
        }

        public async Task<UserResponse> GetOne(int id)
        {
            var user = await _userRepository.GetOne(id);

            if (user == null)
                throw new NotFoundException("user not found");

            return Convert(user);
        }

        public async Task Delete(int id)
        {

            var user = await _userRepository.GetOne(id);

            if (user == null)
                throw new InvalidUserException("user not found");

            if (!await _userRepository.Delete(user))
                throw new UnexpectedUserException("cant delete user");
        }

        public async Task Update(UpdateUser request)
        {

            var user = await _userRepository.GetOne(request.Id);

            if (user == null)
                throw new InvalidUserException("user not found");

            var login = StringHelper.Normalize(request.Login);

            if (await _userRepository.VerifyIfLoginAlreadyExists(request.Id, login))
                throw new InvalidUserException("login already exists");

            user.Login = string.IsNullOrEmpty(login) ? user.Login : login;
            user.Name = string.IsNullOrEmpty(request.Name) ? user.Name : request.Name;

            if (!await _userRepository.Update(user))
                throw new UnexpectedUserException("cant update user");
        }

        public async Task<UserResponse> ValidateAuth(UserAuthentication request)
        {
            var login = StringHelper.Normalize(request.Login);
            var password = Sha256Helper.Convert(request.Password);

            var user = await _userRepository.GetByLoginPassword(login, password);

            if (user != null)
                return Convert(user);

            return null;
        }

        private UserResponse Convert(User user)
        {
            if (user == null)
                return null;

            return new UserResponse(user.Id, user.Name, user.Login);
        }

        public async Task UpdatePassword(int id, UpdateUserPassword request)
        {
            var user = await _userRepository.GetOne(id);

            if (user == null)
                throw new InvalidUserException("user not found");

            var password = Sha256Helper.Convert(request.Password);

            user.Password = password;

            if (!await _userRepository.Update(user))
                throw new UnexpectedUserException("cant update user");
        }
    }
}
