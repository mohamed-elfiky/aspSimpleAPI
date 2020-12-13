using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simpleAPI.DTOs.V1;
using simpleAPI.Models;
using simpleAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simpleAPI.Controllers.V1
{
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        
        //injecting the authrepository
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO request) 
        {
            User newUser = new User { Username = request.Username };
            ServiceResponse<int> response = await _authRepository.Register(newUser, request.Password);
            if (!response.Success) 
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO request) 
        {
            ServiceResponse<string> respone = await _authRepository.Login(request.Username, request.Password);
            if (!respone.Success) 
            {
                return BadRequest(respone);
            }
            return Ok(respone);
        }
        

    }
}
