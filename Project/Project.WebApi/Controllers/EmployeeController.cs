using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Domain;
using Project.Repository;
using Project.WebApi.Dtos;
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
        [Route("GetEmployees")]
        [Authorize(Roles = "Administrator")]
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
        [Route("GetAllEmployeesByUserId/{userId}")]
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

        // POST
        /// <summary>
        /// Create a new Employee.
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     POST /Employee
        ///     {
        ///        "employee_Name": "Darth Vader",
        ///        "employee_Salary": 2500,
        ///        "employee_Age": "59",
        ///        "profile_Image": "",
        ///        "userId": "youUserID"
        ///     }
        ///     
        ///
        /// </remarks>
      
        [HttpPost]
        public async Task<IActionResult> Post(EmployeeDto model)
        {
            try
            {
                var evento = _mapper.Map<Employee>(model);

                _repo.Add(evento);

                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/evento/{model.UserId}", _mapper.Map<EmployeeDto>(evento));
                }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Banco Dados Falhou {ex.Message}");
            }

            return BadRequest();
        }

        // PUT
        /// <summary>
        /// Put a Employee.
        /// To find out what is the Id, user the GetAllEmployeesByUserId controller, then it will list all your employees
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     PUT /Employee
        ///     {
        ///        "Id": "8",
        ///        "employee_Name": "Darth Vader",
        ///        "employee_Salary": 2500,
        ///        "employee_Age": "59",
        ///        "profile_Image": "",
        ///        "userId": "youUserID"
        ///     }
        ///     
        ///
        /// </remarks>


        [HttpPut]
        public async Task<IActionResult> Put(EmployeeDto model)
        {
            try
            {
                if (!model.Id.HasValue) return BadRequest("Please insert the Id in Json, to place the Put");
                var Employee = await _repo.GetEmployeeAsyncById(model.Id.Value);
                if (Employee == null) return NotFound();

                _mapper.Map(model, Employee);

                _repo.Update(Employee);

                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/Employee/{model.Id.Value}", _mapper.Map<EmployeeDto>(Employee));
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