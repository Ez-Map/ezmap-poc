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
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }
    
    
    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromServices] IUnitOfWork uow,
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

        if (await uow.UserRepository.CreateUser(dto)) return Ok("Your account is created!");

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromServices] IConfiguration configuration,
    [FromServices] IUnitOfWork uow, [FromBody] UserSignInDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username)
            || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Either Username or Password is missing");
        }

        Guid? userId = await uow.UserRepository.SignIn(dto);

        if (userId != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["AppSecret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, userId.ToString()!),
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