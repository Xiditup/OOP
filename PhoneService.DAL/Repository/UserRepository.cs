using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;

namespace PhoneService.DAL.Repository
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(ApplicationContext context) : base(context) { }
        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Login == login);
        }
        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Phone == phone);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
        public IEnumerable<User> GetAllExceptAdmin()
        {
            return _dbSet.Where(u => u.Role != "Admin").ToList();
        }
        public IEnumerable<User> GetUsersFiltered(string name, string login, bool employees, bool clients)
        {
            var query = _dbSet.AsQueryable();
            query = query.Where(u => u.Role != "Admin");

            if (!employees)
            {
                query = query.Where(u => u.Role != "Employee");
            }

            if (!clients)
            {
                query = query.Where(u => u.Role != "Client");
            }

            query = query.Where(u => u.Name.Contains(name));

            query = query.Where(u => u.Login.Contains(login));

            return query.ToList();
        }
    }
}
