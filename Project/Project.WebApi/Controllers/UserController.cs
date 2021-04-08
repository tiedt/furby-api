using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.Domain.Identity;
using Project.EmailService;
using Project.EmailService.Model;
using Project.Repository;
using Project.WebApi.Dtos;
using Project.WebAPI.Dtos;

namespace Project.WebAPI.Controllers
{
    [Route("/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        public readonly IProjectRepository _repo;
        private readonly IEmailSender _emailSender;

        public UserController(IConfiguration config,
                              UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IProjectRepository repo,
                              IMapper mapper,
                              IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _repo = repo;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("GetAllUser")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var users = await _repo.GetAllUsersAsync();
                var results = _mapper.Map<UserDto[]>(users);

                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user == null)
                    return BadRequest("Usuário Não Encontrado");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
                var bodyEmail = string.Format("Olá {0}. <br>" +
                    "                           \n Você solicitou a alteração de senha, por favor <a href='{1}'>clique aqui</a> para alterar sua senha. <br>" +
                    "                           \n Caso não tenha solicitado por favor desconsiderar este e-mail.", user.FullName, callback);
                var message = new Message(new string[] { user.Email }, "Reset Password - FURBY", bodyEmail, null);
                await _emailSender.SendEmailAsync(message);

                return Ok("E-mail Enviado com Sucesso");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);

                var result = await _userManager.CreateAsync(user, userDto.Password);

                var userToReturn = _mapper.Map<UserDto>(user);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                    var bodyEmail = string.Format("Olá {0}. <br>" +
    "                           \n Você se cadastrou no FURBY, por favor <a href='{1}'>clique aqui</a> para confirmar a sua conta. <br>" +
    "                           \n Caso não tenha solicitado por favor desconsiderar este e-mail.", user.FullName, confirmationLink);
                    var message = new Message(new string[] { user.Email }, "Confirmation E-mail - FURBY", bodyEmail, null);
                    await _emailSender.SendEmailAsync(message);
                    Created("GetUser", userToReturn);
                    return Ok("Registrado com sucesso, por favor confirme o e-mail que enviamos para você ;)");
                }

                return BadRequest(result.Errors);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userLogin.UserName);

                var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());

                    var userToReturn = _mapper.Map<UserLoginDto>(appUser);

                    return Ok(new
                    {
                        token = GenerateJWToken(appUser).Result,
                        user = userToReturn
                    });
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }
        
        private async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            return View();
        }
        private IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }

        private async Task<string> GenerateJWToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(8),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}