using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    
    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromServices] IUserRepository userRepository,
        [FromBody] UserCreationDto dto)
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

    [HttpPost("SignIn")]
    public IActionResult SignIn([FromServices] IConfiguration configuration,
    [FromServices] IUserRepository userRepository, [FromBody] UserSignInDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username)
            || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Either Username or Password is missing");
        }

        if (userRepository.SignIn(dto))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["AppSecret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, dto.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}