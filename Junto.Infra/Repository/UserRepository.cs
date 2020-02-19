using Junto.Application.Entity;
using Junto.Application.Repository;
using Junto.Infra.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Junto.Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly JuntoContext _context;
        private readonly ILogger _logger;

        public UserRepository(JuntoContext context, ILogger<UserRepository> logger)
        {
            _logger = logger;
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<User> Create(User user)
        {
            IDbContextTransaction transaction = null;

            try
            {
                transaction = await _context.Database.BeginTransactionAsync();

                var entity = await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return entity.Entity;
            }
            catch (Exception e)
            {
                await transaction?.RollbackAsync();
                _logger.LogError(exception: e, "error on create user");
            }
            finally
            {
                await transaction.DisposeAsync();
            }

            return null;
        }

        public async Task<bool> Delete(User user)
        {
            IDbContextTransaction transaction = null;

            try
            {
                transaction = await _context.Database.BeginTransactionAsync();

                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception e)
            {
                await transaction?.RollbackAsync();
                _logger.LogError(exception: e, "error on delete user");
            }
            finally
            {
                await transaction.DisposeAsync();
            }

            return false;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                return await _context.User.ToListAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<User> GetByLoginPassword(string login, string password)
        {
            try
            {
                return await _context.User.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<User> GetOne(int id)
        {
            try
            {
                return await _context.User.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Update(User user)
        {
            IDbContextTransaction transaction = null;

            try
            {
                transaction = await _context.Database.BeginTransactionAsync();

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception e)
            {
                await transaction?.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }

            return false;
        }

        public async Task<bool> VerifyIfLoginAlreadyExists(int id, string login)
        {
            try
            {
                return await _context.User.AnyAsync(x => x.Id != id && x.Login == login);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> VerifyIfLoginAlreadyExists(string login)
        {
            try
            {
                return await _context.User.AnyAsync(x => x.Login == login);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
