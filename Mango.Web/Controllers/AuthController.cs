using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            ResponseDTO responseDTO = await _authService.LoginAsync(obj);

            if(responseDTO != null && responseDTO.IsSuccess)
            {
                LoginResponseDTO loginResponseDTO = 
                    JsonConvert.DeserializeObject<LoginResponseDTO>
                            (Convert.ToString(responseDTO.Result));


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDTO.Message);
                return View(obj);
            }
        }

        public IActionResult Register()
        {
            //ResponseDTO responseDTO = await _authService.LoginAsync(obj);
            //if (responseDTO == null && responseDTO.IsSuccess)
            //{
            //    LoginResponseDTO loginResponseDTO = JsonConvert.
            //        DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDTO.Result));
            //}

            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem() {Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem() {Text=SD.RoleCustomer,Value=SD.RoleCustomer}
            };

            ViewBag.Rolelist = rolelist;  
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO obj)
        {
            ResponseDTO result = await _authService.RegisterAsync(obj);
            ResponseDTO assignRole;

            if(result != null && result.IsSuccess)
            {
                if(string.IsNullOrEmpty(obj.RoleName))
                {
                    obj.RoleName = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(obj);
                if (assignRole!=null && assignRole.IsSuccess)
                {
                    TempData["Success"] = "Registration Successfull";
                    return RedirectToAction(nameof(Login));
                }
            }
            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem() {Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem() {Text=SD.RoleCustomer,Value=SD.RoleCustomer}
            };

            ViewBag.Rolelist = rolelist;
            return View(obj);

        }



        [HttpPost]
        public IActionResult Logout()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);

        }
    }
}
