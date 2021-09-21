using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Domain;
using Project.Domain.Identity;
using Project.Respository;

namespace Project.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _context;
        public ProjectRepository(ProjectContext context)
        {
            _context = context;
        }

        //Gerais
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public void DeleteRange<T>(T[] entityArray) where T : class
        {
            _context.RemoveRange(entityArray);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

         public async Task<User[]> GetAllUsersAsync()
        {
            IQueryable<User> query = _context.Users;
            query = query.AsNoTracking().OrderBy(c => c.Id);
            return await query.ToArrayAsync();
        }
         public async Task<User> GetUserAsyncById(int userId)
        {
            IQueryable<User> query = _context.Users
            .Include(c => c.Employees);
            query = query.AsNoTracking().OrderBy(c => c.Id)
            .Where(c => c.Id == userId);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<Employee[]> GetAllEmployeeAsync()
        {
            IQueryable<Employee> query = _context.Employees;
            query = query.AsNoTracking().OrderBy(c => c.Id);
            return await query.ToArrayAsync();
        }
        public async Task<Employee[]> GetAllEmployeeAsync(string userId)
        {
            IQueryable<Employee> query = _context.Employees
            .Where(c => c.UserId == userId);
            query = query.AsNoTracking().OrderBy(c => c.Id);
            return await query.ToArrayAsync();
        }
        public async Task<Employee> GetEmployeeAsyncById(int employeeId)
        {
            IQueryable<Employee> query = _context.Employees;
            query = query.AsNoTracking().OrderBy(c => c. Id)
            .Where(c => c.Id == employeeId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Role[]> GetAllRoles()
        {
            IQueryable<Role> query = _context.Roles;
            query = query.AsNoTracking().OrderBy(c => c.Id);
            return await query.ToArrayAsync();
        }

        public async Task<UserRole[]> GetAllUserRole()
        {
            IQueryable<UserRole> query = _context.UserRoles;
            query = query.AsNoTracking();
            return await query.ToArrayAsync();
        }

        public async Task<UserRole> GetUserRoleByRoleId(int Id) 
        {
            IQueryable<UserRole> query = _context.UserRoles
            .Include(c => c.Role);
            query = query.AsNoTracking().OrderBy(c => c.RoleId)
            .Where(c => c.RoleId == Id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<UserRole> GetUserRoleByUserId(int Id)
        {
            IQueryable<UserRole> query = _context.UserRoles
           .Include(c => c.User);
            query = query.AsNoTracking().OrderBy(c => c.UserId)
            .Where(c => c.UserId == Id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<User[]> GetAllUserAdmin()
        {
            IQueryable<User> query = _context.Users;
            query = query.AsNoTracking().OrderBy(c => c.Id).Where(c => c.isAdmin == true);
            return await query.ToArrayAsync();
        }

        public async Task<User> GetEmailUserAsync(string email)
        {
            IQueryable<User> query = _context.Users;
            query = query.AsNoTracking().OrderBy(c => c.Email)
            .Where(c => c.Email == email);
            return await query.FirstOrDefaultAsync();
        }
    }
}