using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Infra.Data;

namespace Waiter.Infra.Repositories
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(ApplicationDbContext dbContext)
            : base(dbContext) { }
    }
}
