using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Domain;
using Project.Repository;
using Project.WebAPI.Dtos;
using System.Threading.Tasks;

namespace Project.WebAPI.Controllers
{
    [Route("/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly IProjectRepository _repo;

        public EmployeeController(IProjectRepository repo,
                              IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetEmployee")]
        public async Task<IActionResult> GetEmployee()
        {
            try
            {
                var users = await _repo.GetAllEmployeeAsync();
                var results = _mapper.Map<EmployeeDto[]>(users);

                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetAllEmployeeByUserId/{userId}")]
        public async Task<IActionResult> GetAllEmployeeByUserId(string userId)
        {
            try
            {
                var users = await _repo.GetAllEmployeeAsync(userId);
                var results = _mapper.Map<EmployeeDto[]>(users);

                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{EmployeeId}")]
        public async Task<IActionResult> Get(int EmployeeId)
        {
            try
            {
                var user = await _repo.GetEmployeeAsyncById(EmployeeId);
                var results = _mapper.Map<EmployeeDto>(user);

                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco Dados Falhou");
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post(EmployeeDto model)
        {
            try
            {
                var evento = _mapper.Map<Employee>(model);

                _repo.Add(evento);

                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EmployeeDto>(evento));
                }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco Dados Falhou {ex.Message}");
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("{EmployeeId}")]
        public async Task<IActionResult> Put(int EmployeeId, EmployeeDto model)
        {
            try
            {
                var Employee = await _repo.GetEmployeeAsyncById(EmployeeId);
                if (Employee == null) return NotFound();
                //if (model.Endereco != null)
                //{
                //    var idEndereco = new List<int>();

                //    model.Endereco.ForEach(item => idEndereco.Add(item.EnderecoId));

                //    var Employees = Employee.Endereco.Where(
                //        endereco => !idEndereco.Contains(endereco.EnderecoId)
                //    ).ToArray();

                //    if (Employees.Length > 0) _repo.DeleteRange(Employees);
                //}
                _mapper.Map(model, Employee);

                _repo.Update(Employee);

                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/Employee/{model.Id}", _mapper.Map<EmployeeDto>(Employee));
                }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco Dados Falhou " + ex.Message);
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("{EmployeeId}")]
        public async Task<IActionResult> Delete(int EmployeeId)
        {
            try
            {
                var Employee = await _repo.GetEmployeeAsyncById(EmployeeId);
                if (Employee == null) return NotFound();

                _repo.Delete(Employee);

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