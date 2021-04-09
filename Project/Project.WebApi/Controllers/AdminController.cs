using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Repository;
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
        public async Task<IActionResult> Register(string username)
        {
            try
            {
                var userResult = _repo.GetUserByUserName(username);
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
        public async Task<IActionResult> Delete(string username)
        {
            try
            {
                var userResult = _repo.GetUserByUserName(username);
                if (userResult == null) return NotFound();
                userResult.Result.isAdmin = false;
                _repo.Update(userResult.Result);

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