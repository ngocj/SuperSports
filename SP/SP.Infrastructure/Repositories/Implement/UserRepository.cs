using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;
using SP.Infrastructure.Context;
using SP.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Implement
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SPContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _SPContext.Set<User>()
                .Include(u => u.Role)
                .OrderBy(u => u.Role.RoleName) // Sắp xếp theo tên vai trò
                .ToListAsync();
        }
        public override async Task<User> GetByIdAsync(Guid id)
        {
            return await _SPContext.Set<User>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

    }


}
