using Junto.Application.Entity;
using Junto.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Junto.Application.Repository
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetOne(int id);
        Task<bool> Delete(User user);
        Task<bool> Update(User user);
        Task<User> GetByLoginPassword(string login, string password);
        Task<bool> VerifyIfLoginAlreadyExists(int id, string login);
        Task<bool> VerifyIfLoginAlreadyExists(string login);
    }
}
