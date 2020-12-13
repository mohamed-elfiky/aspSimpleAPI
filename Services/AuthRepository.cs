using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using simpleAPI.Data;
using simpleAPI.DTOs.V1;
using simpleAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace simpleAPI.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; 
        public AuthRepository(DataContext context, IConfiguration configuration) 
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();

            //get the user by the user name
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username.ToLower());

            //if the user doesnt exist
            if (user == null)
            {
                response.Success = false;
                response.Message = "the given username does not exist.";
            }
            // password doenst match with given user name
            else if (!Utility.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "the given pssword does not match the given username";
            }
            else
            {
                response.Data = CreateToken(user); 
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            
            //check if the user already exixts
            if (await UserExists(user.Username)) 
            {
                response.Success = false;
                response.Message = "User Already Exists";
                return response;
            }

            //getting the hashed passowrd and the generated salt
            Utility.CreatePasswordHash(password, out byte[] passwrodHash, out byte[] passowrdSalt);
            user.PasswordHash = passwrodHash;
            user.PasswordSalt = passowrdSalt; 

            //persist the user
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            //pass the response data
            response.Data = user.Id;
        
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            //check if the user exists in the database
            bool userExists = await _context.Users.AnyAsync(x => x.Username.ToLower() == username);
            return userExists ? true : false; 
        }

        //Helper Method
        private string CreateToken(User user) 
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtSettings:Secret").Value));

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

    }
}
