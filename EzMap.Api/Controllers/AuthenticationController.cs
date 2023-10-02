using Azure.Identity;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger)
    {
        _logger = logger;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateUser([FromServices]IUserRepository userRepository, [FromBody] UserCreationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) 
            || string.IsNullOrWhiteSpace(dto.Password)
            || string.IsNullOrWhiteSpace(dto.Email)
            || string.IsNullOrWhiteSpace(dto.DisplayName)
            )
        {
            return BadRequest("Kindly fill all the fields!");
        }
        
        if (await userRepository.CreateUser(dto)) return Ok("Your account is created!");
        
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        
    }

    [HttpPost("/hoangml")]
    public IActionResult SignIn([FromServices] IUserRepository userRepository, [FromBody] UserSignInDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username)
            || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Either Username or Password is missing");
        }

        if (userRepository.SignIn(dto)) return Ok("You are logged in");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}