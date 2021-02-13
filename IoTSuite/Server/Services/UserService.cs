using IoTSuite.Server.Extensions;
using IoTSuite.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IoTSuite.Server.Services
{
    public interface IUserService
    {
        Task<BasicAuthUser> Authenticate(string username, string password);
        Task<IEnumerable<BasicAuthUser>> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BasicAuthUser> Authenticate(string username, string password)
        {
            BasicAuthUser user = _context.User.SingleOrDefault(x => x.Username == username);

            byte[] passwordHash = HashValue(password + user.Salt.ToString().ToUpper());

            var result = await Task.Run(() => user.PasswordHash.SequenceEqual(passwordHash));

            // return null if user not found
            if (!result)
                return null;

            // authentication successful so return user details without password
            return user.WithoutPassword();
        }

        public async Task<BasicAuthUser> CreateUser(BasicAuthUserDTO userDto)
        {
            Guid salt = Guid.NewGuid();

            BasicAuthUser user = new BasicAuthUser
            {
                Username = userDto.Username,
                Salt = salt,
                PasswordHash = HashValue(userDto.Password + salt.ToString().ToUpper())
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return user.WithoutPassword();
        }

        public async Task<IEnumerable<BasicAuthUser>> GetAll()
        {
            return await Task.Run(() => _context.User.AsEnumerable().WithoutPasswords());
        }

        public async Task<BasicAuthUser> GetAll(long Id)
        {
            var user = await Task.Run(() => _context.User.AsEnumerable().Where(user => user.Id.Equals(Id)).FirstOrDefault());
            
            if (user == null)
            {
                return null;
            }

            return user.WithoutPassword();
        }

        public async Task<BasicAuthUser> GetAll(string Username)
        {
            var user = await Task.Run(() => _context.User.AsEnumerable().Where(user => user.Username.Equals(Username)).FirstOrDefault());

            if (user == null)
            {
                return null;
            }

            return user.WithoutPassword();
        }

        public async Task<BasicAuthUser> UpdateUser(long Id, BasicAuthUserDTO userDto)
        {
            BasicAuthUser user = await GetAll(Id);

            user.Username = userDto.Username;
            user.PasswordHash = HashValue(userDto.Password + user.Salt.ToString().ToUpper());

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return user.WithoutPassword();
        }

        public async Task<BasicAuthUser> DeleteUser(long Id)
        {
            var user = await _context.User.FindAsync(Id);
            if (user == null)
            {
                return null;
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user.WithoutPassword();
        }

        private bool UserExists(long Id)
        {
            return _context.User.Any(e => e.Id == Id);
        }

        public static byte[] HashValue(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);

            var sha1 = SHA512.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            sha1.Clear();

            return hashBytes;
        }
    }
}
