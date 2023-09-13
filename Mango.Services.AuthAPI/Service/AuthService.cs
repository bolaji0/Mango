using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using System.Web.Providers.Entities;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(AppDbContext db, UserManager<ApplicationUser>
             userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }



        public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDTO.Email);

                    UserDTO userDTO = new UserDTO()
                    {
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        ID = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }

            }
            catch (Exception)
            {

            }
            return "Error encountered";
        }

       

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == loginRequestDTO.Email);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if(user == null || isValid == false)
            {
                return new LoginResponseDTO() { User = null, Token = "" };
            }

            //if user was found generate jwt token
            var token = _jwtTokenGenerator.GenerateToken(user);

            UserDTO userDTO = new()
            {
                Email = user.Email,
                Name = user.Name,
                ID = user.Id,
                PhoneNumber = user.PhoneNumber,
            };

            LoginResponseDTO loginResponse = new()
            {
                User = userDTO,
                Token = token
            };

            return loginResponse;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == email);
            if(user != null)
            {
                //check if role exists
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //Create role if role does not exists
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                //add role to claim
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        //
    }
}
