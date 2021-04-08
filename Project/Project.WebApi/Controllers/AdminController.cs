using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Domain.Identity;
using Project.Repository;
using Project.WebApi.Dtos;
using Project.WebAPI.Dtos;
using System.Threading.Tasks;

namespace Project.WebAPI.Controllers
{
    [Route("/admin")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IProjectRepository _repo;
        private readonly IMapper _mapper;

        public AdminController(IProjectRepository repo,
                               IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllUserAdmin")]
        public async Task<IActionResult> GetAllUserAdmin()
        {
            try
            {
                var users = await _repo.GetAllUserAdmin();
                var userMap = _mapper.Map<UserAdminDto[]>(users);
                return Ok(userMap);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpPost("AddNewAdmin")]
        public async Task<IActionResult> Register(string userEmail)
        {
            try
            {
                var userResult = _repo.GetUserByEmail(userEmail);
                userResult.Result.isAdmin = true;

                _repo.Update(userResult.Result);

                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/Admin/{userResult.Result.Id}", _mapper.Map<UserAdminDto>(userResult.Result));
                }

                return BadRequest(userResult.Exception);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpDelete("DeleteAdmin")]
        public async Task<IActionResult> Delete(string adminEmail)
        {
            try
            {
                var userResult = _repo.GetUserByEmail(adminEmail);
                if (userResult == null) return NotFound();

                _repo.Delete(userResult.Result);

                if (await _repo.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco Dados Falhou");
            }

            return BadRequest();
        }

    }
}