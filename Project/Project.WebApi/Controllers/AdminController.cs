using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Domain.Identity;
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

        //    [HttpPost("Register")]
        //    [AllowAnonymous]
        //    public async Task<IActionResult> Register(UserDto userDto)
        //    {
        //        try
        //        {
        //            var user = _mapper.Map<User>(userDto);

        //            var result = await _userManager.CreateAsync(user, userDto.Password);

        //            var userToReturn = _mapper.Map<UserDto>(user);

        //            if (result.Succeeded)
        //            {
        //                return Created("GetUser", userToReturn);
        //            }

        //            return BadRequest(result.Errors);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
        //        }
        //    }

        //    [HttpPost("Login")]
        //    [AllowAnonymous]
        //    public async Task<IActionResult> Login(UserLoginDto userLogin)
        //    {
        //        try
        //        {
        //            var user = await _userManager.FindByNameAsync(userLogin.UserName);

        //            var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

        //            if (result.Succeeded)
        //            {
        //                var appUser = await _userManager.Users
        //                    .FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());

        //                var userToReturn = _mapper.Map<UserLoginDto>(appUser);

        //                return Ok(new
        //                {
        //                    token = GenerateJWToken(appUser).Result,
        //                    user = userToReturn
        //                });
        //            }

        //            return Unauthorized();
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
        //        }
        //    }

        //    private async Task<string> GenerateJWToken(User user)
        //    {
        //        var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Name, user.UserName)
        //    };

        //        var roles = await _userManager.GetRolesAsync(user);

        //        foreach (var role in roles)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, role));
        //        }

        //        var key = new SymmetricSecurityKey(Encoding.ASCII
        //            .GetBytes(_config.GetSection("AppSettings:Token").Value));

        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(claims),
        //            Expires = DateTime.Now.AddHours(8),
        //            SigningCredentials = creds
        //        };

        //        var tokenHandler = new JwtSecurityTokenHandler();

        //        var token = tokenHandler.CreateToken(tokenDescriptor);

        //        return tokenHandler.WriteToken(token);
        //    }
        //}
    }
}