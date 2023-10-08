using System.Security.Claims;
using EzMap.Domain.Dtos;
using EzMap.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzMap.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class PoiController : ControllerBase
{

    [Authorize]
    [HttpPost("CreatePoi")]
    public async Task<IActionResult> Create([FromBody] PoiCreateDto dto,[FromServices] IHttpContextAccessor _httpContextAccessor ,[FromServices] IUnitOfWork uow)
    {
            if (string.IsNullOrEmpty(dto.Name)
                && string.IsNullOrEmpty(dto.Address))
            {
                return BadRequest("Kindly fill Name, Address fields!");
            }
            
            var succssful = Guid.TryParse(_httpContextAccessor?.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);

            var userId = succssful ? (Guid?)tempId : null;//TODO: fix this

            uow.PoiRepository.AddPoi(dto.WithUserId(userId ?? Guid.NewGuid()));
            
            if (await uow.SaveAsync() > 0) return Ok("Your point of interest is created successfully!");

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
    
    
    
}