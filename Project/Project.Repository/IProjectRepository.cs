using System.Threading.Tasks;
using Project.Domain;
using Project.Domain.Identity;

namespace Project.Repository
{
    public interface IProjectRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void DeleteRange<T>(T[] entity) where T : class;
        Task<bool> SaveChangesAsync();


        Task<Role[]> GetAllRoles();
        Task<UserRole[]> GetAllUserRole();
        Task<UserRole> GetUserRoleByUserId(int Id);
        Task<UserRole> GetUserRoleByRoleId(int Id);

        Task<User[]> GetAllUserAdmin();
        Task<User> GetUserAdminByEmail(string adminEmail);


        Task<User[]> GetAllUsersAsync();
        Task<User> GetUserAsyncById(int userId);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUserName(string username);

        Task<Employee[]> GetAllEmployeeAsync();
        Task<Employee[]> GetAllEmployeeAsync(string userId);
        Task<Employee> GetEmployeeAsyncById(int employeeId);
    }
}