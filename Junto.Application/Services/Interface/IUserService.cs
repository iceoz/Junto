using Junto.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Junto.Application.Services.Interface
{
    public interface IUserService
    {
        Task<UserResponse> Create(CreateUser request);
        Task<IEnumerable<UserResponse>> GetAll();
        Task<UserResponse> GetOne(int id);
        Task Delete(int id);
        Task Update(UpdateUser request);
        Task<UserResponse> ValidateAuth(UserAuthentication request);
        Task UpdatePassword(int id, UpdateUserPassword request);
    }
}
