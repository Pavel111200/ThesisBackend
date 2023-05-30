using Microsoft.EntityFrameworkCore;
using MyMovieList.Core.Contracts;
using MyMovieList.Core.Models;
using MyMovieList.Infrastructure.Data;
using MyMovieList.Infrastructure.Data.Repositories;
using System.Security.Cryptography;

namespace MyMovieList.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbRepository repo;

        public UserService(IApplicationDbRepository repo)
        {
            this.repo = repo;
        }

        public async Task<UserViewModel> GetUserByEmail(string email)
        {
            CustomUser user = await repo.All<CustomUser>().Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentNullException("This Email does not exist");
            }

            return new UserViewModel
            {
                Email = user.Email,
                UserId = user.Id,
                Role = user.Role,
                Username = user.Username
            };
        }

        public async Task<IEnumerable<UserListViewModel>> GetUsers()
        {
            return await repo.All<CustomUser>()
                .Select(u => new UserListViewModel()
                {
                    Id= u.Id,
                    Email = u.Email,
                    UserName = u.Username,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<bool> Suggestion(UserSuggestionViewModel model)
        {
            bool isSaved = false;
            UserSuggestion userSuggestion = new UserSuggestion
            {
                Title = model.Title,
                Type = model.Type,
            };

            try
            {
                await repo.AddAsync<UserSuggestion>(userSuggestion);
                await repo.SaveChangesAsync();
                isSaved = true;
            }
            catch (Exception)
            {
            }

            return isSaved;
        }

        public async Task DeleteSuggestion(Guid suggestionId)
        {
            try
            {
                await repo.DeleteAsync<UserSuggestion>(suggestionId);
                await repo.SaveChangesAsync();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Invalid id.");
            }
        }

        public async Task<UserViewModel> Register(RegisterUserViewModel requestUser)
        {
            CreatePasswordHash(requestUser.Password, out byte[] passwordHash, out byte[] passwordSalt);
            CustomUser user = new CustomUser
            {
                Username = requestUser.Username,
                Email = requestUser.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            try
            {
                await repo.AddAsync<CustomUser>(user);
                await repo.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("User was not register");
            }

            return await GetUserByEmail(requestUser.Email);
        }

        public async Task<UserViewModel> Login(LoginUserViewModel request)
        {
            var user = await repo.All<CustomUser>().Where(u => u.Email == request.Email).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new ArgumentException("Wrong password");
            }

            return new UserViewModel 
            { 
                Email = user.Email, 
                Role = user.Role, 
                UserId = user.Id, 
                Username = user.Username 
            };
        }

        public async Task ChangeRole(Guid userId, string newRole)
        {
            var user = await repo.All<CustomUser>().Where(u => u.Id == userId).FirstOrDefaultAsync();

            if(user is null)
            {
                throw new ArgumentException("Invalid user");
            }

            try
            {
                user.Role = newRole;
                await repo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
