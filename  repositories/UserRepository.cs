using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace foodOrderingApp.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public UserRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        private string GenerateJwtToken(Guid userId, Role role)
        {

            Console.WriteLine(userId.ToString(), "  ", Roles(role));
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),

            new Claim(ClaimTypes.Name, userId.ToString()),
            new Claim(ClaimTypes.Role,Roles(role))
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            Console.WriteLine("keyy----------------" + key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"]!,
                audience: _configuration["Jwt:Audience"]!,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);


            Console.WriteLine("validddd {0}", token.ValidTo);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public Guid Add(User user)
        {
            if (user == null)
            {

                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            }
            var exsistinguser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (exsistinguser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Id;
        }

        public string Delete(Guid id)
        {
            var foundUser = _context.Users.Find(id);
            if (foundUser == null)
            {
                throw new KeyNotFoundException("User Not Found");
            }
            _context.Remove(foundUser);
            _context.SaveChanges();
            return $"{foundUser.Name} Deleated Sucessfully";
        }

        public IEnumerable<User> GetAll(int pageSize,int pageNumber)
        {
            return _context.Users.Skip(pageSize*(pageNumber-1)).Take(pageSize);
        }

        public User GetById(Guid id)
        {
            var user = _context.Users
                  .Include(u => u.Restaurant)
                  .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("user Not Found");
            }
            return user;
        }

        public string Update(UpdateUserDto user, Guid userId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User Can't be null");
            }

            var existingUser = _context.Users.Find(userId);

            if (existingUser == null)
            {
                throw new KeyNotFoundException("Invalid User id");
            }
            existingUser.Name = user.Name;
            existingUser.Phone = user.Phone;
            existingUser.Email = user.Email;
            _context.Update(existingUser);
            _context.SaveChanges();

            return $"Profile update Sucessfully";
        }

        public object Login(LoginDto user)
        {
            var foundUser = _context.Users.Include(u => u.Restaurant).FirstOrDefault(u => u.Email == user.Email);
            if (foundUser == null)
            {
                throw new UnauthorizedAccessException("Invalid Credentails.");
            }
            bool isPasswordMatched = BCrypt.Net.BCrypt.Verify(user.Password, foundUser.Password);
            if (!isPasswordMatched)
            {
                throw new UnauthorizedAccessException("Invalid Credentails");
            }
            if (foundUser.Role == Role.Owner && foundUser.Restaurant?.IsVerified == false)
            {
                throw new UnauthorizedAccessException("You are not Approved");

            }

            string token = GenerateJwtToken(foundUser.Id, foundUser.Role);
            return new { auth_token = token, role = foundUser.Role };

        }


        public User? Profile(Guid id)
        {
            var user = _context.Users
                    .Include(u => u.Restaurant)
                    .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("user Not Found");
            }
            return user;

        }
        public static string Roles(Role r)
        {
            switch (r)
            {
                case Role.Admin:
                    return "Admin";
                case Role.Customer:
                    return "Customer";
                case Role.Owner:
                    return "Owner";
                default:
                    return "Unknown";
            }
        }

        public string SaveFirebasePushNotificationToken(FirebaseTokenDto firebaseTokenDto)
        {
            if (firebaseTokenDto == null || string.IsNullOrEmpty(firebaseTokenDto.FirebaseToken)){
                return "Invalid token data";
        }

            var existingUserToken =   _context.FirebaseTokens.FirstOrDefault(ft=>ft.UserId ==firebaseTokenDto.UserId);
          if(existingUserToken !=null){
                existingUserToken.FirebaseToken = firebaseTokenDto.FirebaseToken;
                _context.Update(existingUserToken);
            }
            else{
                FirebaseTokensModel firebaseTokensModel = new FirebaseTokensModel(){
                    UserId =firebaseTokenDto.UserId,
                    FirebaseToken=firebaseTokenDto.FirebaseToken
                };
                _context.FirebaseTokens.Add(firebaseTokensModel);
            }
            _context.SaveChanges();
            return "Token added sucessfully";

        }

        public string SaveFirebasePushNotificationToken(Guid UserId)
        {
            var existingtoken = _context.FirebaseTokens.FirstOrDefault<FirebaseTokensModel>(ft=>ft.UserId==UserId);
            if(existingtoken==null){
                throw new KeyNotFoundException("Token not Found");
            }
            return existingtoken.FirebaseToken;
        }
    }
}